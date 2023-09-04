using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawnAnimatioObj : MonoBehaviour
{
    private Animator animator;


    [SerializeField] private GameEvent crateAmmoEvent;
    private static readonly int Play = Animator.StringToHash("Play");
    private static readonly int Stop = Animator.StringToHash("Stop");

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartAnim()
    {
        animator.SetTrigger(Play);
    }

    public void StartAnimManual()
    {
        animator.Play("AmmoSpawnAnim");
    }

    public void StopAnim()
    {
        animator.SetTrigger(Stop);
    }

    public void CreatAmmo()
    {
        crateAmmoEvent.Raise();
    }
}