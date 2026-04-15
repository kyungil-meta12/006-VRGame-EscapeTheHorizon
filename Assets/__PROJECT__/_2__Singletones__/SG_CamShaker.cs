using UnityEngine;

public class SG_CamShaker : MonoBehaviour
{   
    public static SG_CamShaker Inst;
 
    private float currentStrength;
    private float currentTime;
    private Vector3 shakePos;

    void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }

        Inst = this;
        print("[SG_CamShaker] Created instance.");
    }

    void OnDestroy()
    {
        Inst = null;
    }

    void LateUpdate()
    {
        currentStrength = Mathf.Lerp(currentStrength, 0f, Time.deltaTime * 10f);
        currentTime += Time.deltaTime;
        if(currentTime >= 0.016f)
        {
            currentTime -= 0.016f;
            shakePos.x = Random.Range(-currentStrength, currentStrength);
            shakePos.y =  Random.Range(-currentStrength, currentStrength);
            shakePos.z = 0f;
        }

        var camPos = Camera.main.transform.position;
        var modifiedPos = camPos + shakePos;
        Camera.main.transform.position = modifiedPos;
    }

    public void AddShake(float strength)
    {
        currentStrength += strength;
    }
}
