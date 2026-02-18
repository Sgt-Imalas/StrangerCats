using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RadarController : MonoBehaviour
{
    static RadarController Instance;
    Dictionary<GameObject, POI_Pointer> TrackedTargets = new();
    [SerializeField] GameObject PointerPrefab;


	private void Awake()
	{
        if(Instance != null)
        {
            Destroy(Instance);
		}
		Instance = this;
	}

    public void AddPointer(GameObject target)
    {
		if (target.TryGetComponent<POI_Pointer>(out var pointer))
		{

			//TrackedTargets.Add(pointer);
		}
	}
    public void RemovePointer(GameObject target)
    {
        if(target.TryGetComponent<POI_Pointer>(out var pointer))
        {

			//TrackedTargets.Remove(pointer);
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
