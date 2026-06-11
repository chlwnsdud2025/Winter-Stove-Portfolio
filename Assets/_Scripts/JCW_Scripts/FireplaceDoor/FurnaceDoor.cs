using UnityEngine;
using DG.Tweening;

/// <summary>
/// 난로 문 여닫기 (DOTween 기반)
/// XRI XRSimpleInteractable로 클릭/그랩 트리거 연결 필요
/// </summary>
public class FurnaceDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorPivot;          // 문 회전 기준점
    [SerializeField] private float openAngle = -110f;      // 열린 각도 (Y축 기준, 조정 필요)
    [SerializeField] private float closedAngle = 0f;       // 닫힌 각도
    [SerializeField] private float duration = 0.4f;        // 애니메이션 시간
    [SerializeField] private Ease openEase = Ease.OutBack;
    [SerializeField] private Ease closeEase = Ease.InOutSine;

    private bool _isOpen = false;
    private Tween _currentTween;

    // XRI에서 호출 — UnityEvent에 연결
    public void ToggleDoor()
    {
        if (_isOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;

        _currentTween?.Kill();
        _currentTween = doorPivot
            .DOLocalRotate(new Vector3(0f, openAngle, 0f), duration)
            .SetEase(openEase);
    }

    public void Close()
    {
        if (!_isOpen) return;
        _isOpen = false;

        _currentTween?.Kill();
        _currentTween = doorPivot
            .DOLocalRotate(new Vector3(0f, closedAngle, 0f), duration)
            .SetEase(closeEase);
    }

    // 외부에서 상태 조회용 (장작넣기 시스템 연동)
    public bool IsOpen => _isOpen;
}