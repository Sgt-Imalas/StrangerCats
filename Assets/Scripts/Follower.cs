using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject Target;
    private Transform targetTransform;
    public float Offset = -20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetTransform = Target.transform;

	}

    // Update is called once per frame
    void Update()
    {
        var pos = targetTransform.position;
        pos.z += Offset;
        transform.position = pos;

	}
}
