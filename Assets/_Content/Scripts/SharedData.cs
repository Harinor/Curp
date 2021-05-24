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

    [Header("Cameras")]
    public Camera primaryCamera;
    public Camera secondaryCamera;

    /// <summary>
    /// Determines which UI scene to load. The scene name must match the corresponding enum option exactly.
    /// </summary>
    enum UiScenes 
    {
        MainScene = 0,
        UI_Scene_Prototype = 1,
    }
    [Header("UI")]
    [SerializeField] UiScenes uiScene = UiScenes.UI_Scene_Prototype;
    bool hasLoadedUI = false;

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
        //TODO: replace temporary
        //SceneManager.LoadScene("ModSelectionScene", LoadSceneMode.Additive);
        Debug.LogWarning($"{Screen.width}:{Screen.height}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (hasLoadedUI)
            {
                UnloadUI();
            }
            else
            {
                LoadUI();
            }
        }
    }

    #endregion

    private void LoadUI()
    {
        string output = uiScene.ToString();
        Debug.Log($"Loading {output}...");
        SceneManager.LoadScene(output, LoadSceneMode.Additive);
        hasLoadedUI = true;
    }

    private void UnloadUI()
    {
        string output = uiScene.ToString();
        SceneManager.UnloadSceneAsync(output);
        hasLoadedUI = false;
    }

}
