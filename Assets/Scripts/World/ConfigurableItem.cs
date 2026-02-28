using UnityEngine;

public class ConfigurableItem : MonoBehaviour
{
	public FlyTowardsPlayer flyTowards;
	public Sprite TargetSprite;
	SpriteRenderer spriteRenderer;
	public ResourceType resourceType = ResourceType.None;
	public FindableItem item = FindableItem.None;
	public uint Amount = 1;

	public ItemDrop itemCfg;
	public ResourceDrop resourceCfg;

	private void Start()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		spriteRenderer.sprite = TargetSprite;

		if (item != FindableItem.None && itemCfg != null)
		{
			itemCfg.item = item;
			if (resourceCfg != null)
				resourceCfg.TurnOff();
		}
		else if (resourceType != ResourceType.None && resourceCfg != null)
		{
			resourceCfg.ResourceType = resourceType;
			resourceCfg.Amount = Amount;
			if (itemCfg != null)
				itemCfg.TurnOff();
		}
	}

	internal void FlyTowards(GameObject player)
	{
		if (flyTowards != null)
			flyTowards.currentPlayer = player;
	}
}