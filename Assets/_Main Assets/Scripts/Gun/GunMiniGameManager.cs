using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using GPHive.Core;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunMiniGameManager : Singleton<GunMiniGameManager>
{
    public int maxGunAmount;
    [SerializeField] private Transform ammoCreatTransform, moverObj;
    [SerializeField] public GunOfficer gunOfficer;
    private GunOfficer activeGunOfficer = new();

    [SerializeField] private AmmoSpawnAnimatioObj ammoSpawnAnimatioObj;

    [SerializeField] private SwerveController gunSwerveController;
    [SerializeField] private BulletCrafter bulletCrafter;
    [SerializeField] private Transform righPos, leftPos;

    [SerializeField] private List<Transform> bulletMovPointTransforms;
    [SerializeField] private Animator bulletCrafterAnimator;
    private Vector3[] bulletMovPoints;
    private Quaternion[] bulletMovRotations;
    [SerializeField] private ParticleSystem dustParticle;


    private void Awake()
    {
        PlayerPrefs.SetFloat("MeltedWeight", 0);

        bulletMovPoints = new Vector3[bulletMovPointTransforms.Count];
        bulletMovRotations = new Quaternion[bulletMovPointTransforms.Count];
        for (var i = 0; i < bulletMovPointTransforms.Count; i++)
        {
            bulletMovPoints[i] = bulletMovPointTransforms[i].transform.position;
            bulletMovRotations[i] = bulletMovPointTransforms[i].transform.rotation;
        }


        gunOfficer.gunLevel = 0;
        gunOfficer.SetActiveGun();

        ShooterManager.Instance.gunSwerveController = gunSwerveController;
    }


    private void CameraSwich(string CameraName, Transform followingObj)
    {
        CinemachineVirtualCamera myCam = null;
        foreach (var cam in CameraManager.Instance.cameras)
            if (cam.key == CameraName)
                myCam = cam.value;

        myCam.m_Follow = followingObj;
        CameraManager.Instance.SwitchCamera(CameraName);
    }

    public void StartCreatingAmmo()
    {
        CameraSwich("MiniGameCam", transform);
        ammoAmountForCreat = (int)PlayerPrefs.GetFloat("MeltedWeight", 0);
        dustParticle.Play();
        StartCoroutine(CreatAmmoWithOutModel());
        StartCoroutine(CreatAmmoForAnim());
    }

    private float weaponZAdvanceAmountForStart = 20, gapBetweenWeaponsAtBeginning = 1, one = 1;

    private static readonly int GunCount = Animator.StringToHash("GunCount");

    private void StartMoveFoGuns()
    {
        CameraSwich("GunGameCam", moverObj);


        activeGunOfficer.transform.parent = moverObj;


        activeGunOfficer.activeGun.animator.SetFloat(GunCount, activeGunOfficer.activeGun.animCount);


        GameManager.Instance.unlockWeaponBool = true;
        gunSwerveController.enabled = true;
        ShooterManager.Instance.shoot = true;
        ShooterManager.Instance.activeGunOfficers.Add(activeGunOfficer);
        ShooterManager.Instance.StartShootingBullets();
    }

    private bool ammoCreatingStoped = false;

    private void StopCreatingAmmo()
    {
        if (!ammoCreatingStoped)
        {
            ammoCreatingStoped = true;
            ammoSpawnAnimatioObj.StopAnim();
            Invoke(nameof(StartMoveFoGuns), bulletReloadStageTime);
            dustParticle.Stop();
            activeGunOfficer.CloseText();
        }
    }

    private float dotOne = .2f;
    private int bulletModelCount = 25;
    private float timer = 2.5f;
    private int createdAmmoAmountWithOutModel, createdAmmoForAnim, createdAmmoAmount;
    [HideInInspector] public int ammoAmountForCreat;
    private float bulletReloadStageTime = .75f;

    public void CreatAmmo()
    {
        if (createdAmmoAmount < bulletModelCount)
        {
            var bigBullet = Poolable.Get<BigBullet>();
            bigBullet.transform.position = ammoCreatTransform.position;
            bigBullet.gameObject.SetActive(true);
            var scale = bigBullet.transform.localScale;

            createdAmmoAmount++;

            var bulletMovementSequence = DOTween.Sequence();
            var newBulletReloadStageTime = bulletReloadStageTime + Random.Range(0.01f, 0.07f);
            for (var i = 0; i < bulletMovRotations.Length; i++)
                bulletMovementSequence.Append(bigBullet.transform.DORotateQuaternion(bulletMovRotations[i],
                    newBulletReloadStageTime / bulletMovRotations.Length).SetEase(Ease.Linear));

            bigBullet.transform.DOPath(bulletMovPoints, newBulletReloadStageTime, PathType.Linear).SetOptions(false)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    bigBullet.transform.DOMove(gunOfficer.activeGun.bulletReload.transform.position,
                        newBulletReloadStageTime / 2);
                    bigBullet.transform
                        .DOScale(Vector3.one * .1f, newBulletReloadStageTime / 2)
                        .OnComplete(
                            () =>
                            {
                                gunOfficer.activeGun.gunModel.transform.DOComplete();
                                bigBullet.transform.DOKill();
                                bigBullet.transform.localScale = scale;
                                bigBullet.gameObject.SetActive(false);
                            });
                });
        }
        else
        {
            activeGunOfficer = gunOfficer;
            StopCreatingAmmo();
        }
    }


    private IEnumerator CreatAmmoWithOutModel()
    {
        var newBulletReloadStageTime = bulletReloadStageTime;


        int number;
        if (ammoAmountForCreat < 150)
            number = 1;
        else if (ammoAmountForCreat < 300)
            number = 2;
        else if (ammoAmountForCreat < 400)
            number = 3;
        else if (ammoAmountForCreat < 500)
            number = 4;
        else if (ammoAmountForCreat < 1000)
            number = 8;
        else
            number = 16;

        while (createdAmmoAmountWithOutModel < ammoAmountForCreat)
        {
            if (createdAmmoAmountWithOutModel + number > ammoAmountForCreat) number = 1;

            for (var i = 0; i < number; i++)
            {
                bulletCrafter.RemoveDestroyCount();
                createdAmmoAmountWithOutModel++;
                gunOfficer.relodedAmmoAmount++;
                gunOfficer.gunAllRelodedAmmoAmount++;

                gunOfficer.RefreshScoorText();
                gunOfficer.CheckLevelUp();
            }

            yield return BetterWaitForSeconds.Wait(timer / ammoAmountForCreat * number);
        }
    }

    private IEnumerator CreatAmmoForAnim()
    {
        while (createdAmmoForAnim < bulletModelCount + 1)
        {
            ammoSpawnAnimatioObj.StartAnim();
            createdAmmoForAnim++;

            yield return BetterWaitForSeconds.Wait(timer / (bulletModelCount + 1));
        }
    }
}