using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(-10, 10, -10);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        transform.LookAt(target.position);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget; 
    }
}
