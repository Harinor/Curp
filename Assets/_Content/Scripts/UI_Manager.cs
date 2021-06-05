using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UI_Manager : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject settingsPanel;
    public TMP_Dropdown dropdown;
    public GameObject mainCanvas;

    public static UI_Manager instance;

    #region --- UNITY CALLBACKS ----
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        SetUp();
    }

    #endregion

    #region --- METHODS ---
    public void SetUp()
    {
        if (dropdown == null && settingsPanel != null)
        {
            dropdown = GetComponentInChildren<TMP_Dropdown>();
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(Dragoman.instance.languages);
    }

    public void ToggleInfoPanel()
    {
        if (infoPanel != null)
        {
            if (infoPanel.activeSelf)
            {
                infoPanel.SetActive(false);
                mainCanvas.SetActive(true);
            }
            else
            {
                infoPanel.SetActive(true);
                mainCanvas.SetActive(false);
                if (CameraManager.instance.selectionState == CameraManager.SelectionState.Displayed)
                {
                    CameraManager.instance.HideSelectionMenu();
                }
            }
        }
    }

    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            if (settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
            }
            else
            {
                settingsPanel.SetActive(true);
                if (CameraManager.instance.selectionState == CameraManager.SelectionState.Displayed)
                {
                    CameraManager.instance.HideSelectionMenu();
                }
            }
        }
    }

    public void ChangeLanguage(int index)
    {
        //Debug.Log("Dropdown" + index);
        Dragoman.instance.LoadLanguage(index);
    }


    #endregion
} 

