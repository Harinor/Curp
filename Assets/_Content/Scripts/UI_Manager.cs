using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SharedData))]
public class UI_Manager : MonoBehaviour
{
    [SerializeField] Transform mainPanel;
    [SerializeField] GameObject modPrefab;

    SharedData sharedData;
    
    private void Start()
    {
        sharedData = GetComponent<SharedData>();
        UpdateSelections();
    }

    public void ChangeColor(int selection)
    {
        Debug.Log($"Selection: {selection}");
        GameObject.Find("body").GetComponent<Renderer>().material.color = Color.white;
    }

    public void UpdateSelections()
    {
        foreach (Transform child in mainPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var modification in sharedData.activeVehicle.modifications)
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
                UnityEditor.Events.UnityEventTools.AddPersistentListener(dropdown.onValueChanged, ChangeColor);
            }
        }

    }
}
