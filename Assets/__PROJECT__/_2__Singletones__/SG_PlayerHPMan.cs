using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SG_PlayerHPMan : MonoBehaviour
{
    public static SG_PlayerHPMan Inst;
    public int maxHP;
    public Slider slider;
    private int hp;

    void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;
        SetHP(maxHP);

        print("[SG_DamageIndicator] Created instance.");
    }

    void OnDestroy()
    {
        Inst = null;
    }

    void Start()
    {
        slider.gameObject.SetActive(false);
    } 

    public void EnableUI()
    {
        slider.gameObject.SetActive(true);
    }

    public void OnDamage(int val)
    {
        hp -= val;
        slider.value -= val;
        hp = Mathf.Clamp(hp, 0, maxHP);
        slider.value = Mathf.Clamp(slider.value, 0, slider.maxValue);
        if(hp <= 0)
        {
            SG_GameMan.Inst.EnableGameOverState();
        }
    }

    public void SetHP(int val)
    {
        hp = val;
        slider.maxValue = val;
        slider.value = val;
    }
}
