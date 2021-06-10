using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// "Dragoman - an interpreter or guide, especially in countries speaking Arabic, Turkish, or Persian." - Google Translate;
/// Now Dragoman provides translation resources for the assets of this project. XD
/// </summary>
public class Dragoman : MonoBehaviour
{
    #region --- FIELDS & VARIABLES ---
    public static Dragoman instance;

    Dictionary<string, string> lexicon = new Dictionary<string, string>();
    string[] stringSeparators = new string[] { stringSeparator };   
    static string stringSeparator = " => ";
    const string localizationDataPath = "Languages";
    public static string LocalizationDataPath => localizationDataPath;

    const string defaultLanguage = "English";

    List<string> languages = new List<string>() { defaultLanguage };
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

        LoadLanguage(0);
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

        using (StreamWriter sw = new StreamWriter(path))
        {
            List<string> outputList = new List<string>();
            var foundObjects = FindObjectsOfType<Vehicle>();
            Debug.LogError("Vehicles found" + foundObjects.Length);

            foreach (var car in foundObjects)
            {
                outputList.AddOnce(car.Name);

                foreach (var modSlot in car.modificationSlots)
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
                sw.WriteLine(line + stringSeparator);
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
        if (lang == defaultLanguage)
        {
            lexicon.Clear();
        }
        else
        {
            string path = Path.Combine(localizationDataPath, lang);
            Debug.Log(path);

            TextAsset loadedAsset = Resources.Load<TextAsset>(path);
            ProcessLanguage(loadedAsset.ToString());

        }
    }

    public void LoadLanguage(int index)
    {
        if (index < languages.Count && index >= 0)
        {
            LoadLanguage(languages[index]); 
        }
    }
    #endregion

}
