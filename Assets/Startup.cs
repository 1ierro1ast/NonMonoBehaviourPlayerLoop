using UnityEngine;

namespace DefaultNamespace
{
    public class Startup : MonoBehaviour
    {
        private Test _test;
        private void Awake()
        {
            _test = new Test();
        }
    }
}