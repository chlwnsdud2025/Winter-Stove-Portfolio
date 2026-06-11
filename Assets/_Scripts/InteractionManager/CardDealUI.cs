using UnityEngine;
using TMPro;

public class CardDealUI : MonoBehaviour
{
    [Header("연결할 매니저 스크립트")]
    public CardDeckManager deckManager; // 지난번에 만든 카드 분배 매니저

    [Header("3D Text UI")]
    public TMP_Text infoText; // "N개의 카드를 N개의 뭉치로..."가 표시될 3D 텍스트

    [Header("현재 설정 값")]
    public int cardsPerPile = 1; // 뭉치당 카드 수 (초기값)
    public int pileCount = 1;     // 뭉치 개수 (초기값)

    private const int MAX_CARDS = 52; // 조커 제외 총 카드 수

    void Start()
    {
        UpdateUIText();
    }

    // 1. 뭉치당 카드 수 Up/Down 버튼이 호출할 함수
    public void ChangeCardsPerPile(int amount)
    {
        int nextValue = cardsPerPile + amount;

        // 최소 1장 이상이어야 하고, 총합이 50장을 넘지 않는지 체크
        if (nextValue >= 1 && (nextValue * pileCount) <= MAX_CARDS)
        {
            cardsPerPile = nextValue;
            UpdateUIText();
        }
    }

    // 2. 뭉치 개수 Up/Down 버튼이 호출할 함수
    public void ChangePileCount(int amount)
    {
        int nextValue = pileCount + amount;

        // 최소 1개 뭉치 이상이어야 하고, 총합이 50장을 넘지 않는지 체크
        if (nextValue >= 1 && (cardsPerPile * nextValue) <= MAX_CARDS)
        {
            pileCount = nextValue;
            UpdateUIText();
        }
    }

    // 3. 텍스트 UI를 실시간으로 갱신하는 함수
    private void UpdateUIText()
    {
        if (infoText != null)
        {
            // 요청하신 형태로 문자열 구성
            infoText.text = $"Distribute <color=#FFFF00>{cardsPerPile}</color> Cards into <color=#FFFF00>{pileCount}</color> stacks";
        }
    }

    // 4. 최종 [분배] 버튼이 호출할 함수
    public void OnClickDealButton()
    {
        if (deckManager != null)
        {
            // 매니저 스크립트에 현재 UI에 세팅된 값을 넘겨주며 분배 시작
            deckManager.DealCards(pileCount, cardsPerPile);
        }
    }
}