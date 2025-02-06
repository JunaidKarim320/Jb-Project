
using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingController : MonoBehaviour
{
    #region Instance
 
    private static LoadingController _instance;

    public static LoadingController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LoadingController>();
            }

            return _instance;
        }
    }
 
    #endregion
    
    [Header("Panel Container")]
    public Canvas containerGO;
    public string[] Qoutation;
    public GameObject[] Loadings;
    public TextMeshProUGUI[] LoadingText;

    public int TimeToLoad;
    
    private SceneMngmtController _sceneMngmtController;
 
    private void Awake()
    {
        _instance = this;

    }

    public void Start()
    {
    _sceneMngmtController=SceneMngmtController.instance;
        
    }

    public void PrintIndexes()
    {
        print(ApplicationController.SelectedLevel);
        print(ApplicationController.SelectedGameMode);
        print(ApplicationController.SelectedInventoryItem);
    }

    public void display(Scenes s )
    {
        containerGO.enabled = true;
        StartCoroutine(LoadScene(s));
        for (int i = 0; i < Loadings.Length; i++)
        {
            Loadings[i].SetActive(false);
        }
        int x = Random.Range(0, Loadings.Length);
        Loadings[x].SetActive(true);
        LoadingText[x].text = Qoutation[Random.Range(0, Qoutation.Length)];

        /*if (AdmobAdsManager.Instance)
        {
            AdmobAdsManager.Instance.ShowInterstitial();
            AdmobAdsManager.Instance.ShowLargeLeftBanner();
            AdmobAdsManager.Instance.HideTopleftBanner();
        }*/
    }

    public IEnumerator LoadScene(Scenes s)
    {
        yield return new WaitForSeconds(TimeToLoad);
        _sceneMngmtController.LoadScene(s);
    }

    public void hide()
    {
        containerGO.enabled = false;

    }
 
 

}
