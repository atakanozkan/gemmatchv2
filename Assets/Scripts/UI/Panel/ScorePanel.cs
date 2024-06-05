using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace BlockShift
{
    public class ScorePanel : MonoBehaviour
    {
        public TextMeshProUGUI textMeshPro;

        private int totalChange;
        // Start is called before the first frame update
        void Start()
        {
            textMeshPro.text = "0 XP";
        }

        private void ChangeAmount(int amount,int multiplier,bool showMultiplier)
        {
            if (showMultiplier)
            {
                textMeshPro.text = multiplier+"X x "+ amount.ToString()+ " XP";
            }
            else
            {
                textMeshPro.text = amount + " XP";
            }


        }

        public void SetEarnedXpText(int endAmount,int multiplier)
        {
            int startAmount = 0;
            if (multiplier > 1)
            {
                DOTween.To(() => startAmount, x => startAmount = x, endAmount, 1f).OnUpdate(
                        () => ChangeAmount(startAmount, multiplier, true)).
                    OnComplete(()=>
                        DOTween.To(() => startAmount, x => startAmount = x, endAmount, 0.2f).OnUpdate(
                            () => ChangeAmount(startAmount*multiplier,1,false)
                        ));
            }

            else
            {
                DOTween.To(() => startAmount, x => startAmount = x, endAmount, 1f).OnUpdate(
                    () => ChangeAmount(startAmount*multiplier, multiplier, false));

            }


        }

        private void OnEnable()
        {
            GameManager.instance.OnEarnedXp += SetEarnedXpText;
        }
        private void OnDisable()
        {
            GameManager.instance.OnEarnedXp -= SetEarnedXpText;
        }
    }
}

