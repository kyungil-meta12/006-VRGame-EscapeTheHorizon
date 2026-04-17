using UnityEngine;
using System.Collections.Generic;

public class PoolObject : MonoBehaviour
{
    [HideInInspector]
    public Stack<GameObject> stack;

    public void SetStack(Stack<GameObject> stack_)
    {
        stack = stack_;
    }

    public void ReturnInstance(GameObject inst)
    {
        inst.SetActive(false);
        stack.Push(inst);    
    }
}