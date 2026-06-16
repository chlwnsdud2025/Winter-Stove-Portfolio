using UnityEngine;

/// <summary>
/// 자판기 메인 로직 — 동전 투입 시 랜덤 우유병 배출
/// </summary>
public class VendingMachine : MonoBehaviour
{
    [Header("Milk Bottle Prefabs")]
    [SerializeField] private GameObject[] milkBottlePrefabs; // 색깔별 우유병 5개

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint; // 자판기 하단 출구 위치

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip coinInsertClip;
    [SerializeField] private AudioClip dispenseClip;

    /// <summary>
    /// VendingMachineSlot에서 호출
    /// </summary>
    public void InsertCoin()
    {
        if (coinInsertClip != null)
            audioSource.PlayOneShot(coinInsertClip);

        // 랜덤 우유병 배출
        int index = Random.Range(0, milkBottlePrefabs.Length);
        GameObject bottle = Instantiate(
            milkBottlePrefabs[index],
            spawnPoint.position,
            spawnPoint.rotation
        );


        // 자판기 앞으로 튀어나오는 힘 추가
        Rigidbody rb = bottle.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(spawnPoint.forward * 2f + Vector3.up * 1f, ForceMode.Impulse);

   
        if (dispenseClip != null)
            audioSource.PlayOneShot(dispenseClip);
    }
}