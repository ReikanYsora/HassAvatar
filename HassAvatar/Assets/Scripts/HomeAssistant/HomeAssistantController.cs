using HassClient.Models;
using HassClient.WS;
using HomeAssistant.Configuration;
using HomeAssistant.Data;
using HomeAssistant.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HomeAssistantController : MonoBehaviour
{
    #region CONSTANTS
    private const string HASS_AVATAR_CONFIGURATIONS = "HASS_Avatar_Configurations";
    #endregion

    #region ATTRIBUTES
    private HassWSApi _WSApiConnection;

    [Header("Home Assistant configurations")]
    [SerializeField] private List<HomeAssistantServerSettings> _servers;
    [SerializeField] private HomeAssistantServerSettings _selectedServer;
    [SerializeField] private bool _serverSettingsInitialized;

    [Header("Home Assistant elements")]
    [SerializeField] private List<HomeAssistantArea> _areas;
    [SerializeField] private List<HomeAssistantDomain> _domains;
    [SerializeField] private List<HomeAssistantPanel> _panels;
    [SerializeField] private List<HomeAssistantServiceDomain> _serviceDomains;

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
        SaveConfiguration configuration = LoadConfiguration();
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
        _areas = new List<HomeAssistantArea>();
        _domains = new List<HomeAssistantDomain>();
        _panels = new List<HomeAssistantPanel>();

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
        await DisconnectAsync();

        _areas.Clear();
        _domains.Clear();
        _panels.Clear();
        _events.Clear();
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
        await DiscoverServiceDomainsAsync();
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

        IEnumerable<Area> tempAreas = await _WSApiConnection.GetAreasAsync();
        _areas.Clear();

        foreach (Area tempArea in tempAreas)
        {
            _areas.Add(new HomeAssistantArea
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
        _domains.Clear();

        foreach (string domain in tempDomains)
        {
            _domains.Add(new HomeAssistantDomain
            {
                Name = domain,
                Listening = false
            });
        }
    }

    private async Task DiscoverServiceDomainsAsync()
    {
        if (_WSApiConnection.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        IEnumerable<ServiceDomain> serviceDomains = await _WSApiConnection.GetServicesAsync();
        _serviceDomains.Clear();

        foreach (ServiceDomain tempServiceDomain in serviceDomains)
        {
            HomeAssistantServiceDomain tempDomain = new HomeAssistantServiceDomain
            {
                Domain = tempServiceDomain.Domain
            };

            List<HomeAssistantService> tempDomainServices = new List<HomeAssistantService>();

            foreach (KeyValuePair<string, Service> tempService in tempServiceDomain.Services)
            {
                HomeAssistantService tempServiceDefinition = new HomeAssistantService
                {
                    Description = tempService.Value.Description,
                    Name = tempService.Key
                };

                tempServiceDefinition.Fields = new List<HomeAssistantServiceField>();

                foreach (KeyValuePair<string, ServiceField> tempServiceField in tempService.Value.Fields)
                {
                    tempServiceDefinition.Fields.Add(new HomeAssistantServiceField
                    {
                        Description = tempServiceField.Value.Description,
                        Name = tempServiceField.Key,
                        IsAdvanced = tempServiceField.Value.IsAdvanced,
                        IsRequired = tempServiceField.Value.IsRequired
                    });
                }

                tempDomainServices.Add(tempServiceDefinition);
            }

            tempDomain.Services = tempDomainServices;
            _serviceDomains.Add(tempDomain);
        }
        //var t = await _WSApiConnection.CallServiceAsync("conversation", "process", new { text = "Eteint la lumi�re de la mezzanine" });
    }

    private async Task DiscoverPanelsAsync()
    {
        if (_WSApiConnection.ConnectionState != ConnectionStates.Connected)
        {
            return;
        }

        IEnumerable<PanelInfo> tempPanelInfos = await _WSApiConnection.GetPanelsAsync();
        _panels.Clear();

        foreach (PanelInfo panelInfo in tempPanelInfos)
        {
            _panels.Add(new HomeAssistantPanel
            {
                Name = panelInfo.Title,
                URL = panelInfo.UrlPath,
                Icon = panelInfo.Icon
            });
        }
    }
    #endregion

    #region CONFIGURATION MANAGEMENT
    public static void SaveConfiguration(SaveConfiguration settings)
    {
        string json = JsonUtility.ToJson(settings);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        PlayerPrefs.SetString(HASS_AVATAR_CONFIGURATIONS, Convert.ToBase64String(bytes));
        PlayerPrefs.Save();
    }

    public static SaveConfiguration LoadConfiguration()
    {
        if (PlayerPrefs.HasKey(HASS_AVATAR_CONFIGURATIONS))
        {
            string base64 = PlayerPrefs.GetString(HASS_AVATAR_CONFIGURATIONS);
            byte[] bytes = Convert.FromBase64String(base64);
            string json = Encoding.UTF8.GetString(bytes);
            return JsonUtility.FromJson<SaveConfiguration>(json);
        }
        else
        {
            return new SaveConfiguration();
        }
    }
    #endregion

    #region EVENTS MANAGEMENT
    private void RegisterDomainEvents()
    {
        if (_WSApiConnection.ConnectionState == ConnectionStates.Connected)
        {
            foreach (HomeAssistantDomain tempDomain in _domains)
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
            foreach (HomeAssistantDomain tempDomain in _domains)
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