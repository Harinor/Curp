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
    public GameObject helpPanel;
    public GameObject helpButton;
    public TMP_Dropdown dropdown;
    public TMP_Dropdown dropdownEnvironment;
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

        SetUpLanguageDropdown();
        SetUpEnvironmentDropdown();
    }

    #endregion

    #region --- METHODS ---
    public void SetUpLanguageDropdown()
    {
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

    public void SetUpEnvironmentDropdown()
    {
        dropdownEnvironment.ClearOptions();

        List<TMP_Dropdown.OptionData> dropdownList = new List<TMP_Dropdown.OptionData>();

        dropdownList.Add(new TMP_Dropdown.OptionData(string.Empty, EnvironmentManager.instance.none));
        dropdownList.Add(new TMP_Dropdown.OptionData(string.Empty, EnvironmentManager.instance.city));
        dropdownList.Add(new TMP_Dropdown.OptionData(string.Empty, EnvironmentManager.instance.nature));
        dropdown.value = 1;

        dropdownEnvironment.AddOptions(dropdownList);
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
        infoHeader.text = Dragoman.Lexicon(MasterManager.ActiveVehicle.Name);
        infoText.text = Dragoman.Lexicon(MasterManager.ActiveVehicle.information.description);
        infoText.text += "\n";
        foreach (Vehicle.Information.Specification spec in MasterManager.ActiveVehicle.information.specifications)
        {
            string nextLine = "\n" 
                + Dragoman.Lexicon(spec.name) + ": "
                + Dragoman.Lexicon(spec.value) + " " 
                + (String.IsNullOrEmpty(spec.unit) ? string.Empty : Dragoman.Lexicon(spec.unit));
            infoText.text += nextLine;
        }

        infoPrize.text = "$" + MasterManager.ActiveVehicle.information.price.ToString();
    }

    public void ToggleHelpPanel()
    {
        if (helpPanel != null)
        {
            if (helpPanel.activeSelf)
            {
                helpPanel.SetActive(false);
                mainCanvas.SetActive(true);
            }
            else
            {
                helpPanel.SetActive(true);
                UpdateInfoPanel();
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

    internal void ToggleHelpButton(bool value)
    {
        helpButton.SetActive(value);
    }


    #endregion
} 

