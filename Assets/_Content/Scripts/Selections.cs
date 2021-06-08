using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Selections : MonoBehaviour
{
    #region ---FIELDS & VARIABLES---
    public static Selections instance;

    [SerializeField] private SelectionEnvironment _activeEnvironment = SelectionEnvironment.Vehicles;
    public SelectionEnvironment ActiveEnvironment
    {
        get { return _activeEnvironment; }
        private set { _activeEnvironment = value; }
    }
    public enum SelectionEnvironment
    {
        Vehicles,
        Modifications,
        Variants
    }

    [SerializeField] GameObject slotPrefab;

    public List<Vehicle> vehicles = new List<Vehicle>();

    [Header("Selected slots")]
    public int Slot0;
    public int Slot1;
    public int Slot2;

    public SimpleScrollSnap scrollSnapVehicles;
    public SimpleScrollSnap scrollSnapModifications;
    public SimpleScrollSnap scrollSnapVariants;
    public Transform vehiclesContent;
    public Transform modificationsContent;
    public Transform variantsContent;

    /// <summary>
    /// Stores the defaolt position of the Selections GO which is used to calculate positions when viewing different selection environments.
    /// </summary>
    Vector3 defaultPosition;
    readonly float positionOffset = -15;
    [SerializeField, Tooltip("The translation speed between environment lines.")]
    float translationSpeed = 7f;
    readonly float timer = 1.5f;

    [Header("Unity Events")]
    public UnityEvent OnVariantPanelChangedCallback;
    public UnityEvent OnModificationPanelChangedCallback;
    public UnityEvent OnVehiclePanelChangedCallback;

    #endregion

    #region --- UNITY CALLBACKS ---
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        defaultPosition = transform.position;
        UpdateSelectionMenu(true);
    }

    private void Update()
    {
        Slot0 = scrollSnapVehicles.CurrentPanel;
        Slot1 = scrollSnapModifications.CurrentPanel;
        Slot2 = scrollSnapVariants.CurrentPanel;
    } 
    #endregion

    public void SelectionsArrowUp()
    {
        if (ActiveEnvironment == SelectionEnvironment.Variants)
        {
            return;
        }
        else
        {
            ActiveEnvironment++;
            StopAllCoroutines();
            StartCoroutine(TranslateSelections((int)ActiveEnvironment));          
        }
    }

    public void SelectionsArrowDown()
    {
        if (ActiveEnvironment == 0) 
        {
            return;
        }
        else
        {
            ActiveEnvironment--;
            StopAllCoroutines();
            StartCoroutine(TranslateSelections((int)ActiveEnvironment));
        }
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
        var currentModSlot = MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel];
        if (currentModSlot.modification != null)
        {
            currentModSlot.modification.ApplyVariant(scrollSnapVariants.CurrentPanel);
            MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel].activeVariant = scrollSnapVariants.CurrentPanel;
            UpdateActiveVariantPreviewForAllModifications();
        }

        OnVariantPanelChangedCallback.Invoke();
    }

    void UpdateActiveVariantPreviewForAllModifications()
    {
        for (int i = 0; i < modificationsContent.childCount; i++)
        {
            var modSlot = MasterManager.ActiveVehicle.modificationSlots[i];
            var previewSlot = modificationsContent.GetChild(i).GetChild(0);
            if (previewSlot != null && previewSlot.childCount > 0)
            {
                foreach (Transform child in previewSlot)
                {
                    Destroy(child.gameObject);
                }
            }
            GameObject previewPrefab = modSlot.modification.variants[modSlot.activeVariant].previewPrefab;
            GameObject preview = Instantiate(previewPrefab, previewSlot);
            //TODO: kinda repeated code here, possibly better to create GetPreview method on variants...
            float previewScale = modSlot.modification.variants[modSlot.activeVariant].previewScale;
            if (modSlot.modification.variants[modSlot.activeVariant].previewScale != 1)
            {
                preview.transform.localScale = new Vector3(
                    preview.transform.localScale.x * previewScale,
                    preview.transform.localScale.y * previewScale,
                    preview.transform.localScale.z * previewScale);
            }
            MoveToLayer(preview.transform, 7);
        }
    }

    public void OnModificationPanelChanged() //Modifications
    {
        UpdateSelectionMenu();
        UpdatePreviewCameras();
        OnModificationPanelChangedCallback.Invoke();
    }

    public void OnVehiclePanelChanged() //Vehicles
    {
        UpdateSelectionMenu();
        OnVehiclePanelChangedCallback.Invoke();
    }

    public void UpdateSelectionMenu(bool forced = false)
    {
        Debug.Log($"UpdateSelectionMenu - ActiveSelection: {ActiveEnvironment}");
        if (ActiveEnvironment == SelectionEnvironment.Vehicles || forced)
        {
            UpdateSelectionMenu_Modifications();
        }
        else if (ActiveEnvironment == SelectionEnvironment.Modifications || forced)
        {
            UpdateSelectionMenu_Variants();
        }

        Invoke(nameof(UpdateActiveVariantPreviewForAllModifications), .1f);
    }

    public void UpdateSelectionMenu_Variants()
    {
        while (variantsContent.childCount > 0)
        {
            scrollSnapVariants.RemoveFromBack();
        }

        var currentModSlot = MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel];
        int activeVariantIndex = currentModSlot.activeVariant;

        foreach (ModificationVariant variant in currentModSlot.modification.variants)
        {
            var newVariant = Instantiate(slotPrefab);
            newVariant.GetComponentInChildren<TextMeshProUGUI>().text = Dragoman.Lexicon(variant.Name);
            if (variant.previewPrefab != null)
            {
                GameObject preview = Instantiate(variant.previewPrefab, newVariant.transform.GetChild(0));
                if (variant.previewScale != 1)
                {
                    preview.transform.localScale = new Vector3(
                        preview.transform.localScale.x * variant.previewScale,
                        preview.transform.localScale.y * variant.previewScale,
                        preview.transform.localScale.z * variant.previewScale);
                }
                MoveToLayer(preview.transform, 7);
            }
            scrollSnapVariants.AddToBack(newVariant);
            Destroy(newVariant);
        }

        //--- had to change the Setup method to public to make this work ---
        scrollSnapVariants.startingPanel = activeVariantIndex;
        scrollSnapVariants.Setup();
    }

    public void UpdateSelectionMenu_Modifications()
    {
        while (modificationsContent.childCount > 0)
        {
            scrollSnapModifications.RemoveFromBack();
        }

        foreach (Vehicle.ModificationSlot modSlot in MasterManager.ActiveVehicle.modificationSlots)
        {
            Modification mod = modSlot.modification;
            if (mod != null)
            {
                GameObject newMod = Instantiate(slotPrefab);
                newMod.GetComponentInChildren<TextMeshProUGUI>().text = Dragoman.Lexicon(mod.Name);
                scrollSnapModifications.AddToBack(newMod);
                Destroy(newMod);
            }
        }

        UpdateSelectionMenu_Variants();
    }

    public void UpdateSelections_Vehicles()
    {

    }

    public void UpdatePreviewCameras()
    {
        int activeMod = scrollSnapModifications.CurrentPanel;

        for (int i = 0; i < MasterManager.ActiveVehicle.modificationSlots.Count; i++)
        {
            var slot = MasterManager.ActiveVehicle.modificationSlots[i];
            if (slot != null && slot.previewCamera != null)
            {
                if (i == activeMod && CameraManager.instance.selectionState == CameraManager.SelectionState.Displayed 
                    && ActiveEnvironment != SelectionEnvironment.Vehicles)
                {
                    slot.previewCamera.Priority = 100;
                }
                else
                {
                    slot.previewCamera.Priority = 0;
                }
            }
        }
    }

    /// <summary>
    /// Moves this transforms GameObject and all its children into the supplied layer.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="layer"></param>
    void MoveToLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            MoveToLayer(child, layer);
    }
}
