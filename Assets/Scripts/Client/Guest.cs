using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using Calci.CommandLine;

namespace Game.Client
{
	using Shared.Enums;

	public class Guest : MonoBehaviour, INetEventListener
	{
		public static Guest inst;
		
		private NetManager netManager = default;
		private NetDataWriter netDataWriter = new NetDataWriter();

		private NetPeer host;
		
		public event Action<long, string> onChatReceive = delegate(long sender, string s) {  };
		
		public event Action<long, Vector3, Vector3> onBeginMove = delegate(long sender, Vector3 startPos, Vector3 direction) {  }; 
		public event Action<long, Vector3, Vector3> onEndMove = delegate(long sender, Vector3 endPos, Vector3 direction) {  }; 
		public event Action<long, Vector3, Vector3> onMoving = delegate(long sender, Vector3 pos, Vector3 direction) {  }; 
		
		public event Action<long, Vector3, Vector3> onSpawnCharacter = delegate(long sender, Vector3 pos, Vector3 direction) {  }; 
		public event Action<long, EDieReason> onDestroyCharacter = delegate(long sender, EDieReason reason) {  }; 

		private int port;
		public long processId { get; protected set; }
        
		private void Awake()
		{
			inst = this;
			
			int serverPort = CommandLineParser.GetInt("serverPort", 39999);
			port = CommandLineParser.GetInt("port", 39999);
			
			string address = CommandLineParser.GetString("address");
            
			netManager = new NetManager(this)
			{
			};

			StartCoroutine(Startup(port, serverPort, address));
		}

		private IEnumerator Startup(int port, int serverPort, string address)
		{
			for (int i = 0; i < 100; i++)
			{
				try
				{
					port += 10;
					Debug.Log($"host:{host}, {port} -> {serverPort}");
					
					netManager.Start(port);
					break;
				}
				catch (Exception e)
				{
					// ignore
				}
				
				yield return new WaitForSeconds(0.1f);
			}
			
			try
			{
				host = netManager.Connect(new IPEndPoint(IPAddress.Parse(address), serverPort), "Guest");
			}
			catch (Exception e)
			{
				Debug.LogError(address);
				Debug.LogException(e);
			}
		}

		private void Update()
		{
			netManager.PollEvents();
		}

		public void OnPeerConnected(NetPeer peer)
		{
			processId = Process.GetCurrentProcess().Id;
			// long processId = Random.Range(200, 2340);
			
			netDataWriter.Reset();
			netDataWriter.Put(EPacketType.HANDSHAKE.ToShort());
			netDataWriter.Put(processId);

			host.Send(netDataWriter, DeliveryMethod.Sequenced);
		}

		public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			Debug.Log("Disconnected");
		}

		public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
		{
			EPacketType packetType = (EPacketType)reader.GetShort();
			if (packetType == EPacketType.BROADCAST)
			{
				EBroadcastType broadcastType = (EBroadcastType)reader.GetInt();
				
				if (broadcastType == EBroadcastType.ChatBroadcast)
				{
					long sender = reader.GetLong();
					string message = reader.GetString();

					onChatReceive(sender, message);
				}
				else if (broadcastType == EBroadcastType.BEGIN_MOVE)
				{
					long sender = reader.GetLong();
					
					float startPosX = reader.GetInt() * 0.01f;
					float startPosY = reader.GetInt() * 0.01f;
					float startPosZ = reader.GetInt() * 0.01f;

					Vector3 pos = new Vector3(startPosX, startPosY, startPosZ);
					
					Debug.Log($"{pos.x:N2}, {pos.y:N2}, {pos.z:N2}");

					float directionY = reader.GetInt() * 0.01f;

					onBeginMove(sender, pos, Vector3.up * directionY);
				}
				else if (broadcastType == EBroadcastType.END_MOVE)
				{
					long sender = reader.GetLong();
					
					float destX = reader.GetInt() * 0.01f;
					float destY = reader.GetInt() * 0.01f;
					float destZ = reader.GetInt() * 0.01f;

					Vector3 pos = new Vector3(destX, destY, destZ);

					float directionY = reader.GetInt() * 0.01f;

					onEndMove(sender, pos, Vector3.up * directionY);
				}
				else if (broadcastType == EBroadcastType.MOVING)
				{
					long sender = reader.GetLong();
					
					float curX = reader.GetInt() * 0.01f;
					float curY = reader.GetInt() * 0.01f;
					float curZ = reader.GetInt() * 0.01f;

					Vector3 pos = new Vector3(curX, curY, curZ);

					float directionY = reader.GetInt() * 0.01f;

					onMoving(sender, pos, Vector3.up * directionY);
				}
				else if (broadcastType == EBroadcastType.SpawnCharacter)
				{
					long sender = reader.GetLong();
					
					float curX = reader.GetInt() * 0.01f;
					float curY = reader.GetInt() * 0.01f;
					float curZ = reader.GetInt() * 0.01f;

					Vector3 pos = new Vector3(curX, curY, curZ);

					float directionY = reader.GetInt() * 0.01f;
					onSpawnCharacter(sender, pos, Vector3.up * directionY);
				}
				else if (broadcastType == EBroadcastType.DestroyCharacter)
				{
					long sender = reader.GetLong();
					EDieReason reason = (EDieReason)reader.GetInt();

					onDestroyCharacter(sender, reason);
				}
			}
		}

		public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
		{
		}
		
		public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
		{
		}

		public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
		{
		}

		public void OnConnectionRequest(ConnectionRequest request)
		{
			request.Reject();
		}

		public void SendChat(string message)
		{
			netDataWriter.Reset();
			
			netDataWriter.Put(EPacketType.REQUEST.ToShort());
			netDataWriter.Put(ERequestType.SendChatRequest.ToInt());
			
			netDataWriter.Put(message);
			host.Send(netDataWriter, DeliveryMethod.Sequenced);
		}

		public void SendMoveRequest(Vector3 destination)
		{
			netDataWriter.Reset();
			
			netDataWriter.Put(EPacketType.REQUEST.ToShort());
			netDataWriter.Put(ERequestType.MoveToRequest.ToInt());
			
			netDataWriter.Put(processId);

			int x = (int)(destination.x * 100);
			int y = (int)(destination.y * 100);
			int z = (int)(destination.z * 100);
			
			netDataWriter.Put(x);
			netDataWriter.Put(y);
			netDataWriter.Put(z);
			
			host.Send(netDataWriter, DeliveryMethod.ReliableOrdered);
		}

		#region API

		private void OnApplicationQuit()
		{
			if (netManager != null)
			{
				netManager.DisconnectAll();
				netManager.Stop();
                
				netManager = null;
			}
		}

		private void OnDestroy()
		{
			if (netManager != null)
			{
				netManager.DisconnectAll();
				netManager.Stop();
                
				netManager = null;
			}
		}

		private void OnDisable()
		{
			if (netManager != null)
			{
				netManager.DisconnectAll();
				netManager.Stop();
                
				netManager = null;
			}
		}
		
		#endregion
	}
}