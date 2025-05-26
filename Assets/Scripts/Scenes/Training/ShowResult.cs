namespace Scenes.Training
{
    public class ShowResult : TrainingState
    {
        private readonly Gameplay.Completeness.GroupOfStepsProcessor[] processedGroups;
        private readonly System.Action retry;
        private readonly System.Action goToLobby;

        public ShowResult(Gameplay.Completeness.GroupOfStepsProcessor[] ProcessedGroups,
                          View CompleteView,
                          System.Action Retry,
                          System.Action GoToLobby) : base(CompleteView)
        {
            processedGroups = ProcessedGroups;
            retry = Retry;
            goToLobby = GoToLobby;
        }

        public override void Enter()
        {
            var builder = new System.Text.StringBuilder();
            foreach (var step in processedGroups)
            {
                builder.Append(step.GetOneLineInfo()).Append('\n');
            }
            CompleteView.ShowAsFinalScreen(builder.ToString(), retry, goToLobby);
        }
    }
}