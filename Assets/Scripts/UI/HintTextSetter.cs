using TMPro;
using UnityEngine;

public class HintTextSetter : MonoBehaviour
{
	[SerializeField]TextMeshProUGUI text;

	public void SetText(string Text)
    {
		text.text = Text;
    }
}
