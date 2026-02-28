public class ItemDrop : Collectible
{
	public FindableItem item = FindableItem.None;

	protected override void DoCollect()
	{
		Global.Instance.Upgrades.CollectFindableItem(item);
		base.DoCollect();
		Destroy(gameObject);
	}
}