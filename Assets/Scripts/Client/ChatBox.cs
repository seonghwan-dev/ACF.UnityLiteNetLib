using UnityEngine;
using UnityEngine.UI;

namespace Game.Client.UI
{
	public class ChatBox : MonoBehaviour
	{
		public Text m_senderText;
		public Text m_messageText;

		public void SetContent(long sender, string message)
		{
			m_senderText.text = $" [User{sender}] : ";
			m_messageText.text = message;
			m_senderText.color = Color.black;
			
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
		}

		public void SetSentMessage(string message)
		{
			m_senderText.text = " [Me] : ";
			m_messageText.text = message;
			m_senderText.color = Color.green;
			
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
		}
	}
}