using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Selections : MonoBehaviour
{
    public static Selections instance;

    private int _activeSelection = 0;
    public int ActiveSelection
    {
        get { return _activeSelection; }
        private set { _activeSelection = value; }
    }

    [SerializeField] GameObject slotPrefab;

    [Header("Selected slots")]
    public int Slot0;
    public int Slot1;
    public int Slot2;
  
    public SimpleScrollSnap scrollSnapVehicles;
    public SimpleScrollSnap scrollSnapModifications;
    public SimpleScrollSnap scrollSnapVariants;
    public Transform content0;
    public Transform content1;
    public Transform content2;

    Vector3 defaultPosition;
    float positionOffset = -15;
    [SerializeField, Tooltip("The translation speed between selection lines.")]
    float translationSpeed = 7f;
    float timer = 1.5f;

    [Header("Unity Events")]
    public UnityEvent OnVariantPanelChangedCallback;
    public UnityEvent OnModificationPanelChangedCallback;
    public UnityEvent OnVehiclePanelChangedCallback;

    #region --- UNITY CALLBACKS ---
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        defaultPosition = transform.position;
    }

    private void Update()
    {
        Slot0 = scrollSnapVehicles.CurrentPanel;
        Slot1 = scrollSnapModifications.CurrentPanel;
        Slot2 = scrollSnapVariants.CurrentPanel;
    } 
    #endregion

    public void MoveIn()
    {
        if (ActiveSelection == 0) return;
        ActiveSelection--;
        UpdateSelectionMenu();
        StopAllCoroutines();
        StartCoroutine(TranslateSelections(ActiveSelection));
        //Debug.LogError($"MoveIn-Selection: {ActiveSelection}");
    }

    public void MoveOut()
    {
        if (ActiveSelection == 2) return;
        ActiveSelection++;
        UpdateSelectionMenu();
        StopAllCoroutines();
        StartCoroutine(TranslateSelections(ActiveSelection));
        //Debug.LogError($"MoveOut-Selection: {ActiveSelection}");
    }

    IEnumerator TranslateSelections(int slot)
    {
        float targetZ = defaultPosition.z + (slot * positionOffset);
        float time = 0;
        while (transform.position.z != targetZ && time < timer)
        {
            time += Time.deltaTime;
            float newZ = Mathf.Lerp(transform.position.z, targetZ, Time.deltaTime* translationSpeed);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            yield return new WaitForEndOfFrame();
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, targetZ);
        Debug.Log("Translation complete.");
    }

    public void OnVariantPanelChanged() //Variants
    {
        var currentMod = MasterManager.ActiveVehicle.modifications[scrollSnapModifications.CurrentPanel];
        if (currentMod != null)
        {
            currentMod.ApplyVariant(scrollSnapVariants.CurrentPanel);
        }

        OnVariantPanelChangedCallback.Invoke();
    }

    public void OnModificationPanelChanged() //Modifications
    {
        UpdateSelectionMenu();
        OnModificationPanelChangedCallback.Invoke();
    }

    public void OnVehiclePanelChanged() //Vehicles
    {
        UpdateSelectionMenu();
        OnVehiclePanelChangedCallback.Invoke();
    }

    public void UpdateSelectionMenu(bool forced = false)
    {
        Debug.Log($"UpdateSelectionMenu - ActiveSelection: {ActiveSelection}");
        if (ActiveSelection == 0 || forced)
        {
            UpdateSelectionMenu_Modifications();
        }
        else if (ActiveSelection == 1 || forced)
        {
            UpdateSelectionMenu_Variants();
        }
    }

    public void UpdateSelectionMenu_Variants()
    {
        while (content2.childCount > 0)
        {
            scrollSnapVariants.RemoveFromBack();
        }        
        
        foreach (ModificationVariant variant in MasterManager.ActiveVehicle.modifications[scrollSnapModifications.CurrentPanel].variants)
        {
            var newVariant = Instantiate(slotPrefab);
            newVariant.GetComponentInChildren<Text>().text = variant.Name;
            if (variant.previewPrefab != null)
            {
                GameObject preview =  Instantiate(variant.previewPrefab, newVariant.transform.GetChild(0));
                preview.layer = 7;
                MoveToLayer(preview.transform, 7);

            }
            scrollSnapVariants.AddToBack(newVariant);
            Destroy(newVariant);

            void MoveToLayer(Transform root, int layer)
            {
                root.gameObject.layer = layer;
                foreach (Transform child in root)
                    MoveToLayer(child, layer);
            }
        }
    }

    public void UpdateSelectionMenu_Modifications()
    {
        while (content1.childCount > 0)
        {
            scrollSnapModifications.RemoveFromBack();
        }

        foreach (Modification mod in MasterManager.ActiveVehicle.modifications)
        {
            var newMod = Instantiate(slotPrefab);
            newMod.GetComponentInChildren<Text>().text = mod.Name;
            scrollSnapModifications.AddToBack(newMod);
            Destroy(newMod);
        }

        UpdateSelectionMenu_Variants();
    }
}
