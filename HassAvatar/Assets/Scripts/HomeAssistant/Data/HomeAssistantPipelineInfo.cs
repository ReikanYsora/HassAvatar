using System;
using UnityEngine;

namespace HomeAssistant.Data
{
    [Serializable]
    public struct HomeAssistantPipelineInfo
    {
        [SerializeField]
        public string ConversationEngine;

        [SerializeField]
        public string ConversationLanguage;

        [SerializeField]
        public string Language;

        [SerializeField]
        public string Name;

        [SerializeField]
        public string STTEngine;

        [SerializeField]
        public string STTLanguage;

        [SerializeField]
        public string TTSEngine;

        [SerializeField]
        public string TTSLanguage;

        [SerializeField]
        public string TTSVoice;

        [SerializeField]
        public string ID;
    }
}

