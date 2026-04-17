using System.Reflection;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GunController : MonoBehaviour
{
    public Transform rayOrigin;

    public GameObject ammoParticlePrefab;
    public Transform ammoPosition;

    public AudioClip[] fireSounds;

    public GameObject gunFlamePrefab;
    public Transform flamePosition;
    public GameObject cylinder;
    public float cylinderRotationMultiply;
    public GameObject hammer;
    public float hammerRotationMultiply;
    public GameObject trigger;
    public float triggerRotationMultiply;

    public float enterDepth; //공이가 쳐지는 입력 깊이
    public float exitDeth; // 공이가 복귀되는 입력 깊이
    public float fireHapticDuration; // 격발 시 진동 유지 시간

    public bool semiAuto; // 단발식 여부
    public float fireInterval; // 연사 간격

    public float recoil; // 사격 시 반동
    public float shake; // 사격 시 흔들림
    public float recoilReturnSpeed; // 반동 복원 속도

    private bool hammerHit = false; // 공이가 한 번 쳐진 상태라면 더 이상 발사되지 않는다. // 단발식 총기 한정
    private float triggerDepth = 0f; // 트리거 입력 깊이

    private Vector3 cylinderOriginRot;
    private float cylinderRotOffset;
    private Vector3 hammerOriginRot;
    private float hammerRotOffset;
    private Vector3 triggerOriginRot;
    private float triggerRotOffset;

    private Vector3 gunOriginPoision;
    private float gunRecoilOffset; // 총기 z위치 오프셋

    private float currentTime;

    private bool hapticSupported;
    private InputDevice device;

    private AudioSource audio_;

    void Awake()
    {
        audio_ = GetComponent<AudioSource>();
    }

    void Start()
    {
        if(cylinder)
        {
            cylinderOriginRot = cylinder.transform.localRotation.eulerAngles;
        }
        if(hammer)
        {
            hammerOriginRot = hammer.transform.localRotation.eulerAngles;
        }
        if(trigger)
        {
            triggerOriginRot = trigger.transform.localRotation.eulerAngles;
        }

        gunOriginPoision = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(SG_GameMan.Inst.pauseState)
        {
            return;
        }
        
        device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        hapticSupported = device.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse;
        device.TryGetFeatureValue(CommonUsages.trigger, out triggerDepth);

        if(semiAuto) { // 단발식일 경우
            if(triggerDepth > enterDepth)
            {
                if(!hammerHit) {
                    FireGun();
                    hammerHit = true;
                    cylinderRotOffset = 0f;
                    hammerRotOffset = 0f;
                    if (hapticSupported)
                    {
                        device.SendHapticImpulse(0, 1.0f, fireHapticDuration);
                    }
                }
            }
            else if(triggerDepth < exitDeth)
            {
                if(hammerHit) {
                    hammerHit = false;
                }
            }

            if(!hammerHit)
            {
                cylinderRotOffset = triggerDepth * cylinderRotationMultiply;
                hammerRotOffset = triggerDepth * hammerRotationMultiply;
            }
        }
        else // 자동 연사일 경우
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Clamp(currentTime, 0f, 10f);

            if(triggerDepth > enterDepth)
            {
                if(currentTime <= 0f)
                {
                    currentTime += fireInterval;
                    FireGun();
                }
            }
        }

        // 방아쇠는 예외적으로 실시간으로 계속 회전값을 반영한다.
        triggerRotOffset = triggerDepth * triggerRotationMultiply;

        // 반동 수치 복원
        gunRecoilOffset = Mathf.Lerp(gunRecoilOffset, 0f, Time.deltaTime * recoilReturnSpeed);

        if(cylinder) // 실린더 회전
        {
            cylinder.transform.localRotation = Quaternion.Euler(cylinderOriginRot.x, cylinderOriginRot.y, cylinderOriginRot.z + cylinderRotOffset);
        }
        if(hammer) // 헤머 회전
        {
            hammer.transform.localRotation = Quaternion.Euler(hammerOriginRot.x - hammerRotOffset, hammerOriginRot.y, hammerOriginRot.z);
        }
        if(trigger) // 방아쇠 회전
        {
            trigger.transform.localRotation = Quaternion.Euler(triggerOriginRot.x + triggerRotOffset, triggerOriginRot.y, triggerOriginRot.z);
        }

        transform.localPosition = gunOriginPoision + new Vector3(0f, 0f, -gunRecoilOffset);
    }

    public void FireGun()
    {
        var zombieColliderMask = 1 << LayerMask.NameToLayer("EnemyCollider");
        var zombieHeadMask = 1 << LayerMask.NameToLayer("EnemyHeadCollider");
        var obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");
        var rayCastMask = zombieColliderMask | zombieHeadMask | obstacleMask;
        
        if(Physics.Raycast(rayOrigin.position, rayOrigin.forward, out var hit, 100f, rayCastMask)) {
            var collidedLayer = 1 << hit.collider.gameObject.layer;
            if((collidedLayer & zombieColliderMask) != 0)
            {
                var comp = hit.collider.gameObject.GetComponentInParent<Zombie>();
                comp.GiveDamage(hit.collider.attachedRigidbody, hit.point, hit.normal, 20, false);
            }
            else if((collidedLayer & zombieHeadMask) != 0)
            {
                var comp = hit.collider.gameObject.GetComponentInParent<Zombie>();
                comp.GiveDamage(hit.collider.attachedRigidbody, hit.point, hit.normal, 20, true);
            }
        }

        var newFlame = SG_ObjectPool.Inst.GetInstance(gunFlamePrefab);
        newFlame.transform.position = flamePosition.position;
        newFlame.transform.rotation = flamePosition.rotation;
        newFlame.GetComponent<GunFlame>().Play();

        var newAmmo = SG_ObjectPool.Inst.GetInstance(ammoParticlePrefab);
        newAmmo.transform.position = ammoPosition.position;
        newAmmo.transform.right = ammoPosition.right;
        newAmmo.GetComponent<Ammo>().Play();

        var soundIndex = Random.Range(0, fireSounds.Length);
        audio_.PlayOneShot(fireSounds[soundIndex]);

        gunRecoilOffset += recoil;

        if (hapticSupported)
        {
            device.SendHapticImpulse(0, 1.0f, fireHapticDuration);
        }
    }
}
