using UnityEngine;
using UnityEngine.Video;

public class TVController : MonoBehaviour
{
    [Header("TV Settings")]
    public VideoPlayer videoPlayer;
    public GameObject screenMesh; // 화면 역할을 하는 3D 오브젝트 (꺼졌을 때 까맣게 만들기 위함)

    private bool isTVOn = false;

    private void Start()
    {
        // 게임 시작 시 TV는 꺼진 상태로 초기화
        TurnOffTV();
    }

    // 나중에 리모컨에서 이 함수를 호출할 예정입니다!
    public void TogglePower()
    {
        isTVOn = !isTVOn;

        if (isTVOn)
        {
            TurnOnTV();
        }
        else
        {
            TurnOffTV();
        }
    }

    private void TurnOnTV()
    {
        screenMesh.SetActive(true); // 화면 켜기
        videoPlayer.Play();         // 영상 재생 시작
        Debug.Log("[TV] 전원이 켜졌습니다.");
    }

    private void TurnOffTV()
    {
        videoPlayer.Pause();        // 영상 일시정지 (또는 Stop() 사용)
        screenMesh.SetActive(false); // 화면 끄기 (검은색으로 보이게 됨)
        Debug.Log("[TV] 전원이 꺼졌습니다.");
    }
}