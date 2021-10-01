using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Server.UI
{
	public class DedicatePanel : MonoBehaviour
	{
		public ScrollRect m_scrollView;
		public RectTransform m_scrollViewRoot;
		public ChatBox m_prefab;

		private List<ChatBox> messages = new List<ChatBox>();
		
		private void Start()
		{
			Dedicate.inst.onChatRequest += OnChatRequest;
			Dedicate.inst.onConnected += OnConnected;

			OnChatRequest(0, Dedicate.inst.port.ToString());
		}

		private void OnConnected(long connectedUser)
		{
			ChatBox inst = Instantiate(m_prefab, m_scrollViewRoot);
			inst.SetContent(connectedUser,"Connected!");

			messages.Add(inst);
			m_scrollView.normalizedPosition = new Vector2(0, 1);
			LayoutRebuilder.MarkLayoutForRebuild(m_scrollViewRoot);
		}
		
		private void OnChatRequest(long sender, string message)
		{
			ChatBox inst = Instantiate(m_prefab, m_scrollViewRoot);
			inst.SetContent(sender, message);

			messages.Add(inst);
			m_scrollView.normalizedPosition = new Vector2(0, 1);
			LayoutRebuilder.MarkLayoutForRebuild(m_scrollViewRoot);
		}
	}
}