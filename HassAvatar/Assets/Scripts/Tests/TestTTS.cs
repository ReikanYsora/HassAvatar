using HassClient.WS;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class TestTTS : MonoBehaviour
{
    #region ATTRIBUTES
    [SerializeField] private string[] _intents;
    [SerializeField] private int _currentIndex = 0;
    [SerializeField] private float _delay = 5f;
    private bool _initialized;
    private bool _started;
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        HomeAssistantController.Instance.OnConnectionChanged += Handle_OnConnectionChanged;
    }

    private void Handle_OnConnectionChanged(ConnectionStates connectionState)
    {
        switch (connectionState)
        {
            default:
            case ConnectionStates.Disconnected:
            case ConnectionStates.Connecting:
            case ConnectionStates.Authenticating:
            case ConnectionStates.Restoring:
                break;
            case ConnectionStates.Connected:
                _initialized = true;
                _started = false;
                break;
        }
    }

    private void Update()
    {
        if (_initialized && !_started)
        {
            _started = true;
            StartCoroutine(PlayIntentsLoop());
        }

    }
    #endregion

    #region METHODS
    private IEnumerator PlayIntentsLoop()
    {
        while (true)
        {
            string intent = _intents[_currentIndex];

            Task.Run(async () =>
            {
                await StartTest(intent);
                _currentIndex = (_currentIndex + 1) % _intents.Length;
            });

            yield return new WaitForSeconds(_delay);
        }
    }

    private async Task StartTest(string intent)
    {
        await HomeAssistantController.Instance.RunIntentPipeline(intent);
    }
    #endregion
}
