using System;
using System.Collections.Generic;
using UnityEngine;


namespace HomeAssistant.Data
{
    [Serializable]
    public struct HomeAssistantServiceDomain
    {
        [SerializeField]
        public string Domain;

        [SerializeField]
        public List<HomeAssistantService> Services;
    }

}
