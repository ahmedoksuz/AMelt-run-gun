using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GPHive.Game;
using MoreMountains.NiceVibrations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageableFromGun
{
    [SerializeField] private float damagePunchTime = 1;
    [SerializeField] private int health, moneyAmount;
    [SerializeField] private Animator animator;
    [SerializeField] private Material grayMaterial;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Collider collider;
    [SerializeField] private TextMeshPro healthText;
    [SerializeField] private ParticleSystem moneyParticle;
    private static readonly int Die1 = Animator.StringToHash("Die");
    [SerializeField] private TextMeshPro moneyText;
    [SerializeField] private Animator moneyTextAnimator;
    private static readonly int Play = Animator.StringToHash("Play");
    [SerializeField] private CoefficientUpgrade incomeUpgrade;

    private void Start()
    {
        RefreshText();
    }

    public void TakeDamge()
    {
        if (health - 1 <= 0)
        {
            Die();
        }
        else
        {
            health -= 1;
            RefreshText();
            if (!DOTween.IsTweening(transform))
                transform.DOPunchScale(Vector3.one * .9f, damagePunchTime);
        }
    }

    public void Action()
    {
        Invoke(nameof(OpenCollider), .5f);
    }

    private void RefreshText()
    {
        healthText.text = health.ToString();
    }

    private void OpenCollider()
    {
        collider.enabled = true;
    }

    public void Die()
    {
        moneyText.text = "$ " + moneyAmount * (1 + PlayerEconomy.Instance.levelByIncome * incomeUpgrade.Level);
        moneyText.gameObject.SetActive(true);
        moneyTextAnimator.enabled = true;
        moneyTextAnimator.SetTrigger(Play);
        // moneyParticle.Play();
        PlayerEconomy.Instance.AddMoney(moneyAmount);
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        transform.DOComplete();
        healthText.transform.parent.gameObject.SetActive(false);
        meshRenderer.material = grayMaterial;
        animator.SetTrigger(Die1);
        collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            var particle = Poolable.Get<BulletExplodeParticle>();
            particle.transform.position = other.transform.position;
            particle.gameObject.SetActive(true);


            other.gameObject.SetActive(false);
            TakeDamge();
        }
    }
}