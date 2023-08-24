using DG.Tweening;
using HassClient.WS;
using UnityEngine;

public class BotBehaviorController : MonoBehaviour
{
    #region ATTRIBUTES
    [Header("Bot settings")]
    [SerializeField] private GameObject _bot;

    
    [Header("Animations")]
    [SerializeField] private float _levitationTime;
    [SerializeField] private float _levitationDistance;
    [SerializeField] private Transform _frontCover;
    [SerializeField] private Transform _backCover;
    [SerializeField] private float _backCoverRotationSpeed;    
    #endregion

    #region PROPERTIES
    public static BotBehaviorController Instance { get; private set; }
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
        Sequence sequence = DOTween.Sequence();
        AnimationCurve easeUpDown = AnimationCurve.EaseInOut(0, 0, 1, 1);
        sequence.Append(_bot.transform.DOMoveY(_bot.transform.position.y + _levitationDistance, _levitationTime).SetEase(easeUpDown));
        sequence.SetLoops(-1, LoopType.Yoyo);
        sequence.Play();
    }

    public void FrontCoverAnimate(float rotation, float time)
    {

        Sequence sequenceBody = DOTween.Sequence();
        sequenceBody.Append(_frontCover.DOLocalRotate(new Vector3(0f, 0f, rotation), time));
        sequenceBody.Play();
    }

    public void BackCoverAnimate()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_backCover.DOLocalRotate(new Vector3(0f, 0f, 360f), _backCoverRotationSpeed, RotateMode.FastBeyond360));
        sequence.OnComplete(BackCoverAnimate);
        sequence.Play();
    }

    private void InitializeBot()
    {
        //Initialize camera with created avatar
        CameraController.Instance.InitializeCamera(_bot.transform);

        //Subscribe events for trigger animations
        HomeAssistantController.Instance.OnDomainEvent += Handle_OnDomainEvent;

        //Create infinite rotation on back cover
        BackCoverAnimate();
    }
    #endregion

    #region CALLBACKS
    private void Handle_OnDomainEvent(HomeAssistantEventArgs obj)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            FrontCoverAnimate(180f, 1f);
        });
    }

    private void Handle_OnConnectionChanged(ConnectionStates connectionState)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            if (connectionState == ConnectionStates.Connected)
            {
                InitializeBot();
            }
        });
    }
    #endregion
}
