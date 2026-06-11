using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class VRUIResetButton : MonoBehaviour
{
    [Header("이 UI 버튼이 초기화할 아이템 종류 선택")]
    public ResettableItem.ItemType buttonTargetType;

    void Awake()
    {
        
    }

    public  void OnButtonClicked()
    {
        // 씬에 있는 모든 ResettableItem을 다 찾습니다.
        ResettableItem[] allItems = FindObjectsOfType<ResettableItem>();

        int resetCount = 0;
        foreach (ResettableItem item in allItems)
        {
            if (item.itemType == buttonTargetType)
            {
                item.ResetIfMatchingType(buttonTargetType);
                resetCount++;
            }
        }

        Debug.Log($"[3D UI 버튼 작동] {buttonTargetType} : 총 {resetCount}개 리셋 완료.");
    }
}