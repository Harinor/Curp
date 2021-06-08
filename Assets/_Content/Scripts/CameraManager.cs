using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera primaryCamera;
    public Camera secondaryCamera;
    public OrbitalCamera orbitalCamera;

    [Header("Setting")]
    [SerializeField] float primaryCameraLimitY = 0.5f;
    [SerializeField] float transitionTimeLimit = 0.9f;

    bool isTransitioning = false;

    public enum SelectionState { Hidden, Displayed }
    [HideInInspector]
    public SelectionState selectionState = SelectionState.Hidden;

    public static CameraManager instance;

    #region --- UNITY CALLBACKS ----
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    #region --- METHODS ---
    public void ToggleSelectionMenu()
    {
        if (!isTransitioning)
        {
            switch (selectionState)
            {
                case SelectionState.Hidden:
                    ShowSelectionMenu();
                    break;
                case SelectionState.Displayed:
                    HideSelectionMenu();
                    break;
                default:
                    break;
            }
        }
    }  
        
    public void ShowSelectionMenu()
    {
        if (isTransitioning == false)
        {
            StartCoroutine(AdjustSelectionMenuCoroutine(SelectionState.Displayed, 5f));   
        }      
    }

    public void HideSelectionMenu()
    {
        if (isTransitioning == false)
        {
            StartCoroutine(AdjustSelectionMenuCoroutine(SelectionState.Hidden, 5f));
            Selections.instance.UpdatePreviewCameras();
        }
    }

    private IEnumerator AdjustSelectionMenuCoroutine(SelectionState selectionState, float transitionSpeed)
    {  
        isTransitioning = true;
        float targetPrimaryPosY = 0;
        float targetSecondaryPosY = -1;

        switch (selectionState)
        {
            case SelectionState.Hidden:
                this.selectionState = SelectionState.Hidden;
                targetPrimaryPosY = 0;
                targetSecondaryPosY = -1; 
                break;
            case SelectionState.Displayed:
                this.selectionState = SelectionState.Displayed;
                targetPrimaryPosY = primaryCameraLimitY;
                targetSecondaryPosY = -primaryCameraLimitY;
                Selections.instance.UpdateSelectionMenu(true);
                break;
            default:
                break;
        }

        float timeElapsed = 0;

        while (primaryCamera.rect.y != targetPrimaryPosY && timeElapsed < transitionTimeLimit)
        {
            timeElapsed += Time.deltaTime;
            float newPrimaryCameraPosY = Mathf.Lerp(primaryCamera.rect.y, targetPrimaryPosY, Time.deltaTime * (transitionSpeed + timeElapsed));
            float newSecondaryCameraPosY = Mathf.Lerp(secondaryCamera.rect.y, targetSecondaryPosY, Time.deltaTime * (transitionSpeed + timeElapsed));

            primaryCamera.rect = new Rect(0, newPrimaryCameraPosY, 1, 1);
            secondaryCamera.rect = new Rect(0, newSecondaryCameraPosY, 1, 1);
            //Debug.Log(primaryCamera.rect.y);
            yield return new WaitForEndOfFrame();
        }

        primaryCamera.rect = new Rect(0, targetPrimaryPosY, 1, 1);
        secondaryCamera.rect = new Rect(0, targetSecondaryPosY, 1, 1);
        isTransitioning = false;
        //Debug.LogError("Adjustment done!");
    }

    public void ToggleCamera()
    {
        orbitalCamera.ToggleCamera();    
    }

    #endregion
}
