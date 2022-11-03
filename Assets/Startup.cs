using UnityEngine;

namespace DefaultNamespace
{
    public class Startup : MonoBehaviour
    {
        private Test _test;
        private void Awake()
        {
            Debug.Log("123");
            _test = new Test();

            gameObject.AddComponent<TestBehaviour>();
        }
    }
}