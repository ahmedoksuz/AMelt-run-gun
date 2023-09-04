using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Door : IMeltable
{
    public enum DoorType
    {
        power,
        range,
        speed
    }

    [SerializeField] private int doorAmount;
    [SerializeField] private TextMeshPro amountText;
    [SerializeField] private float hpForIncrease = 10;
    private float currentDamge;
    [SerializeField] private DoorType doorType;
    [SerializeField] private Color red, green;
    [SerializeField] private List<Collider> neighborDoors = new();
    private Collider collider;
    private MeshRenderer meshRenderer;
    private string doorTypeText;
    [SerializeField] private GameObject flameWall;
    [SerializeField] private ParticleSystem doorGec;

    private Coroutine increaseCoroutine;

    private float one = .5f;

    private void Start()
    {
        collider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        switch (doorType)
        {
            case DoorType.power:
                doorTypeText = "Power";
                break;
            case DoorType.range:
                doorTypeText = "Range";
                break;
            case DoorType.speed:
                doorTypeText = "Speed";
                break;
        }

        RefreshText();
    }

    private void RefreshText()
    {
        if (doorAmount < 0)
        {
            amountText.text = doorAmount + "\n" + doorTypeText;
            meshRenderer.materials[0].SetColor("_BaseColor", red);
            meshRenderer.materials[1].SetColor("_BaseColor", red);
            doorGec.startColor = red;
        }
        else
        {
            amountText.text = "+" + doorAmount + "\n" + doorTypeText;
            meshRenderer.materials[0].SetColor("_BaseColor", green);
            meshRenderer.materials[1].SetColor("_BaseColor", green);
            doorGec.startColor = green;
        }
    }

    public override void StartMelt(string _state)
    {
        if (!melting) melting = true;
    }

    public override void TakeDamage(float damage)
    {
      
        currentDamge += (int)damage;
        if (currentDamge >= hpForIncrease)
        {
            doorAmount += (int)damage;
            currentDamge -= hpForIncrease;
        }

        RefreshText();
    }

    public void Die()
    {
        foreach (var neighborDoor in neighborDoors) neighborDoor.enabled = false;
        doorGec.Play();
        flameWall.SetActive(false);
        collider.enabled = false;

        transform.DOScale(0 * Vector3.one, one).OnComplete(() => { gameObject.SetActive(false); });
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        PlayerUpgrade.Instance.Upgrade(doorTypeText, doorAmount);
    }

    public override void StopMelt()
    {
    }

    public override void ForceStopAction()
    {
    }

    public override void Invisiable()
    {
    }
}