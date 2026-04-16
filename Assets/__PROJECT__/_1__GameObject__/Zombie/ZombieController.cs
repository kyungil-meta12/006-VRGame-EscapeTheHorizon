using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : PoolObject
{
    public Transform trackTarget;
    public Collider[] ragdollColliders;
    public Collider headCollider;
    public int totalHp;
    int currHp;
    Animator anim;
    NavMeshAgent agent;
    bool isAttack = false;
    bool isDead = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ResetState();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
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
        // 1. 경로를 계산 중이지 않고
        if (!agent.pathPending)
        {
            // 2. 남은 거리가 정지 거리보다 작거나 같으며
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // 3. 경로가 없거나 속도가 거의 없을 때 (정지 상태)
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
