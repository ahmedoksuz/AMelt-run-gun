using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun Config", menuName = "GP Hive Objects/Gun")]
public class GunConfig : ScriptableObject
{
    public int ammoCapasty;
    public float fireRateTime;
}