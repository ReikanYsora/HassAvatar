using System;
using System.Text;
using UnityEngine;

public static class ConfigurationController
{
    #region METHODS
    public static void SaveConfiguration(SaveConfiguration settings)
    {
        string json = JsonUtility.ToJson(settings);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        PlayerPrefs.SetString("HASS_Avatar_Configurations", Convert.ToBase64String(bytes));
        PlayerPrefs.Save();
    }

    public static SaveConfiguration LoadConfiguration()
    {
        if (PlayerPrefs.HasKey("HASS_Avatar_Configurations"))
        {
            string base64 = PlayerPrefs.GetString("HASS_Avatar_Configurations");
            byte[] bytes = Convert.FromBase64String(base64);
            string json = Encoding.UTF8.GetString(bytes);
            return JsonUtility.FromJson<SaveConfiguration>(json);
        }
        else
        {
            return new SaveConfiguration();
        }
    }
    #endregion
}