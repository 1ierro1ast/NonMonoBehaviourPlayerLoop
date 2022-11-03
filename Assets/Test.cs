using NonMonoBehaviourPlayerLoop;
using NonMonoBehaviourPlayerLoop.Callbacks;
using UnityEngine;

namespace DefaultNamespace
{
    public class Test : NonMonoBehaviour, IEarlyUpdate, IPreUpdate, IUpdate, IPreLateUpdate, IPostLateUpdate
    {
        public Test()
        {
            Debug.Log("ctor1");
        }

        public void OnEarlyUpdate()
        {
            Debug.Log("=======================");
            Debug.Log(nameof(OnEarlyUpdate));
        }

        public void OnPreUpdate()
        {
            Debug.Log(nameof(OnPreUpdate));
        }

        public void OnUpdate()
        {
            Debug.Log(nameof(OnUpdate));
        }

        public void OnPreLateUpdate()
        {
            Debug.Log(nameof(OnPreLateUpdate));
        }

        public void OnPostLateUpdate()
        {
            Debug.Log(nameof(OnPostLateUpdate));
            Debug.Log("=======================");
        }
    }
}