using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Client.UI
{
	public class UIController : MonoBehaviour
	{
		public CharacterUI prefab;
		
		private Dictionary<long, CharacterUI> map = new Dictionary<long, CharacterUI>();

		private Camera cam;
		private CanvasScaler scaler;

		private void Awake()
		{
			cam = Camera.main;
			scaler = GetComponent<CanvasScaler>();
			
			Character.onSpawn += OnCreate;
			Character.onDestroy += OnRemove;
		}

		private void OnCreate(Character character)
		{
			CharacterUI instance = Instantiate(prefab, transform);
			instance.Init(character);

			Vector3 screenPos = cam.WorldToScreenPoint(instance.GetPosition());
			((RectTransform)(instance.transform)).anchoredPosition = screenPos;

			map[character.Id] = instance;
		}

		private void OnRemove(Character character)
		{
			if (map.TryGetValue(character.Id, out var ui))
			{
				Destroy(ui.gameObject);
				map.Remove(character.Id);
			}
		}

		private void Update()
		{
			var ratioX = scaler.referenceResolution.x / Screen.width;
			var ratioY = scaler.referenceResolution.y / Screen.height;
			
			foreach (CharacterUI ui in map.Values)
			{
				Vector3 screenPos = cam.WorldToScreenPoint(ui.GetPosition());
				((RectTransform)ui.transform).anchoredPosition = screenPos * new Vector2(ratioX, ratioY);
			}
		}
	}
}