namespace Gameplay.EventLoop
{
    [System.Serializable]
    public class ActionEvent
    {
        public readonly string ExpectedAction;
        public readonly string Target;

        public ActionEvent(string expectedAction, string target)
        {
            ExpectedAction = expectedAction;
            Target = target;
        }
    }
}