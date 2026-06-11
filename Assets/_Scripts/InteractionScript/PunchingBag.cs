using UnityEngine;
using TMPro;

public class PunchingBag : MonoBehaviour
{
    [Header("점수 세팅")]
    public float scoreMultiplier = 100f;
    public float minimumImpact = 2.0f; // 이 수치가 범인일 수 있습니다.
    public float hitCooldown = 0.5f;

    [Header("UI & 이펙트")]
    public TMP_Text scoreText;

    private float lastHitTime = 0f;


    public VoidEventChannel punchChannel;
    private void OnCollisionEnter(Collision collision)
    {
        // [CCTV 1] 무언가 샌드백에 부딪히긴 했는지 확인
        Debug.Log($"[디버그] 샌드백이 '{collision.collider.name}'(이)랑 부딪혔습니다! (태그: {collision.collider.tag})");

        if (Time.time - lastHitTime < hitCooldown)
        {
            Debug.Log("[디버그] 쿨타임 때문에 무시됨.");
            return;
        }

        if (collision.collider.CompareTag("Glove"))
        {
            float impactForce = collision.relativeVelocity.magnitude;

            // [CCTV 2] 글러브랑 부딪혔다면, 도대체 속도가 몇으로 인식되고 있는지 확인
            Debug.Log($"[디버그] 글러브 인식 성공! 현재 충돌 속도: {impactForce}");

            if (impactForce >= minimumImpact)
            {
                CalculateAndShowScore(impactForce, collision.contacts[0].point);
                lastHitTime = Time.time;
            }
            else
            {
                // [CCTV 3] 닿긴 했는데 파워가 약해서 무시된 경우
                Debug.Log($"[디버그] 속도({impactForce})가 최소 기준치({minimumImpact})보다 낮아서 점수 판정 실패!");
            }
        }
    }

    private void CalculateAndShowScore(float impact, Vector3 hitPoint)
    {
        int finalScore = Mathf.RoundToInt(impact * scoreMultiplier);
        Debug.Log($"[샌드백] 충돌력: {impact:F2} -> 펀치력: {finalScore}점!");
        if( finalScore >= 1000)
        {
            punchChannel.RaiseEvent();
        }
        if (scoreText != null)
        {
            scoreText.text = "PunchScore : "+finalScore.ToString() + " !!";
        }
    }
}