using System;
using Game.Cook;
using UnityEngine;

public class CookBtn : MonoBehaviour
{
    private void OnMouseDown()
    {
        CookManager.instance.Cook();
    }
}
