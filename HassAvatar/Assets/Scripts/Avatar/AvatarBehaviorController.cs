using System;
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
        EventController.Instance.OnDomainEvent += Handle_OnDomainEvent;
    }

    private void Update()
    {

    }
    #endregion

    #region METHODS
    #endregion

    #region CALLBACKS
    private void Handle_OnDomainEvent(EventControllerArgs obj)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            try
            {
                if (_animator != null)
                {
                    _animator.SetTrigger("OnLight");
                }
            }
            catch (Exception ex)
            {
                string t = ex.GetBaseException().Message;
            }
        });
    }
    #endregion
}
