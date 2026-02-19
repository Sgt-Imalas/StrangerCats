using System.Collections;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
	Follower CamFollower;
	private void Awake()
	{
		CamFollower = GetComponent<Follower>();
	}
	public void SetCameraOffset(float offset)
	{
		if(CamFollower)
			CamFollower.Offset = offset;
	}

	public void AnimateOffsetChange(float targetOffset, float duration, System.Action onComplete = null, bool LockInputs = false)
    {
		if (LockInputs)
		{
			Global.Instance.InCameraTransition = true;
		}

		StartCoroutine(OffsetChange(targetOffset, duration, onComplete));
	}
    IEnumerator OffsetChange(float targetOffset, float duration, System.Action onComplete = null, bool unlock = false)
	{
		float currentOffset = CamFollower.Offset;
		float timePerStep = duration / Mathf.Abs(currentOffset - targetOffset);

		if (currentOffset == targetOffset)
			yield break;
		if (currentOffset < targetOffset)
		{
			while (CamFollower.Offset < targetOffset)
			{
				CamFollower.Offset++;
				yield return new WaitForSeconds(timePerStep);
			}
		}
		else if (currentOffset > targetOffset)
		{
			while (CamFollower.Offset > targetOffset)
			{
				CamFollower.Offset--;
				yield return new WaitForSeconds(timePerStep);
			}
		}
		if(onComplete != null)
			onComplete.Invoke();

		if (unlock)
		{
			Global.Instance.InCameraTransition = false;
		}
	}
}
