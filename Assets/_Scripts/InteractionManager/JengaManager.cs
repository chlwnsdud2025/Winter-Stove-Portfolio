using UnityEngine;
using TMPro;

public class JengaManager : MonoBehaviour
{
    public static JengaManager Instance;

    public VoidEventChannel jengaChannel;

    [Header("도전과제 세팅")]
    public int targetSuccessCount = 5; // 목표 성공 개수
    private int currentSuccessCount = 0;

    [HideInInspector]
    public bool isGameOver = false;

    public ConditionData jenga_7time;
    //[Header("UI 연결")]
    //public TMP_Text statusText; // 화면에 정보를 띄울 TextMeshPro

    void Awake()
    {
        // 다른 스크립트에서 JengaManager.Instance로 쉽게 접근할 수 있도록 싱글톤 세팅
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }
    
    // 블록이 탈출하면 JengaBlock에서 이 함수를 호출합니다.
    public void AddSuccessCount()
    {
        if (isGameOver) return;

        currentSuccessCount++;
        UpdateUI();

        if (currentSuccessCount >= targetSuccessCount)
        {
            GameClear();
        }
    }

    // 바닥에 설치한 실패 트리거에 젠가가 닿으면 이 함수를 호출하게 합니다.
    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        //statusText.text = "실패... 타워가 무너졌습니다! ";
        Debug.Log("게임 오버!");
    }

    private void GameClear()
    {
        isGameOver = true; // 성공 시 더 이상 카운트가 오르지 않게 막음
        jengaChannel.RaiseEvent();
        jenga_7time.achived = true;
        //statusText.text = " 도전과제 달성! 성공! ";
        Debug.Log("성공!");
    }

    private void UpdateUI()
    {
        //if (statusText != null)
        //{
        //    statusText.text = $"성공한 블록: {currentSuccessCount} / {targetSuccessCount}";
        //}
    }
}