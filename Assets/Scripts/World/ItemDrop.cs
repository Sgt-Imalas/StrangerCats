
using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : Collectible
{
	public FindableItem item;
	protected override void DoCollect()
	{
		Global.Instance.Upgrades.CollectFindableItem(item);
		base.DoCollect();
	}
}