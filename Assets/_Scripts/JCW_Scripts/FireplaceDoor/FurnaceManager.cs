using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 난로 연료 상태 관리 — 타이머로 연료 소모, 불 꺼짐/켜짐 처리
/// </summary>
public class FurnaceManager : MonoBehaviour
{
    [Header("Fuel Settings")]
    [SerializeField] private float maxFuel = 120f;       // 최대 연료(초)
    [SerializeField] private float initialFuel = 60f;    // 초기 연료(초)
    [SerializeField] private float burnRate = 1f;        // 초당 소모량

    [Header("Fire VFX/SFX")]
    [SerializeField] private ParticleSystem fireParticle;
    [SerializeField] private Light fireLight;
    [SerializeField] private AudioSource fireCrackleAudio;

    [Header("Events")]
    public UnityEvent onFireLit;    // 불 켜질 때
    public UnityEvent onFireOut;    // 불 꺼질 때

    private float _currentFuel;
    private bool _isBurning;

    public bool IsBurning => _isBurning;
    public float FuelRatio => _currentFuel / maxFuel; // 0~1, UI 연동용

    private void Start()
    {
        _currentFuel = initialFuel;
        SetFireState(_currentFuel > 0);
    }

    private void Update()
    {
        if (!_isBurning) return;

        _currentFuel -= burnRate * Time.deltaTime;

        if (_currentFuel <= 0f)
        {
            _currentFuel = 0f;
            SetFireState(false);
        }

        // 연료 잔량에 따라 불 세기 조절
        UpdateFireIntensity();
    }

    /// <summary>
    /// FurnaceSocket에서 호출
    /// </summary>
    public void AddFuel(float amount)
    {
        _currentFuel = Mathf.Min(_currentFuel + amount, maxFuel);

        if (!_isBurning)
            SetFireState(true);
    }

    private void SetFireState(bool burning)
    {
        _isBurning = burning;

        if (fireParticle != null)
        {
            if (burning) fireParticle.Play();
            else fireParticle.Stop();
        }

        if (fireLight != null)
            fireLight.enabled = burning;

        if (fireCrackleAudio != null)
        {
            if (burning && !fireCrackleAudio.isPlaying)
                fireCrackleAudio.Play();
            else if (!burning)
                fireCrackleAudio.Stop();
        }

        if (burning) onFireLit?.Invoke();
        else onFireOut?.Invoke();
    }

    private void UpdateFireIntensity()
    {
        if (fireLight == null) return;

        // 연료 50% 이하부터 빛 약해짐
        float ratio = Mathf.Clamp01(_currentFuel / (maxFuel * 0.5f));
        fireLight.intensity = Mathf.Lerp(0.2f, 1.5f, ratio);
    }
}