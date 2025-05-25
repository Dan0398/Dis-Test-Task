using UnityEngine;

namespace Gameplay.EventLoop
{
    public abstract class BaseAction : MonoBehaviour
    {
        [SerializeField] private string Target;
        private ActionsEventLoop eventLoop;
        
        protected abstract string ActionName { get; }

        protected void CallAction()
        {
            eventLoop.Call(new ActionEvent(ActionName, Target));
        }

        internal void Bind(ActionsEventLoop actionsEventLoop)
        {
            eventLoop = actionsEventLoop;
        }
    }
}