using HassClient.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    #region ATTRIBUTES
    private HashSet<string> _listeningDomains;
    #endregion

    #region PROPERTIES

    public static EventController Instance { get; private set; }
    #endregion

    #region EVENTS
    public event Action<EventControllerArgs> OnDomainEvent;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        _listeningDomains = new HashSet<string>();

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        CheckRegisteredDomains();
    }
    #endregion

    #region METHODS
    private void CheckRegisteredDomains()
    {
        if (APIController.Instance.IsConnected)
        {
            foreach (Domain tempDomain in HomeAssistantController.Instance.Domains)
            {
                if (tempDomain.Listening && !_listeningDomains.Contains(tempDomain.Name))
                {
                    _listeningDomains.Add(tempDomain.Name);
                    APIController.Instance.Connection.StateChagedEventListener.SubscribeDomainStatusChanged(tempDomain.Name, Handle_StateChanged);
                }
                else if (!tempDomain.Listening && _listeningDomains.Contains(tempDomain.Name))
                {
                    _listeningDomains.Remove(tempDomain.Name);
                    APIController.Instance.Connection.StateChagedEventListener.UnsubscribeDomainStatusChanged(tempDomain.Name, Handle_StateChanged);
                }
            }
        }
    }

    private void Handle_StateChanged(object sender, StateChangedEvent stateChangedArgs)
    {
        OnDomainEvent?.Invoke(new EventControllerArgs
        {
            Domain = stateChangedArgs.Domain,
            Entity = stateChangedArgs.EntityId,
            OldState = stateChangedArgs.OldState.State,
            NewState = stateChangedArgs.NewState.State
        });
    }
    #endregion
}
