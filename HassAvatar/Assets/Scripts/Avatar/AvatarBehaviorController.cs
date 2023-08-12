using UnityEngine;

public class AvatarBehaviorController : MonoBehaviour
{
    #region ATTRIBUTES
    [SerializeField] private Animator _animator;
    #endregion
    #region PROPERTIES

    public static AvatarBehaviorController Instance { get; private set; }
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
        EventsController.Instance.OnDomainEvent += Handle_OnDomainEvent;
    }
    #endregion

    #region CALLBACKS

    private void Handle_OnDomainEvent(EventControllerArgs obj)
    {
        if (_animator != null)
        {
            _animator.SetTrigger("OnLight");
        }
    }

    #endregion
}
