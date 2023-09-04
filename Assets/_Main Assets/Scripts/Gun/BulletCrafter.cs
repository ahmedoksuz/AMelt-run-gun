using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BulletCrafter : MonoBehaviour
{
    [SerializeField] private TextMeshPro bulletCountText;
    [SerializeField] private SkinnedMeshRenderer lavaSkinned;
    private int destroyCount;
    private float dotTwo = .2f;
    public int maxAmmo = 0;

    private void Start()
    {
        maxAmmo = GunMiniGameManager.Instance.gunOfficer.activeGun.gunConfig.ammoCapasty;
        RefreshText();
    }

    public void RemoveDestroyCount()
    {
        destroyCount--;
        RefreshText();
    }

    private void RefreshText()
    {
        bulletCountText.text = (destroyCount).ToString();
        var value = 100 * destroyCount / maxAmmo;

        if (value > 100)
            value = 100;
        else if (value < 0) value = 0;

        TweenBlendshape(value);
    }

    public void TweenBlendshape(float targetWeight)
    {
        lavaSkinned.DOKill();

        DOTween.To(() => lavaSkinned.GetBlendShapeWeight(0),
            weight => lavaSkinned.SetBlendShapeWeight(0, weight),
            targetWeight,
            dotTwo);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Melted"))
        {
            other.gameObject.SetActive(false);

            destroyCount += (int)other.GetComponent<MeltedObject>().weight;
            RefreshText();
        }
    }
}