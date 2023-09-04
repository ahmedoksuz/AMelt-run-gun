using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Obsticle : MonoBehaviour
{
    [SerializeField] private Collider collider;


    public void Action()
    {
        Invoke(nameof(OpenCollider), .5f);
    }


    private void OpenCollider()
    {
        collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            var particle = Poolable.Get<BulletExplodeParticle>();
            particle.transform.position = other.transform.position;
            particle.gameObject.SetActive(true);

            other.gameObject.SetActive(false);
        }
    }
}