using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace BlockShift
{

    public class CurrencyIndicator : MonoBehaviour
    {
        public CurrencyItemType currencyItemType;
        public TextMeshProUGUI amountText;
        public bool dynamicUpdate;
        
        private void Start()
        {
            int totalGold = CurrencyManager.instance.GetCurrencyItem(currencyItemType).amount;
            amountText.text = totalGold.ToString();
        }

        public void ChangeAmount(int amount)
        {
            amountText.text = amount.ToString();
        }

        public void SetCurrencyText(int startAmount,int endAmount,float duration)
        {
            int end = endAmount;
            DOTween.To(() => startAmount, x => startAmount = x, endAmount, duration).OnUpdate(
                () => ChangeAmount(startAmount)
                );
        }

        private void OnEnable()
        {
    
            if (dynamicUpdate)
            {
                ChangeAmount(CurrencyManager.instance.GetItemCount(currencyItemType));
                CurrencyManager.instance.GetCurrencyItem(currencyItemType).OnCurrencyAmountChange += ChangeAmount;
            }
        }

        private void OnDisable()
        {
            if (dynamicUpdate)
            {
                CurrencyManager.instance.GetCurrencyItem(currencyItemType).OnCurrencyAmountChange -= ChangeAmount;
            }
        } 
    }
}