using HassClient.WS;
using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AvatarBehaviorController : MonoBehaviour
{
    #region ATTRIBUTES
    [Header("Avatar")]
    [SerializeField] private GameObject _avatar;
    [SerializeField] private float _levitationTime;
    [SerializeField] private float _levitationDistance;
    [SerializeField] private Transform _frontBody;
    [SerializeField] private Transform _topFrontWing;
    [SerializeField] private Transform _bottomFrontWing;
    [SerializeField] private Transform _rightFrontWing;
    [SerializeField] private Transform _leftFrontWing;
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
        AnimateLevitation();
        HomeAssistantController.Instance.OnConnectionChanged += Handle_OnConnectionChanged;
    }
    #endregion

    #region METHODS
    private void AnimateLevitation()
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        AnimationCurve easeUpDown = AnimationCurve.EaseInOut(0, 0, 1, 1);
        sequence.Append(_avatar.transform.DOMoveY(_avatar.transform.position.y + _levitationDistance, _levitationTime).SetEase(easeUpDown));
        sequence.SetLoops(-1, LoopType.Yoyo);
        sequence.Play();
    }

    public void FrontCoverAnimate(float rotation, float time)
    {

        DG.Tweening.Sequence sequenceBody = DOTween.Sequence();
        sequenceBody.Append(_frontBody.DOLocalRotate(new Vector3(0f, 0f, rotation), time));
        sequenceBody.Play();
    }

    public void WingsAnimate(float rotation, float time)
    {
        DG.Tweening.Sequence sequenceTop = DOTween.Sequence();
        sequenceTop.Append(_topFrontWing.DOLocalRotate(new Vector3(rotation, 0f, 0f), time));
        sequenceTop.Append(_topFrontWing.DOLocalRotate(Vector3.zero, time));
        sequenceTop.Play();

        DG.Tweening.Sequence sequenceBottom = DOTween.Sequence();
        sequenceBottom.Append(_bottomFrontWing.DOLocalRotate(new Vector3(-rotation, 0f, 0f), time));
        sequenceBottom.Append(_bottomFrontWing.DOLocalRotate(Vector3.zero, time));
        sequenceBottom.Play();

        DG.Tweening.Sequence sequenceLeft = DOTween.Sequence();
        sequenceLeft.Append(_leftFrontWing.DOLocalRotate(new Vector3(0f, rotation, 0f), time));
        sequenceLeft.Append(_leftFrontWing.DOLocalRotate(Vector3.zero, time));
        sequenceLeft.Play();

        DG.Tweening.Sequence sequenceRight = DOTween.Sequence();
        sequenceRight.Append(_rightFrontWing.DOLocalRotate(new Vector3(0f, -rotation, 0f), time));
        sequenceRight.Append(_rightFrontWing.DOLocalRotate(Vector3.zero, time));
        sequenceRight.Play();
        
    }

    private void InitializeAvatar()
    {
        //Initialize camera with created avatar
        CameraController.Instance.InitializeCamera(_avatar.transform);

        //Subscribe events for trigger animations
        HomeAssistantController.Instance.OnDomainEvent += Handle_OnDomainEvent;
    }
    #endregion

    #region CALLBACKS
    private void Handle_OnDomainEvent(HomeAssistantEventArgs obj)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            FrontCoverAnimate(180f, 1f);
            WingsAnimate(45f, 0.5f);
        });
    }

    private void Handle_OnConnectionChanged(ConnectionStates connectionState)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            if (connectionState == ConnectionStates.Connected)
            {
                InitializeAvatar();
            }
        });
    }
    #endregion
}
