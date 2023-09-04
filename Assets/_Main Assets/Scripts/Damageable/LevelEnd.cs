using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GPHive.Core;
using GPHive.Game;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;

public class LevelEnd : MonoBehaviour, IDamageableFromGun
{
    [SerializeField] private float damagePunchTime = .3f;
    [SerializeField] private int health, moneyAmount;
    [SerializeField] private Collider collider;
    [SerializeField] private GameObject money, canvas;
    [SerializeField] private TextMeshPro healthText;
    [SerializeField] private ParticleSystem moneyParticle;
    [SerializeField] private MeshRenderer meshRenderer;

    private static readonly int Die1 = Animator.StringToHash("Die");

    private void Start()
    {
        RefreshText();
    }

    public void TakeDamge()
    {
        if (GameManager.Instance.finish)
        {
            if (health - 1 <= 0)
            {
                Die();
            }
            else
            {
                if (!DOTween.IsTweening(transform))
                    transform.DOPunchScale(Vector3.one * .1f, damagePunchTime);
                health -= 1;
                RefreshText();
            }
        }
    }

    public void Die()
    {
        PlayerEconomy.Instance.AddMoney(moneyAmount);
        meshRenderer.enabled = false;
        money.SetActive(false);
        canvas.gameObject.SetActive(false);
        moneyParticle.Play();
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        transform.DOKill();
        collider.enabled = false;
    }

    private void RefreshText()
    {
        healthText.text = health.ToString();
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