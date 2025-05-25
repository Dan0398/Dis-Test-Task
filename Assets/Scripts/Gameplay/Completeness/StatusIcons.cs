namespace Gameplay.Completeness
{
    public static class StatusIcons
    {
        public static string GetIcon(this Status status)
        {
            return status switch
            {
                Status.NotCompleted => "<sprite name=\"Not Completed\"/>",
                Status.Success => "<sprite name=\"Success\"/>",
                Status.Fail => "<sprite name=\"Fail\"/>",
                _ => throw new System.Exception("Trying to get icon for status \"" + status + "\". Not found.")
            };
        }
    }
}