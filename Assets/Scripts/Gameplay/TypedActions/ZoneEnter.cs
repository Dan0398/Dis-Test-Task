using UnityEngine;

namespace Gameplay.TypedActions
{
    public class ZoneEnter : EventLoop.BaseAction
    {
        protected override string ActionName => "Подойти";

        private void OnTriggerEnter(Collider col)
        {
            if (col != null && col.TryGetComponent<CharacterController>(out _))
            {
                CallAction();
            }
        }
    }
}