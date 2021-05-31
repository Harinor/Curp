using DanielLochner.Assets.SimpleScrollSnap;
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
    }

    private void Update()
    {
        Slot0 = scrollSnapVehicles.CurrentPanel;
        Slot1 = scrollSnapModifications.CurrentPanel;
        Slot2 = scrollSnapVariants.CurrentPanel;
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log($"MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel].activeVariant = {MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel].activeVariant}; scrollSnapVariants.CurrentPanel: {scrollSnapVariants.CurrentPanel}");
        
            if (MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel].activeVariant != scrollSnapVariants.CurrentPanel)
            {
  
                int activeVariantIndex = MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel].activeVariant;
                GameObject newPreview = 
                    MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel].modification.variants[activeVariantIndex].previewPrefab;
                Instantiate(newPreview, modificationsContent.GetChild(scrollSnapModifications.CurrentPanel));
            }        
        }



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
            UpdateSelectionMenu();
            StopAllCoroutines();
            StartCoroutine(TranslateSelections((int)ActiveEnvironment));
            Debug.LogError($"SelectionsArrowUp-Selection: {ActiveEnvironment}");
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
            UpdateSelectionMenu();
            StopAllCoroutines();
            StartCoroutine(TranslateSelections((int)ActiveEnvironment));
            Debug.LogError($"SelectionsArrowDown-Selection: {ActiveEnvironment}");
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
        var currentMod = MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel].modification;
        if (currentMod != null)
        {
            currentMod.ApplyVariant(scrollSnapVariants.CurrentPanel);
            MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel].activeVariant = scrollSnapVariants.CurrentPanel;
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
        Debug.Log($"UpdateSelectionMenu - ActiveSelection: {ActiveEnvironment}");
        if (ActiveEnvironment == SelectionEnvironment.Vehicles || forced)
        {
            UpdateSelectionMenu_Modifications();
        }
        else if (ActiveEnvironment == SelectionEnvironment.Modifications || forced)
        {
            UpdateSelectionMenu_Variants();
        }
    }

    public void UpdateSelectionMenu_Variants()
    {
        while (variantsContent.childCount > 0)
        {
            scrollSnapVariants.RemoveFromBack();
        }

        //Initial slot
        var currentModSlot = MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel];
        int activeVariantIndex = currentModSlot.activeVariant;
        CreateVariantFromIndex(currentModSlot.modification, activeVariantIndex);

        for (int i = activeVariantIndex - 1; i >= 0; i--)
        {
            CreateVariantFromIndex(currentModSlot.modification, i, true);
        }

        for (int i = activeVariantIndex + 1; i < currentModSlot.modification.variants.Count; i++)
        {
            CreateVariantFromIndex(currentModSlot.modification, i);
        }

        scrollSnapVariants.startingPanel = activeVariantIndex;

        scrollSnapVariants.Setup();



        //foreach (ModificationVariant variant in MasterManager.ActiveVehicle.modificationSlots[scrollSnapModifications.CurrentPanel].modification.variants)
        //{
        //    var newVariant = Instantiate(slotPrefab);
        //    newVariant.GetComponentInChildren<Text>().text = variant.Name;
        //    if (variant.previewPrefab != null)
        //    {
        //        GameObject preview = Instantiate(variant.previewPrefab, newVariant.transform.GetChild(0));
        //        MoveToLayer(preview.transform, 7);

        //    }
        //    scrollSnapVariants.AddToBack(newVariant);
        //    Destroy(newVariant);
        //}

        void CreateVariantFromIndex(Modification mod, int index, bool toFront = false)
        {
            var variant = mod.variants[index];
            var newVariant = Instantiate(slotPrefab);
            newVariant.GetComponentInChildren<Text>().text = variant.Name;
            if (variant.previewPrefab != null)
            {
                GameObject preview = Instantiate(variant.previewPrefab, newVariant.transform.GetChild(0));
                MoveToLayer(preview.transform, 7);
            }

            if (toFront)
            {
                scrollSnapVariants.AddToFront(newVariant);
            }
            else
            {
                scrollSnapVariants.AddToBack(newVariant);
            }

            Destroy(newVariant);
        }
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
            var newMod = Instantiate(slotPrefab);
            newMod.GetComponentInChildren<Text>().text = mod.Name;
            scrollSnapModifications.AddToBack(newMod);
            Destroy(newMod);
        }


        UpdateSelectionMenu_Variants();
    }

    //TODO: Implement me, cars are static for now
    public void UpdateSelectionMenu_Vehicles()
    {
        while (vehiclesContent.childCount > 0)
        {
            scrollSnapVehicles.RemoveFromBack();
        }

        //...
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
