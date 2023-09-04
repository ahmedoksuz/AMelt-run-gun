using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using Unity.VisualScripting;
using UnityEngine;

public class Target : MonoBehaviour, IDamageableFromGun
{
    [SerializeField] private float damagePunchTime = .3f;
    [SerializeField] private int health;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private List<Rigidbody> brokenParts = new();
    [SerializeField] private Collider collider;

    public void TakeDamge()
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
        }
    }

    public void Action()
    {
        Invoke(nameof(OpenCollider), .5f);
    }


    private void OpenCollider()
    {
        collider.enabled = true;
    }

    public void Die()
    {
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        transform.DOKill();
        meshRenderer.enabled = false;

        foreach (var broken in brokenParts) broken.isKinematic = false;
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