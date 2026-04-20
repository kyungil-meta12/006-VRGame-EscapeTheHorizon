using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

// 게임 상태를 관리하는 싱글톤 모듈.
public class SG_GameMan : MonoBehaviour
{
    public static SG_GameMan Inst;

    public TextMeshProUGUI text;
    public int destKillCount;
    public float baseSpawnInterval;
    public float spawnIntervalMultiply;

    [HideInInspector]
    public bool gameStarted = false;
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
        currentSpawnInterval = baseSpawnInterval;
        print("[SG_GameManager] Created instance.");
    }

    void OnDestroy()
    {
        Inst = null;
    }

    void Start()
    {
        SG_ScoreMan.Inst.EnableTitleScoreUI();
        SG_ScoreMan.Inst.LoadHighScore();
    }

    void Update()
    {
        if(!gameStarted) // 우측 A키를 눌러 시작
        {
            var deviceRight = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            deviceRight.TryGetFeatureValue(CommonUsages.primaryButton, out var rightPress);
            if(rightPress)
            {
                StartGame();
            }
        }
        else {
            // 일정 킬 횟수를 채우면 스폰 간격이 줄어든다.
            if(currentKill == destKillCount)
            {
                currentKill = 0;
                currentSpawnInterval *= spawnIntervalMultiply;
            }
        }
    }

    // 게임을 시작한다.
    public void StartGame()
    {
        pauseState = false;
        gameOverState = false;
        gameStarted = true;
        text.gameObject.SetActive(false);
        SG_ScoreMan.Inst.ResetCurrentScore();
        SG_ScoreMan.Inst.DisableTitleScoreUI();
        SG_ScoreMan.Inst.EnableIngameScoreUI();
        SG_PlayerHPMan.Inst.EnableUI();
        SG_AmmoIndicator.Inst.EnableUI();
        Time.timeScale = 1f;
    }

    // 게임 오버 상태로 전환한다.
    public void EnableGameOverState()
    {
        gameOverState = true;
        Time.timeScale = 1f;
        SG_ScoreMan.Inst.CheckHighScore();
        SceneManager.LoadScene("PlayScene");
    }

    // 게임 일시정지 상태로 전환한다.
    public void SetPauseState(bool flag)
    {
        pauseState = flag;
        Time.timeScale = 0f;
    }
}
