using UnityEngine;

namespace Scenes.Training
{
    public class TrainingSceneController : MonoBehaviour, Gameplay.EventLoop.IClient
    {
        [SerializeField] private Gameplay.Steps.GroupOfSteps[] groups;
        [SerializeField] private View completeView;
        [SerializeField] private Sounds statusSounds;
        [SerializeField] private Gameplay.EventLoop.ActionsEventLoop eventLoop;
        private Gameplay.Completeness.GroupOfStepsProcessor[] groupProcessors;
        private TrainingState[] states;
        private int actualStateIndex;

        private void Start()
        {
            eventLoop.RegisterClient(this);
            Retry();
        }

        public void GoToLobby()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }

        public void Retry()
        {
            groupProcessors = new Gameplay.Completeness.GroupOfStepsProcessor[groups.Length];
            states = new TrainingState[groups.Length + 1];
            for (int i = 0; i < groups.Length; i++)
            {
                groupProcessors[i] = new(groups[i]);
                states[i] = new ProcessGroup(groupProcessors[i], completeView, IncrementStep);
            }
            states[^1] = new ShowResult(groupProcessors, completeView, Retry, GoToLobby);
            actualStateIndex = 0;
            states[actualStateIndex].Enter();
        }

        private void IncrementStep()
        {
            actualStateIndex++;
            states[actualStateIndex].Enter();
        }

        public void ReceiveAction(Gameplay.EventLoop.ActionEvent @event)
        {
            if (actualStateIndex < groupProcessors.Length)
            {
                groupProcessors[actualStateIndex].ReceiveAction(@event, out var status);
                if (status != Gameplay.Completeness.Status.NotCompleted && statusSounds != null)
                {
                    statusSounds.PlaySoundByStatus(status);
                }
            }
        }

        private void OnDestroy()
        {
            eventLoop.RemoveClient(this);
        }
    }
}