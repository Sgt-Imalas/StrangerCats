using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(CircleCollider2D))]
	public class PlayerTracker : MonoBehaviour
	{
		public bool isInRange;

		private CircleCollider2D detectionCollider;

		void Awake()
		{
			detectionCollider = GetComponent<CircleCollider2D>();
		}


		void OnCollisionEnter2D(Collision2D collision)
		{
			isInRange = true;
		}

		void OnCollisionExit2D(Collision2D collision)
		{
			isInRange = false;
		}
	}
}
