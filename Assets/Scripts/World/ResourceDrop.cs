
using Assets.Scripts;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;

public class ResourceDrop : Collectible
{
	public ResourceType ResourceType;
	public uint Amount = 1; 
	protected override void DoCollect()
	{
		Global.Instance.SpaceshipResources.CollectResource(ResourceType, Amount);
		base.DoCollect();
	}

}