using UnityEngine.XR.Interaction.Toolkit;

namespace Gameplay.TypedActions
{
    public class PushButton : EventLoop.BaseAction
    {
        private XRSimpleInteractable interactable;

        protected override string ActionName => "Нажать на физическую кнопку";

        private void Awake()
        {
            interactable = GetComponent<XRSimpleInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.AddListener(ReactOnSelected);
            }
        }

        private void ReactOnSelected(SelectEnterEventArgs arg0)
        {
            CallAction();
        }

        void OnDestroy()
        {
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(ReactOnSelected);
            }
        }
    }
}