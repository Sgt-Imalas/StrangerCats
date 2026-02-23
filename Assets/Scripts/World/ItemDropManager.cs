using Assets.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemDropManager : MonoBehaviour
{
	public Tilemap TileMap;
	public ConfigurableItem Res_Drop_Meat, Res_Drop_Rust, Res_Drop_Ball, Res_Drop_Dust;
	public ConfigurableItem Drop_Meat_Item, Drop_Radar_Item, Drop_Ball_Item, Drop_Comb_Item;
	public GameObject Player;

	private void Start()
	{
		GlobalEvents.Instance.OnTileDestroyed += HandleTileDestroyed;
		GlobalEvents.Instance.DropProgressionItem += HandleCollectibleContainerDestroyed;
		GlobalEvents.Instance.OnEnemyKilled += HandleEnemyKilled;
	}

	private void OnDestroy()
	{
		GlobalEvents.Instance.OnTileDestroyed -= HandleTileDestroyed;
		GlobalEvents.Instance.DropProgressionItem -= HandleCollectibleContainerDestroyed;
		GlobalEvents.Instance.OnEnemyKilled -= HandleEnemyKilled;
	}

	private void HandleEnemyKilled(Vector3 pos, ResourceType type)
	{
		ConfigurableItem prefab = null;
		switch (type)
		{
			case ResourceType.Meat:
				prefab = Res_Drop_Meat;
				break;
			case ResourceType.Rust:
				prefab = Res_Drop_Rust;
				break;
			case ResourceType.Ball:
				prefab = Res_Drop_Ball;
				break;
			case ResourceType.Dust:
				prefab = Res_Drop_Dust;
				break;
		}

		if (prefab == null)
		{
			Debug.LogWarning("invalid drop type: " + type);
			return;
		}

		var itemDrop = Instantiate(prefab);
		itemDrop.Amount = 1;
		itemDrop.transform.position = pos;
		itemDrop.FlyTowards(Player);
		itemDrop.gameObject.SetActive(true);
		
	}


	private void HandleTileDestroyed(Vector3Int pos, int matIdx)
	{
		var mat = Materials.GetMaterial(matIdx);
		if (mat == null)
		{
			Debug.LogWarning("Could not find material with type " + matIdx);
		}
		ConfigurableItem prefab = null;
		switch (mat.resourceType)
		{
			case ResourceType.Meat:
				prefab = Res_Drop_Meat;
				break;
			case ResourceType.Rust:
				prefab = Res_Drop_Rust;
				break;
			case ResourceType.Ball:
				prefab = Res_Drop_Ball;
				break;
			case ResourceType.Dust:
				prefab = Res_Drop_Dust;
				break;
		}

		if (prefab == null)
		{
			Debug.LogWarning("invalid drop type: " + mat.resourceType);
			return;
		}

		var itemDrop = Instantiate(prefab);
		itemDrop.Amount = (uint)Mathf.Max(mat.resourceAmount, 1);
		itemDrop.transform.position = TileMap.CellToWorld(pos) + new Vector3(0.5f, 0.5f);
		itemDrop.gameObject.SetActive(true);
	}
	private void HandleCollectibleContainerDestroyed(Vector3 pos, FindableItem item)
	{
		ConfigurableItem prefab = null;
		switch (item)
		{
			case FindableItem.Meat:
				prefab = Drop_Meat_Item;
				break;
			case FindableItem.Radar:
				prefab = Drop_Radar_Item;
				break;
			case FindableItem.Tennis:
				prefab = Drop_Ball_Item;
				break;
			case FindableItem.Desert:
				prefab = Drop_Comb_Item;
				break;

		}

		if (prefab == null)
		{
			Debug.LogWarning("invalid item type: " + item);
			return;
		}

		var itemDrop = Instantiate(prefab);
		itemDrop.gameObject.SetActive(true);
		itemDrop.transform.position = pos;		

	}
}