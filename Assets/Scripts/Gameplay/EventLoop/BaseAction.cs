using UnityEngine;

namespace Gameplay.EventLoop
{
    public abstract class BaseAction : MonoBehaviour
    {
        [SerializeField] private string target;
        private ActionsEventLoop eventLoop;
        
        protected abstract string ActionName { get; }

        protected void CallAction()
        {
            eventLoop.Call(new ActionEvent(ActionName, target));
        }

        internal void Bind(ActionsEventLoop actionsEventLoop)
        {
            eventLoop = actionsEventLoop;
        }
    }
}