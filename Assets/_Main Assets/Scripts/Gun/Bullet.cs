using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Poolable
{
    [SerializeField] private float closingTime;


    private void OnEnable()
    {
        HideMe();
    }

    private void HideMe()
    {
        Invoke(nameof(Hide), closingTime);
    }

    private void Hide()
    {
        ShooterManager.Instance.bullets.Remove(this);
        gameObject.SetActive(false);
    }
}