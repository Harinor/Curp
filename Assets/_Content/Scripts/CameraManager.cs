using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera primaryCamera;
    public Camera secondaryCamera;

    [Header("Setting")]
    [SerializeField] float primaryCameraLimitY = 0.5f;
    [SerializeField] float transitionTimeLimit = 0.9f;

    bool isTransitioning = false;

    enum SelectionState { Hidden, Displayed }
    SelectionState selectionState = SelectionState.Displayed;

    #region --- METHODS ---
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
        }
    }

    private IEnumerator AdjustSelectionMenuCoroutine(SelectionState selectionState, float transitionSpeed)
    {  
        //if (isTransitioning) yield break;

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
            Debug.Log(primaryCamera.rect.y);
            yield return new WaitForEndOfFrame();
        }

        primaryCamera.rect = new Rect(0, targetPrimaryPosY, 1, 1);
        secondaryCamera.rect = new Rect(0, targetSecondaryPosY, 1, 1);
        isTransitioning = false;
        Debug.LogError("Adjustment done!");
    }
    #endregion
}
