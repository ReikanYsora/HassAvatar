using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class MainThreadDispatcher : MonoBehaviour
{
    #region ATTRIBUTES
    [SerializeField] private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    #endregion

    #region PROPERTIES
    public static MainThreadDispatcher Instance { get; private set; }
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
    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    #endregion

    #region METHODS
    public void Enqueue(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => {
                StartCoroutine(action);
            });
        }
    }
    public void Enqueue(Action action)
    {
        Enqueue(ActionWrapper(action));
    }
    public Task EnqueueAsync(Action action)
    {
        var tcs = new TaskCompletionSource<bool>();

        void WrappedAction()
        {
            try
            {
                action();
                tcs.TrySetResult(true);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }

        Enqueue(ActionWrapper(WrappedAction));
        return tcs.Task;
    }

    private IEnumerator ActionWrapper(Action action)
    {
        action();
        yield return null;
    }
    #endregion
}