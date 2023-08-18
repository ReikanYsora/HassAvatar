using System;
using System.Collections.Generic;
using UnityEngine;

namespace HomeAssistant.Data
{
    [Serializable]
    public struct HomeAssistantService
    {
        [SerializeField]
        public string Name;

        [SerializeField]
        public string Description;

        [SerializeField]
        public List<HomeAssistantServiceField> Fields;
    }
}