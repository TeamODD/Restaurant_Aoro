using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisableOtherObjects : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects = new();

    public void DisableOthers(String name)
    {
        foreach (var obj in objects.Where(obj => obj.name != name))
        {
            obj.SetActive(false);
        }
    }
}
