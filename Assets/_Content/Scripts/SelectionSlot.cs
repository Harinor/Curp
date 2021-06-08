using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSlot : MonoBehaviour
{
    bool clicked = false;
    
    private void OnMouseDown()
    {        
        StartCoroutine(ClickTimer());
    }

    private void OnMouseUp()
    {
        if (clicked)
        {
            clicked = false;
            Selections.instance.SelectionsArrowUp();
        }
    }

    IEnumerator ClickTimer()
    {
        clicked = true;
        yield return new WaitForSeconds(0.1f);
        clicked = false;
    }
}
