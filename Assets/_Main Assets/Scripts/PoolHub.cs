using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolHub : Singleton<PoolHub>
{
    [SerializeField] private GameObject bulletReferance;
    [SerializeField] private int bulletPoolCount;
    [SerializeField] private int bulletExpandCount;

    [SerializeField] private GameObject bigBulletReferance;
    [SerializeField] private int bigBulletPoolCount;
    [SerializeField] private int bigBulletExpandCount;

    [SerializeField] private GameObject meltedPeaceReferance;
    [SerializeField] private int meltedPeacePoolCount;
    [SerializeField] private int meltedPeaceExpandCount;

    [SerializeField] private GameObject bulletExplodeParticleReferance;
    [SerializeField] private int bulletExplodeParticlePoolCount;
    [SerializeField] private int bulletExplodeParticleExpandCount;

    private void Start()
    {
        Poolable.CreatePool<MeltedObject>(meltedPeaceReferance, meltedPeacePoolCount, meltedPeaceExpandCount);
        Poolable.CreatePool<Bullet>(bulletReferance, bulletPoolCount, bulletExpandCount);
        Poolable.CreatePool<BigBullet>(bigBulletReferance, bigBulletPoolCount, bigBulletExpandCount);
        Poolable.CreatePool<BulletExplodeParticle>(bulletExplodeParticleReferance, bulletExplodeParticlePoolCount,
            bulletExplodeParticleExpandCount);
    }
}