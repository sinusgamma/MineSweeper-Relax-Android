using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/*************************************

**************************************/


public class volumeSliders : MonoBehaviour {

    public Slider musicSlider;
    public Slider soundSlider;


    // preset the value sliders according to the actual sound level
    /*
    public void presetSliders()
    {
        musicSlider.value = SoundManager.Instance.menuMusic.volume;  //  set the sliders according to the music
        soundSlider.value = SoundManager.Instance.buttonSound.volume;  // set the slider according to a sound
    }
    */


    // call from slider - set music level
    public void setMusicVolume()
    {
        SoundManager.Instance.menuMusic.volume = musicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }


    // call from slider - set sound level
    public void setSoundVolume()
    {
        SoundManager.Instance.buttonSound.volume = soundSlider.value;
        SoundManager.Instance.flagSound.volume = soundSlider.value;
        SoundManager.Instance.soundBird.volume = soundSlider.value;
        SoundManager.Instance.soundExplosion.volume = soundSlider.value;
        SoundManager.Instance.soundThunder.volume = soundSlider.value;
        SoundManager.Instance.soundWin.volume = soundSlider.value;
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
    }


    // save sound settings
    public void saveAudioSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.Save();
    }
    
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        setMusicVolume();
        setSoundVolume();
    }
}
