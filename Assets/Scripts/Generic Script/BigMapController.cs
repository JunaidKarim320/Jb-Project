using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMapController : MonoBehaviour
{
    public static GameObject instPlayerIcon;

    
    public GameObject bigMap;
    public GameObject GameplayControl;
    public MapUI mapUI;

    private Vector3 curPosBigMap;// current position of big map
    public Transform player;

        private InGameController _inGameController;
 
    // Start is called before the first frame update
    void Start()
    {
        curPosBigMap = mapUI.bigMapView.transform.position;
        instPlayerIcon = Instantiate(mapUI.playerIcon, Vector3.zero, Quaternion.identity) as GameObject;
        //player = AIContoller.manager.player;
                _inGameController = InGameController.instance;

    }
    void Update()
    {
        if (bigMap.activeSelf)
        {
            mapUI.bigMapView.orthographicSize = Mathf.Clamp(mapUI.bigMapView.orthographicSize, -500, -200);
        }
        else
        {
            instPlayerIcon.transform.rotation = Quaternion.Euler(90, player.eulerAngles.y, 0);
            instPlayerIcon.transform.position = new Vector3(player.transform.position.x,-0.5f , player.transform.position.z);//mapUI.mapPlane.position.y + 5
        }
    }
    public void ShowBigMap(bool active)
    {

        mapUI.bigMapView.transform.position=curPosBigMap;

        /*if (mapUI.mapPlane.GetComponent<MoveMap>())
            mapUI.mapPlane.GetComponent<MoveMap>().enabled = active;*/

        bigMap.SetActive(active);
        
        if (GameplayControl) GameplayControl.SetActive(!active);  // only for touch mode
       // if (TPSControl) //TPSControl.SetActive(!active);  // only for touch mode
       _inGameController.m_CarControllerUI.gameObject.SetActive(!active);
       _inGameController.m_ControllerUI.gameObject.SetActive(!active);
    }
    
    public void MapSize(float value)
    {
        mapUI.bigMapView.orthographicSize += value;
    }
}


[System.Serializable]

public class MapUI
{
    public GameObject playerIcon;
    public Camera bigMapView;
    public Transform mapPlane;
}