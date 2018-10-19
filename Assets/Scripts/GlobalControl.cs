using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;

    public int rows;
    public int columns;
    public int mines;


    public int difficulty; // 1 2 3 4 easy/medium/hard/vh

    public GameDataSaved LocalCopyOfData; // the class to serialize - containes the information between game runs


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

        GlobalControl.Instance.LoadData();  // load the data from file when menu is up
    }


    void Update()
    {
        Debug.Log("FT:" + PlayerPrefs.GetFloat("FlaggingTime"));
    }


    // set the grids from start menu
    public void gridSetEasy()
    {
        GlobalControl.Instance.rows = 4;
        GlobalControl.Instance.columns = 6;
        GlobalControl.Instance.mines = 4;
        GlobalControl.Instance.difficulty = 1;
    }

    public void gridSetMedium()
    {
        GlobalControl.Instance.rows = 8;
        GlobalControl.Instance.columns = 8;
        GlobalControl.Instance.mines = 10;
        GlobalControl.Instance.difficulty = 2;
    }


    public void gridSetHard()
    {
        GlobalControl.Instance.rows = 16;
        GlobalControl.Instance.columns = 16;
        GlobalControl.Instance.mines = 40;
        GlobalControl.Instance.difficulty = 3;
    }


    public void gridSetVeryHard()
    {
        GlobalControl.Instance.rows = 16;
        GlobalControl.Instance.columns = 30;
        GlobalControl.Instance.mines = 99;
        GlobalControl.Instance.difficulty = 4;
    }


    // load Levels
    public void loadScene()
    {
        int sceneID = SceneManager.GetActiveScene().buildIndex;  // get active scene

        if (sceneID == 0)  // if from main scene
        {
            switch (GlobalControl.Instance.LocalCopyOfData.styleGroupName)
            {
                case "ToggleStyle1":
                    SceneManager.LoadScene(1);
                    break;
                case "ToggleStyle2":
                    SceneManager.LoadScene(2);
                    break;
                case "ToggleStyle3":
                    SceneManager.LoadScene(3);
                    break;
                default:
                    SceneManager.LoadScene(1);
                    break;
            }
        }

        if (sceneID != 0) {
            SceneManager.LoadScene(0);
            GlobalControl.Instance.difficulty = 0;
        }  // from other scenes always load main scene      
    }


    // save the gamedata for offline
        public void SaveData()
    {
        BinaryFormatter formatter = new BinaryFormatter();  // handle the serialization work
        FileStream file = File.Create(Application.persistentDataPath + "/gameData.gd");  //  essentially a pathway to a new file that we can send data
        formatter.Serialize(file, LocalCopyOfData);  // writing a class in its raw binary form - savefile is the file to save, localcopyofdata is theclass with the data
        file.Close();
        Debug.Log("DATA SAVED");
    }


    // load the data from offline
    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/gameData.gd"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameData.gd", FileMode.Open);
            LocalCopyOfData = (GameDataSaved)formatter.Deserialize(file);
            file.Close();
        }
    }
} // END
