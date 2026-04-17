using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : PoolObject
{
    public Collider[] ragdollColliders;
    public Collider headCollider;
    public int totalHp;
    int currHp;
    Animator anim;
    NavMeshAgent agent;
    bool isAttack = false;
    bool isDead = false;

    
    [HideInInspector]
    public Transform trackTarget;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ResetState();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead || !trackTarget)
        {
            return;
        }
        
        agent.SetDestination(trackTarget.position);
        isAttack = IsDestinationReached();
        anim.SetBool("IsAttack", isAttack);
    }

    public void EnableRagdoll()
    {
        foreach(var c in ragdollColliders)
        {
            c.isTrigger = true;
            c.attachedRigidbody.isKinematic = true;
        }
        isDead = true;
        agent.enabled = false;
        anim.enabled = false;
        SG_GameMan.Inst.currentKill++;
    }

    public void ResetState()
    {
        foreach(var c in ragdollColliders)
        {
            c.isTrigger = true;
            c.attachedRigidbody.isKinematic = true;
        }
        currHp = totalHp;
        isDead = false;
        isAttack = false;
        agent.enabled = true;
        agent.Warp(transform.position);
        anim.enabled = true;
        anim.SetBool("IsAttack", isAttack);
    }

    public void GiveDamage(int dmg, bool isHeadShot)
    {
        if(isDead)
        {
            return;
        }

        currHp -= isHeadShot ? dmg * 2 : dmg;
        if(currHp < 0)
        {
            EnableRagdoll();
        }
    }

    // 애니메이션 클립 이벤트 호출 함수
    public void OnHit()
    {
        
    }

    bool IsDestinationReached()
    {
        return !agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance) && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }
}
