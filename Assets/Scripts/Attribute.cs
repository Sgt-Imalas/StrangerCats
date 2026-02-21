using Assets.Scripts;
using System;
using UnityEngine;

[Serializable]
public class Attribute
{
	public AttributeType id;
	public string tooltip;
	public float minValue = 0.0f, maxValue = 1.0f;
	public float value = 0.0f;

	public Action<float> OnChanged;

	public Attribute(AttributeType id, float value, float minValue = 0.0f, float maxValue = 1.0f)
	{
		this.id = id;
		this.minValue = minValue;
		this.maxValue = maxValue;
		this.value = value;
	}

	
	public void Set(float newValue)
	{
		value = Mathf.Clamp(newValue, minValue, maxValue);

		OnChanged?.Invoke(value);
	}

	public void Add(float value)
	{
		Set(this.value + value);
	}
}
