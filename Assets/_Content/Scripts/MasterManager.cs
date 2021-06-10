using System;
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

    [Tooltip("The parent transform for instantiated vehicles.")]
    [SerializeField] Transform activeVehiclesParent;
    public static Transform ActiveVehiclesParent => instance.activeVehiclesParent;

    [Tooltip("These prefabs are instantiated at start up to be viewed by the primary camera.")]
    [SerializeField] List<GameObject> availableVehicles = new List<GameObject>();
    public static List<GameObject> AvailableVehicles => instance.availableVehicles;

    Vehicle activeVehicle;
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

    #region --- EVENTS ---
    public delegate void OnActiveVehicleMaterialChangedEvent();
    public event OnActiveVehicleMaterialChangedEvent OnActiveVehicleMaterialChanged;
    #endregion

    #region --- UNITY CALLBACKS ---
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        InitializeVehicles();
    }

    private void Start()
    {
        cameraManager = GetComponent<CameraManager>();
        Debug.Log($"{Screen.width}:{Screen.height}");
        StartCoroutine(PostStart());
    }

    private void Update()
    {
        ProcessInput();
    }
    #endregion

    #region --- METHODS ---
    IEnumerator PostStart()
    {
        yield return new WaitForEndOfFrame();
        SetActiveVehicle();
    }

    private void InitializeVehicles()
    {
        if (ActiveVehiclesParent == null)
        {
            activeVehiclesParent = GameObject.Find("ActiveVehiclesParent").transform;
        }

        foreach (Transform child in ActiveVehiclesParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < availableVehicles.Count; i++)
        {
            var newVehicle = Instantiate(availableVehicles[i], ActiveVehiclesParent);
            if (i == 0)
            {
                newVehicle.SetActive(true);
                ActiveVehicle = newVehicle.GetComponent<Vehicle>();
            }
            else
            {
                newVehicle.SetActive(false);
            }
        }
    }

    private void ProcessInput()
    {
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

    public static void SetActiveVehicle(int index = 0)
    {
        for (int i = 0; i < ActiveVehiclesParent.childCount; i++)
        {
            if (i == index)
            {
                GameObject newActiveVehicle = ActiveVehiclesParent.GetChild(i).gameObject;
                newActiveVehicle.SetActive(true);
                ActiveVehicle = newActiveVehicle.GetComponent<Vehicle>();
            }
            else
            {
                ActiveVehiclesParent.GetChild(i).gameObject.SetActive(false);
            }
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
    }

    /// <summary>
    /// X-D
    /// </summary>
    /// <param name="input"></param>
    public static void cout(string input)
    {
        instance.console.text += $"\n{input}";
    }

    public void TriggerOnActiveVehicleMaterialChangedEvent()
    {
        OnActiveVehicleMaterialChanged?.Invoke();
    }

    #endregion
}
