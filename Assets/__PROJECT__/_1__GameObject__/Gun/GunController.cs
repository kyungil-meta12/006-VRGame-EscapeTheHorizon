using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GunController : MonoBehaviour
{
    public GameObject gunFlamePrefab;
    public GameObject flamePosition;
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

    private bool hammerHit = false; // 공이가 한 번 쳐진 상태라면 더 이상 발사되지 않는다. // 단발식 총기 한정
    private float triggerDepth = 0f; // 트리거 입력 깊이

    private Vector3 cylinderOriginRot;
    private float cylinderRotOffset;
    private Vector3 hammerOriginRot;
    private float hammerRotOffset;
    private Vector3 triggerOriginRot;
    private float triggerRotOffset;

    private float currentTime;

    private bool hapticSupported;
    private InputDevice device;

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

        device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        hapticSupported = device.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void FireGun()
    {
        //print("fire");
        Instantiate(gunFlamePrefab, flamePosition.transform.position, flamePosition.transform.rotation);
        if (hapticSupported)
        {
            device.SendHapticImpulse(0, 1.0f, fireHapticDuration);
        }
    }
}
