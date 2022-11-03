namespace NonMonoBehaviourPlayerLoop
{
    public abstract class NonMonoBehaviour
    {
        protected NonMonoBehaviour()
        {
            PlayerLoop.AddListeners(this);
        }
    }
}