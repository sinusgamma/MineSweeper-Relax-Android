using UnityEngine;

public class DontDestroy : MonoBehaviour {

    public static DontDestroy Instance;

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
}
