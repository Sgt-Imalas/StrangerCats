
using Assets.Scripts;
using UnityEngine;

public class ResourceDrop : Collectible
{
	public ResourceType ResourceType;
	public uint Amount = 1; 
	protected override void DoCollect()
	{
		var multiplier = PersistentPlayer.GetAttribute(AttributeType.ResourceTileMultiplier, 1f);

		int amount = Mathf.RoundToInt(multiplier * Amount);
		Global.Instance.SpaceshipResources.CollectResource(ResourceType,(uint)amount);
		base.DoCollect();
	}
}