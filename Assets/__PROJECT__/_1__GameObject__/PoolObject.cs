using UnityEngine;
using System.Collections.Generic;

public class PoolObject : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> pool;

    public void SetPool(List<GameObject> pool_)
    {
        pool = pool_;
    }

    public void ReturnInstance(GameObject inst)
    {
        inst.SetActive(false);
        pool.Add(inst);    
    }
}