using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour
{
	public OnAttributeChangedDelegate OnAttributeChanged;

	public List<Attribute> baseValues = new();
	public Dictionary<AttributeType, float> currentValues = new();
	public List<AttributeModifier> mods = new();

	public delegate void OnAttributeChangedDelegate(AttributeType type, float finalValue);

	void Start()
	{
		UpdateMods();
		foreach (var baseValue in baseValues)
			OnAttributeChanged?.Invoke(baseValue.id, baseValue.value);
	}

	public void SetBaseValue(AttributeType attributeId, float value, float minValue, float maxValue)
	{
		var attr = GetBaseAttribute(attributeId, false);

		if (attr == null)
			baseValues.Add(new Attribute(attributeId, value, minValue, maxValue));
		else
			attr.Set(value);

		OnAttributeChanged?.Invoke(attributeId, value);
	}

	public float Get(AttributeType attribute)
	{
		return currentValues.TryGetValue(attribute, out var value) ? value : 0.0f;
	}

	public void AddMod(string modifierId, AttributeType attributeId, float value, bool isMultiplier = false)
	{
		var mod = new AttributeModifier();
		mod.id = modifierId;
		mod.value = value;
		mod.multiplier = isMultiplier;
		mod.attributeId = attributeId;

		AddMod(mod);
	}

	private void AddMod(AttributeModifier mod)
	{
		mods.Add(mod);
		UpdateMods();

		OnAttributeChanged?.Invoke(mod.attributeId, mod.value);
	}

	public void RemoveMod(string modId)
	{
		var modIdx = mods.FindIndex(mod => mod.id == modId);
		if (modIdx == -1)
			return;

		var mod = mods[modIdx];
		OnAttributeChanged?.Invoke(mod.attributeId, mod.value);

		mods.RemoveAt(modIdx);
		UpdateMods();
	}

	private Attribute GetBaseAttribute(AttributeType type, bool warnIfMissing = true)
	{
		var idx = baseValues.FindIndex(attr => attr.id == type);

		if (idx == -1)
		{
			if (warnIfMissing)
				Debug.LogWarning($"There is no attribute id {type} on {name}");

			return null;
		}

		return baseValues[idx];
	}

	private void UpdateMods()
	{
		currentValues.Clear();

		foreach (var key in baseValues)
			currentValues[key.id] = key.value;

		foreach (var mod in mods)
		{
			var attributeId = mod.attributeId;

			if (!currentValues.ContainsKey(attributeId))
				currentValues[attributeId] = 0.0f;

			var defaultAttr = GetBaseAttribute(attributeId);

			var value = mod.multiplier ? (defaultAttr.value * mod.value) : currentValues[attributeId] + mod.value;
			value = Mathf.Clamp(value, defaultAttr.minValue, defaultAttr.maxValue);
			currentValues[attributeId] = value;
		}
	}
}
