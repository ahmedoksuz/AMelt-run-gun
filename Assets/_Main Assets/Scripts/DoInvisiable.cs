using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoInvisiable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Meltable")) other.GetComponent<IMeltable>().Invisiable();
    }
}