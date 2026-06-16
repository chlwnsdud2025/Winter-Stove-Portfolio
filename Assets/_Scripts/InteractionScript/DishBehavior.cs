using UnityEngine;

public class DishBehavior : MonoBehaviour
{
    [Header("설거지 설정")]
    public float requiredTime = 2.0f;
    public Material cleanMaterial;

    private float currentTime = 0f;
    public  bool isClean = false;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionStay(Collision collision)
    {

        if (isClean) return;

        if (collision.gameObject.CompareTag("Sponge"))
        {
            currentTime += Time.deltaTime;

            if (currentTime >= requiredTime)
            {
                isClean = true;
                meshRenderer.material = cleanMaterial;
                Debug.Log("[설거지 완료] 접시가 깨끗해졌습니다!");
            }
        }
    }
}
