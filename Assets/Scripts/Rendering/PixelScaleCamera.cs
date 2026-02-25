using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Rendering
{
	[RequireComponent(typeof(Camera))]
	public class PixelScaleCamera : MonoBehaviour
	{
		public Vector2 aspectRatio;
		public float zoom = 12.0f;
		public float zoomSpeed = 2.0f;
		public float zoomSmoothing = 0.8f;
		public float minZoom = 7.0f, maxZoom = 40.0f;

		private float _ratio;
		private Camera _camera;

		private float _previousWidth, _previousHeight;
		public float _targetZoom;
		private PlayerControls controls;

		void Awake()
		{
			controls = new();
			controls.Enable();

			controls.Player.Zoom.performed += OnZoom;
		}

		private void OnZoom(InputAction.CallbackContext context)
		{
			var value = context.ReadValue<Vector2>();
			_targetZoom = zoom - (value.y * zoomSpeed);
			_targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
			if (_camera)
				_camera.orthographicSize = zoom;
		}

		void Start()
		{
			_camera = GetComponent<Camera>();
			UpdateScale();
		}

		private void Update()
		{
			if (_targetZoom != zoom)
			{
				zoom = Mathf.MoveTowards(zoom, _targetZoom, zoomSmoothing * Time.deltaTime);
				_camera.orthographicSize = zoom;
			}

			if (_previousWidth != Screen.width || _previousHeight != Screen.height)
				UpdateScale();
		}

		void UpdateScale()
		{
			_ratio = aspectRatio.x / aspectRatio.y;

			var currentRatio = Screen.width / (float)Screen.height;

			if (currentRatio >= _ratio)
				_camera.orthographicSize = zoom;
			else
			{
				var difference = _ratio / currentRatio;
				_camera.orthographicSize = zoom * difference;
			}

			_previousWidth = Screen.width;
			_previousHeight = Screen.height;
		}
	}
}
