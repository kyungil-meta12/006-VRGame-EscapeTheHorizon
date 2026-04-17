using UnityEngine;
using System.Collections.Generic;

public class SG_ObjectPool : MonoBehaviour
{
    public static SG_ObjectPool Inst;
    private Dictionary<GameObject, Stack<GameObject>> pool = new();

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
            pool[prefab] = new Stack<GameObject>();
        }
        if(pool[prefab].Count == 0)
        {
            var newInst = Instantiate(prefab);
            newInst.GetComponent<PoolObject>().SetStack(pool[prefab]);
            return newInst;
        }

        var stack = pool[prefab];
        var inst = stack.Peek();
        inst.SetActive(true);
        stack.Pop();
        return inst;
    }
}
