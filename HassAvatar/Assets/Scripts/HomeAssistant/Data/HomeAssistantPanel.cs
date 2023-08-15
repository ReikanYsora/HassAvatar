using System;
using UnityEngine;

namespace HomeAssistant.Data
{
    [Serializable]
    public struct HomeAssistantPanel
    {
        #region ATTRIBUTES
        [SerializeField] public string Name;
        [SerializeField] public string URL;
        [SerializeField] public string Icon;
        #endregion
    }
}
