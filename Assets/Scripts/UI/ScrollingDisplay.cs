using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ScrollingDisplay : MonoBehaviour
{
	public Vector3 motion = Vector3.zero;
	public float speedUpFactor = 10.0f;
	public float endingY = 4500.0f;
	private bool _finished;

	private void OnEnable()
	{
		_finished = false;
	}

	void Update()
	{
		if (_finished)
			return;

		var anyInput =
			(Keyboard.current != null && Keyboard.current.anyKey.isPressed) ||
			(Mouse.current != null && Mouse.current.leftButton.isPressed) ||
			(Gamepad.current != null && Gamepad.current.allControls.Any(c => c is ButtonControl b && b.isPressed));

		if (anyInput)
			transform.position += motion * speedUpFactor;
		else
			transform.position += motion;

		if (transform.position.y > endingY)
		{
			SceneLoader.Instance.LoadSceneByName("MainMenu");
			_finished = true;
		}
	}
}
