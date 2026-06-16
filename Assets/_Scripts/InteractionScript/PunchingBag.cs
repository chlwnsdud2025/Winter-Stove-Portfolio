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
    public ConditionData Pucnch_1000Score;
    private void OnCollisionEnter(Collision collision)
    {

        if (Time.time - lastHitTime < hitCooldown)
        {
            return;
        }

        if (collision.collider.CompareTag("Glove"))
        {
            float impactForce = collision.relativeVelocity.magnitude;


            if (impactForce >= minimumImpact)
            {
                CalculateAndShowScore(impactForce, collision.contacts[0].point);
                lastHitTime = Time.time;
            }
            else
            {
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
            Pucnch_1000Score.achived = true;
        }
        if (scoreText != null)
        {
            scoreText.text = "PunchScore : "+finalScore.ToString() + " !!";
        }
    }
}