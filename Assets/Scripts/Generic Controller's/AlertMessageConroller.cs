using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertMessageConroller : MonoBehaviour
{

    #region Instance

    private static AlertMessageConroller _instance;

    public static AlertMessageConroller instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AlertMessageConroller>();
            }

            return _instance;
        }
    }

    #endregion
    
    public GameObject AlertPopup;

    public TextMeshProUGUI alertText;
    private void Awake()
    {
        _instance = this;
    }

    // Update is called once per frame
    public void ShowPop(string msg)
    {
        AlertPopup.SetActive(true);
        alertText.text = msg;
    }
    
    public void ShowPop(string msg, float t)
    {
        AlertPopup.SetActive(true);
        alertText.text = msg;
        Invoke(nameof(HidePopup),t);
    }

    public void HidePopup()
    {
        AlertPopup.SetActive(false);
    }

}
