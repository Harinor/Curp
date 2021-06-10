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

    [Header("Vehicle Arrows")]
    [SerializeField] GameObject vehicleArrowUp;
    [SerializeField] GameObject vehicleArrowLeft;
    [SerializeField] GameObject vehicleArrowRight;
    [Header("Modification Arrows")]
    [SerializeField] GameObject modificationArrowUp;
    [SerializeField] GameObject modificationArrowDown;
    [SerializeField] GameObject modificationArrowLeft;
    [SerializeField] GameObject modificationArrowRight;
    [Header("Variants Arrows")]
    [SerializeField] GameObject variantsArrowDown;
    [SerializeField] GameObject variantsArrowLeft;
    [SerializeField] GameObject variantsArrowRight;
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
        VerifyArrowsVisibility();
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
            UpdatePreviewCameras();
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
            UpdatePreviewCameras();
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

    /// <summary>
    /// Updates the slections menu.
    /// </summary>
    /// <param name="init">Set to TRUE to force vehicles and all rows generation.</param>
    public void UpdateSelectionMenu(bool init = false)
    {
        if (true)
        {
            UpdateSelectionMenu_Vehicles(init);
        }     
        
        if (ActiveEnvironment == SelectionEnvironment.Vehicles || init)
        {
            UpdateSelectionMenu_Modifications();
        }
        else if (ActiveEnvironment == SelectionEnvironment.Modifications || init)
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

        if (MasterManager.ActiveVehicle.modificationSlots.Count == 0)
        {
            return;
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

    public void UpdateSelectionMenu_Vehicles(bool init = false)
    {
        if (vehiclesContent.childCount != MasterManager.ActiveVehiclesParent.childCount || init)
        {
            while (vehiclesContent.childCount > 0)
            {
                scrollSnapVehicles.RemoveFromBack();
            }

            foreach (Transform child in MasterManager.ActiveVehiclesParent)
            {
                GameObject slotObject = Instantiate(slotPrefab);                

                if (child.TryGetComponent(out Vehicle newVehicle) && newVehicle.previewPrefab)
                {
                    slotObject.GetComponentInChildren<TextMeshProUGUI>().text = Dragoman.Lexicon(newVehicle.Name);
                    GameObject preview = Instantiate(newVehicle.previewPrefab, slotObject.transform.GetChild(0));
                    if (newVehicle.previewScale != 1)
                    {
                        preview.transform.localScale = new Vector3(
                            preview.transform.localScale.x * newVehicle.previewScale,
                            preview.transform.localScale.y * newVehicle.previewScale,
                            preview.transform.localScale.z * newVehicle.previewScale);
                    }
                    MoveToLayer(preview.transform, 7);
                }

                scrollSnapVehicles.AddToBack(slotObject);
                Destroy(slotObject);
            } 
        }
        MasterManager.SetActiveVehicle(scrollSnapVehicles.CurrentPanel);
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

    private void VerifyArrowsVisibility()
    {
        if (modificationsContent.childCount == 0)
        {
            vehicleArrowUp.SetActive(false);
            modificationArrowLeft.SetActive(false);
            modificationArrowRight.SetActive(false);          
            modificationArrowUp.SetActive(false);          
            modificationArrowDown.SetActive(false);          
        }
        else
        {
            vehicleArrowUp.SetActive(true);

            if (scrollSnapModifications.CurrentPanel == 0)
            {
                modificationArrowLeft.SetActive(false);
            }
            else
            {
                modificationArrowLeft.SetActive(true);
            }
            if (scrollSnapModifications.CurrentPanel == scrollSnapModifications.Panels.Length - 1)
            {
                modificationArrowRight.SetActive(false);
            }
            else
            {
                modificationArrowRight.SetActive(true);
            }
            modificationArrowUp.SetActive(true);
            modificationArrowDown.SetActive(true);
        }

        if (variantsContent.childCount == 0 || modificationsContent.childCount == 0)
        {
            variantsArrowDown.SetActive(false);
            variantsArrowLeft.SetActive(false);
            variantsArrowRight.SetActive(false);
        }
        else
        {
            variantsArrowDown.SetActive(true);
            if (scrollSnapVariants.CurrentPanel == 0)
            {
                variantsArrowLeft.SetActive(false);
            }
            else
            {
                variantsArrowLeft.SetActive(true);
            }
            if (scrollSnapVariants.CurrentPanel == scrollSnapVariants.Panels.Length - 1)
            {
                variantsArrowRight.SetActive(false);
            }
            else
            {
                variantsArrowRight.SetActive(true);
            }
        }

        if (scrollSnapVehicles.CurrentPanel == 0)
        {
            vehicleArrowLeft.SetActive(false);
        }
        else
        {
            vehicleArrowLeft.SetActive(true);
        }
        if (scrollSnapVehicles.CurrentPanel == scrollSnapVehicles.Panels.Length - 1)
        {
            vehicleArrowRight.SetActive(false);
        }
        else
        {
            vehicleArrowRight.SetActive(true);
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
