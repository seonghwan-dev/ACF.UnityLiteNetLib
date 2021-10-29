using System;
using Game.Shared.Enums;
using UnityEngine;

namespace Game.Client
{
	public class Character : MonoBehaviour
	{
		public static event Action<Character> onSpawn = delegate(Character character) {  }; 
		public static event Action<Character> onDestroy = delegate(Character character) {  };

		private Vector3 startPos;
		private Vector3 endPos;
		private Vector3 currentPos;

		private Vector3 estimatedDirection;

		private bool isMoving;

		private long ownerId;
		public long Id {
			get => ownerId;
		}

		public void Init(long sender)
		{
			ownerId = sender;
			
			onSpawn(this);
		}

		public void Clean(EDieReason reason)
		{
			onDestroy(this);
			
			gameObject.SetActive(false);
			Destroy(gameObject, 1.0f);
		}

		public void BeginMove(Vector3 pos, Vector3 direction)
		{
			isMoving = true;
			
			this.startPos = pos;
			this.estimatedDirection = direction;
			
			SetPosition(pos);
			
			Debug.Log("Begin Move");
		}

		public void EndMove(Vector3 pos, Vector3 direction)
		{
			isMoving = false;
			this.endPos = pos;
			this.estimatedDirection = direction;
			
			SetPosition(pos);
			currentPos = endPos;
			
			Debug.Log("End Move");
		}

		public void Moving(Vector3 pos, Vector3 arg3)
		{
			currentPos = pos;
			estimatedDirection = arg3;
		}

		private void SetPosition(Vector3 position)
		{
			position.y = 1;
			transform.position = position;
		}

		private void Update()
		{
			if (isMoving)
			{
				SetPosition(Vector3.Lerp(transform.position, currentPos, 0.5f));
			}
			else
			{
				// transform.position = Vector3.Lerp(transform.position, endPos, 0.5f);
			}
			
			if (estimatedDirection != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(estimatedDirection),
					3.0f * Time.deltaTime);
				
				transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
			}
		}
	}
}