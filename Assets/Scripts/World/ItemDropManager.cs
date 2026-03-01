using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;

public class ItemDropManager : MonoBehaviour
{
	public Tilemap TileMap;
	public ConfigurableItem Res_Drop_Meat, Res_Drop_Rust, Res_Drop_Ball, Res_Drop_Dust;
	public ConfigurableItem Drop_Meat_Item, Drop_Radar_Item, Drop_Ball_Item, Drop_Comb_Item;
	public GameObject Player;

	public Dictionary<ResourceType, ObjectPool<ConfigurableItem>> itemsPools;

	private void Start()
	{
		GlobalEvents.Instance.OnTileDestroyed += HandleTileDestroyed;
		GlobalEvents.Instance.DropProgressionItem += HandleCollectibleContainerDestroyed;
		GlobalEvents.Instance.OnEnemyKilled += HandleEnemyKilled;
		GlobalEvents.Instance.CollectedResource += OnResourceCollected;

		itemsPools = new Dictionary<ResourceType, ObjectPool<ConfigurableItem>>
		{
			[ResourceType.Dust] = CreatePool(Res_Drop_Dust),
			[ResourceType.Meat] = CreatePool(Res_Drop_Meat),
			[ResourceType.Rust] = CreatePool(Res_Drop_Rust),
			[ResourceType.Ball] = CreatePool(Res_Drop_Ball),
		};
	}
	
	private void OnResourceCollected(ResourceDrop collectible)
	{
		if (itemsPools.TryGetValue(collectible.ResourceType, out var pool))
		{
			pool.Release(collectible.GetComponent<ConfigurableItem>());
		}
	}

	private ObjectPool<ConfigurableItem> CreatePool(ConfigurableItem prefab)
	{
		return new ObjectPool<ConfigurableItem>(
			() => CreateItem(prefab),
			OnGet,
			OnRelease,
			OnDestroyItem,
			true,
			16,
			10000);
	}

	private ConfigurableItem CreateItem(ConfigurableItem prefab)
	{
		var itemDrop = Instantiate(prefab);
		itemDrop.FlyTowards(Player);
		itemDrop.gameObject.SetActive(true);

		return itemDrop;
	}

	private void OnGet(ConfigurableItem item)
	{
		item.itemCfg.TurnOn();
		item.gameObject.SetActive(true);
	}

	private void OnRelease(ConfigurableItem item)
	{
		item.itemCfg.TurnOff();
		item.gameObject.SetActive(false);
	}

	private void OnDestroyItem(ConfigurableItem item) => Destroy(item.gameObject);

	private void OnDestroy()
	{
		GlobalEvents.Instance.OnTileDestroyed -= HandleTileDestroyed;
		GlobalEvents.Instance.DropProgressionItem -= HandleCollectibleContainerDestroyed;
		GlobalEvents.Instance.OnEnemyKilled -= HandleEnemyKilled;
		GlobalEvents.Instance.CollectedResource -= OnResourceCollected;
	}

	private void HandleEnemyKilled(Vector3 pos, ResourceType type)
	{
		if (itemsPools.TryGetValue(type, out var pool))
		{
			var item = pool.Get();
			item.Amount = 1;
			item.transform.position = pos;
		}
		else
		{
			Debug.LogWarning("invalid drop type: " + type);
		}
	}

	private void HandleTileDestroyed(Vector3Int pos, int matIdx)
	{
		var mat = Materials.GetMaterial(matIdx);
		if (mat == null)
		{
			Debug.LogWarning("Could not find material with type " + matIdx);
		}

		if (itemsPools.TryGetValue(mat.resourceType, out var pool))
		{
			var item = pool.Get();
			item.Amount = (uint)Mathf.Max(mat.resourceAmount, 1);
			item.transform.position = TileMap.CellToWorld(pos) + new Vector3(0.5f, 0.5f);
		}
		else
		{
			Debug.LogWarning($"invalid drop type: {mat.resourceType}");
		}
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
		itemDrop.item = item;
		itemDrop.gameObject.SetActive(true);
		itemDrop.transform.position = pos;
	}
}
