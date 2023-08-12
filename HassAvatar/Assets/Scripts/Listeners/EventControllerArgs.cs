using System;

public class EventControllerArgs : EventArgs
{
    #region PROPERTIES
    public string Domain { set; get; }

    public string Entity { set; get; }

    public string OldState { set; get; }

    public string NewState { set; get; }

    #endregion
}
