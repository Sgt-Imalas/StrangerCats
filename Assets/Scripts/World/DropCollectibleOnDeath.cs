using Assets.Scripts;
using UnityEditor.Toolbars;
using UnityEngine;

public class DropCollectibleOnDeath : MonoBehaviour
{
	public FindableItem ToDrop = FindableItem.None;

	private void OnDestroy()
	{
		if (ToDrop != FindableItem.None)
		{
			GlobalEvents.Instance.OnFindableItemRevealed?.Invoke(transform.position, ToDrop);
		}
	}
}