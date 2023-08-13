using UnityEngine;

public class InputController : MonoBehaviour
{
    #region ATTRIBUTES
    [SerializeField] private float _rotationSpeed = 2.0f;
    private Transform _target;
    #endregion

    #region PROPERTIES
    public static InputController Instance { get; private set; }
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
    #endregion

    #region METHODS
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void RotateRight()
    {
        if (_target != null)
        {
            _target.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        }
    }

    public void RotateLeft()
    {
        if (_target != null)
        {
            _target.Rotate(Vector3.up, -_rotationSpeed * Time.deltaTime);
        }
    }
    #endregion
}
