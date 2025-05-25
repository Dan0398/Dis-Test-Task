namespace Gameplay.Completeness
{
    public class GroupOfStepsProcessor: EventLoop.IClient
    {
        public string Info { get; private set; }
        public System.Action OnChanged;

        private readonly Steps.GroupOfSteps group;
        private readonly StepProcessor[] steps;
        private readonly System.Text.StringBuilder infoBuilder;
        private int actualStep;

        public GroupOfStepsProcessor(Steps.GroupOfSteps Group)
        {
            group = Group;
            steps = new StepProcessor[group.Steps.Length];
            for (int i = 0; i < steps.Length; i++)
            {
                steps[i] = new StepProcessor(group.Steps[i], ReactOnStepChanged);
            }
            actualStep = 0;
            infoBuilder = new System.Text.StringBuilder(64);
            ReactOnStepChanged();
        }

        private void ReactOnStepChanged()
        {
            if (actualStep >= steps.Length) return;
            SeekCompletenessNotActualStep();
            SeekActualStepCompleted();
            RefreshTextInfo();
            OnChanged?.Invoke();
        }

        private void SeekCompletenessNotActualStep()
        {
            if (!SteppedOver()) return;
            while (actualStep < steps.Length)
            {
                steps[actualStep].ForceSetAsFail();
                actualStep++;
            }

            bool SteppedOver()
            {
                for (int i = actualStep + 1; i < steps.Length; i++)
                {
                    if (steps[i].Status == Status.Success)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private void SeekActualStepCompleted()
        {
            if (actualStep >= steps.Length) return;
            if (steps[actualStep].Status == Status.NotCompleted) return;
            actualStep++;
            for (int i = actualStep + 1; i < steps.Length; i++)
            {
                steps[i].ForceSetAsDefault();
            }
        }

        private void RefreshTextInfo()
        {
            infoBuilder.Clear();
            infoBuilder.Append(group.Name).Append('\n');
            foreach (var step in steps)
            {
                step.FillInfo(infoBuilder);
            }
            Info = infoBuilder.ToString();
        }

        public void ReceiveAction(EventLoop.ActionEvent @event)
        {
            for (int i = actualStep; i < steps.Length; i++)
            {
                steps[i].ReceiveEvent(@event, i == actualStep);
            }
        }
    }
}