using UnityEngine;

public class DialogueInteractable : Interactable
{
	[SerializeField] private PawtalDialogue Dialogue;
	public override void OnRadiusEnter(Collider2D collision)
	{
		base.OnRadiusEnter(collision);

		InteractHint.SetText("Call On Radio");
	}
	public override void OnInteractPressed()
	{
		base.OnInteractPressed();
		Dialogue.ShowPawtalDialogue();
	}
}