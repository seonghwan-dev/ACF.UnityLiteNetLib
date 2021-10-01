using UnityEngine;
using UnityEngine.UI;

namespace Game.Server.UI
{
	public class ChatBox : MonoBehaviour
	{
		public Text m_senderText;
		public Text m_messageText;

		public void SetContent(long sender, string message, Color color)
		{
			m_senderText.text = $" [User{sender}] : ";
			m_messageText.text = message;
			
			m_senderText.color = color;
			
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
		}
	}
}