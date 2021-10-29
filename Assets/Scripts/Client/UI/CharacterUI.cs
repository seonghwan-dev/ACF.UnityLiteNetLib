using UnityEngine;
using UnityEngine.UI;

namespace Game.Client.UI
{
	public class CharacterUI : MonoBehaviour
	{
		public Text text;
		private Character character;
		
		public void Init(Character character)
		{
			this.character = character;
			
			this.text.text = character.Id.ToString();

			bool isMe = (Guest.inst.processId == character.Id);
			Color color = isMe ? Color.green : Color.red;

			text.color = color;
		}

		public Vector3 GetPosition()
		{
			return character.transform.position + Vector3.up;
		}
	}
}