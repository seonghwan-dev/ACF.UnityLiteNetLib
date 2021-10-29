using UnityEngine;
using UnityEngine.AI;

namespace Game.Server
{
	public class Character : MonoBehaviour
	{
		private NavMeshAgent agent;
		
		public long Id { get; protected set; }

		private const float tick = 0.1f;
		private float lastTick;

		private bool isMoving;
		
		public void Init(long owner)
		{
			this.Id = owner;
			agent = GetComponent<NavMeshAgent>();
		}

		public void Clean()
		{
			gameObject.SetActive(false);
			Destroy(gameObject, 1.0f);
		}

		public void MoveTo(Vector3 destination)
		{
			agent.SetDestination(destination);
			isMoving = true;

			var pos = transform.position;
			pos.y = destination.y;
			
			Dedicate.inst.BroadcastBeginMove(Id, pos, transform.rotation.eulerAngles.y);
		}

		private void Update()
		{
			// sync position
			if (isMoving)
			{
				if (!agent.pathPending)
				{
					if (agent.remainingDistance <= agent.stoppingDistance)
					{
						if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
						{
							// stopped!

							isMoving = false;
							Dedicate.inst.BroadcastEndMove(Id, agent.destination, transform.rotation.eulerAngles.y);
						}
					}
				}

				// if (Time.realtimeSinceStartup - lastTick > tick)
				{
					lastTick = Time.realtimeSinceStartup;
					
					var pos = transform.position;
					pos.y = agent.destination.y;
					
					Dedicate.inst.BroadcastMoving(Id, pos, transform.rotation.eulerAngles.y);
				}
			}
		}
	}
}