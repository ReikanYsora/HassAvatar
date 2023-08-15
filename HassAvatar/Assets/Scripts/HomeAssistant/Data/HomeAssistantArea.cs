using System;
using UnityEngine;

namespace HomeAssistant.Data
{
    [Serializable]
    public struct HomeAssistantArea
    {
        [SerializeField] public string Name;
        [SerializeField] public string ID;
    }
}
