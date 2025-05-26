using Gameplay.EventLoop;
using System;

namespace Gameplay.Completeness
{
    public class ActionProcessor
    {
        private readonly Steps.StepAction stepAction;
        private readonly System.Action onRefresh;
        private Status status;

        public Status Status => status;

        public ActionProcessor(Steps.StepAction StepAction, System.Action OnRefresh)
        {
            stepAction = StepAction;
            onRefresh = OnRefresh;
        }

        public void FillInfo(System.Text.StringBuilder infoBuilder)
        {
            infoBuilder.Append(' ').Append(' ').Append(status.GetIcon()).Append(stepAction.Description).Append('\n');
        }

        public void SetAsFailIfNotComplete()
        {
            if (status == Status.NotCompleted)
            {
                status = Status.Fail;
            }
        }

        public void SetAsDefault()
        {
            status = Status.NotCompleted;
        }

        public bool IsReceivedEvent(ActionEvent @event, bool allowFail, out Status resultStaus)
        {
            resultStaus = Status.NotCompleted;
            if (status != Status.NotCompleted) return false;
            if (@event.ExpectedAction.Equals(stepAction.ExpectedAction, StringComparison.InvariantCultureIgnoreCase))
            {
                var success = @event.Target.Equals(stepAction.Target, StringComparison.InvariantCultureIgnoreCase);
                if (!success && !allowFail) return false;
                status = success ? Status.Success : Status.Fail;
                resultStaus = status;
                onRefresh();
                return true;
            }
            return false;
        }
    }
}