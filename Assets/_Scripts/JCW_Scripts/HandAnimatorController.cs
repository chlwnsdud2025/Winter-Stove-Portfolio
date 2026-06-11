using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// VR 컨트롤러 Grip 입력 → 손가락 애니메이션 연동
/// </summary>
public class HandAnimatorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator leftHandAnimator;
    [SerializeField] private Animator rightHandAnimator;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference leftGripAction;
    [SerializeField] private InputActionReference rightGripAction;

    private void LateUpdate()
    {
        float leftGrip = leftGripAction.action.ReadValue<float>();
        float rightGrip = rightGripAction.action.ReadValue<float>();
        Debug.Log($"Grip Values - Left: {leftGrip:F2}, Right: {rightGrip:F2}");

        leftHandAnimator.SetFloat("Grip", leftGrip);
        rightHandAnimator.SetFloat("Grip", rightGrip);

    }
}