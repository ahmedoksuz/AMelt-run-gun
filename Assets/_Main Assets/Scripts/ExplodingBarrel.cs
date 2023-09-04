using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplodingBarrel : IMeltable
{
    #region Garbage

    private float dotfive = .5f;
    private float dottwo = .2f;
    private float dotfifteen = .15f;

    #endregion

    [SerializeField] private float explosionRadius = 5f; // Patlama yarıçapı
    [SerializeField] private LayerMask meltableLayer;

    [SerializeField] private int explodPower = 10; // Yukarı doğru kuvvet değiştiricisi

    private Coroutine MeltCO;
    private float meltedAmount;
    private bool dead;
    [SerializeField] private ParticleSystem explodeParticle;

    private float MeltedAmount
    {
        get => meltedAmount;
        set
        {
            meltedAmount = value;
            SetHealthText();
        }
    }

    private Collider collider;
    private float currentMeltedWeight;
    [SerializeField] private GameObject flameWall;

    private void Start()
    {
        collider = GetComponent<Collider>();
        SetHealthText();
    }

    public override void StartMelt(string _state)
    {
        if (!melting)
        {
            melting = true;
            StopMeltCO();
            MeltCO = StartCoroutine(CO_Melt());
        }
    }

    public override void StopMelt()
    {
        melting = false;
        StopMeltCO();
    }

    public override void ForceStopAction()
    {
    }

    public override void Invisiable()
    {
    }

    private void StopMeltCO()
    {
        if (MeltCO != null)
        {
            StopCoroutine(MeltCO);
            MeltCO = null;
        }
    }

    public override void TakeDamage(float damage)
    {
        if (MeltedAmount < health)
        {
            MeltedAmount += damage;
            currentMeltedWeight += damage;

            if (currentMeltedWeight > health)
                currentMeltedWeight = health;
        }
    }


    private void SetHealthText()
    {
        var _health = (int)(health - meltedAmount);
        if (_health >= 1)
            healthText.text = _health.ToString() + " lbs";
    }


    [SerializeField] private Vector3 startScale = new(1f, 1f, 1f);
    [SerializeField] private Vector3 targetScale = new(1.3f, 1.3f, 1.3f);
    [SerializeField] private float scaleUpDowndurationTime = 2f;

    private float timer = 0f;
    private bool isScalingUp = true;

    private IEnumerator CO_Melt()
    {
        while (MeltedAmount < health)
            /*timer += Time.deltaTime;

            var t = Mathf.PingPong(timer, scaleUpDowndurationTime) / scaleUpDowndurationTime;

            if (isScalingUp)
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            else
                transform.localScale = Vector3.Lerp(targetScale, startScale, t);

            if (timer >= scaleUpDowndurationTime)
            {
                timer = 0f;
                isScalingUp = !isScalingUp;
            }*/
            yield return new WaitForEndOfFrame();

        meltScaleMesh.DOKill();
        dead = true;
        Die();
    }

    private void Die()
    {
        Explode();

        if (flameWall)
            flameWall.SetActive(false);

        collider.enabled = false;
        healthText.transform.parent.DOScale(Vector3.zero, dottwo);

        enabled = false;
        gameObject.SetActive(false);
    }

    public void Explode()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, meltableLayer);

        foreach (var hitCollider in hitColliders)
            if (hitCollider.TryGetComponent(out IMeltable meltable))
            {
                meltable.StartMelt("Gun");
                meltable.TakeDamage(explodPower);
                if (meltable.health > explodPower) meltable.ForceStopAction();
            }

        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        explodeParticle.Play();
    }
}