using UnityEngine;

namespace Scenes.Training
{
    public class Sounds : MonoBehaviour
    {
        [SerializeField] private AudioSource successSource, errorSource;

        public void PlaySoundByStatus(Gameplay.Completeness.Status status)
        {
            successSource.Stop();
            errorSource.Stop();
            var targetSource = status == Gameplay.Completeness.Status.Success ? successSource : errorSource;
            targetSource.Play();
        }
    }
}