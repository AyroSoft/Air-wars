using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HoverGame.UI
{
    public class SelectLevelMenu : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button startBtn;
        [SerializeField] private Button exitBtn;
        [SerializeField] private Button twitterBtn;

        [SerializeField] private ToggleLevelButton[] toggleLevelBtns;

        [Header("Settings")]
        [SerializeField] private string twitterUrl;

        private ToggleLevelButton selectedBtn;

        private void OnEnable()
        {
            foreach (var btn in toggleLevelBtns)
                btn.OnSelected += SelectedBtnHandle;

            startBtn.onClick.AddListener(StartClick);
            exitBtn.onClick.AddListener(ExitClick);
            twitterBtn.onClick.AddListener(TwitterClick);

            toggleLevelBtns[0].IsSelected = true;
        }

        private void TwitterClick()
        {
            Application.OpenURL(twitterUrl);
        }

        private void ExitClick()
        {
            Application.Quit();
        }

        private void StartClick()
        {
            SceneManager.LoadScene(selectedBtn.GetLevelName());
        }

        private void SelectedBtnHandle(ToggleLevelButton clickedBtn)
        {
            selectedBtn = clickedBtn;
            foreach (var btn in toggleLevelBtns)
                if (btn != selectedBtn)
                    btn.IsSelected = false;
        }

        private void OnDisable()
        {
            foreach (var btn in toggleLevelBtns)
                btn.OnSelected -= SelectedBtnHandle;

            startBtn.onClick.RemoveListener(StartClick);
            exitBtn.onClick.RemoveListener(ExitClick);
            twitterBtn.onClick.RemoveListener(TwitterClick);
        }
    }
}