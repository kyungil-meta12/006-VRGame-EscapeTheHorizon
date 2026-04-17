using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : PoolObject
{
    public GameObject bloodParticlePrefab;
    public Collider[] ragdollColliders;
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

    IEnumerator AutoReturn()
    {
        yield return new WaitForSeconds(3f);
        ReturnInstance(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead) {
            agent.SetDestination(trackTarget.position);
            isAttack = IsDestinationReached();
            anim.SetBool("IsAttack", isAttack);
        }
    }

    public void EnableRagdoll()
    {
        foreach(var c in ragdollColliders)
        {
            c.isTrigger = false;
            c.attachedRigidbody.isKinematic = false;
        }
        isDead = true;
        agent.enabled = false;
        anim.enabled = false;
        SG_GameMan.Inst.currentKill++;
        StartCoroutine(AutoReturn()); // 3초 후 인스턴스 리턴
    }

    public void SetPosition(Vector3 pos)
    {
        agent.enabled = true;
        agent.Warp(pos);
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
        anim.enabled = true;
        anim.SetBool("IsAttack", isAttack);
    }

    public void GiveDamage(Vector3 hitPoint, Vector3 direction, int dmg, bool isHeadShot)
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

        var newBloodParticle = SG_ObjectPool.Inst.GetInstance(bloodParticlePrefab);
        newBloodParticle.transform.position = hitPoint;
        newBloodParticle.transform.forward = -direction;
        newBloodParticle.GetComponent<Blood>().Play();
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
