using HassClient.WS;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class APIConnectionController : MonoBehaviour
{
    public string HomeAssistantURL = "http://192.168.1.2:8123";
    public string Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJhYWFmNWMyZGFlM2U0MGQ5OGQzY2U2Y2JkY2Q5OTk4MCIsImlhdCI6MTY5MTc3MTczMSwiZXhwIjoyMDA3MTMxNzMxfQ.bZWKR9t256dMGBlQ6PSCetcKycBpCdI864TTUnbtTek";

    #region ATTRIBUTES
    private bool _isConnected;
    #endregion

    #region PROPERTIES
    public HassWSApi Connection { private set; get; }

    public static APIConnectionController Instance { get; private set; }

    public bool IsConnected
    {
        set
        {
            if (_isConnected != value)
            {
                _isConnected = value;
                OnConnectionChanged?.Invoke(_isConnected);
            }
        }
        get
        {
            return _isConnected;
        }
    }
    #endregion

    #region EVENTS
    public event Action<bool> OnConnectionChanged;
    #endregion

    #region UNITY METHODS
    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        await ConnectAsync();
    }
    #endregion

    #region METHODS
    private async Task ConnectAsync()
    {
        try
        {
            Connection = new HassWSApi();
            var connectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl(HomeAssistantURL, Token);
            await Connection.ConnectAsync(connectionParameters);
            IsConnected = true;
        }
        catch (Exception)
        {
            IsConnected = false;
        }
    }

    private async Task DisconnectAsync()
    {
        try
        {
            await Connection.CloseAsync();
        }
        catch (Exception)
        {
            IsConnected = false;
        }
    }
    #endregion
}
