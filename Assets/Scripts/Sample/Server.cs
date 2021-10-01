// using System.Collections;
// using System.Collections.Generic;
// using LiteNetLib;
// using UnityEngine;
//
// public class Server : MonoBehaviour {
// 	
// 	NetManager netManager;
// 	NetListener netListener;
//
// 	// Start is called before the first frame update
// 	void Start() {
// 		netListener = new EventBasedNetListener();
//
// 		netListener.ConnectionRequestEvent += (request) => {
// 			request.Accept();
// 		};
//
// 		netListener.PeerConnectedEvent += (client) => {
// 			Debug.LogError($"Client connected: {client}");
// 		};
//
// 		netManager = new NetManager(netListener);
// 	}
//
// 	// Update is called once per frame
// 	void Update() {
// 		netManager.PollEvents();
// 	}
// }