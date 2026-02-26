using UnityEngine;

namespace Assets.Scripts.Rendering
{
	public class FixedBG : MonoBehaviour
	{
		public Transform cam;
		private Vector3 _offset;

		void Start()
		{
			_offset = transform.position - cam.position;
		}

		void LateUpdate()
		{
			transform.position = cam.position + _offset;
		}
	}
}
