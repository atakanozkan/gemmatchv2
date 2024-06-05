using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockShift
{
    [Serializable]
    public class ObjectPool
    {
        public PoolItemType poolItemType;
        public List<PoolItem> availableQueue;
        public List<PoolItem> usingQueue;

        public ObjectPool(PoolItemType poolItemType, int size)
        {
            this.poolItemType = poolItemType;
            availableQueue = new List<PoolItem>(size);
            usingQueue = new List<PoolItem>(size);
        }

        public PoolItemType GetPoolItemType()
        {
            return poolItemType;
        }

        public void SetPoolItemType(PoolItemType poolItemType)
        {
            this.poolItemType = poolItemType;
        }

        public int GetAvailableCount()
        {
            return availableQueue.Count;
        }
    }
}