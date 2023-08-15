using System;
using System.Collections.Generic;

namespace HomeAssistant.Configuration
{
    [Serializable]
    public class HomeAssistantServerSettings
    {
        public string Name;
        public string URL;
        public string Token;
        public string AvatarName;
        public List<string> EnabledDomains;
    }
}