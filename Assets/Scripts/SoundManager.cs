using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance;

    public AudioSource menuMusic;
    public AudioSource buttonSound;
    public AudioSource flagSound;
    public AudioSource soundThunder;
    public AudioSource soundExplosion;
    public AudioSource soundBird;
    public AudioSource soundWin;

    private bool isSound = true;

    void Awake()
    {
        // there is one single public static instance of one class
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


	// Use this for initialization
	void Start () {
        StartMenuMusic();
    }


    public void StartMenuMusic()
    {
        SoundManager.Instance.menuMusic.loop = true;
        SoundManager.Instance.menuMusic.Play();
    }

    
    public void buttonTrigger()
    {
        SoundManager.Instance.buttonSound.Play();
    }


    public void flagTrigger()
    {
        SoundManager.Instance.flagSound.Play();
    }


    public void ToggleSound()
    {
        isSound = !isSound; // inverts the value. false -> true. true -> false
        AudioListener.volume = isSound ? 1f : 0f; // ? operator allows you to assing values depending on a boolean expression (expression?true:false;)
    }


    // play sound when losing the game - used in ingameManager
    public void loosingSounds()
    {
        int sceneID = SceneManager.GetActiveScene().buildIndex;

        switch (sceneID)
        {
            case 1:
                SoundManager.Instance.soundExplosion.Play();
                break;
            case 2:
                SoundManager.Instance.soundExplosion.Play();
                break;
            case 3:
                SoundManager.Instance.soundThunder.Play();
                break;
            default:
                SoundManager.Instance.soundExplosion.Play();
                break;
        }
    }


    // play sound when winning the game - used in ingameManager
    public void winningSounds()
    {
        int sceneID = SceneManager.GetActiveScene().buildIndex;

        switch (sceneID)
        {
            case 1:
                SoundManager.Instance.soundWin.Play();
                break;
            case 2:
                SoundManager.Instance.soundWin.Play();
                break;
            case 3:
                SoundManager.Instance.soundBird.Play();
                break;
            default:
                SoundManager.Instance.soundWin.Play();
                break;
        }
    }

}
