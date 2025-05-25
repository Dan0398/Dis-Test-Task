namespace Gameplay.EventLoop
{
    public interface IClient
    {
        void ReceiveAction(ActionEvent @event);
    }
}