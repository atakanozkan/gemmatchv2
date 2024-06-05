using UnityEngine;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UnityEngine.UI;

namespace BlockShift
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup winPanel;
        [SerializeField] private Image mainMask;
        public TextMeshProUGUI textMesh;
        public void SetMaskState(Image mask, bool isActive, Action onClickAction = null)
        {
            if (isActive)
            {
                mask.gameObject.SetActive(true);
                textMesh.text = LevelManager.instance.levelCount.ToString();
                SetMaskClickAction(mask, onClickAction);

                //mask.DOFade(0.5f, 0.15f).From(0f);
            }
            else
            {
                mask.gameObject.SetActive(false);
            }
        }

        private void SetMaskClickAction(Image mask, Action action)
        {
            EventTrigger trigger = mask.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            trigger.triggers.Clear();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { action?.Invoke(); });
            trigger.triggers.Add(entry);
        }

        public void FadeInWinPanel()
        {

            winPanel.gameObject.SetActive(true);
   
        }

        public Image GetMainMask()
        {
            return mainMask;
        }
        
        private void OnEnable()
        {
            GameManager.instance.SetUIManager(this);
        }
        
        
        
        private void OnDisable()
        {
        }
    }
}

