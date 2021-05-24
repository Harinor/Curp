using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
    void Start()
    {
        SetAsSharedDataMainPanel();
    }

    public bool SetAsSharedDataMainPanel()
    {
        if (SharedData.MainPanel == null)
        {
            SharedData.MainPanel = this.transform;
            return true;
        }
        return false;
    }
}
