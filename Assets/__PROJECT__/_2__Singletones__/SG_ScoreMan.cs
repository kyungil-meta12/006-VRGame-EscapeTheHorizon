using TMPro;
using UnityEngine;

// 플레이 점수를 담당하는 싱글톤 모듈,
public class SG_ScoreMan : MonoBehaviour
{
    public static SG_ScoreMan Inst;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI ingameText;

    public int currScore = 0;
    public int highScore = 0;

    // 문자열 상수
    const string HIGH_SCORE = "HIGH_SCORE";

    void Awake()
    {
        if (Inst && Inst != this)
        {
            DestroyImmediate(this);
        }

        Inst = this;
        DontDestroyOnLoad(Inst);
        print("[SG_ScoreManager] Created instance.");
    }

    void Start()
    {
        ingameText.gameObject.SetActive(false);
    }

    public void EnableTitleScoreUI()
    {
        titleText.gameObject.SetActive(true);
    }

    public void DisableTitleScoreUI()
    {
        titleText.gameObject.SetActive(false);
    }

    public void EnableIngameScoreUI()
    {
        ingameText.gameObject.SetActive(true);
    }

    // 최고 점수를 로드한다.
    public void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE);
        titleText.text = $"HighScore\n{highScore}";
    }

    // 최고 점수를 초기화 한다.
    public void ResetHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE, 0);
        highScore = 0;
    }

    // 최고 점수 갱신 여부를 확인하고, 갱신했다면 갱신된 점수를 저장한다.
    public bool CheckHighScore()
    {
        if(currScore > highScore)
        {
            PlayerPrefs.SetInt(HIGH_SCORE, currScore);
            highScore = currScore;
            return true;
        }
        return false;
    }

    // 현재 점수에 점수를 추가한다.
    public void AddCurrentScore(int val)
    {
        currScore += val;
        ingameText.text = $"{currScore}";
    } 

    // 현재 점수를 초기화 한다.
    public void ResetCurrentScore()
    {
        currScore = 0;
        ingameText.text = "0";
    }
}
