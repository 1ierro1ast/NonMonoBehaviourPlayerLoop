using NonMonoBehaviourPlayerLoop;
using UnityEngine;

namespace DefaultNamespace
{
    public class Test : IUpdate, IFixedUpdate
    {
        public void OnUpdate()
        {
            Debug.Log("Update");
        }

        public void OnFixedUpdate()
        {
            Debug.Log("FixedUpdate");
        }
    }
}