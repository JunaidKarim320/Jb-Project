using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedReason : MonoBehaviour
{
    public string FailReason;
    private GameOverController _gameOverController;
    // Start is called before the first frame update
    void Start()
    {

        _gameOverController = GameOverController.instance;
    }

    // Update is called once per frame
    public void QuitMission()
    {
        _gameOverController.display(FailReason);
    }
}
