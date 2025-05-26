using UnityEngine.XR.Interaction.Toolkit;

namespace Gameplay.TypedActions
{
    public class GrabInteractable : EventLoop.BaseAction
    {
        private XRGrabInteractable interactable;

        protected override string ActionName => "Граб";

        private void Awake()
        {
            interactable = GetComponent<XRGrabInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.AddListener(ReactOnActivation);
            }
        }

        private void ReactOnActivation(SelectEnterEventArgs arg0)
        {
            CallAction();
        }

        void OnDestroy()
        {
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(ReactOnActivation);
            }
        }
    }
}