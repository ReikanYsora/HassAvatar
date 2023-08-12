using HassClient.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscoverController : MonoBehaviour
{
    #region ATTRIBUTES
    [SerializeField] public List<Area> Areas;
    [SerializeField] public List<Domain> Domains;
    #endregion

    #region PROPERTIES

    public static DiscoverController Instance { get; private set; }
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
        APIConnectionController.Instance.OnConnectionChanged += Handle_OnConnectionChanged;
    }
    #endregion

    #region METHODS
    private async void DiscoverEntitiesAsync()
    {
        if (!APIConnectionController.Instance.IsConnected)
        {
            return;
        }

        IEnumerable<EntityRegistryEntry> entities = await APIConnectionController.Instance.Connection.GetEntitiesAsync();
    }

    private async void DiscoverAreasAsync()
    {
        if (!APIConnectionController.Instance.IsConnected)
        {
            return;
        }

        Areas = new List<Area>();
        IEnumerable<HassClient.Models.Area> tempAreas = await APIConnectionController.Instance.Connection.GetAreasAsync();

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
        if (!APIConnectionController.Instance.IsConnected)
        {
            return;
        }

        IEnumerable<EntityRegistryEntry> entities = await APIConnectionController.Instance.Connection.GetEntitiesAsync();
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
        if (!APIConnectionController.Instance.IsConnected)
        {
            return;
        }

        IEnumerable<PanelInfo> entities = await APIConnectionController.Instance.Connection.GetPanelsAsync();
        
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
    private void Handle_OnConnectionChanged(bool connectionState)
    {
        if (connectionState)
        {
            Discover();
        }
    }
    #endregion
}
