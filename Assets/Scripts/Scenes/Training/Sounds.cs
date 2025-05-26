using UnityEngine;

namespace Scenes.Training
{
    public class Sounds : MonoBehaviour
    {
        [SerializeField] private AudioSource SuccessSource, ErrorSource;

        public void PlaySoundByStatus(Gameplay.Completeness.Status status)
        {
            SuccessSource.Stop();
            ErrorSource.Stop();
            var targetSource = status == Gameplay.Completeness.Status.Success ? SuccessSource : ErrorSource;
            targetSource.Play();
        }
    }
}