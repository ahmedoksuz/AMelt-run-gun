using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BarrelParent : MonoBehaviour
{
    [SerializeField] private List<GameObject> barrels = new();

    private void Start()
    {
        foreach (var barrel in barrels) barrel.SetActive(false);
        barrels[Random.Range(0, barrels.Count)].SetActive(true);
    }
}