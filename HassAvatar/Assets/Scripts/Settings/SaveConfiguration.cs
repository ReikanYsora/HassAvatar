using System;
using System.Collections.Generic;

[Serializable]
public class SaveConfiguration
{
    public List<HomeAssistantServerSettings> Servers;

    public SaveConfiguration()
    {
        Servers = new List<HomeAssistantServerSettings>();
    }
}