using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockItem : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI TitleText;
   

    // Update is called once per frame
    public void setItem(Image icon , string title)
    {
        Icon = icon;
        TitleText.text = title;
    }
}
