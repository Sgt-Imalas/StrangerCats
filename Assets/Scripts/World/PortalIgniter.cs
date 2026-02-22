using System;
using UnityEngine;

public class PortalIgniter : MonoBehaviour
{
	public AudioClip RekindlePortal;
	public GameObject PortalLight;
    void Start()
    {
		if (!Global.Instance.Upgrades.MeatWorldItemFound || !Global.Instance.Upgrades.TennisWorldItemFound || !Global.Instance.Upgrades.DesertWorldItemFound)
		{
			Debug.Log("Started listening to item collection for warp portal opening");
			InvokeRepeating(nameof(TryRekindlePortal), 1, 1f);
		}		
		TryRekindlePortal();
	}

	private void TryRekindlePortal()
	{
		if(Global.Instance.Upgrades.MeatWorldItemFound && Global.Instance.Upgrades.TennisWorldItemFound && Global.Instance.Upgrades.DesertWorldItemFound)
		{
			if (RekindlePortal && !Global.Instance.PortalOpened)
			{
				MusicManager.PlayFx(RekindlePortal);
				Global.Instance.PortalOpened = true;
			}
			PortalLight.SetActive(true);
		}
	}
}
