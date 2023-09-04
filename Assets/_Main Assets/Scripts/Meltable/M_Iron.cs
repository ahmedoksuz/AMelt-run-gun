using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GPHive.Game;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class M_Iron : IMeltable
{
    #region Garbage

    private float dotfive = .5f;
    private float dottwo = .2f;
    private float dotfifteen = .15f;

    #endregion

    private Coroutine MeltCO;

    private float meltedAmount;
    private bool dead;

    public float currentFrame;
    [SerializeField] private List<Transform> meshParents = new();
    private SkinnedMeshRenderer[] meshRenderer;
    [SerializeField] private MeltedObject meltedObject;

    [SerializeField] private float meltAmountMultiple;

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
    private MaterialPropertyBlock heatingBlock;
    private float currentMeltedWeight;
    [SerializeField] private GameObject flameWall;

    private void Awake()
    {
        maxHealth = health;
    }

    private void Start()
    {
        foreach (var parent in meshParents)
            parent.gameObject.SetActive(false);

        var parentMesh = meshParents[Random.Range(0, meshParents.Count)];
        parentMesh.gameObject.SetActive(true);


        meshRenderer = new SkinnedMeshRenderer[parentMesh.transform.childCount];

        for (var i = 0; i < parentMesh.transform.childCount; i++)
            meshRenderer[i] = parentMesh.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();


        collider = GetComponent<Collider>();
        SetHealthText();
        currentFrame = 0;
        heatingBlock = new MaterialPropertyBlock();
        currentMeshRenderer = meshRenderer[0];
    }

    public override void Invisiable()
    {
        if (!melting)
            foreach (var skinnedMesh in meshRenderer)
                skinnedMesh.material.SetFloat("_Opacity", 0.5f);
    }

    public override void StartMelt(string state)
    {
        if (!melting && !dead)
        {
            melting = true;
            StopMeltCO();
            MeltCO = StartCoroutine(CO_Melt());
        }
    }

    public override void StopMelt()
    {
        melting = false;
        ForceStopAction();
        StopMeltCO();
    }

    public override void ForceStopAction()
    {
        if (!dead) CreateMeltedObject();
    }

    private void StopMeltCO()
    {
        if (MeltCO != null)
        {
            StopCoroutine(MeltCO);
            MeltCO = null;
        }
    }

    private void SetHealthText()
    {
        var _health = (int)(health - meltedAmount);
        if (_health >= 1)
            healthText.text = _health.ToString() + " lbs";
    }

    private float t1 = .1f;
    private float t2 = .2f;
    private int meshId;
    private SkinnedMeshRenderer currentMeshRenderer;
    private bool scaleCanEffectByFrame;

    private IEnumerator CO_Melt()
    {
        while (currentFrame < .9f || MeltedAmount < health)
        {
            var _targetFrame = 1f / health * MeltedAmount;
            var _newFrame = Mathf.Lerp(currentFrame, _targetFrame, t2);
            currentFrame = _newFrame;

            if (currentFrame > 1)
                currentFrame = 1;

            var _shape1 = Mathf.Abs(currentFrame * 100 * 4 - 100 * meshId);
            if (_shape1 >= 100)
            {
                //_shape1 = 100;
                meshId++;
                if (meshId >= 3) meshId = 3;
            }

            currentMeshRenderer.SetBlendShapeWeight(0, _shape1);
            var _id = 0;
            foreach (var _renderer in meshRenderer)
            {
                if (meshId == _id)
                {
                    currentMeshRenderer = _renderer;
                    _renderer.gameObject.SetActive(true);
                }
                else
                {
                    _renderer.gameObject.SetActive(false);
                }

                _id++;
            }

            //heatingBlock.SetFloat("_MELT", currentFrame);
            currentMeshRenderer.SetPropertyBlock(heatingBlock);
            yield return new WaitForEndOfFrame();
        }

        meltScaleMesh.DOKill();
        currentMeshRenderer.SetBlendShapeWeight(0, 100);
        dead = true;
        Die();
    }

    private void CreateMeltedObjectForce()
    {
        var _clone = Poolable.Get<MeltedObject>();

        _clone.ForcedToSpawn(meshRenderer[^1]);
        if (!_clone)
        {
            CreateMeltedObjectForce();
            return;
        }

        _clone.maxHP = maxHealth;
        _clone.transform.SetParent(transform);
        _clone.transform.localScale = Vector3.one;
        _clone.transform.localPosition = Vector3.zero;

        var _y = -.15f;
        _clone.posY = _y;
        _clone.weight = currentMeltedWeight * meltAmountMultiple;


        _clone.enabled = true;

        _clone.gameObject.SetActive(true);
    }

    private void CreateMeltedObject()
    {
        if (currentMeltedWeight > 0)
        {
            if (dead)
            {
                meltedObject.weight = currentMeltedWeight * meltAmountMultiple;

                var _y = -.15f;
                meltedObject.posY = _y;
                meltedObject.enabled = true;
                meltedObject.maxHP = maxHealth;
                meltedObject.gameObject.SetActive(true);
                meltedObject.ForcedToSpawn(meshRenderer[^1]);
            }
            else
            {
                CreateMeltedObjectForce();
            }

            currentMeltedWeight = 0;
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


    private void Die()
    {
        if (flameWall)
            flameWall.SetActive(false);

        foreach (var _renderer in meshRenderer)
            _renderer.enabled = false;

        collider.enabled = false;
        healthText.transform.parent.DOScale(Vector3.zero, dottwo);

        enabled = false;
        CreateMeltedObject();
    }
}