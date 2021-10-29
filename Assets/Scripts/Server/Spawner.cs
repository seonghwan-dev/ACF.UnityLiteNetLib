using System.Collections.Generic;
using Game.Shared.Enums;
using UnityEngine;

namespace Game.Server
{
	public class Spawner : MonoBehaviour
	{
		public GameObject prefab;
		
		private readonly Dictionary<long, Character> characters = new Dictionary<long, Character>();

		private void Start()
		{
			Dedicate.inst.onSpawnCharacter += OnSpawnCharacter;
			Dedicate.inst.onConnectionLost += OnConnectionLost;
			Dedicate.inst.onHandshake += OnHandShake;
			Dedicate.inst.onCharacterMoveRequest += OnMoveRequest;
		}

		private void OnMoveRequest(long owner, Vector3 destination)
		{
			if (characters.TryGetValue(owner, out var character))
			{
				character.MoveTo(destination);
			}
		}

		private void OnHandShake(long owner)
		{
			foreach (Character character in characters.Values)
			{
				Dedicate.inst.SendSpawnCharacter(owner, character);
			}
		}

		private void OnConnectionLost(long owner)
		{
			if (characters.TryGetValue(owner, out var character))
			{
				character.Clean();
				characters.Remove(owner);
			}
			
			Dedicate.inst.BroadcastDestroyCharacter(owner, EDieReason.Disconnected);
		}

		private void OnSpawnCharacter(long sender)
		{
			var posX = Random.Range(-10, 10);
			var posZ = Random.Range(-10, 10);

			var pos = new Vector3(posX, 1, posZ);

			var direction = Random.Range(0, 360);
			
			var inst = Instantiate(prefab, pos, Quaternion.Euler(direction * Vector3.up));
			var character = inst.AddComponent<Character>();
			
			character.Init(sender);
			characters.Add(sender, character);

			Dedicate.inst.BroadcastSpawnCharacter(sender, pos, Vector3.up * direction);
		}
	}
}