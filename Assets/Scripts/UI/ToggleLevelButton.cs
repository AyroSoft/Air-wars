using System;
using UnityEngine;
using UnityEngine.UI;

namespace HoverGame.UI
{
    public class ToggleLevelButton : MonoBehaviour
    {
        public event Action<ToggleLevelButton> OnSelected;

        [SerializeField] private Image hightlight;
        [SerializeField] private string levelName;

        private bool isSelected;
        public bool IsSelected 
        { 
            get { return isSelected; } 
            set 
            { 
                isSelected = value;
                hightlight.gameObject.SetActive(isSelected);
                if (isSelected)
                    OnSelected?.Invoke(this);
            } 
        }

        private Button btn;

        private void Awake()
        {
            btn = GetComponent<Button>();
        }

        private void OnEnable()
        {
            btn.onClick.AddListener(ClickHandle);
        }

        private void ClickHandle()
        {
            Handheld.Vibrate();
            IsSelected = true;
        }

        private void OnDisable()
        {
            btn.onClick.RemoveListener(ClickHandle);
        }

        public string GetLevelName()
        {
            return levelName;
        }
    }
}