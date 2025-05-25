using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.EventLoop
{
    public class ActionsEventLoop : MonoBehaviour
    {
        private readonly List<IClient> loopClients = new();

        public void RegisterClient(IClient client)
        {
            if (client == null)
            {
                throw new System.ArgumentNullException("Trying to register Event loop client. Client is null.");
            }
            if (loopClients.Contains(client))
            {
                throw new System.Exception("Trying to register Event loop client. Client already registered.");
            }
            loopClients.Add(client);
        }

        public void RemoveClient(IClient client)
        {
            if (client == null)
            {
                throw new System.ArgumentNullException("Trying to remove Event loop client. Client is null.");
            }
            if (!loopClients.Remove(client))
            {
                throw new System.Exception("Trying to remove Event loop client. Client not registered at clients.");
            }
        }

        public void Call(ActionEvent @event)
        {
#if UNITY_EDITOR
            Debug.Log("Event created: " + @event.ExpectedAction + ": " + @event.Target);
#endif
            foreach (var client in loopClients)
            {
                client.ReceiveAction(@event);
            }
        }

        private void Start()
        {
            var allActions = GameObject.FindObjectsByType<BaseAction>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var action in allActions)
            {
                action.Bind(this);
            }
        }
    }
}