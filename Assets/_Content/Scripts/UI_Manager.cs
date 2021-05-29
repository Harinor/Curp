using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject mainCanvas;

    public static UI_Manager instance;

    #region --- UNITY CALLBACKS ----
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    #region --- PUBLIC METHODS ---
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
    #endregion
} 

