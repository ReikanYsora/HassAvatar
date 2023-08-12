using System;

public class SimpleTimer
{
    #region EVENTS
    public event Action OnTimeReached;
    #endregion

    #region PROPERTIES
    public float Time { set; get; }

    public float CurrentTime { private set; get; }

    public SimpleTimerState State { private set; get; }
    #endregion


    #region CONSTRUCTOR
    public SimpleTimer(float time)
    {
        Time = time;
        State = SimpleTimerState.Initialized;
    }
    #endregion

    #region METHODS
    public void Update(float delta)
	{
        if (State == SimpleTimerState.Playing)
        {
            CurrentTime += delta;

            if (CurrentTime > Time)
            {
                State = SimpleTimerState.Stopped;
                OnTimeReached?.Invoke();
            }
        }
	}

    public void Reset()
    {
        State = SimpleTimerState.Initialized;
        CurrentTime = 0f;
    }

    public void Play()
    {
        State = SimpleTimerState.Playing;
    }

    public void Restart()
    {
        Reset();
        Play();
    }
	#endregion
}

public enum SimpleTimerState
{
    Initialized,
    Playing,
    Stopped
}
