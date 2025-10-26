using UnityEngine;

public class LightRotation : MonoBehaviour
{
    public bool _rotation = false;
    public Quaternion initialRotation;
    public float rotationSpeed = 30f;
    private float _currentX = 0f;

    public void EnableRotation()
    {
        _rotation = true;
    }

    public void DisableRotation()
    {
        _rotation = false;
        transform.rotation = initialRotation;
    }

    private void Awake()
    {
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (_rotation)
        {
            RotateLight();
        }
    }

    private void RotateLight()
    {
        _currentX += rotationSpeed * Time.deltaTime;

        if (_currentX >= 360f)
            _currentX -= 360f;

        transform.rotation = initialRotation * Quaternion.Euler(_currentX, 0f, 0f);
    }
}
