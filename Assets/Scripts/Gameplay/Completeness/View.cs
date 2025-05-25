using Gameplay.EventLoop;
using UnityEngine;

namespace Gameplay.Completeness
{
    public class View : MonoBehaviour, EventLoop.IClient
    {
        [SerializeField] private Steps.GroupOfSteps linkToSteps;
        [SerializeField] private TMPro.TMP_Text text;
        [SerializeField] EventLoop.ActionsEventLoop eventLoop;
        private GroupOfStepsProcessor groupProcessor;

        private void Start()
        {
            groupProcessor = new(linkToSteps)
            {
                OnChanged = RefreshText
            };
            RefreshText();
            eventLoop.RegisterClient(this);
        }

        private void RefreshText()
        {
            text.text = groupProcessor.Info;
        }

        public void ReceiveAction(ActionEvent @event)
        {
            groupProcessor.ReceiveAction(@event);
        }

        private void OnDestroy()
        {
            if (eventLoop != null)
            {
                eventLoop.RemoveClient(this);
            }
        }
    }
}