using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveController : MonoBehaviour
{
    public Animator TextEffect;
    public GameObject ObjectivePopup;
    public TextMeshProUGUI ObjectiveText;
    public Image Icon;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public  void ShowObjective(string text, Sprite icon)
    {
        
        TextEffect.Play("Objective Text");
        ObjectivePopup.SetActive(true);
        ObjectiveText.text = text;
        Icon.sprite = icon;
    }
    public  void ShowObjective(string text)
    {
        TextEffect.Play("Objective Text");
        ObjectivePopup.SetActive(true);
        ObjectiveText.text = text;
    }
    public   void HidePopup()
    {
        ObjectivePopup.SetActive(false);
    }
}
