using System;
using System.Collections;
using System.Collections.Generic;
using BlockShift;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class XPPanelController : MonoBehaviour
{
    [SerializeField] private GameObject xpPanel;
    public Image image;

    public Sprite awesome_image;

    public Sprite combo_image;

    public Sprite great_image;

    public Sprite wow_image;

    private float min_rate = 0.2f;
    private int min_multis = 15;
    
    public TextMeshProUGUI textMeshPro;

    private int needXp;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        needXp = LevelManager.instance.totalXpNeeded;
    }


    public Sprite GetCorrectImage(int xpGained,int multiplier)
    {
        Sprite temp_sprite;
        float rate = (float)xpGained / (float)needXp;

        if (multiplier >= min_multis)
        {
            return combo_image;
        }

        if (rate > 0.4f)
        {
            temp_sprite = awesome_image;
        }
        else if (rate > 0.3f)
        {
            temp_sprite = great_image;
        }
        else if(rate > min_rate)
        {
            temp_sprite = wow_image;
        }
        else
        {
            return null;
        }

        return temp_sprite;

    }



    public void PopUpXPWinScreen(int xp,int multiplier)
    {
        if (LevelManager.instance.level.state == LevelState.Completed)
        {
            return;
        }
        GameManager.instance.ChangeGameState(GameState.Loading);
        int xpGained = xp * multiplier;
        float rate = (float)xpGained / (float)needXp;
        bool check = rate <= min_rate;
        Debug.LogFormat("Pop up rate : "+check+"   "+rate);
        if (rate <= min_rate)
        {
            return;
        }
        Debug.Log("Pop up rate : "+rate);
        Sprite sprite = GetCorrectImage(xpGained, multiplier);
        
        xpPanel.SetActive(true);
        //GameManager.instance.GetUIManager().SetMaskState(GameManager.instance.GetUIManager().GetMainMask(),true,GameManager.instance.OnXpPanelDown);
        image.sprite = sprite;
        
        int end = xpGained;
        int startAmount = 0;

        Vector3 scale = image.transform.localScale;
        image.transform.localScale = Vector3.zero;
        image.transform.DOScale(scale * 1.5f, 0.6f).OnComplete(
            ()=>image.transform.DOScale(scale,0.3f));
        DOTween.To(() => startAmount, x => startAmount = x, end, 1f).OnUpdate(
            () => ChangeAmount(startAmount)
        );
        

    }
    
    public void ChangeAmount(int amount)
    {
        textMeshPro.text = amount.ToString()+" XP";
    }

    private void ClosePanel()
    {
        xpPanel.SetActive(false);
        GameManager.instance.GetUIManager().SetMaskState(GameManager.instance.GetUIManager().GetMainMask(),false);
        StartCoroutine(WaitToPanelGone());
    }

    private IEnumerator WaitToPanelGone()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.ChangeGameState(GameState.Playing);
    }

    private void OnEnable()
    {
        //GameManager.instance.OnXpPanelDown += ClosePanel;
        GameManager.instance.OnEarnedXp += PopUpXPWinScreen;
    }

    private void OnDisable()
    {
        //GameManager.instance.OnXpPanelDown -= ClosePanel;
        GameManager.instance.OnEarnedXp -= PopUpXPWinScreen;
    }
}
