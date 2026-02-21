using Assets.Scripts;
using System;
using System.Linq;
using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
	public bool CanInteract;
	public HintTextSetter InteractHint;
	protected Collider2D currentPlayer = null;
	private PlayerControls controls;
	private void Awake()
	{
		controls = new PlayerControls();
		controls.Player.Interact.performed += OnInteract;
	}

	private void OnInteract(InputAction.CallbackContext context)
	{
		if (!CanInteract)
			return;
		OnInteractPressed();
	}

	private void OnEnable()
	{
		controls.Player.Enable();
	}
	private void OnDisable()
	{
		controls.Player.Disable();
	}
	public virtual void OnInteractPressed()
	{

	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag != "Player")
			return;
		currentPlayer = null;
		OnRadiusExit(collision);
	}

	protected void ToggleInteract(bool on)
	{
		if (InteractHint == null)
			return;
		InteractHint.gameObject.SetActive(on);
	}
	public virtual void OnRadiusEnter(Collider2D collision)
	{
		ToggleInteract(true);
		CanInteract = true;
	}
	public virtual void OnRadiusExit(Collider2D collision)
	{
		ToggleInteract(false);
		CanInteract = false;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag != "Player")
			return;
		currentPlayer = collision;
		OnRadiusEnter(collision);
	}
}
