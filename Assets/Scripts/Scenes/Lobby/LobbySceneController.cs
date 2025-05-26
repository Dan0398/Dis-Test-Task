using UnityEngine;

namespace Scenes.Training
{
    public class LobbySceneController : MonoBehaviour
    {
        public void Button_GoToTraining()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Training");
        }
    }
}