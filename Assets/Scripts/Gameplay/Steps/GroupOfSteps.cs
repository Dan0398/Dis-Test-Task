using UnityEngine;

namespace Gameplay.Steps
{
    [CreateAssetMenu(menuName = "Dan398/Group of steps")]
    public class GroupOfSteps : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Step[] Steps { get; private set; }
    }
}