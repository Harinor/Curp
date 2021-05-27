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
    public SimpleScrollSnap scrollSnap0;
    public SimpleScrollSnap scrollSnap1;
    public SimpleScrollSnap scrollSnap2;
    public Transform content0;
    public Transform content1;
    public Transform content2;

    Vector3 defaultPosition;
    float positionOffset = -15;
    [SerializeField, Tooltip("The translation speed between selection lines.")]
    float translationSpeed = 3f;

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
        Slot0 = scrollSnap0.CurrentPanel;
        Slot1 = scrollSnap1.CurrentPanel;
        Slot2 = scrollSnap2.CurrentPanel;
    } 
    #endregion

    public void MoveIn()
    {
        if (ActiveSelection == 0) return;
        ActiveSelection--;
        //transform.Translate(new Vector3(0, 0, 15));
        StopAllCoroutines();
        StartCoroutine(TranslateSelections(ActiveSelection));
        Debug.LogError($"MoveIn-Selection: {ActiveSelection}");
    }

    public void MoveOut()
    {
        if (ActiveSelection == 2) return;
        ActiveSelection++;
        //transform.Translate(new Vector3(0, 0, -15));
        StopAllCoroutines();
        StartCoroutine(TranslateSelections(ActiveSelection));
        Debug.LogError($"MoveOut-Selection: {ActiveSelection}");
    }

    IEnumerator TranslateSelections(int slot)
    {
        float targetZ = defaultPosition.z + (slot * positionOffset);
        while (transform.position.z != targetZ)
        {
            float newZ = Mathf.Lerp(transform.position.z, targetZ, Time.deltaTime* translationSpeed);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Translation complete.");
    }

    public void OnVariantPanelChanged() //Variants
    {
        int activePanelIndex = scrollSnap0.CurrentPanel;

        var currentMod = MasterManager.ActiveVehicle.modifications[scrollSnap1.CurrentPanel];
        if (currentMod != null)
        {
            currentMod.ApplyVariant(scrollSnap0.CurrentPanel);
        }

        OnVariantPanelChangedCallback.Invoke();
    }

    public void OnModificationPanelChanged() //Modifications
    {
        UpdateSelectionMenu(1);
        OnModificationPanelChangedCallback.Invoke();
    }

    public void OnVehiclePanelChanged() //Vehicles
    {
        UpdateSelectionMenu(2);
        OnVehiclePanelChangedCallback.Invoke();
    }

    public void UpdateSelectionMenu(int slot)
    {
        Debug.LogError($"Updating selection from changes within panel {slot}");

        for (int i = 0; i < content1.childCount; i++)
        {
            scrollSnap1.RemoveFromFront();
        }

        foreach (var modification in MasterManager.ActiveVehicle.modifications)
        {
            if (modification.variants.Count < 1)
            {
                continue;
            }
            else
            {
                var newMod = Instantiate(slotPrefab, content1);
                scrollSnap1.AddToBack(newMod);

                newMod.transform.GetComponentInChildren<Text>().text = modification.Name;

                //List<string> newModVariants = new List<string>();
                //foreach (var variant in modification.variants)
                //{
                //    newModVariants.Add(variant.Name);
                //}

            }
        }

    }
}
