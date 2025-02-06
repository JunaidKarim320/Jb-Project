using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadioController : MonoBehaviour
{

    [SerializeField] private string[] m_soundName;
    [SerializeField] private TextMeshProUGUI m_soundNameText;

    private int musicIndex;
    
    private GameplaySoundController _gameplaySoundController;
    // Start is called before the first frame update
    void Start()
    {
        _gameplaySoundController= GameplaySoundController.instance;
        m_soundNameText.text = m_soundName[musicIndex];

    }

    public void ChangeSound()
    {
        musicIndex++;
        
        _gameplaySoundController.PlayNextMusic(musicIndex);
        m_soundNameText.text = m_soundName[musicIndex];
        if (musicIndex == _gameplaySoundController.musicSounds.Length-1)
            musicIndex = -1;
    }
}
