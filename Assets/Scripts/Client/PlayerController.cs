using System;
using UnityEngine;

namespace Game.Client
{
	public class PlayerController : MonoBehaviour
	{
		private Camera cam;
		public LayerMask groundLayer;

		private void Awake()
		{
			cam = Camera.main;
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(1))
			{
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, groundLayer))
				{
					Debug.Log("Hit");

					Vector3 pos = hitInfo.point;
					Guest.inst.SendMoveRequest(pos);
				}
				else
				{
					Debug.Log("Missed");
				}
			}

			if (Input.GetMouseButton(1))
			{
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, groundLayer))
				{
					Debug.DrawRay(cam.transform.position, hitInfo.point - cam.transform.position, Color.red);
				}
			}

			// var hits = new RaycastHit[10];
			// int hitCount = Physics.RaycastNonAlloc(ray, hits, 100f, groundLayer, QueryTriggerInteraction.Collide);
			//
			// foreach (RaycastHit hit in hits)
			// {
			// 	
			// }

		}
	}
}