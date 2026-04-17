using UnityEngine;

// 게임 상태를 관리하는 싱글톤 모듈.
public class SG_GameMan : MonoBehaviour
{
    public static SG_GameMan Inst;

    public int destKillCount;
    public float baseSpawnInterval;
    public float spawnIntervalMultiply;

    [HideInInspector]
    public bool gameOverState = false;
    [HideInInspector]
    public bool pauseState = false;
    [HideInInspector]
    public int currentKill;
    [HideInInspector]
    public float currentSpawnInterval;

    void Awake()
    {
        if (Inst && Inst != this)
        {
            DestroyImmediate(this);
        }

        Inst = this;
        DontDestroyOnLoad(Inst);
        currentSpawnInterval = baseSpawnInterval;
        print("[SG_GameManager] Created instance.");
    }

    void OnDestroy()
    {
        Inst = null;
    }

    void Update()
    {
        // 일정 킬 횟수를 채우면 스폰 간격이 줄어든다.
        if(currentKill == destKillCount)
        {
            currentKill = 0;
            currentSpawnInterval *= spawnIntervalMultiply;
        }
    }

    // 게임을 시작한다.
    public void StartGame()
    {
        pauseState = false;
        gameOverState = false;
        SG_ScoreMan.Inst.ResetCurrentScore();
        Time.timeScale = 1f;
    }

    // 게임 오버 상태로 전환한다.
    public void EnableGameOverState()
    {
        gameOverState = true;
        Time.timeScale = 1f;
    }

    // 게임 일시정지 상태로 전환한다.
    public void SetPauseState(bool flag)
    {
        pauseState = flag;
        Time.timeScale = 0f;
    }
}
