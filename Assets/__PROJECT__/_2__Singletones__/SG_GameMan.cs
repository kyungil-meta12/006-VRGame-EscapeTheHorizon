using UnityEngine;

// 게임 상태를 관리하는 싱글톤 모듈.
public class SG_GameMan : MonoBehaviour
{
    public static SG_GameMan Inst;

    public bool gameOverState = false;
    public bool pauseState = false;

    void Awake()
    {
        if (Inst && Inst != this)
        {
            DestroyImmediate(this);
        }

        Inst = this;
        DontDestroyOnLoad(Inst);
        print("[SG_GameManager] Created instance.");
    }

    // 게임을 시작한다.
    public void StartGame()
    {
        pauseState = false;
        gameOverState = false;
        SG_ScoreMan.Inst.ResetCurrentScore();
    }

    // 게임 오버 상태로 전환한다.
    public void EnableGameOverState()
    {
        gameOverState = true;
    }

    // 게임 일시정지 상태로 전환한다.
    public void SetPauseState(bool flag)
    {
        pauseState = flag;
    }
}
