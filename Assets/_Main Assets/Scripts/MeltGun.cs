using System;
using System.Collections;
using System.Collections.Generic;
using GPHive.Core;
using NaughtyAttributes;
using UnityEngine;

public class MeltGun : MonoBehaviour
{
    [SerializeField] private Transform meltRay_StartPoint;
    [SerializeField] private LayerMask meltableLayer;
    public IMeltable currentMeltableObject;
    
    private float lastMeltCurrentTime;

    private bool IsGunActive;
    
    private bool canDetect = true;
    
    public void GunActive() => IsGunActive = true;
    public void GunDisable() => IsGunActive = false;

    private void OnEnable()
    {
        GunActive();
    }

    private void Start()
    {
        GunActive();
    }
    

    private bool dragonTarget;
    private void Update()
    {
        if (!IsGunActive || GameManager.Instance.GameState != GameState.Playing)
        {
            if (currentMeltableObject)
            {
                currentMeltableObject.StopMelt();
                currentMeltableObject = null;
            }
            return;
        }
        
        
        if (Physics.Raycast(meltRay_StartPoint.position, Vector3.forward, out var hitInfo, (PlayerUpgrade.Instance.Length), meltableLayer))
        {
            if (hitInfo.transform.TryGetComponent(out IMeltable _meltable))
            {
                if (canDetect)
                {
                    canDetect = false;
                    if (currentMeltableObject)
                    {
                        StopMelt();
                        currentMeltableObject = _meltable;
                    }
                    else
                    {
                        currentMeltableObject = _meltable;                    
                    }    
                }
                else
                {
                    if (currentMeltableObject)
                    {
                        if (!currentMeltableObject.enabled)
                        {
                            canDetect = true;
                            StopMelt();
                        }
                    }
                }

            }
            else
            {
                canDetect = true;
                StopMelt();
            }

            if (currentMeltableObject)
            {
                currentMeltableObject.StartMelt("Gun");
                
                if (Time.time - lastMeltCurrentTime >= 1 / PlayerUpgrade.Instance.Speed)
                {
                    lastMeltCurrentTime = Time.time;
                    
                    currentMeltableObject.TakeDamage(PlayerUpgrade.Instance.Power);
                    
                }
            }

          
        }
        else
        {
            canDetect = true;
            StopMelt();
        }
        
    }

    private void StopMelt()
    {
        if (currentMeltableObject)
        {
            currentMeltableObject.StopMelt();
            currentMeltableObject = null;
        }
    }
}
