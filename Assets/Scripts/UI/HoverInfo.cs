using UnityEngine;
using UnityEngine.EventSystems;
using static HoverOverlay;

public class HoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] public Allignment Allignment = Allignment.BottomRight;

	[TextArea]
	public string Text = string.Empty;

	public void OnPointerEnter(PointerEventData eventData)
	{
		HoverOverlay.OnStartDisplaying(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HoverOverlay.OnStopDisplaying(this);
	}
}