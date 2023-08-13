using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    #region ATTRIBUTES
    private StringBuilder _consoleLinesBuilder;
    [SerializeField] public TextMeshProUGUI _consoleText;
    [SerializeField] public ScrollRect _consoleScollRect;
    #endregion

    #region PROPERTIES
    public static ConsoleController Instance { get; private set; }
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
        _consoleLinesBuilder = new StringBuilder();
    }

    private void Start()
    {
        EventController.Instance.OnDomainEvent += Handle_OnDomainEvent;
    }
    #endregion

    #region CALLBACKS
    private void Handle_OnDomainEvent(EventControllerArgs obj)
    {
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            _consoleLinesBuilder.AppendLine(string.Format($"{obj.Domain}[{obj.Entity}] : {obj.NewState}"));
            _consoleText.text = _consoleLinesBuilder.ToString();
            var pos = new Vector2(0f, Mathf.Sin(Time.time * 10f) * 100f);
            _consoleScollRect.content.localPosition = pos;
        });
    }
    #endregion
}
