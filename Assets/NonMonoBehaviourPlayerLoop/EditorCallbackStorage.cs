using System.Collections.Generic;
using NonMonoBehaviourPlayerLoop.Callbacks;

namespace NonMonoBehaviourPlayerLoop
{
    internal sealed class EditorCallbackStorage
    {
#if UNITY_EDITOR
	    public List<IEarlyUpdate> EarlyUpdates { get; } = new List<IEarlyUpdate>();
	    public List<IPreUpdate> PreUpdates { get; } = new List<IPreUpdate>();
	    public List<IUpdate> Updates { get; } = new List<IUpdate>();
	    public List<IPreLateUpdate> PreLateUpdates { get; } = new List<IPreLateUpdate>();
	    public List<IPostLateUpdate> PostLateUpdates { get; } = new List<IPostLateUpdate>();
#endif
    }
}