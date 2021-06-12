using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExternalButtonClickHelper : MonoBehaviour
{
    [SerializeField] Button button;
    
    public void Click()
    {
        if (button == null)
        {
            GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.Invoke();
        }
    }
}
