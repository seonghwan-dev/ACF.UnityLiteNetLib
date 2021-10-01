using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace Game.Server
{
    using Shared.CommandLine;
    using Shared.Enums;
    
    public class Dedicate : MonoBehaviour, INetEventListener
    {
        public static Dedicate inst;
        
        private NetManager netManager = default;
        private NetDataWriter netDataWriter = new NetDataWriter();
        
        private readonly Dictionary<int, NetPeer> connections = new Dictionary<int, NetPeer>();
        
        private readonly Dictionary<long, int> userPeerMap = new Dictionary<long, int>();
        private readonly Dictionary<int, long> peerUserMap = new Dictionary<int, long>();
        
        public event Action<long, string> onChatRequest = delegate(long sender, string s) {  };
        public event Action<long> onConnected = delegate(long l) {  };

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
            connections.Add(peer.Id, peer);
            onConnected(peer.Id);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            connections.Remove(peer.Id);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Debug.Log("OnReceive");
            
            EPacketType packetType = (EPacketType)reader.GetShort();

            if (packetType == EPacketType.HANDSHAKE)
            {
                long clientId = reader.GetLong();

                userPeerMap[clientId] = peer.Id;
                peerUserMap[peer.Id] = clientId;
                
                Debug.Log("OnHandShake");
            }
            else if (packetType == EPacketType.REQUEST)
            {
                Debug.Log("OnRequest");
                
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
            Debug.Log("Requested");
            
            request.AcceptIfKey("Guest");
        }

        private void BroadcastEx(int excludePeerId)
        {
            foreach (int peerId in connections.Keys)
            {
                if (peerId == excludePeerId) continue;

                NetPeer peer = connections[peerId];
                peer.Send(netDataWriter, DeliveryMethod.Sequenced);
            }
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
