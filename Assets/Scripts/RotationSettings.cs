using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RotationSettings : MonoBehaviour {

    public Slider rotationSlider;

    // save rotation settings
    public void saveRotationSettings()
    {
        PlayerPrefs.SetFloat("RotationType", rotationSlider.value);
        PlayerPrefs.Save();
    }


    // set rotation settings
    public void setRotationType()
    {
        switch ((int)rotationSlider.value)
        {
            case 0:  // auto
                Screen.orientation = ScreenOrientation.AutoRotation;
                PlayerPrefs.SetFloat("RotationType", rotationSlider.value);
                break;
            case 1:  // portrait
                Screen.orientation = ScreenOrientation.Portrait;
                PlayerPrefs.SetFloat("RotationType", rotationSlider.value);
                break;
            case 2:  // landscape
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                PlayerPrefs.SetFloat("RotationType", rotationSlider.value);
                break;
            default:
                Screen.orientation = ScreenOrientation.AutoRotation;
                break;
        }
    }

    // Use this for initialization
    void Start () {
        rotationSlider.value = PlayerPrefs.GetFloat("RotationType");
        setRotationType();
    }
}
