using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeltablesController : MonoBehaviour
{
    [SerializeField]private List<GameObject> MeltableBlocks = new();

    private void Start()
    {
        foreach (var melable in MeltableBlocks)
        {
            melable.SetActive(false);
        }

        int rnd = Random.Range(0, MeltableBlocks.Count);
        MeltableBlocks[rnd].SetActive(true);

    }
}
