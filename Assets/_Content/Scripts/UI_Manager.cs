using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] GameObject modPrefab;


    #region --- UNITY CALLBACKS ----
    private void Start()
    {
        UpdateSelections();
    }

    #endregion

    #region --- PUBLIC METHODS ---
    public void UpdateSelections()
    {
        Transform mainPanel = MasterManager.MainPanel;
        if (mainPanel == null) return;

        foreach (Transform child in mainPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var modification in MasterManager.ActiveVehicle.modifications)
        {
            if (modification.variants.Count < 1)
            {
                continue;
            }
            else
            {
                var newMod = Instantiate(modPrefab, mainPanel);
                newMod.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = modification.Name;
                List<string> newModVariants = new List<string>();
                foreach (var variant in modification.variants)
                {
                    newModVariants.Add(variant.Name);
                }
                var dropdown = newMod.GetComponentInChildren<TMP_Dropdown>();
                dropdown.AddOptions(newModVariants);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(dropdown.onValueChanged, modification.ApplyVariant);
            }
        }

    }

    public void OnButtonClickUp()
    {
        Selections.instance.MoveOut();
    }

    public void OnButtonClickDown()
    {
        Selections.instance.MoveIn();
    }
    #endregion
} 

