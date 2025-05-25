using UnityEngine;

namespace Gameplay.Steps
{
    [System.Serializable]
    public class Step
    {
        [field:SerializeField] public string Name { get; private set; }
        [field: SerializeField] public StepAction[] Actions { get; private set; }
    }
}