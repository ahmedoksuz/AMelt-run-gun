using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GPHive.Core;
using GPHive.Game;
using TMPro;
using UnityEngine;

public class MeltedObject : Poolable
{
    #region Garbage

    private Vector3 scale = new(0.3579648f, 0.46f, 0.3579648f);
    private Vector3 canvasScale = new(.5f, .5f, .5f);
    private float scaleTime = .05f;
    private float scaleTime2 = .2f;
    private float domoveX = -5.747f;
    private float domoveXTime = .3f;
    private float one = 1f;
    private float y = 2.21f;

    #endregion

    private bool canMove;
    public float weight;


    private string meltedMaterialKey = "_height";

    private Material meltedMaterial;

    [Header("Melt Text")] [SerializeField] private Transform canvas;
    [SerializeField] private TextMeshPro meltText;

    public Transform meltMesh;
    private SkinnedMeshRenderer meltMeshRenderer;
    private float speed;

    private void Awake()
    {
        meltMesh = transform.GetChild(0).GetChild(0);
        meltMeshRenderer = meltMesh.GetComponent<SkinnedMeshRenderer>();
        meltedMaterial = meltMeshRenderer.material;
    }

    private Quaternion g1 = Quaternion.Euler(0, -180f, 0);
    private float dotfive = .5f;
    private Vector3 g2 = new(1.234829f, 0.5285704f, 0.7285879f); // meltmesh (ilk child) hareket ederkenki scale
    private Vector3 g3 = new(-10.22f, -2.87f); // parent borudaki pozisyonu ama parent level
    private float g4 = -1.1125f;
    private float dottwo = .2f;
    private float zero = 0f;
    private Vector3 g5 = new(0.4f, 2f, 0.7f); // meltmesh (ilk child) borudaki scale
    [HideInInspector] public float posY;

    public void ForcedToSpawn(SkinnedMeshRenderer skinnedMesh)
    {
        //Renk Değiştir
        meltMeshRenderer.material.SetColor("_BaseColor", skinnedMesh.material.GetColor("_BaseColor"));

        meltMesh.localScale = Vector3.zero;
    }

    [SerializeField] private float minScale, maxScale;
    [SerializeField] private Transform ScaleParent;

    public float maxHP;

    private void Scaler()
    {
        var firstCalculate = (maxScale - minScale) / maxHP;
        var result = firstCalculate * weight;
        if (result < minScale) result = minScale;
        ScaleParent.localScale = Vector3.one * result;
        ScaleParent.transform.position -= new Vector3(0, -(result / 2), 0);
    }

    private void OnEnable()
    {
        transform.DOKill();
        transform.SetParent(LevelManager.Instance.ActiveLevel.transform);
        Scaler();
        var _weight = weight >= 1 ? weight : 1;
        meltText.text = _weight.ToString("0");

        meltMesh.DOScale(g2, dottwo).OnComplete(() => { meltMesh.DOScale(g5, dottwo).OnComplete(() => { }); });
        g3.z = transform.position.z;
        g3.y = posY;
        canMove = true;
        speed = 5f;
        transform.DOLocalMoveY(g3.y, dotfive).OnComplete(() => { });
        transform.DOLocalMoveX(g3.x, dotfive).OnComplete(() =>
        {
            meltMesh.DOLocalMoveY(g4, dottwo);
            canvas.DOScale(canvasScale, scaleTime2);

            speed = 30f;
        });

        PlayerPrefs.SetFloat("MeltedWeight", PlayerPrefs.GetFloat("MeltedWeight", 0) + weight);
    }

    private void Update()
    {
        if (canMove)
            transform.Translate(Vector3.forward * (Time.deltaTime * speed));
    }
}