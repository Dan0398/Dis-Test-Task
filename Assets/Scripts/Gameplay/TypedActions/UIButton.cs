using UnityEngine.EventSystems;

namespace Gameplay.TypedActions
{
    public class UIButton : EventLoop.BaseAction, IPointerClickHandler
    {
        protected override string ActionName => "Нажать кнопку";

        public void OnPointerClick(PointerEventData eventData)
        {
            CallAction();
        }
    }
}