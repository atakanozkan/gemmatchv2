using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BlockShift
{
    public class ScrollBarManager : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        public float maxValue= 1f;
        public int tempXp;
        private void Start()
        {
            textMeshProUGUI.text = "LEVEL " + LevelManager.instance.levelCount;

            tempXp = LevelManager.instance.GetTotalXp();
            
            ChangeAmount(tempXp,LevelManager.instance.totalXpNeeded);
        }

        public void ChangeLevelText()
        {
            textMeshProUGUI.text = "LEVEL " + LevelManager.instance.levelCount;
            tempXp = 0;
            ChangeAmount(LevelManager.instance.totalXpNeeded,LevelManager.instance.totalXpNeeded);
        }
        
        public void ChangeAmount(int amount, int neededXp)
        {
            image.fillAmount = (float) ((float)amount / (float)neededXp);
        }

        public void SetXp(int endAmount,int multiplier)
        {
            endAmount *= multiplier;
            int startAmount = tempXp;
            tempXp = startAmount + endAmount;
            int neededXp = LevelManager.instance.totalXpNeeded;
            DOTween.To(() => startAmount, x => startAmount = x, startAmount+endAmount, 1f).OnUpdate(
                () => ChangeAmount(startAmount, neededXp)
            );
        }
        
        private void OnEnable()
        {
            GameManager.instance.OnEarnedXp += SetXp;
            GameManager.instance.OnLevelUp += ChangeLevelText;
        }
        private void OnDisable()
        {
            GameManager.instance.OnEarnedXp -= SetXp;
            GameManager.instance.OnLevelUp -= ChangeLevelText;
        }
    }
}