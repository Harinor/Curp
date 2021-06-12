using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInactiveOnStart : MonoBehaviour
{
    [SerializeField] float timer = .5f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
    }
}
