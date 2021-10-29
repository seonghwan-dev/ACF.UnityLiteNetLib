using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace Game.Server
{
    using Calci.CommandLine;
    using Shared.Enums;
    
    public class Dedicate : MonoBehaviour, INetEventListener
    {
        public static Dedicate inst;
        
        private NetManager netManager = default;
        private NetDataWriter netDataWriter = new NetDataWriter();
        
        private readonly Dictionary<int, NetPeer> peerConnections = new Dictionary<int, NetPeer>();
        
        private readonly Dictionary<long, int> userPeerMap = new Dictionary<long, int>();
        private readonly Dictionary<int, long> peerUserMap = new Dictionary<int, long>();
        
        public event Action<long, string> onChatRequest = delegate(long sender, string s) {  };
        
        public event Action<long> onHandshake = delegate(long l) {  };
        public event Action<long> onConnectionLost = delegate(long l) {  };
        
        public event Action<long> onSpawnCharacter = delegate(long l) {  }; 
        public event Action<long, Vector3> onCharacterMoveRequest = delegate(long sender, Vector3 pos) {  }; 

        public int port;

        private void Awake()
        {
            inst = this;
            
            Debug.Log(NetUtils.GetLocalIp(LocalAddrType.IPv4));
            Debug.Log(NetUtils.GetLocalIp(LocalAddrType.IPv6));
            
            port = CommandLineParser.GetInt("serverPort", 39999);
            
            netManager = new NetManager(this)
            {
            };

            netManager.Start(port);
        }
        
        private void Update()
        {
            netManager.PollEvents();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            peerConnections.Add(peer.Id, peer);
            
            Debug.Log($"New Peer : {peer.Id}");
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log($"Outdated Peer : {peer.Id}");
            
            peerConnections.Remove(peer.Id);

            long userId = peerUserMap[peer.Id];

            peerUserMap.Remove(peer.Id);
            userPeerMap.Remove(userId);

            onConnectionLost(userId);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            EPacketType packetType = (EPacketType)reader.GetShort();

            if (packetType == EPacketType.HANDSHAKE)
            {
                long clientId = reader.GetLong();

                userPeerMap[clientId] = peer.Id;
                peerUserMap[peer.Id] = clientId;
                
                onHandshake(clientId);
                
                StartCoroutine(LazySpawn(clientId));
            }
            else if (packetType == EPacketType.REQUEST)
            {
                ERequestType requestType = (ERequestType)reader.GetInt();
                if (requestType == ERequestType.SendChatRequest)
                {
                    string chatData = reader.GetString();
                    
                    onChatRequest(peerUserMap[peer.Id], chatData);
                    
                    // broadcast
                    netDataWriter.Reset();
                    
                    // packet type define
                    netDataWriter.Put(EPacketType.BROADCAST.ToShort());
                    netDataWriter.Put(EBroadcastType.ChatBroadcast.ToInt());
                    
                    netDataWriter.Put(peerUserMap[peer.Id]);
                    netDataWriter.Put(chatData);
                    
                    BroadcastEx(peer.Id);
                }
                else if (requestType == ERequestType.MoveToRequest)
                {
                    long pid = reader.GetLong();

                    float x = reader.GetInt() * 0.01f;
                    float y = reader.GetInt() * 0.01f;
                    float z = reader.GetInt() * 0.01f;

                    onCharacterMoveRequest(pid, new Vector3(x, y, z));
                }
            }
        }

        /// <summary>
        /// 캐릭터 스폰 (접속 전 스폰된 캐릭터 동기화)
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="character"></param>
        public void SendSpawnCharacter(long receiver, Character character)
        {
            if (userPeerMap.TryGetValue(receiver, out var peerId))
            {
                if (peerConnections.TryGetValue(peerId, out var peer))
                {
                    netDataWriter.Reset();
                    
                    // packet type define
                    netDataWriter.Put(EPacketType.BROADCAST.ToShort());
                    netDataWriter.Put(EBroadcastType.SpawnCharacter.ToInt());
            
                    netDataWriter.Put(character.Id);

                    netDataWriter.Put((int)(character.transform.position.x * 100));
                    netDataWriter.Put((int)(character.transform.position.y * 100));
                    netDataWriter.Put((int)(character.transform.position.z * 100));
            
                    netDataWriter.Put((int)(character.transform.rotation.eulerAngles.y * 100));
                    
                    peer.Send(netDataWriter, DeliveryMethod.Sequenced);
                }
            }
        }
        
        /// <summary>
        /// 캐릭터 생성 브로드캐스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pos"></param>
        /// <param name="direction"></param>
        public void BroadcastSpawnCharacter(long sender, Vector3 pos, Vector3 direction)
        {
            // broadcast
            netDataWriter.Reset();
                    
            // packet type define
            netDataWriter.Put(EPacketType.BROADCAST.ToShort());
            netDataWriter.Put(EBroadcastType.SpawnCharacter.ToInt());
            
            netDataWriter.Put(sender);

            netDataWriter.Put((int)(pos.x * 100));
            netDataWriter.Put((int)(pos.y * 100));
            netDataWriter.Put((int)(pos.z * 100));
            
            netDataWriter.Put((int)(direction.y * 100));
            
            Broadcast();
        }

        /// <summary>
        /// 캐릭터 파괴 브로드캐스트
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="reason"></param>
        public void BroadcastDestroyCharacter(long owner, EDieReason reason)
        {
            netDataWriter.Reset();
                    
            // packet type define
            netDataWriter.Put(EPacketType.BROADCAST.ToShort());
            netDataWriter.Put(EBroadcastType.DestroyCharacter.ToInt());
            
            netDataWriter.Put(owner);
            netDataWriter.Put((int)reason);
            
            Broadcast();
        }

        public void BroadcastBeginMove(long owner, Vector3 pos, float direction)
        {
            netDataWriter.Reset();
                    
            // packet type define
            netDataWriter.Put(EPacketType.BROADCAST.ToShort());
            netDataWriter.Put(EBroadcastType.BEGIN_MOVE.ToInt());
            
            netDataWriter.Put(owner);
            
            netDataWriter.Put((int)(pos.x * 100));
            netDataWriter.Put((int)(pos.y * 100));
            netDataWriter.Put((int)(pos.z * 100));
            
            netDataWriter.Put((int)(direction * 100));
            
            Broadcast();
        }

        public void BroadcastEndMove(long owner, Vector3 pos, float direction)
        {
            netDataWriter.Reset();
                    
            // packet type define
            netDataWriter.Put(EPacketType.BROADCAST.ToShort());
            netDataWriter.Put(EBroadcastType.END_MOVE.ToInt());
            
            netDataWriter.Put(owner);
            
            netDataWriter.Put((int)(pos.x * 100));
            netDataWriter.Put((int)(pos.y * 100));
            netDataWriter.Put((int)(pos.z * 100));
            
            netDataWriter.Put((int)(direction * 100));
            
            Broadcast();
        }

        public void BroadcastMoving(long owner, Vector3 pos, float direction)
        {
            netDataWriter.Reset();
                    
            // packet type define
            netDataWriter.Put(EPacketType.BROADCAST.ToShort());
            netDataWriter.Put(EBroadcastType.MOVING.ToInt());
            
            netDataWriter.Put(owner);
            
            netDataWriter.Put((int)(pos.x * 100));
            netDataWriter.Put((int)(pos.y * 100));
            netDataWriter.Put((int)(pos.z * 100));
            
            netDataWriter.Put((int)(direction * 100));
            
            Broadcast();
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            Debug.Log("Requested");
            
            request.AcceptIfKey("Guest");
        }

        private void BroadcastEx(int excludePeerId)
        {
            foreach (int peerId in peerConnections.Keys)
            {
                if (peerId == excludePeerId) continue;

                NetPeer peer = peerConnections[peerId];
                peer.Send(netDataWriter, DeliveryMethod.Sequenced);
            }
        }

        private void Broadcast()
        {
            foreach (int peerId in peerConnections.Keys)
            {
                NetPeer peer = peerConnections[peerId];
                peer.Send(netDataWriter, DeliveryMethod.Sequenced);
            }
        }

        private IEnumerator LazySpawn(long sender)
        {
            yield return new WaitForSeconds(1.0f);
            onSpawnCharacter(sender);
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
