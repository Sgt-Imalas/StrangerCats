
using Assets.Scripts;
using UnityEngine;

public class ResourceDrop : Collectible
{
	public ResourceType ResourceType = ResourceType.None;
	public uint Amount = 1;
	protected override void DoCollect()
	{
		var multiplier = PersistentPlayer.GetAttribute(AttributeType.ResourceTileMultiplier, 1f);

		var amount = Mathf.RoundToInt(multiplier * Amount);
		Global.Instance.SpaceshipResources.CollectResource(ResourceType, (uint)amount);
		GlobalEvents.Instance.CollectedResource?.Invoke(this);
		base.DoCollect();
	}
}