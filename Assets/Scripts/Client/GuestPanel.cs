using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Game.Client.UI
{
	public class GuestPanel : MonoBehaviour
	{
		public InputField m_inputField;
		public Button m_sendButton;

		public ScrollRect m_scrollView;
		public RectTransform m_scrollViewRoot;
		public ChatBox m_prefab;

		private List<ChatBox> messages = new List<ChatBox>();

		private void Start()
		{
			Guest.inst.onChatReceive += OnChatReceived;
			m_sendButton.onClick.AddListener(SendChat);
		}

		private void Update()
		{
			if (Application.isFocused)
			{
				if (Input.GetKeyDown(KeyCode.Return))
				{
					SendChat();
					m_inputField.Select();
				}
			}
		}

		private void SendChat()
		{
			string message = m_inputField.text;
			m_inputField.text = string.Empty;
			
			Guest.inst.SendChat(message);
			
			OnChatSent(message);
		}

		private void OnChatReceived(long sender, string message)
		{
			ChatBox inst = Instantiate(m_prefab, m_scrollViewRoot);
			inst.SetContent(sender, message);

			messages.Add(inst);
			m_scrollView.normalizedPosition = new Vector2(0, 1);
			LayoutRebuilder.MarkLayoutForRebuild(m_scrollViewRoot);
		}

		private void OnChatSent(string message)
		{
			ChatBox inst = Instantiate(m_prefab, m_scrollViewRoot);
			inst.SetSentMessage(message);

			messages.Add(inst);
			m_scrollView.normalizedPosition = new Vector2(0, 1);
			LayoutRebuilder.MarkLayoutForRebuild(m_scrollViewRoot);
		}
	}
}