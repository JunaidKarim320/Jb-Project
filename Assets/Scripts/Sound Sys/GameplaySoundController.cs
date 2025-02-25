﻿using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;
using UnityEngine;

public class GameplaySoundController : MonoBehaviour
{
    #region Instance
 
    private static GameplaySoundController _instance;

    public static GameplaySoundController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameplaySoundController>();
            }

            return _instance;
        }
    }
 
    #endregion
     
 
    private void OnEnable()
    {
        DataController.sfxVolumeChanged += sfxVolumeChanged;
        DataController.musicVolumeChanged += musicVolumeChanged;
        GameController.gamePaused += gamePaused;
        GameController.gameResumed += gameResumed;
        GameController.gameRestarted += gameRestarted;
    }
	
    private void OnDisable()
    {
        DataController.sfxVolumeChanged -= sfxVolumeChanged;
        DataController.musicVolumeChanged -= musicVolumeChanged;
        GameController.gamePaused -= gamePaused;
        GameController.gameResumed -= gameResumed;
        GameController.gameRestarted -= gameRestarted;
    }
    
    private void Awake()
    {
        _instance = this;
  
    }
    
    private void Start()
    {
        CreatePool();
        sfxVolumeChanged(DataController.instance.sfxVolume);
        musicVolumeChanged(DataController.instance.musicVolume);
        audioMusic.clip = musicSounds[Random.Range(0, musicSounds.Length)];
        audioMusic.Play();
        //DOTween.To(x=>audioMusic.volume=x,0f,DataController.instance.musicVolume ,1f );
    }

    public void CreatePool()
    {
        GameObject gameObject = new GameObject();
        gameObject.name = "AudioPool";
        audioList = new Dictionary<AudioLibrary, GameAudioPool>();
        foreach (GameAudioPool audioPool in pool)
        {
            audioPool.length = audioPool.clips.Length;
            audioPool.sources = new AudioSource[audioPool.length];
            for (int j = 0; j < audioPool.length; j++)
            {
                AudioClip audioClip = audioPool.clips[j];
                GameObject gameObject2 = new GameObject();
                gameObject2.name = audioClip.name;
                gameObject2.transform.parent = gameObject.transform;
                gameObject2.AddComponent<AudioSource>();
                gameObject2.GetComponent<AudioSource>().clip = audioClip;
                audioPool.sources[j] = gameObject2.GetComponent<AudioSource>();
            }
            audioList[audioPool.type] = audioPool;
        }
    }
    public void PlayNextMusic(int i)
    {
        if (musicSounds.Length > 0)
        {
            audioMusic.loop = true;
            audioMusic.volume = 0.6f;
            audioMusic.clip = musicSounds[i];
            audioMusic.Play();
            //DOTween.To(x=>audioMusic.volume=x,0f,DataController.instance.musicVolume ,1f );
        }
    }
    public void playFromPool(AudioLibrary audioType)
    {
        audioList[audioType].play();
    }
    public void sfxVolumeChanged(float newVolume)
    {
        sfxVolume = newVolume;
        foreach (KeyValuePair<AudioLibrary, GameAudioPool> keyValuePair in audioList)
        {
            keyValuePair.Value.setVolume(newVolume);
        }

        foreach (var obj in sfx_Audio)
        {
            obj.volume = newVolume;
        }
    }
    
    public void musicVolumeChanged(float newVolume)
    {
        musicVolume = newVolume;
        audioMusic.volume = newVolume;
		
    }
    public void gamePaused()
    {
      //  fadeOutAll();
      
    }
	
    public void gameResumed()
    {
      //  fadeInAll();
    }
    
    public void gameRestarted()
    {
      //  fadeInMusicVolume();
    }
    
    
    private float sfxVolume;
    
    private float musicVolume;
    
    public AudioSource audioMusic;

    public AudioClip[] musicSounds;
    
    public AudioSource[] sfx_Audio;
    
    private Dictionary<AudioLibrary, GameAudioPool> audioList;
    
    public GameAudioPool[] pool;



}
