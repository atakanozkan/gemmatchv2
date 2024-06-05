using System;
using System.Collections;
using System.Collections.Generic;
using BlockShift;
using UnityEngine;

public class ParticleBehaviour : MonoBehaviour
{
    [SerializeField] private PoolItem _poolItem;
    [SerializeField] private ParticleSystem _particleSystem;
    private void OnEnable()
    {
        StartCoroutine(WaitThenSendPool());
    }

    private IEnumerator WaitThenSendPool()
    {
        yield return new WaitForSeconds(2f);
        if (_poolItem.GetPoolItemType() != PoolItemType.RainbowParticle)
        {
            _particleSystem.Stop();
            PoolManager.instance.ResetPoolItem(_poolItem);
        }
    }
    
}
