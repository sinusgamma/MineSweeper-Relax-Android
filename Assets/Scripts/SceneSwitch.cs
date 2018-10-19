using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitch : MonoBehaviour {

    public void LoadShop()
    {
        SceneManager.LoadScene(4);
    }

    public void LoadStart()
    {
        SceneManager.LoadScene(0);
    }
}