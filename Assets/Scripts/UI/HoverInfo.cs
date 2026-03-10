using UnityEngine;
using UnityEngine.EventSystems;
using static HoverOverlay;

public class HoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] public Allignment Allignment = Allignment.BottomRight;

	[TextArea]
	public string Text = string.Empty;

	public float TooltipDelay = 0.2f;

	float hoverTimeEnter;
	bool entered = false, displayed = false;

	public void OnPointerEnter(PointerEventData eventData)
	{
		entered = true;
		displayed = false;
		hoverTimeEnter = 0;
	}
	private void Update()
	{
		if (!entered || displayed) return;

		hoverTimeEnter += Time.unscaledDeltaTime;
		if(hoverTimeEnter > TooltipDelay)
		{
			HoverOverlay.OnStartDisplaying(this);
			displayed = true;
		}

	}

	public void OnPointerExit(PointerEventData eventData)
	{
		entered = false;
		HoverOverlay.OnStopDisplaying(this);
	}
}