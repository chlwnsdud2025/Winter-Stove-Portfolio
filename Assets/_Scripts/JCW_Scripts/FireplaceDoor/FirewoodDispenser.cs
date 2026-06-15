using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// 장작 리스포너 — 소켓에 투입되면 제자리에 새 장작 생성
/// </summary>
public class FirewoodDispenser : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject firewoodPrefab;
    [SerializeField] private Transform spawnPoint; // 비워두면 본인 위치

    private void Start()
    {
        SpawnFirewood();
    }

    public void OnFirewoodConsumed()
    {
        SpawnFirewood();
    }

    private void SpawnFirewood()
    {
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject spawned = Instantiate(firewoodPrefab, pos, transform.rotation);

        FurnaceFirewood firewood = spawned.GetComponent<FurnaceFirewood>();
        if (firewood != null)
            firewood.OnConsumed += OnFirewoodConsumed;
    }
}