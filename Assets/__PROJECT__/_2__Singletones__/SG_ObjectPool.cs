using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

public class SG_ObjectPool : MonoBehaviour
{
    public static SG_ObjectPool Inst;
    private Dictionary<GameObject, List<GameObject>> pool = new();

    void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }

        Inst = this;
        print("[SG_ObjectPool] Created instance.");
    }

    void OnDestroy()
    {
        Inst = null;
    }

    public GameObject GetInstance(GameObject prefab)
    {
        if(!pool.ContainsKey(prefab))
        {
            pool[prefab] = new List<GameObject>();
        }
        if(pool[prefab].Count == 0)
        {
            var newInst = Instantiate(prefab);
            newInst.GetComponent<PoolObject>().SetPool(pool[prefab]);
            return newInst;
        }
        
        var list = pool[prefab];
        var index = list.Count - 1;
        var inst = list[index];
        list.RemoveAt(index);
        inst.SetActive(true);
        inst.GetComponent<PoolObject>().SetPool(list);
        return inst;
    }
}
