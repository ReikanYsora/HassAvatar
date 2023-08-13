using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region ATTRIBUTES
    [SerializeField] private Transform _target;
    [SerializeField] private float _distance = 5.0f;
    [SerializeField] private float _minDistance = 2.0f;
    [SerializeField] private float _maxDistance = 10.0f;
    [SerializeField] private float _rotationSpeed = 2.0f;
    private float _rotationX = 180.0f;
    private Bounds _targetBounds;
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        if (_target == null)
        {
            Debug.LogWarning("Camera target not defined");
            return;
        }

        CalculateTargetBounds();
    }

    private void Update()
    {
        HandleInput();
        ManageCamera();
    }
    #endregion

    #region METHODS

    private void HandleInput()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            _rotationX += mouseX * _rotationSpeed;
        }

        _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
    }

    private void CalculateTargetBounds()
    {
        Renderer[] renderers = _target.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found on target or its children.");
            return;
        }

        _targetBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            _targetBounds.Encapsulate(renderers[i].bounds);
        }
    }

    private void ManageCamera()
    {
        if (_target == null)
        {
            Debug.LogWarning("Camera target not defined");
            return;
        }

        Vector3 rotationEuler = new Vector3(0, _rotationX, 0);
        Quaternion rotation = Quaternion.Euler(rotationEuler);

        Vector3 desiredPosition = _target.position - rotation * Vector3.forward * _distance;
        float targetCenterY = _target.position.y + (_targetBounds.center.y - _target.position.y);
        desiredPosition.y = targetCenterY;
        Vector3 targetPosition = _target.position;
        targetPosition.y = desiredPosition.y;
        transform.position = desiredPosition;
        transform.LookAt(targetPosition);
    }
    #endregion
}
