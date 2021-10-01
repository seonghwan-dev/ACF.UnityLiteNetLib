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

namespace Game.Client
{
	using Shared.CommandLine;
	using Shared.Enums;

	public class Guest : MonoBehaviour, INetEventListener
	{
		public static Guest inst;
		
		private NetManager netManager = default;
		private NetDataWriter netDataWriter = new NetDataWriter();

		private NetPeer host;
		
		public event Action<long, string> onChatReceive = delegate(long sender, string s) {  };
        
		private void Awake()
		{
			inst = this;
			
			int serverPort = CommandLineParser.GetInt("serverPort", 39999);
			int port = CommandLineParser.GetInt("port", 39999);
			
			string address = CommandLineParser.GetString("host");
            
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
			
			// netManager.Start(port);
			// host = netManager.Connect(address, serverPort, "Guest");

			host = netManager.Connect(new IPEndPoint(IPAddress.Parse("192.168.0.22"), serverPort), "Guest");
		}

		private void Update()
		{
			netManager.PollEvents();
		}

		public void OnPeerConnected(NetPeer peer)
		{
			// long processId = Process.GetCurrentProcess().Id;
			long processId = Random.Range(200, 2340);
			
			netDataWriter.Reset();
			netDataWriter.Put(EPacketType.HANDSHAKE.ToShort());
			netDataWriter.Put(processId);

			host.Send(netDataWriter, DeliveryMethod.Sequenced);
		}

		public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			Debug.Log("Disconnected");
		}

		public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
		{
			
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
			}
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
	}
}