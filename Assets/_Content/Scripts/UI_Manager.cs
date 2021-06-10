using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class UI_Manager : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject settingsPanel;
    public TMP_Dropdown dropdown;
    public GameObject mainCanvas;

    [Header("Information")]
    [SerializeField] TextMeshProUGUI infoHeader;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI infoPrize;

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

        List<TMP_Dropdown.OptionData> dropdownList = new List<TMP_Dropdown.OptionData>();
        var loadedSprites = Resources.LoadAll<Sprite>(Dragoman.LocalizationDataPath);
        List<Sprite> loadedSpritesList = loadedSprites.OfType<Sprite>().ToList();

        foreach (string language in Dragoman.Languages)
        {
            Sprite sprite = loadedSpritesList.FirstOrDefault(x => x.name == language);
            dropdownList.Add(new TMP_Dropdown.OptionData(language, sprite));
        }

        dropdown.AddOptions(dropdownList);
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
                UpdateInfoPanel();
                mainCanvas.SetActive(false);
                if (CameraManager.instance.selectionState == CameraManager.SelectionState.Displayed)
                {
                    CameraManager.instance.HideSelectionMenu();
                }
            }
        }
    }

    private void UpdateInfoPanel()
    {
        infoHeader.text = MasterManager.ActiveVehicle.Name;
        infoText.text = MasterManager.ActiveVehicle.information.description;
        infoPrize.text = "$" + MasterManager.ActiveVehicle.information.price.ToString();
    }

    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            if (settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                settingsPanel.SetActive(true);
                if (CameraManager.instance.selectionState == CameraManager.SelectionState.Displayed)
                {
                    CameraManager.instance.HideSelectionMenu();
                }
                Time.timeScale = 0;
            }
        }
    }

    public void ChangeLanguage(int index)
    {
        Dragoman.instance.LoadLanguage(index);
    }


    #endregion
} 

