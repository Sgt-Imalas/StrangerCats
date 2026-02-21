using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : MaskableGraphic
{
	public Vector2[] points;

	public float thickness = 10f;
	public bool center = true;

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();

		if (points == null || points.Length < 2)
			return;

		Vector2 offset = center ? (rectTransform.sizeDelta / 2) : Vector2.zero;

		UIVertex vertex = UIVertex.simpleVert;
		vertex.color = color;

		int vertIndex = 0;

		for (int i = 0; i < points.Length; i++)
		{
			Vector2 prev = i == 0 ? points[i] : points[i - 1];
			Vector2 current = points[i];
			Vector2 next = i == points.Length - 1 ? points[i] : points[i + 1];

			Vector2 dirA = (current - prev).normalized;
			Vector2 dirB = (next - current).normalized;

			if (i == 0)
				dirA = dirB;
			if (i == points.Length - 1)
				dirB = dirA;

			Vector2 normalA = new Vector2(-dirA.y, dirA.x);
			Vector2 normalB = new Vector2(-dirB.y, dirB.x);

			// Miter vector
			Vector2 miter = (normalA + normalB).normalized;

			// Prevent extreme spikes at sharp angles
			float miterLength = thickness * 0.5f / Vector2.Dot(miter, normalA);
			miterLength = Mathf.Clamp(miterLength, -thickness, thickness);

			Vector2 miterOffset = miter * miterLength;

			vertex.position = current - miterOffset - offset;
			vh.AddVert(vertex);

			vertex.position = current + miterOffset - offset;
			vh.AddVert(vertex);

			if (i > 0)
			{
				vh.AddTriangle(vertIndex - 2, vertIndex - 1, vertIndex);
				vh.AddTriangle(vertIndex, vertIndex - 1, vertIndex + 1);
			}

			vertIndex += 2;
		}
	}
}
