using System;
using System.Collections;
using System.Collections.Generic;
//using JUTPS.InventorySystem;
//using JUTPS.ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RewardedPopController : MonoBehaviour
{

    #region Instance

    private static RewardedPopController _instance;

    public static RewardedPopController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RewardedPopController>();
            }

            return _instance;
        }
    }

    #endregion
    
    public GameObject RewardedPopup;
    public TextMeshProUGUI PopupText;
    
    public GameObject RandomRewardedPopup;
    public Image Icon;
    public TextMeshProUGUI Title;

    public GameObject CashRewardedPopup;

    //private JUInventory _juInventory;
    public delegate void CarIn();

    public static event CarIn m_CarIn;

    private void Awake()
    {
        _instance = this;
        //_juInventory = FindObjectOfType<JUInventory>();
        if (ApplicationController.LastSelectedLevel >= 5)
        {
            Invoke("ShowRandomRewarded",60f);
            Invoke("ShowCashRewarded",30f);
       }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            CarInReward();
    }

    public void ShowPop()
    {
        RewardedPopup.SetActive(true);
    }
    
    public void ShowPop(string msg)
    {
        RewardedPopup.SetActive(true);
        PopupText.text = msg;
    }
    
    public void HidePopup()
    {
        RewardedPopup.SetActive(false);
    }
    
    public void CarInReward()
    {
        m_CarIn?.Invoke();
    }


    private int RandomX;
    public void ShowRandomRewarded()
    {
        /*if(_juInventory.LockedItem.Count>0)
        {
             RandomRewardedPopup.SetActive(true);
             RandomX= Random.Range(0, _juInventory.LockedItem.Count);
             HoldableItem holdableItem= _juInventory.LockedItem[RandomX];
             Icon.sprite = holdableItem.ItemIcon;
             Title.text = holdableItem.ItemName;
        }*/
    }


    /*public void RewardGun()
    {
        HoldableItem holdableItem= _juInventory.LockedItem[RandomX];
        holdableItem.Unlocked = true;
        _juInventory.EquipItem(_juInventory.LockedItem[RandomX].ItemSwitchID);
        RandomRewardedPopup.SetActive(false);
        Invoke("ShowRandomRewarded",60f);
    }*/
    /*public void CloseRandomRewarded()
    {
        RandomRewardedPopup.SetActive(false);
        Invoke("ShowRandomRewarded",60f);
        if(AdmobAdsManager.Instance)
            AdmobAdsManager.Instance.ShowInterstitial();
    }*/
    
    
    public void ShowCashRewarded()
    {
        CashRewardedPopup.SetActive(true);
        Invoke("HideCashRewarded",10f);
    }
    
    public void HideCashRewarded()
    {
        CashRewardedPopup.SetActive(false);
        Invoke("ShowCashRewarded",30f);

    }
}
