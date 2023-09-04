using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using GPHive.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YourBest : MonoBehaviour
{
    private Vector3 offset, startPos;
    private bool follow = false;
    private Transform followTarget;
    [SerializeField] private TextMeshPro rangeText;
    private float one = 1;
    private BoxCollider collider;

    private void OnEnable()
    {
        startPos = transform.localPosition;
        EventManager.LevelStarted += GameStart;
        EventManager.LevelFailed += GameEnd;
        EventManager.LevelSuccessed += GameEnd;
    }

    private void OnDisable()
    {
        EventManager.LevelStarted -= GameStart;
        EventManager.LevelFailed -= GameEnd;
        EventManager.LevelSuccessed -= GameEnd;
    }

    private void GameStart()
    {
        collider = GetComponent<BoxCollider>();

        var transformZOffset = PlayerPrefs.GetFloat("TransformZOffset", 0);
        transform.localPosition = new Vector3(startPos.x, startPos.y, startPos.z + transformZOffset);
        rangeText.text = "Your Best" + "\n" + (int)(transform.localPosition.z - startPos.z) + " M";

        //  collider.center = new Vector3(collider.center.x, collider.center.y, -transform.localPosition.z);
    }

    private void GameEnd()
    {
        if (follow)
        {
            transform.position = new Vector3(followTarget.transform.position.x, 7,
                followTarget.transform.position.z - offset.z - 2);


            rangeText.text = "Your Best" + "\n" + (int)(transform.localPosition.z - startPos.z) + " M";

            PlayerPrefs.SetFloat("TransformZOffset", transform.localPosition.z - startPos.z);
            transform.DOPunchScale(Vector3.one, one, 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GunParent"))
        {
            offset = other.transform.position - transform.position;
            followTarget = other.transform;
            follow = true;
        }
    }
}