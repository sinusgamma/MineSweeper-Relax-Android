using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlaggingSettings : MonoBehaviour {

    public Slider flaggingSlider;
    public Text flaggingText;

    public void setFlaggingTime()
    {
        PlayerPrefs.SetFloat("FlaggingTime", flaggingSlider.value);
    }


	// Use this for initialization
	void Start () {
        if (PlayerPrefs.GetFloat("FlaggingTime") > 0.09f) // if no or wrong data in playerprefs => set default
        {
            flaggingSlider.value = PlayerPrefs.GetFloat("FlaggingTime");
        }
        else
        {
            flaggingSlider.value = 0.4f;
        }
        setFlaggingTime();
    }
	
	// Update is called once per frame
	void Update () {
        flaggingText.text = flaggingSlider.value.ToString("0.00") + " sec (default: 0.40)";  // write the actual time under slider
	}
}
