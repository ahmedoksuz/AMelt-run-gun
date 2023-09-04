using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GPHive.Core;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterManager : Singleton<ShooterManager>
{
    [HideInInspector] public List<GunOfficer> activeGunOfficers = new();
    public List<Bullet> bullets = new();
    [HideInInspector] public bool shoot;
    [SerializeField] private float speed;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public SwerveController gunSwerveController;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void StartShootingBullets()
    {
        foreach (var activeGun in activeGunOfficers) activeGun.StratShoot();
    }

    private void StopShootingBullets()
    {
        foreach (var activeGun in activeGunOfficers) activeGun.StopShoot();
    }

    private void Update()
    {
        if (shoot) MoveBullets();
    }

    private void MoveBullets()
    {
        for (var i = 0; i < bullets.Count; i++)
            bullets[i].transform.position += bullets[i].transform.forward * (speed * Time.deltaTime);
    }

    private float one = 1;

    public void JumpBack()
    {
        gunSwerveController.enabled = false;
        transform.DOMoveZ(transform.position.z - 10, one).OnComplete(() => { gunSwerveController.enabled = true; });
    }


    public void ChackFail()
    {
        if (activeGunOfficers.Count <= 0 && !GameManager.Instance.finish)
        {
            GameManager.Instance.LoseLevel();
            rb.isKinematic = true;
        }
        else if (activeGunOfficers.Count <= 0 && GameManager.Instance.finish)
        {
            GameManager.Instance.WinLevel();
            rb.isKinematic = true;
        }
    }
}