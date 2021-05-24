using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SharedData : MonoBehaviour
{
    public static SharedData instance;

    private Transform _mainPanel;
    public static Transform MainPanel
    {
        get { return instance._mainPanel; }
        set { instance._mainPanel = value; }
    }


    [SerializeField] Vehicle activeVehicle;
    public static Vehicle ActiveVehicle
    {
        get { return instance.activeVehicle; }
        set { instance.activeVehicle = value; }
    }

    private bool isIdle;
    public static bool IsIdle
    {
        get { return instance.isIdle; }
        set { instance.isIdle = value; }
    }

    /// <summary>
    /// Determines which UI scene to load. The scene name must match the corresponding enum option exactly.
    /// </summary>
    enum UiScenes 
    {
        MainScene = 0,
        UI_Scene_Prototype = 1,
    }
    [SerializeField] UiScenes uiScene = UiScenes.UI_Scene_Prototype;

    #region --- UNITY CALLBACKS ---
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (activeVehicle == null)
        {
            activeVehicle = FindObjectOfType<Vehicle>();
        }
    }

    private void Start()
    {
        LoadUI();
    }

    #endregion

    private void LoadUI()
    {
        string output = uiScene.ToString();
        Debug.Log($"Loading {output}...");
        SceneManager.LoadScene(output, LoadSceneMode.Additive);
    }

}
