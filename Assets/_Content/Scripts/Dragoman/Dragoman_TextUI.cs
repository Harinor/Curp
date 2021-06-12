using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dragoman_TextUI : MonoBehaviour
{
    [Header("State")]
    public bool disableForThisObject = false;
    
    [Header("Text fields")]
    [SerializeField] Text text;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    [Header("Lexicon")]
    public string lexiconEntry;

    public void Init()
    {
        if (disableForThisObject) return;

        text = GetComponent<Text>();
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        if (textMeshProUGUI && !string.IsNullOrEmpty(textMeshProUGUI.text))
        {
            lexiconEntry = textMeshProUGUI.text;
        }
        else if (text)
        {
            lexiconEntry = text.text;
        }
    }

    public void UpdateText()
    {
        if (disableForThisObject) return;

        if (text)
        {
            text.text = Dragoman.Lexicon(lexiconEntry);
        }
        if (textMeshProUGUI)
        {
            textMeshProUGUI.text = Dragoman.Lexicon(lexiconEntry);
        }
    }

    private void OnEnable()
    {
        if (disableForThisObject) return;

        Dragoman.instance.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    private void OnDisable()
    {
        if (disableForThisObject) return;

        Dragoman.instance.OnLanguageChanged -= UpdateText;
    }
}
