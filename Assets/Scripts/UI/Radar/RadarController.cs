using Assets.Scripts;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RadarController : MonoBehaviour
{
    static RadarController Instance;
    public Dictionary<GameObject, RadarPointer> TrackedTargets = new();
    [SerializeField] GameObject PointerPrefab;
    [SerializeField] GameObject PlayerGO;

	public float MaximumRadarRange = 100;

	private void Awake()
	{
        if (Instance != null)
        {
            Destroy(Instance);
		}
		Instance = this;
	}
	void Start()
	{
		//PersistentPlayer.Instance.attributes.OnAttributeChanged += OnPlayerAttributeChanged;
		//MaximumRadarRange = PersistentPlayer.GetAttribute(AttributeType.RadarRange);

		InvokeRepeating("RefreshRadarTargetsVisibility", 0, 0.2f);
	}

	void RefreshRadarTargetsVisibility()
	{
		foreach (var target in TrackedTargets)
		{
			if(target.Key == null)
			{
				Destroy(target.Value.gameObject);
				TrackedTargets.Remove(target.Key);
				continue;
			}
			var distance = Vector3.Distance(PlayerGO.transform.position, target.Key.transform.position);
			target.Value.gameObject.SetActive(distance <= MaximumRadarRange);
		}
	}

	private void OnPlayerAttributeChanged(AttributeType attributeId, float value)
	{
		switch (attributeId)
		{
			case AttributeType.RadarRange:
				MaximumRadarRange = PersistentPlayer.GetAttribute(AttributeType.RadarRange);
				break;
		}
	}

	public static void AddPointer(GameObject target, Color? optionalTint) => Instance?.AddPointerInternal(target, optionalTint);
	void AddPointerInternal(GameObject target, Color? optionalTint)
    {
        if(PointerPrefab == null)
           Debug.LogError("Pointer prefab is not set on RadarController??");

		var pointerGO = Instantiate(PointerPrefab, transform);
        pointerGO.SetActive(true);
        if(pointerGO.TryGetComponent<RadarPointer>(out var pointer))
		{
			pointer.Target = target;
			pointer.Player = PlayerGO;
			if (optionalTint.HasValue)
			{
				pointer.Tint = optionalTint;
			}

			TrackedTargets.Add(target, pointer);
		}
        else
            Debug.LogError("Pointer prefab does not have a POI_Pointer component??");
		//Debug.Log($"Added pointer for {target.name}");
	}


    public static void RemovePointer(GameObject target) => Instance?.RemovePointerÌnternal(target);
	void RemovePointerÌnternal(GameObject target)
    {
        if(TrackedTargets.TryGetValue(target, out var pointer))
        {
			if(pointer != null)
				Destroy(pointer.gameObject);
            TrackedTargets.Remove(target);
		}
	}
}
