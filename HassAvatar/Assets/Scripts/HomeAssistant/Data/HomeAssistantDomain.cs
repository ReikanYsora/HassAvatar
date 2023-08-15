using System;
using UnityEngine;

namespace HomeAssistant.Data
{
    [Serializable]
    public struct HomeAssistantDomain
    {
        #region ATTRIBUTES
        [SerializeField] public string Name;
        [SerializeField] public bool Listening;
        #endregion
    }
}
