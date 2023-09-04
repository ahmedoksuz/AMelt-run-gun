using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GPHive.Core;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunOfficer : MonoBehaviour
{
    [SerializeField] private List<Gun> guns = new();
    [HideInInspector] public Gun activeGun;
    public int gunLevel;
    public int relodedAmmoAmount, gunAllRelodedAmmoAmount;
    [HideInInspector] public Vector3 startMovePos;
    private bool shoot = false;
    private Rigidbody rigidbody;
    private Coroutine shootingCo;
    private static readonly int Play = Animator.StringToHash("Play");
    private Collider collider;
    [SerializeField] private ParticleSystem muzleFlashParticle;
    [SerializeField] private TextMeshPro scoorText;
    [SerializeField] private Material grayMaterial, ghostMatterial;
    [SerializeField] private bool ghost;
    [SerializeField] private GameObject lockObj;
    [SerializeField] private ParticleSystem levelUpParticle;
    [SerializeField] private TextMeshPro gunPartAmmoText;

    private void Start()
    {
        if (ghost)
        {
            lockObj.SetActive(true);
            scoorText.enabled = false;


            var materials = activeGun.meshRenderer.materials;
            for (var i = 0; i < materials.Length; i++) materials[i] = ghostMatterial;
            activeGun.meshRenderer.materials = materials;
        }
        else
        {
            RefreshScoorText();
        }

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public void CheckLevelUp()
    {
        if (relodedAmmoAmount >= activeGun.gunConfig.ammoCapasty) LevelUp();
    }

    public void RefreshScoorText()
    {
        scoorText.text = "Level " + (gunLevel + 1) + "                     " + relodedAmmoAmount + "/" +
                         activeGun.gunConfig.ammoCapasty;
    }

    public void RefresGunPartAmmoText()
    {
        if (gunAllRelodedAmmoAmount <= 10) gunPartAmmoText.color = Color.red;

        gunPartAmmoText.text = gunAllRelodedAmmoAmount + "/" + GunMiniGameManager.Instance.ammoAmountForCreat;
    }

    public void LevelUp()
    {
        if (gunLevel + 1 < GunMiniGameManager.Instance.maxGunAmount)
        {
            levelUpParticle.Play();
            gunLevel++;
            RefreshGun();
        }
    }

    private void RefreshGun()
    {
        relodedAmmoAmount = 0;
        RefreshScoorText();
        SetActiveGun();
    }

    public void SetActiveGun()
    {
        foreach (var gun in guns) gun.gunModel.SetActive(false);
        activeGun = guns[gunLevel];
        activeGun.gunModel.SetActive(true);
        muzleFlashParticle.transform.position = activeGun.bulletSpawnPoint.position + new Vector3(0, 0, 0.3f);
        muzleFlashParticle.transform.parent = activeGun.bulletSpawnPoint;
    }

    public void StratShoot()
    {
        shoot = true;
        gunPartAmmoText.transform.parent.gameObject.SetActive(true);
        shootingCo = StartCoroutine(ShooterCO());
    }

    public void StopShoot()
    {
        if (shootingCo != null)
        {
            StopCoroutine(shootingCo);
            shootingCo = null;
            shoot = false;
        }
    }


    private void GetBullet()
    {
        if (gunAllRelodedAmmoAmount > 0)
        {
            gunAllRelodedAmmoAmount--;
            var _bullet = Poolable.Get<Bullet>();
            _bullet.transform.position = activeGun.bulletSpawnPoint.position;
            _bullet.gameObject.SetActive(true);
            ShooterManager.Instance.bullets.Add(_bullet);
            activeGun.animator.SetTrigger(Play);
            muzleFlashParticle.Play();
            RefresGunPartAmmoText();
        }
        else
        {
            GameManager.Instance.OutOfAmmoMeth();
            GameManager.Instance.WinLevel();
        }
    }

    private IEnumerator ShooterCO()
    {
        while (shoot && GameManager.Instance.GameState == GameState.Playing)
        {
            GetBullet();
            yield return new WaitForSeconds(activeGun.gunConfig.fireRateTime);
        }

        StopShoot();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obsticle"))
        {
            if (TryGetComponent<Obsticle>(out var obsticle)) obsticle.Action();
            else if (TryGetComponent<Enemy>(out var enemy)) enemy.Action();
            else if (TryGetComponent<Target>(out var target)) target.Action();

            other.enabled = false;
            Drope();
            ShooterManager.Instance.JumpBack();
        }

        if (other.CompareTag("FinishObj"))
            if (GameManager.Instance.GameState != GameState.End)
            {
                Drope();
                ShooterManager.Instance.JumpBack();
            }
    }


    public void Drope()
    {
        gunPartAmmoText.transform.parent.gameObject.SetActive(false);
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        collider.enabled = false;
        activeGun.animator.enabled = false;
        activeGun.rigidbody.isKinematic = false;
        StopShoot();
        transform.parent = LevelManager.Instance.ActiveLevel.transform;

        var rnd = Random.Range(0, 2);
        if (rnd == 0)
            rnd = 1;
        else
            rnd = -1;
        var forceDirection = Vector3.left * (rnd * 7) + Vector3.down * 20 + Vector3.forward * -20;
        activeGun.rigidbody.AddForce(forceDirection, ForceMode.Impulse);

        var materials = activeGun.meshRenderer.materials;
        for (var i = 0; i < materials.Length; i++) materials[i] = grayMaterial;
        activeGun.meshRenderer.materials = materials;

        ShooterManager.Instance.activeGunOfficers.Remove(this);
        ShooterManager.Instance.ChackFail();
    }

    public void CloseText()
    {
        scoorText.transform.parent = LevelManager.Instance.ActiveLevel.transform;
    }
}

[Serializable]
public struct Gun
{
    public GunConfig gunConfig;
    public Transform bulletSpawnPoint, bulletReload;
    public GameObject gunModel;
    public SkinnedMeshRenderer meshRenderer;
    public Animator animator;
    public Rigidbody rigidbody;
    public int animCount;
}