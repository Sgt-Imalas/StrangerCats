using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class DestructibleTerrain : MonoBehaviour
{
	Tilemap tileMap;
	public float penetrationTest = -0.1f;

	void Start()
	{
		tileMap = GetComponent<Tilemap>();
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		var point = collision.GetContact(0).point;
		var normal = collision.GetContact(0).normal;

		var insetHitPosition = point - normal * penetrationTest;

		var cell = tileMap.WorldToCell(insetHitPosition);

		tileMap.SetTile(cell, null);

		tileMap.RefreshTile(cell);
		tileMap.RefreshAllTiles();
		tileMap.CompressBounds();
	}
}
