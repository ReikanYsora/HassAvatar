using UnityEngine;

public class AvatarBehaviorController : MonoBehaviour
{
    #region ATTRIBUTES
    [SerializeField] private Animator _animator;
    private SimpleTimer _eventTimer;
    [SerializeField] private float _timeBetweenEvents;
    [SerializeField] private bool _idle;
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
        _idle = true;
    }

    private void Start()
    {
        EventController.Instance.OnDomainEvent += Handle_OnDomainEvent;
        _eventTimer = new SimpleTimer(_timeBetweenEvents);
        _idle = true;
    }

    private void Update()
    {
        _eventTimer.Update(Time.deltaTime);
    }
    #endregion

    #region METHODS
    private void CreateTimers()
    {
        _eventTimer = new SimpleTimer(_timeBetweenEvents);
        _eventTimer.OnTimeReached += Handle_OnTimeReached;
    }
    #endregion

    #region CALLBACKS
    private void Handle_OnDomainEvent(EventControllerArgs obj)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            if (_idle && _animator != null)
            {
                _idle = false;
                _eventTimer.Restart();
                _animator.SetTrigger("OnLight");
            }
        });
    }

    private void Handle_OnTimeReached()
    {
        _idle = true;
    }
    #endregion
}
