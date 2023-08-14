using HassClient.WS;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    #region ATTRIBUTES
    private Button _connectionButton;
    private VisualElement _bottomContainer;
    #endregion

    #region PROPERTIES
    public static UIController Instance { get; private set; }
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
        WebSocketController.Instance.OnConnectionChanged += Handle_OnConnectionChanged;
    }

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _connectionButton = root.Q<Button>("ConnectionButton");
        _bottomContainer = root.Q<VisualElement>("BottomContainer");
        _connectionButton.clicked += () => StartConnection();
    }

    #endregion

    #region METHODS
    public void StartConnection()
    {
        WebSocketController.Instance.StartConnection();
    }
    #endregion

    #region CALLBACKS
    private void Handle_OnConnectionChanged(ConnectionStates connectionState)
    {
        if (connectionState == ConnectionStates.Connected)
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                _bottomContainer.visible = false;
            });
        }
    }
    #endregion
}
