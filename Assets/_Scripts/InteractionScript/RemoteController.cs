using UnityEngine;

public class RemoteController : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Transform raycastOrigin; // 광선이 나갈 빈 오브젝트 위치
    public float rayDistance = 15f; // 리모컨 신호가 닿는 최대 거리

    // XR Grab Interactable의 'Activated' 이벤트에 연결할 함수입니다.
    public void UseRemote()
    {
        if (raycastOrigin == null)
        {
            Debug.LogWarning("[리모컨] 발사 위치(raycastOrigin)가 할당되지 않았습니다!");
            return;
        }

        // 1. RaycastOrigin 위치에서 앞(forward)을 향해 보이지 않는 광선을 생성
        Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
        RaycastHit hit;

        // 2. 광선을 쏴서 무언가 맞았는지 확인
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // 3. 맞은 물체(혹은 그 부모)에 TVController 스크립트가 있는지 확인
            TVController tv = hit.collider.GetComponentInParent<TVController>();

            if (tv != null)
            {
                // 4. TV를 맞췄다면 전원 토글 함수 실행!
                tv.TogglePower();
                Debug.Log("[리모컨] TV에 신호를 보냈습니다!");
            }
            else
            {
                // TV가 아닌 다른 벽이나 물체를 맞췄을 때
                Debug.Log($"[리모컨] 빗나감! 맞춘 물체: {hit.collider.name}");
            }
        }
    }

    // (선택 사항) 에디터 씬(Scene) 창에서 광선이 어디로 나가는지 노란색 선으로 보여줍니다.
    private void OnDrawGizmos()
    {
        if (raycastOrigin != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(raycastOrigin.position, raycastOrigin.forward * rayDistance);
        }
    }
}