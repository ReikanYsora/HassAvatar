using HassClient.Models;
using HassClient.WS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class HomeAssistantController : MonoBehaviour
{
    #region ATTRIBUTES
    private HassWSApi _WSApiConnection;

    [Header("Home Assistant configurations")]
    [SerializeField] private List<HomeAssistantServerSettings> _servers;
    [SerializeField] private HomeAssistantServerSettings _selectedServer;
    [SerializeField] private bool _serverSettingsInitialized;

    [Header("Home Assistant elements")]
    [SerializeField] private List<Area> _areas;
    [SerializeField] private List<Domain> _domains;

    [Header("Home Assistant events management")]
    [SerializeField] private float _lifeTime;
    [SerializeField] private List<HomeAssistantEventEntry> _events;
    private object _eventsLock = new object();
    #endregion

    #region EVENTS
    public event Action<ConnectionStates> OnConnectionChanged;
    public event Action<HomeAssistantEventArgs> OnDomainEvent;
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

        //Get configuration
        SaveConfiguration configuration = ConfigurationController.LoadConfiguration();
        _servers = configuration.Servers;

        if (_servers != null && _servers.Count > 0)
        {
            _selectedServer = _servers[0];
            _serverSettingsInitialized = true;
        }

        //Prepare API connection
        _WSApiConnection = new HassWSApi();
        _WSApiConnection.ConnectionStateChanged += Handle_ConnectionStateChanged;

        //Prepare baked data
        _areas = new List<Area>();
        _domains = new List<Domain>();

        //Prepare event listening
        _events = new List<HomeAssistantEventEntry>();
    }

    private void Update()
    {
        //Manage event entries
        ManageEventEntries();
    }

    private async void OnDisable()
    {
        _events.Clear();
        await DisconnectAsync();
    }
    #endregion

    #region METHODS
    #region HOME ASSISTANT
    public async Task ConnectAsync()
    {
        if (_selectedServer == null || !_serverSettingsInitialized)
        {
            return;
        }

        try
        {
            var connectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl(_selectedServer.URL, _selectedServer.Token);
            await _WSApiConnection.ConnectAsync(connectionParameters);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private async Task DisconnectAsync()
    {
        try
        {
            UnregisterDomainEvents();
            await _WSApiConnection.CloseAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Discover()
    {
        await DiscoverEntitiesAsync();
        await DiscoverAreasAsync();
        await DiscoverDomainsAsync();
        await DiscoverPanelsAsync();
    }

    private async Task DiscoverEntitiesAsync()
    {
        if (_WSApiConnection.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        IEnumerable<EntityRegistryEntry> entities = await _WSApiConnection.GetEntitiesAsync();
    }

    private async Task DiscoverAreasAsync()
    {
        if (_WSApiConnection.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        _areas = new List<Area>();
        IEnumerable<HassClient.Models.Area> tempAreas = await _WSApiConnection.GetAreasAsync();

        foreach (HassClient.Models.Area tempArea in tempAreas)
        {
            _areas.Add(new Area
            {
                ID = tempArea.Id,
                Name = tempArea.Name
            });
        }
    }

    private async Task DiscoverDomainsAsync()
    {
        if (_WSApiConnection.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        IEnumerable<EntityRegistryEntry> entities = await _WSApiConnection.GetEntitiesAsync();
        HashSet<string> tempDomains = entities.Select(x => x.Domain).ToHashSet();

        foreach (string domain in tempDomains)
        {
            _domains.Add(new Domain
            {
                Name = domain,
                Listening = false
            });
        }
    }

    private async Task DiscoverPanelsAsync()
    {
        if (_WSApiConnection.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        IEnumerable<PanelInfo> entities = await _WSApiConnection.GetPanelsAsync();
    }
    #endregion

    #region EVENTS MANAGEMENT
    private void RegisterDomainEvents()
    {
        if (_WSApiConnection.ConnectionState == ConnectionStates.Connected)
        {
            foreach (Domain tempDomain in _domains)
            {
                if (_selectedServer.EnabledDomains.Contains(tempDomain.Name))
                {
                    _WSApiConnection.StateChagedEventListener.SubscribeDomainStatusChanged(tempDomain.Name, Handle_StateChanged);
                }
            }
        }
    }

    private void UnregisterDomainEvents()
    {
        if (_WSApiConnection.ConnectionState == ConnectionStates.Connected)
        {
            foreach (Domain tempDomain in _domains)
            {
                _WSApiConnection.StateChagedEventListener.UnsubscribeDomainStatusChanged(tempDomain.Name, Handle_StateChanged);
            }
        }
    }

    private void ManageEventEntries()
    {
        if (_WSApiConnection.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        lock (_eventsLock)
        {
            if (_events != null && _events.Count > 0)
            {
                _events.RemoveAll(x => x.Time < DateTime.Now - TimeSpan.FromSeconds(_lifeTime));
            }
        }
    }
    #endregion

    #endregion

    #region CALLBACKS
    private async void Handle_ConnectionStateChanged(object sender, ConnectionStates state)
    {
        switch (state)
        {
            default:
            case ConnectionStates.Disconnected:
            case ConnectionStates.Connecting:
            case ConnectionStates.Authenticating:
            case ConnectionStates.Restoring:
                break;
            case ConnectionStates.Connected:
                await Discover();
                RegisterDomainEvents();
                break;
        }

        OnConnectionChanged?.Invoke(state);
    }
    private void Handle_StateChanged(object sender, StateChangedEvent stateChangedArgs)
    {
        _events.Add(new HomeAssistantEventEntry
        {
            Time = DateTime.Now,
            Domain = stateChangedArgs.Domain,
            EntityID = stateChangedArgs.EntityId,
            NewState = stateChangedArgs.NewState.State,
            OldState = stateChangedArgs.OldState.State
        });

        OnDomainEvent?.Invoke(new HomeAssistantEventArgs
        {
            Domain = stateChangedArgs.Domain,
            Entity = stateChangedArgs.EntityId,
            OldState = stateChangedArgs.OldState.State,
            NewState = stateChangedArgs.NewState.State
        });
    }
    #endregion
}
