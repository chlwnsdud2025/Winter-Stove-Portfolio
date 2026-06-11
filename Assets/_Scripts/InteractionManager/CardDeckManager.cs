using System.Collections.Generic;
using UnityEngine;

public class CardDeckManager : MonoBehaviour
{
    [Header("덱 세팅")]
    [Tooltip("씬에 있는 50장의 카드를 모두 여기에 넣어주세요")]
    public List<GameObject> deck = new List<GameObject>();
    public Transform deckPosition; // 카드를 예쁘게 쌓아둘 기준 위치(빈 오브젝트)

    [Header("분배 세팅")]
    public Transform dealStartPoint; // 분배를 시작할 기준 위치
    public float pileSpacing = 0.2f; // 덱뭉치(플레이어) 간의 간격
    private float cardThickness = 0.01f; // 카드 한 장의 물리적 두께

    // [추가] 분배로 인해 사라진 카드를 원상복구하기 위한 원본 백업 리스트
    private List<GameObject> masterDeck = new List<GameObject>();

    void Start()
    {
        // 게임 시작 시 인스펙터에 등록된 50장의 카드를 안전하게 복사해 둡니다.
        if (deck != null && deck.Count > 0)
        {
            masterDeck = new List<GameObject>(deck);
        }
    }

    // [기능 추가] 모든 카드를 회수하여 초기 상태 덱으로 정렬하는 함수 (UI 버튼 연결용)
    public void ResetAndGatherDeck()
    {
        if (masterDeck.Count == 0) return;

        // 1. 줄어들었던 deck 리스트를 원본 50장 상태로 꽉 채워 되돌립니다.
        deck.Clear();
        deck.AddRange(masterDeck);

        // 2. 모으기 함수 호출하여 물리 초기화 후 차곡차곡 쌓기
        GatherDeck();

        Debug.Log($"[카드 시스템] 모든 카드 회수 완료. 현재 덱 카드 수: {deck.Count}장");
    }

    // 1. 셔플 기능 (섞고 다시 예쁘게 쌓기)
    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            GameObject temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        GatherDeck();
    }

    // 카드를 덱 위치로 안전하게 모으는 함수
    private void GatherDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Rigidbody rb = deck[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }

            deck[i].transform.position = deckPosition.position + new Vector3(0, i * cardThickness, 0);
            deck[i].transform.rotation = deckPosition.rotation;

            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
        Debug.Log("[카드 시스템] 셔플 완료 및 정렬됨.");
    }

    // 2. 분배 기능 (중앙 정렬 알고리즘 반영)
    public void DealCards(int pileCount, int cardsPerPile)
    {
        int totalNeeded = pileCount * cardsPerPile;
        if (deck.Count < totalNeeded)
        {
            Debug.LogWarning("덱에 남은 카드가 부족합니다!");
            return;
        }

        // [핵심 변경: 중앙 정렬 계산]
        // 전체 뭉치들이 차지할 총 가로 길이를 계산합니다.
        float totalWidth = (pileCount - 1) * pileSpacing;
        // 시작점을 왼쪽으로 절반만큼 이동시켜 오프셋(Offset)을 구합니다.
        Vector3 centerOffset = dealStartPoint.right * (-totalWidth / 2f);

        int cardIndex = deck.Count - 1;

        for (int p = 0; p < pileCount; p++)
        {
            // 기준점(dealStartPoint)에서 좌측 오프셋을 더한 뒤, p만큼 우측으로 나열합니다.
            Vector3 pilePos = dealStartPoint.position + centerOffset + (dealStartPoint.right * p * pileSpacing);

            for (int c = 0; c < cardsPerPile; c++)
            {
                if (cardIndex < 0) break;

                GameObject card = deck[cardIndex];
                deck.RemoveAt(cardIndex);

                Rigidbody rb = card.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.linearVelocity = Vector3.zero;
                    rb.Sleep();
                }

                card.transform.position = pilePos + new Vector3(0, c * cardThickness, 0);
                card.transform.rotation = dealStartPoint.rotation;

                if (rb != null) rb.isKinematic = false;

                cardIndex--;
            }
        }
        Debug.Log($"[카드 시스템] {pileCount}명에게 {cardsPerPile}장씩 분배 완료. (남은 카드: {deck.Count}장)");
    }
}