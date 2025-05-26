using Gameplay.EventLoop;

namespace Gameplay.Completeness
{
    public class StepProcessor
    {
        private readonly Steps.Step step;
        private readonly ActionProcessor[] actions;
        private readonly System.Action onRefreshed;
        private int actualAction;
        private Status status;

        public Status Status => status;

        public StepProcessor(Steps.Step Step, System.Action OnRefreshed)
        {
            step = Step;
            actions = new ActionProcessor[step.Actions.Length];
            for (int i = 0; i < step.Actions.Length; i++)
            {
                actions[i] = new(step.Actions[i], RefreshCompleteness);
            }
            onRefreshed = OnRefreshed;
            actualAction = 0;
        }

        private void RefreshCompleteness()
        {
            if (actions[actualAction].Status == Status.NotCompleted) return;
            if (actions[actualAction].Status == Status.Fail)
            {
                status = Status.Fail;
            }
            else
            {
                actualAction++;
                if (actualAction >= actions.Length)
                {
                    status = Status.Success;
                }
            }
            onRefreshed();
        }

        public void FillInfo(System.Text.StringBuilder infoBuilder)
        {
            infoBuilder.Append(' ').Append(status.GetIcon()).Append(step.Name).Append('\n');
            foreach (var action in actions)
            {
                action.FillInfo(infoBuilder);
            }
        }

        public void ForceSetAsFail()
        {
            status = Status.Fail;
            foreach (var action in actions)
            {
                action.SetAsFailIfNotComplete();
            }
        }

        public void ForceSetAsDefault()
        {
            foreach (var action in actions)
            {
                action.SetAsDefault();
            }
            actualAction = 0;
        }

        public bool IsReceivedEvent(ActionEvent @event, bool allowFail, out Status resultStatus)
        {
            return actions[actualAction].IsReceivedEvent(@event, allowFail, out resultStatus);
        }
    }
}