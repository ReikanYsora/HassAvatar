using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region ATTRIBUTES
    [Header("Camera settings")]
    [SerializeField] private float _distance = 5.0f;
    [SerializeField] private float _minDistance = 2.0f;
    [SerializeField] private float _maxDistance = 10.0f;
    private Transform _target;
    private Bounds _targetBounds;
    #endregion

    #region PROPERTIES
    public static CameraController Instance { get; private set; }
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (_target != null)
        {
            InitializeCamera(_target);
        }
    }

    private void Update()
    {
        if (_target != null)
        {
            ManageCamera();
        }
    }
    #endregion

    #region METHODS
    public void InitializeCamera(Transform target)
    {
        _target = target;

        CalculateTargetBounds();
    }

    private void CalculateTargetBounds()
    {
        Renderer[] renderers = _target.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
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
        _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);

        Vector3 desiredPosition = _target.position + _target.forward * _distance;
        float targetCenterY = _target.position.y + (_targetBounds.center.y - _target.position.y);
        desiredPosition.y = targetCenterY;
        Vector3 targetPosition = _target.position;
        targetPosition.y = desiredPosition.y;
        transform.position = desiredPosition;
        transform.LookAt(targetPosition);
    }
    #endregion
}
