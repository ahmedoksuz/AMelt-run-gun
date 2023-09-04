using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GPHive.Core;
using GPHive.Game;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public abstract class IMeltable : MonoBehaviour
{
    #region Garbage

    private float dotfiveee = .5f;
    private float dotone = .1f;

    #endregion

    // [ShowIf("meltObject")] public Color meltColorCode;
    [ShowIf("meltObject")] public TextMeshPro healthText;
    [ShowIf("meltObject")] public Transform meltScaleMesh;

    public float health;
    public bool melting;
    public bool meltObject;
    [HideInInspector] public float maxHealth;

    public abstract void StartMelt(string _state);

    public abstract void TakeDamage(float damage);

    public abstract void StopMelt();
    public abstract void ForceStopAction();

    public abstract void Invisiable();
}