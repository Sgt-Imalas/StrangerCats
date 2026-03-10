using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveStateLoader : MonoBehaviour
{
    public List<Landable> LandableWorlds;
    public static SaveStateLoader Instance;

	private void Awake()
	{
        Instance = this;
		if(LandableWorlds == null||!LandableWorlds.Any())
        {
            LandableWorlds = GetComponentsInChildren<Landable>().ToList();
        }
	}
	private void OnDestroy()
	{
		Instance = null;
	}

	void Start()
    {
        if (Global.Instance.FlightState == Global.PlayerState.InSpace)
            return;

        if (Global.Instance.LandedPlanet < 0 || Global.Instance.LandedPlanet >= LandableWorlds.Count)
            return;

        LandableWorlds[Global.Instance.LandedPlanet].LandInstantly();

	}
}
