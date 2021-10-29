using System.Collections.Generic;
using Game.Shared.Enums;
using UnityEngine;

namespace Game.Client
{
	public class Spawner : MonoBehaviour
	{
		public GameObject prefab;

		private readonly Dictionary<long, Character> characters = new Dictionary<long, Character>();

		private void Start()
		{
			Guest.inst.onSpawnCharacter += OnSpawnCharacter;
			Guest.inst.onDestroyCharacter += OnDestroyCharacter;
			
			Guest.inst.onBeginMove += OnBeginMove;
			Guest.inst.onEndMove += OnEndMove;
			Guest.inst.onMoving += OnMoving;
		}

		private void OnMoving(long arg1, Vector3 arg2, Vector3 arg3)
		{
			if (characters.TryGetValue(arg1, out var character))
			{
				character.Moving(arg2, arg3);
			}
		}

		private void OnEndMove(long arg1, Vector3 arg2, Vector3 arg3)
		{
			if (characters.TryGetValue(arg1, out var character))
			{
				character.EndMove(arg2, arg3);
			}
		}

		private void OnBeginMove(long arg1, Vector3 arg2, Vector3 arg3)
		{
			if (characters.TryGetValue(arg1, out var character))
			{
				character.BeginMove(arg2, arg3);
			}
		}

		private void OnSpawnCharacter(long sender, Vector3 pos, Vector3 direction)
		{
			GameObject instance = Instantiate(prefab, pos, Quaternion.Euler(direction));
			Character character;
			if (sender == Guest.inst.processId)
			{
				character = instance.AddComponent<MyCharacter>();
			}
			else
			{
				character = instance.AddComponent<Character>();
			}
			 
			characters[sender] = character;
			character.Init(sender);
		}

		private void OnDestroyCharacter(long sender, EDieReason reason)
		{
			if (characters.TryGetValue(sender, out var character))
			{
				character.Clean(reason);
				characters.Remove(sender);
			}
		}
	}
}