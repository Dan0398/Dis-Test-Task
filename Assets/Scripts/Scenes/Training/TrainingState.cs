namespace Scenes.Training
{
    public abstract class TrainingState
    {
        protected readonly View CompleteView;

        public TrainingState(View completeView)
        {
            CompleteView = completeView;
        }

        public abstract void Enter();
    }
}