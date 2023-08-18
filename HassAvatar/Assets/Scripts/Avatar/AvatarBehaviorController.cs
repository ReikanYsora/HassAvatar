using HassClient.WS;
using System;
using UnityEngine;

public class AvatarBehaviorController : MonoBehaviour
{
    #region ATTRIBUTES
    [Header("Avatar selection")]
    [SerializeField] private int _selectedAvatar;
    [SerializeField] private GameObject[] _avatars;
    private GameObject _avatar;

    [Header("Avatar creation attributes")]
    [SerializeField] private Transform _spawnPosition;
    private Animator _animator;
    #endregion

    #region PROPERTIES
    public static AvatarBehaviorController Instance { get; private set; }

    public bool Initialized { get; private set; }
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
        Initialized = false;
    }

    private void Start()
    {
        HomeAssistantController.Instance.OnConnectionChanged += Handle_OnConnectionChanged;
    }

    private void Update()
    {

    }
    #endregion

    #region METHODS
    private void CreateAvatar()
    {
        if (_selectedAvatar > _avatars.Length)
        {
            Initialized = false;
            return;
        }

        _avatar = Instantiate(_avatars[_selectedAvatar]);
        _avatar.transform.position = _spawnPosition.position;
        _avatar.transform.parent = transform;
        _animator = _avatar.GetComponentInChildren<Animator>();

        //Only if avatar have animator
        if (_animator != null)
        {
            Initialized = true;

            //Initialize camera with created avatar
            CameraController.Instance.InitializeCamera(_avatar.transform);

            //Subscribe events for trigger animations
            HomeAssistantController.Instance.OnDomainEvent += Handle_OnDomainEvent;
        }
    }
    #endregion

    #region CALLBACKS
    private void Handle_OnDomainEvent(HomeAssistantEventArgs obj)
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

    private void Handle_OnConnectionChanged(ConnectionStates connectionState)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            if (connectionState == ConnectionStates.Connected)
            {
                if (_animator != null)
                {
                    Destroy(_avatar);
                }

                CreateAvatar();
            }
        });
    }
    #endregion
}
