using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class CycleEventViewer : MonoBehaviour
{
    public GameObject prefab;
    public CycleEvent[] cycles;

    List<TextMeshProUGUI> tmpros = new();
    StringBuilder builder;
    int maxUICnt;
    private void Awake()
    {
        maxUICnt = cycles.Length;
        for (int i = 0; i < maxUICnt; i++)
        {
            var obj = Instantiate(prefab, transform);
            tmpros.Add(obj.GetComponent<TextMeshProUGUI>());
        }

        builder = new();
    }
    private void Update()
    {
        for(int i = 0; i < maxUICnt; i++)
        {
            builder.Clear();
            tmpros[i].text = builder.Append("ID: ").Append(cycles[i].data.id).Append("\n")
                .Append("Rate: ").Append(cycles[i].data.rate).Append("\n")
                .Append("On: ").Append(cycles[i].IsPlaying ? 1 : 0).Append("\n")
                .Append((Time.time - cycles[i].StartTime).ToString("F2")).Append("/").Append(cycles[i].data.elapseTimeNeed).Append("\n")
                .ToString();
        }
    }
}
