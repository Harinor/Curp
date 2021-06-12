using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// "Dragoman - an interpreter or guide, especially in countries speaking Arabic, Turkish, or Persian." - Google Translate;
/// Now Dragoman provides translation resources for the assets of this project. XD
/// </summary>
public class Dragoman : MonoBehaviour
{
    #region --- FIELDS & VARIABLES ---
    public static Dragoman instance;

    [Header("Localization sources")]
    /// <summary>
    /// Must be correctly set in the inpector for the localization file export to work.
    /// </summary>
    [Tooltip("Must be correctly set in the inpector for the localization file export to work.")] 
    public MasterManager masterManager;
    /// <summary>
    /// Must be correctly set in the inpector for the localization file export to work.
    /// </summary>
    [Tooltip("Must be correctly set in the inpector for the localization file export to work.")]
    public UI_Manager uiManager;

    [SerializeField] TMP_Dropdown languageDropdown;

    Dictionary<string, string> lexicon = new Dictionary<string, string>();
    string[] stringSeparators = new string[] { stringSeparator };   
    static string stringSeparator = " => ";
    const string localizationDataPath = "Languages";
    public static string LocalizationDataPath => localizationDataPath;

    const string defaultLanguage = "English";

    List<string> languages = new List<string>();// { defaultLanguage };
    public static List<string> Languages => instance.languages;

    private int _currentLanguage = 0;
    public int CurrentLanguage
    {
        get { return _currentLanguage; }
        private set 
        {
            if (value >= languages.Count)
            {
                _currentLanguage = 0;
            }
            else
            {
                _currentLanguage = value; 
            }
        }
    }

    [Header("New Localization")]
    public string fileName;

    public event Action OnLanguageChanged;

    #endregion

    #region --- UNITY CALLBACKS ---
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        var loadedAssets = Resources.LoadAll<TextAsset>(localizationDataPath);
        if (loadedAssets.Length > 0)
        {
            foreach (var asset in loadedAssets)
            {
                languages.Add(asset.name);
            }
        }
        else
        {
            Debug.Log("No assets loaded");
        }
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(.5f);
        LoadLanguage(defaultLanguage);
    }
    #endregion

    #region --- METHODS ---
    public static string Lexicon(string key)
    {
        if (instance.lexicon.ContainsKey(key) && !String.IsNullOrEmpty(instance.lexicon[key]))
        {
            return instance.lexicon[key];
        }
        return key;
    }

    public void CreateNewLocalizationFile()
    {
        string path = Path.Combine(Application.dataPath, "Resources", localizationDataPath, $"{fileName}.txt");

        if (File.Exists(path))
        {
            Debug.LogWarning($"'{fileName}.txt' already exists!");
            return;
        }

        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            List<string> outputList = new List<string>();
            List<Vehicle> foundVehicles = new List<Vehicle>();

            Dragoman_TextUI[] uiTexts = Resources.FindObjectsOfTypeAll(typeof(Dragoman_TextUI)) as Dragoman_TextUI[];
            foreach (Dragoman_TextUI item in uiTexts)
            {
                if (!String.IsNullOrEmpty(item.lexiconEntry))
                {
                    outputList.AddOnce(item.lexiconEntry);
                }
            }

            foreach (GameObject carObject in masterManager.AvailableVehiclesLocal)
            {
                if (carObject.TryGetComponent(out Vehicle vehicle))
                {
                    foundVehicles.Add(vehicle);
                }
            }

            foreach (var car in foundVehicles)
            {
                outputList.AddOnce(car.Name);

                outputList.AddOnce(car.information.description);

                foreach (Vehicle.Information.Specification spec in car.information.specifications)
                {
                    if (String.IsNullOrEmpty(spec.name))
                    {
                        //Skip unnamed specifications (obviously)
                        continue;   
                    }

                    //Add spec name
                    outputList.AddOnce(spec.name);

                    //Add spec value unless it is a number
                    if (!String.IsNullOrEmpty(spec.value)) 
                    {
                        if (float.TryParse(spec.value, out float _))
                        {
                            Debug.Log($"{spec.value} is a number.");
                        }
                        else
                        {
                            outputList.AddOnce(spec.unit);
                        }                        
                    }

                    //Add spec unit
                    if (!String.IsNullOrEmpty(spec.unit)) 
                    {
                        outputList.AddOnce(spec.unit);
                    }
                }

                foreach (Vehicle.ModificationSlot modSlot in car.modificationSlots)
                {
                    var mod = modSlot.modification;
                    outputList.AddOnce(mod.Name);

                    foreach (var variant in mod.variants)
                    {
                        outputList.AddOnce(variant.Name);
                    }
                }
            }

            foreach (string line in outputList)
            {
                streamWriter.WriteLine(line + stringSeparator);
            }
        }

    }

    private void ProcessLanguage(string content)
    {
        lexicon.Clear();
        using (StringReader reader = new StringReader(content))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var result = line.Split(stringSeparators, StringSplitOptions.None);
                if (result.Length == 2)
                {
                    lexicon.Add(result[0].TrimStart(), result[1].TrimStart());
                    Debug.Log($"Adding - key: {result[0]}, value: {result[1]}.");
                }
            }
        }
    }

    public void LoadLanguage(string lang)
    {
        string path = Path.Combine(localizationDataPath, lang);
        Debug.Log(path);

        TextAsset loadedAsset = Resources.Load<TextAsset>(path);
        ProcessLanguage(loadedAsset.ToString());

        OnLanguageChanged?.Invoke();

        uiManager.BroadcastMessage("UpdateText", SendMessageOptions.DontRequireReceiver);


        for (int i = 0; i < languageDropdown.options.Count; i++)
        {
            if (languageDropdown.options[i].text == lang)
            {
                languageDropdown.value = i;
            }
        }
    }

    public void LoadLanguage(int index)
    {
        if (index < languages.Count && index >= 0)
        {
            LoadLanguage(languages[index]); 
        }
    }

    public void GenerateTMProLocalizationComponents()
    {
#if UNITY_EDITOR
        Debug.Log("GenerateTMProLocalizationComponents");
        TextMeshProUGUI[] tmpTexts = Resources.FindObjectsOfTypeAll(typeof(TextMeshProUGUI)) as TextMeshProUGUI[];

        foreach (TextMeshProUGUI tmpText in tmpTexts)
        {
            if (tmpText.gameObject.TryGetComponent(out Dragoman_TextUI dragoman))
            {
                dragoman.Init();
            }
            else
            {
                tmpText.gameObject.AddComponent<Dragoman_TextUI>();
                if (tmpText.gameObject.TryGetComponent(out Dragoman_TextUI dragomanB))
                {
                    dragomanB.Init();
                }
            }
        } 
#endif
    }

    #endregion

}
