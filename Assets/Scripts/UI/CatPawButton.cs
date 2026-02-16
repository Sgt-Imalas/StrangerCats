using UnityEngine;
using UnityEngine.UI;

public class CatPawButton : Button
{
	[SerializeField]
	public GameObject CatPawImage;

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);

		if (CatPawImage == null)
			return;

		bool setCatPawVisible = state == SelectionState.Selected || state == SelectionState.Pressed;
		//bool setCatPawPressed = state == SelectionState.Pressed;
		CatPawImage.SetActive(setCatPawVisible);		
	}
}
