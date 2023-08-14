using HassClient.WS;
using UnityEngine;

public class UIController : MonoBehaviour
{
    #region ATTRIBUTES
    [SerializeField] private Transform _connectionPanel;
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

    private void Handle_OnConnectionChanged(ConnectionStates connectionState)
    {
        if (connectionState == ConnectionStates.Connected)
        {
            _connectionPanel.gameObject.SetActive(false);
        }
    }
    #endregion

    #region METHODS
    public void StartConnection()
    {
        WebSocketController.Instance.StartConnection();
    }
    #endregion
}
