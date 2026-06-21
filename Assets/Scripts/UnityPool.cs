using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UnityPool : MonoBehaviour
{
    private ObjectPool<SingleItem> pool;
    private int poolSize = 10;
    private int maxPoolSize = 100;
    public SingleItem _item;
    // Start is called before the first frame update
    void Awake()
    {
        pool = new ObjectPool<SingleItem>(OnCreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, poolSize, maxPoolSize);
    }
    // Update is called once per frame
    private SingleItem OnCreatePooledItem()
    {
        SingleItem item = Instantiate(_item);
        return item;
    }

    private void OnTakeFromPool(SingleItem item)
    {
        item.gameObject.SetActive(true);
    }
    private void OnReturnedToPool(SingleItem item)
    {
        item.gameObject.SetActive(false);
    }
    private void OnDestroyPoolObject(SingleItem item)
    {
        Destroy(item.gameObject);
    }

    public SingleItem GetPooledItem()
    {
        return pool.Get();
    }

    public void ReleasePooledItem(SingleItem item)
    {
        pool.Release(item);
    }
}
