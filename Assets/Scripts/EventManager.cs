using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    public static EventManager Instance;

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


    //won any
    public delegate void OnWonHandler(int difficulty, int life, int rank);
    public static event OnWonHandler OnWonEvent;
    public static void OnWon(int difficulty, int life, int rank)
    {
        if (OnWonEvent != null) OnWonEvent(difficulty, life, rank);
    }


    //Record
    public delegate void OnRecordHandler(int difficulty);
    public static event OnRecordHandler OnRecordEvent;
    public static void OnRecord(int difficulty)
    {
        if (OnRecordEvent != null) OnRecordEvent(difficulty);
    }


    // Add watched
    public delegate void OnAddWatchedHandler();
    public static event OnAddWatchedHandler OnAddWatchedEvent;
    public static void OnAddWatched()
    {
        if (OnAddWatchedEvent != null) OnAddWatchedEvent();
    }

    // Lose
    public delegate void OnLoseHandler();
    public static event OnLoseHandler OnLoseEvent;
    public static void OnLose()
    {
        if (OnLoseEvent != null) OnLoseEvent();
    }

    // Life used
    public delegate void OnLifeUsedHandler();
    public static event OnLifeUsedHandler OnLifeUsedEvent;
    public static void OnLifeUsed()
    {
        if (OnLifeUsedEvent != null) OnLifeUsedEvent();
    }

}
