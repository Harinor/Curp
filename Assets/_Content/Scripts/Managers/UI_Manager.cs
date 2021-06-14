using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;
using System.Text;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    public GameObject infoPanel;
    public GameObject settingsPanel;
    /// <summary>
    /// This is the full menu with descriptions
    /// </summary>
    public GameObject helpPanel;
    /// <summary>
    /// This is the compact menu panel
    /// </summary>
    public GameObject burgerPanel;

    public TMP_Dropdown dropdown;
    public TMP_Dropdown dropdownEnvironment;
    public GameObject mainCanvas;

    public CanvasScaler scaler;

    [Header("Information")]
    [SerializeField] TextMeshProUGUI infoHeader;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI infoPrize;

    private bool showingMainMenu = false;
    private bool compactMenuEnabled = false;

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

    private void Update()
    {
        if (IsWidescreen())
        {
            scaler.matchWidthOrHeight = 1;
        }
        else
        {
            scaler.matchWidthOrHeight = 0;
        }
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
        //-- HEADER ---
        infoHeader.text = Dragoman.Lexicon(MasterManager.ActiveVehicle.Name);

        //---TEXT---
        StringBuilder descriptionBuilder = new StringBuilder(300);
        descriptionBuilder.Append("<i>");
        descriptionBuilder.Append(Dragoman.Lexicon(MasterManager.ActiveVehicle.information.description));
        descriptionBuilder.Append("</i>");
        descriptionBuilder.AppendLine();

        foreach (Vehicle.Information.Specification spec in MasterManager.ActiveVehicle.information.specifications)
        {
            descriptionBuilder.AppendLine("<u>");
            descriptionBuilder.Append(Dragoman.Lexicon(spec.name));
            descriptionBuilder.Append("</u>: ");
            //descriptionBuilder.AppendLine();
            descriptionBuilder.Append(Dragoman.Lexicon(spec.value));
            if (!String.IsNullOrEmpty(spec.unit))
            {
                descriptionBuilder.Append(" ");
                descriptionBuilder.Append(Dragoman.Lexicon(spec.unit));  
            }                  
        }

        infoText.text = descriptionBuilder.ToString();

        //---PRIZE---
        infoPrize.text = "$" + MasterManager.ActiveVehicle.information.price.ToString();
    }

    //private void UpdateInfoPanel()
    //{
    //    infoHeader.text = Dragoman.Lexicon(MasterManager.ActiveVehicle.Name);

    //    infoText.text = Dragoman.Lexicon(MasterManager.ActiveVehicle.information.description);
    //    infoText.text += "\n";
    //    foreach (Vehicle.Information.Specification spec in MasterManager.ActiveVehicle.information.specifications)
    //    {
    //        string nextLine = "\n" 
    //            + Dragoman.Lexicon(spec.name) + ": "
    //            + Dragoman.Lexicon(spec.value) + " " 
    //            + (String.IsNullOrEmpty(spec.unit) ? string.Empty : Dragoman.Lexicon(spec.unit));
    //        infoText.text += nextLine;
    //    }

    //    infoPrize.text = "$" + MasterManager.ActiveVehicle.information.price.ToString();
    //}



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

    public void ToggleCompactMenuButton(bool value)
    {
        compactMenuEnabled = value;
    }

    public void ToggleMainMenu()
    {
        if (showingMainMenu)
        {
            showingMainMenu = false;
            burgerPanel.SetActive(false);
            helpPanel.SetActive(false);
        }
        else
        {
            showingMainMenu = true;
            if (compactMenuEnabled)
            {
                helpPanel.SetActive(false);
                burgerPanel.SetActive(true);
            }
            else
            {
                burgerPanel.SetActive(false);
                helpPanel.SetActive(true);
            }
        }
    }

    private bool IsWidescreen()
    {
        if (Screen.width > Screen.height)
        {
            return true;
        }

        return false;
    }
    #endregion
} 

