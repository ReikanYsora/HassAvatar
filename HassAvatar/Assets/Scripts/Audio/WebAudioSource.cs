using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebAudioSource : MonoBehaviour
{
    #region ATTRIBUTES
    private AudioSource _audioSource;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    #endregion

    #region METHODS
    public void PlayAudioFromURL(string audioUrl)
    {
        StartCoroutine(DownloadAndPlay(audioUrl));
    }

    private IEnumerator DownloadAndPlay(string audioUrl)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);

                if (audioClip != null)
                {
                    while (audioClip.loadState != AudioDataLoadState.Loaded)
                    {
                        yield return null;
                    }

                    Destroy(gameObject, audioClip.length);
                    _audioSource.clip = audioClip;
                    _audioSource.Play();
                }
                else
                {
                    Destroy(this);
                }
            }
            else
            {
                Destroy(this);
            }
        }
    }
    #endregion
}
