using UnityEngine;
using UnityEngine.UI;

public class SG_PlayerHPMan : MonoBehaviour
{
    public static SG_PlayerHPMan Inst;
    public int maxHP;
    public Image img;
    public Slider slider;
    private float opacity;
    private IntPair hp = new();

    void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;
        SetAlpha(0f);
        SetHP(maxHP);

        print("[SG_DamageIndicator] Created instance.");
    }

    void Update()
    {
        if(hp.IsDifferent())
        {
            opacity = 1f;
            hp.MakeSame();
        }
        opacity -= Time.deltaTime * 2f;
        opacity = Mathf.Clamp(opacity, 0f, 1f);
        SetAlpha(opacity);
    }

    void SetAlpha(float opacity)
    {
        var color = img.color;
        color.a = opacity;
        img.color = color;
    }

    public void OnDamage(int val)
    {
        hp.curr -= val;
        slider.value -= val;
        hp.curr = Mathf.Clamp(hp.curr, 0, maxHP);
        slider.value = Mathf.Clamp(slider.value, 0, slider.maxValue);
    }

    public void SetHP(int val)
    {
        hp.curr = val;
        hp.prev = val;
        slider.maxValue = val;
        slider.value = val;
    }
}
