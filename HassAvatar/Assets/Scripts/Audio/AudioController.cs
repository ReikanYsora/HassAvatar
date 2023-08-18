using UnityEngine;

public class AudioController : MonoBehaviour
{
    #region ATTRIBUTES
    public GameObject webAudioSourcePrefab;
    #endregion

    #region PROPERTIES
    public static AudioController Instance { get; private set; }
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        HomeAssistantController.Instance.OnTTSAudioReceived += Handle_OnTTSAudioReceived;
    }

    private void Handle_OnTTSAudioReceived(string audioUrl)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            CreateWebAudioSource(audioUrl);
        });
    }

    #endregion

    #region METHODS
    private void CreateWebAudioSource(string audioUrl)
    {
        GameObject tempWebAudioSourceObject = Instantiate(webAudioSourcePrefab);
        tempWebAudioSourceObject.transform.SetParent(transform);
        WebAudioSource tempAudioSource = tempWebAudioSourceObject.GetComponent<WebAudioSource>();

        if (tempAudioSource != null)
        {
            tempAudioSource.PlayAudioFromURL(audioUrl);
        }
    }
    #endregion
}
