using System;

[Serializable]
public class EventEntry
{
	#region PROPERTIES
	public DateTime Time;

	public string Domain;

	public string EntityID;

	public string OldState;

	public string NewState;
	#endregion
}
