using Assets.Scripts;
using UnityEngine;

public class DropCollectibleOnDeath : MonoBehaviour
{
	public FindableItem ToDrop = FindableItem.None;

	private void OnDestroy()
	{
		if (ToDrop != FindableItem.None)
		{
			GlobalEvents.Instance.DropProgressionItem?.Invoke(transform.position, ToDrop);
		}
	}
}