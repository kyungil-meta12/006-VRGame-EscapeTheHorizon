using TMPro;
using UnityEditor;
using UnityEngine;

public class SG_AmmoIndicator : MonoBehaviour
{
    public static SG_AmmoIndicator Inst;
    public TextMeshProUGUI text;
    private IntPair ammoValue = new();
    private float textScale = 1f;
    
    void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }

        Inst = this;

        print("[SG_AmmoIndicator] Created instance.");
    }

    void Update()
    {
        // 장탄수에 변화가 있으면 텍스트 스케일을 순간적으로 늘린 후 다시 lerp를 사용해 스케일이 줄어드는 효과를 부여한다.
        if(ammoValue.IsDifferent())
        {
            textScale = 2f;
            ammoValue.MakeSame();
        }
        textScale = Mathf.Lerp(textScale, 1f, Time.deltaTime * 5f);
        text.transform.localScale = new Vector3(textScale, textScale, 1f);
    }

    public void InputAmmo(int val)
    {
        ammoValue.curr = val;
        text.text = $"{ammoValue.curr}";
    }
}
