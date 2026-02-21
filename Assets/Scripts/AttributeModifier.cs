using Assets.Scripts;

public class AttributeModifier
{
	public string id;
	public AttributeType attributeId;
	public float value;
	public bool multiplier;

	public AttributeModifier(string id, AttributeType attributeId, float value, bool multiplier = false)
	{
		this.id = id;
		this.attributeId = attributeId;
		this.value = value;
		this.multiplier = multiplier;
	}

	public AttributeModifier() { }
}
