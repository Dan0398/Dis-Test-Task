using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

namespace Gameplay.TypedActions
{
    public class PushButton : EventLoop.BaseAction
    {
        [SerializeField] private bool ProvideActionsWhenSelecting;
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