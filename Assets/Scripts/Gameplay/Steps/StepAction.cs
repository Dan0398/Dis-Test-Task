using UnityEngine;

namespace Gameplay.Steps
{
    [System.Serializable]
    public class StepAction
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public string ExpectedAction { get; private set; }
        [field: SerializeField] public string Target { get; private set; }
    }
}