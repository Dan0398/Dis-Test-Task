using UnityEngine;

namespace Scenes.Training
{
    public class View : MonoBehaviour
    {
        [SerializeField] private GameObject usualParent;
        [SerializeField] private TMPro.TMP_Text usualTextContent;
        [SerializeField] private GameObject incrementButtonTurnable;
        [Space()]
        [SerializeField] private GameObject finalParent;
        [SerializeField] private TMPro.TMP_Text finalTextContent;
        private System.Action onRetry, onGoToLobby, onGoNext, onModeChanged;

        public void ShowAsUsual(Gameplay.Completeness.GroupOfStepsProcessor processor, System.Action OnGoNext)
        {
            onModeChanged?.Invoke();
            
            usualParent.SetActive(true);
            finalParent.SetActive(false);
            
            onGoNext = OnGoNext;
            onRetry = null;
            onGoToLobby = null;

            RefreshViewOfUsual();
            processor.OnChanged += RefreshViewOfUsual;
            onModeChanged = () => processor.OnChanged -= RefreshViewOfUsual;

            void RefreshViewOfUsual()
            {
                usualTextContent.text = processor.Info;
                incrementButtonTurnable.SetActive(processor.Completed);
            }
        }

        public void Button_IncrementStep()
        {
            onGoNext.Invoke();
        }

        public void ShowAsFinalScreen(string content, System.Action retry, System.Action goToLobby)
        {
            onModeChanged?.Invoke();
            onModeChanged = null;

            usualParent.SetActive(false);
            finalParent.SetActive(true);
            finalTextContent.text = content;
            
            onGoNext = null;
            onRetry = retry;
            onGoToLobby = goToLobby;
        }

        public void Button_Retry()
        {
            onRetry?.Invoke();
        }

        public void Button_GoToLobby()
        {
            onGoToLobby?.Invoke();
        }
    }
}