using System;
using System.Collections.Generic;
using UnityEngine;

namespace HomeAssistant.Data
{
    [Serializable]
    public struct HomeAssistantPipelineList
    {
        [SerializeField]
        public List<HomeAssistantPipelineInfo> Pipelines;

        [SerializeField]
        public string PreferredPipeline;
    }
}