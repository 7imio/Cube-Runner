using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target;

    [Header("Offsets")]
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    [Header("Smoothness")]
    [Range(0f, 1f)]
    public float smoothspeed = 0.1f;
    [Range(0f, 1f)]
    public float rotationSmooth = 0.12f;

    [Header("Aim")]
    public bool lookAtTarget = true;
    public Vector3 lookAtOffset = new Vector3(0f, 1f, 0f);

    private Vector3 _velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        // ideal camera position
        Vector3 desiredPosition = target.position + offset;

        // smooth interpolation
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _velocity,
            smoothspeed
            );

        transform.position = smoothedPosition;

        if (lookAtTarget)
        {
            Vector3 aimPoint = target.position + lookAtOffset;
            // keep clean roll : process a rotation towards point
            Quaternion desiredRotation = Quaternion.LookRotation(aimPoint - transform.position, Vector3.up);
            // slerp for smooth
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, 1f - Mathf.Exp(-rotationSmooth * 60f * Time.deltaTime));

        }

    }
}
