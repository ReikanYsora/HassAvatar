using System;
using UnityEngine;

namespace HomeAssistant.Data
{
    [Serializable]
    public struct HomeAssistantServiceField
    {
        [SerializeField]
        public string Name;

        [SerializeField]
        public string Description;

        [SerializeField]
        public string IsRequired;

        [SerializeField]
        public string IsAdvanced;
    }
}