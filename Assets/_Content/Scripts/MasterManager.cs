using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterManager : MonoBehaviour
{
    public static MasterManager instance;

    public TextMeshProUGUI console;

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

    [HideInInspector]
    public CameraManager cameraManager;

    public GameObject defaultDirectionalLight;

    /// <summary>
    /// Determines which UI scene to load. The scene name must match the corresponding enum option exactly.
    /// </summary>
    enum UiScenes 
    {
        None,
        UI_Scene_Prototype,
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
        cameraManager = GetComponent<CameraManager>();
        LoadUI();
        //TODO: replace temporary
        //SceneManager.LoadScene("ModSelectionScene", LoadSceneMode.Additive);
        Debug.LogWarning($"{Screen.width}:{Screen.height}");
    }

    private void Update()
    {
        ProcessInput();
    }
    #endregion

    #region --- METHODS ---
    private void LoadUI()
    {
        if (uiScene != UiScenes.None)
        {
            string output = uiScene.ToString();
            Debug.Log($"Loading {output}...");
            SceneManager.LoadScene(output, LoadSceneMode.Additive);
        }

        hasLoadedUI = true;
    }

    private void UnloadUI()
    {
        string output = uiScene.ToString();
        SceneManager.UnloadSceneAsync(output);
        hasLoadedUI = false;
    }

    private void ProcessInput()
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

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            cameraManager.HideSelectionMenu();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            cameraManager.ShowSelectionMenu();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Selections.instance.UpdateSelectionMenu();
        }
    }

    public void ToggleVehicleLights()
    {
        if (ActiveVehicle != null && ActiveVehicle.lights != null)
        {
            if (ActiveVehicle.lights.activeSelf)
            {
                ActiveVehicle.lights.SetActive(false);
                if (defaultDirectionalLight != null)
                {
                    defaultDirectionalLight.SetActive(false);
                }
            }
            else
            {
                ActiveVehicle.lights.SetActive(true);
                if (defaultDirectionalLight != null)
                {
                    defaultDirectionalLight.SetActive(true);
                }
            }
        }
    }

    public void ToggleInfoPanel()
    {
        UI_Manager.instance.ToggleInfoPanel();
    }

    public void ToggleSettingsPanel()
    {
        UI_Manager.instance.ToggleSettingsPanel();
    }

    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// X-D
    /// </summary>
    /// <param name="input"></param>
    public static void cout(string input)
    {
        instance.console.text += $"\n{input}";
    }
    #endregion
}
