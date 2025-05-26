using Gameplay.Completeness;

namespace Scenes.Training
{
    public class ProcessGroup : TrainingState
    {
        private GroupOfStepsProcessor group;
        private System.Action incrementStep;

        public ProcessGroup(GroupOfStepsProcessor Group, View CompleteView, System.Action IncrementStep) :
            base(CompleteView)
        {
            group = Group;
            incrementStep = IncrementStep;

        }

        public override void Enter()
        {
            CompleteView.ShowAsUsual(group, incrementStep);
        }
    }
}