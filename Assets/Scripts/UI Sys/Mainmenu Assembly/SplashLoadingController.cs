
using UnityEngine;

public class SplashLoadingController : MonoBehaviour

{
    #region Instance
 
    private static SplashLoadingController _instance;

    public static SplashLoadingController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SplashLoadingController>();
            }

            return _instance;
        }
    }
 
    #endregion
    
    
    #region Initialization

     SceneMngmtController _sceneMngmtController;
     public GameObject[] Loadings;

     public int TimeToLoad;
    #endregion
    

    private void Awake()
    {
        _instance = this;
  
    }


    private void Start()
    {
        _sceneMngmtController = SceneMngmtController.instance;

        if (!PlayerPrefs.HasKey("RestorePrefs"))
        {
           PlayerPrefs.DeleteAll();
           PlayerPrefs.SetString("RestorePrefs","Restored");
        }
        
        
        Invoke(nameof(LoadScene),TimeToLoad);
        for (int i = 0; i < Loadings.Length; i++)
        {
            Loadings[i].SetActive(false);
        }
        int x = Random.Range(0, 2);
        Loadings[x].SetActive(true);
    }

    public void LoadScene()
    {
        
        _sceneMngmtController.LoadScene(Scenes.Mainmenu);
    }
}
