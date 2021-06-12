using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager instance;

    public Sprite city;
    public Sprite nature;
    public Sprite none;

    public Material cityMat;
    public Material cityMatGround;
    public Material natureMat;
    public Material natureMatGround;

    public Environment environment;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        UI_Manager.instance.dropdownEnvironment.value = 1;
    }

    public void ChangeEnvironment(int index = 1)
    {
        Debug.Log("Changing environ ment to " + index);

        Material newBackgroundMaterial = index switch
        {
            0 => null,
            1 => cityMat,
            2 => natureMat,
            _ => null,
        };

        foreach (Renderer renderer in environment.horizons)
        {
            if (newBackgroundMaterial == null)
            {
                renderer.enabled = false;
            }
            else
            {
                renderer.enabled = true;
                renderer.material = newBackgroundMaterial;
            }
        }

        Material newGroundMaterial = index switch
        {
            0 => null,
            1 => cityMatGround,
            2 => natureMatGround,
            _ => null,
        };

        if (newGroundMaterial == null)
        {
            environment.ground.enabled = false;
        }
        else
        {
            environment.ground.enabled = true;
            environment.ground.material = newGroundMaterial;
        }
    }

    [System.Serializable]
    public class Environment
    {
        public Renderer ground;
        public Renderer[] horizons;
    }
}
