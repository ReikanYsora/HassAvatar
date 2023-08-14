using HassClient.Models;
using HassClient.WS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HomeAssistantController : MonoBehaviour
{
    #region ATTRIBUTES
    [SerializeField] public List<Area> Areas;
    [SerializeField] public List<Domain> Domains;
    #endregion

    #region PROPERTIES

    public static HomeAssistantController Instance { get; private set; }
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

        Areas = new List<Area>();
        Domains = new List<Domain>();
    }

    private void Start()
    {
        WebSocketController.Instance.OnConnectionChanged += Handle_OnConnectionChanged;
    }
    #endregion

    #region METHODS
    private async void DiscoverEntitiesAsync()
    {
        if (WebSocketController.Instance.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        IEnumerable<EntityRegistryEntry> entities = await WebSocketController.Instance.Connection.GetEntitiesAsync();
    }

    private async void DiscoverAreasAsync()
    {
        if (WebSocketController.Instance.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        Areas = new List<Area>();
        IEnumerable<HassClient.Models.Area> tempAreas = await WebSocketController.Instance.Connection.GetAreasAsync();

        foreach (HassClient.Models.Area tempArea in tempAreas)
        {
            Areas.Add(new Area
            {
                ID = tempArea.Id,
                Name = tempArea.Name
            });
        }
    }

    private async void DiscoverDomainsAsync()
    {
        if (WebSocketController.Instance.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        IEnumerable<EntityRegistryEntry> entities = await WebSocketController.Instance.Connection.GetEntitiesAsync();
        HashSet<string> tempDomains = entities.Select(x => x.Domain).ToHashSet();

        foreach (string domain in tempDomains)
        {
            Domains.Add(new Domain
            {
                Name = domain,
                Listening = false
            });
        }
    }

    private async void DiscoverPanelsAsync()
    {
        if (WebSocketController.Instance.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        IEnumerable<PanelInfo> entities = await WebSocketController.Instance.Connection.GetPanelsAsync();        
    }

    public void Discover()
    {
        DiscoverEntitiesAsync();
        DiscoverAreasAsync();
        DiscoverDomainsAsync();
        DiscoverPanelsAsync();
    }
    #endregion

    #region CALLBACKS
    private void Handle_OnConnectionChanged(ConnectionStates connectionState)
    {
        switch (connectionState)
        {
            default:
            case ConnectionStates.Disconnected:
                break;
            case ConnectionStates.Connecting:
                break;
            case ConnectionStates.Authenticating:
                break;
            case ConnectionStates.Restoring:
                break;
            case ConnectionStates.Connected:
                Discover();
                break;
        }
    }
    #endregion
}
