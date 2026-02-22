using System;
using UnityEngine;

public class PortalIgniter : MonoBehaviour
{
	public AudioClip RekindlePortal;
	public GameObject PortalLight;
    void Start()
    {
		if(!Global.Instance.Upgrades.MeatWorldItemFound || !Global.Instance.Upgrades.TennisWorldItemFound || !Global.Instance.Upgrades.DesertWorldItemFound)
		{
			Debug.Log("Started listening to item collection for warp portal opening");
			InvokeRepeating("TryRekindlePortal", 5, 5f);
		}
	}

	private void TryRekindlePortal()
	{
		if(Global.Instance.Upgrades.MeatWorldItemFound && Global.Instance.Upgrades.TennisWorldItemFound && Global.Instance.Upgrades.DesertWorldItemFound)
		{
			if (RekindlePortal)
				MusicManager.PlayFx(RekindlePortal);

			PortalLight.SetActive(true);

			Destroy(gameObject);
		}
	}
}
