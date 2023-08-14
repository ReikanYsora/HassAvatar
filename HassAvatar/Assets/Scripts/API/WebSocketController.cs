using HassClient.WS;
using System;
using System.Threading.Tasks;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class WebSocketController : MonoBehaviour
{
    public string DistantUTL = "https://zy0dbiq9io70u5rrfxqk0g10jiryuzsb.ui.nabu.casa/";
    public string HomeAssistantURL = "http://192.168.1.2:8123";
    public string Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJhYWFmNWMyZGFlM2U0MGQ5OGQzY2U2Y2JkY2Q5OTk4MCIsImlhdCI6MTY5MTc3MTczMSwiZXhwIjoyMDA3MTMxNzMxfQ.bZWKR9t256dMGBlQ6PSCetcKycBpCdI864TTUnbtTek";

    #region PROPERTIES
    public HassWSApi Connection { private set; get; }

    public static WebSocketController Instance { get; private set; }

    public ConnectionStates ConnectionState
    {
        get
        {
            if (Connection == null)
            {
                return ConnectionStates.Disconnected;
            }

            return Connection.ConnectionState;
        }
    }
    #endregion

    #region EVENTS
    public event Action<ConnectionStates> OnConnectionChanged;
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
    public async void StartConnection()
    {
        try
        {
            await ConnectAsync();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.GetBaseException().Message);
        }
    }

    private async Task ConnectAsync()
    {
        try
        {
            Connection = new HassWSApi();
            Connection.ConnectionStateChanged += Handle_ConnectionStateChanged;
            var connectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl(DistantUTL, Token);
            await Connection.ConnectAsync(connectionParameters);
        }
        catch (Exception)
        {

        }
    }

    private void Handle_ConnectionStateChanged(object sender, ConnectionStates state)
    {
        OnConnectionChanged?.Invoke(state);
    }

    private async Task DisconnectAsync()
    {
        try
        {
            await Connection.CloseAsync();
        }
        catch (Exception)
        {

        }
    }
    #endregion
}
