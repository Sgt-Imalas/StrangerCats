using Assets.Scripts;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class HangarArea : PlayerDetection
{
	public bool Landed => currentPlayer != null;
	public MothershipAnimator mothership;
	protected override void OnTriggerEnter2D(Collider2D collision)
	{
		base.OnTriggerEnter2D(collision);
		TogglePlayerInside();
	}
	protected override void OnTriggerExit2D(Collider2D collision)
	{
		base.OnTriggerExit2D(collision);
		TogglePlayerInside();
	}
	void TogglePlayerInside()
	{
		PersistentPlayer.Instance.InHangar = Landed;
	}

}