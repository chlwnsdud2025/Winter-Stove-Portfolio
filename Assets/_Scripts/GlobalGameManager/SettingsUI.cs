using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Global")]
    [SerializeField] GameObject[] taps;
    [SerializeField] private SettingsSO _settings;

    [Header("Volume")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Demo")]
    [SerializeField] Transform cycleEventDemoUIParent;
    [SerializeField] GameObject cycleEventDemoUIPrefab;
    int curtap = 0;

    private CycleEvent[] cycles;
    private DemoCycleEventUI[] cycleUis;
    private void Start()
    {
        for(int i = 1; i < taps.Length; i++)
        {
            if (taps[i] != null) taps[i].SetActive(false);
        }
        taps[curtap].SetActive(true);

        _settings.Load();

        masterVolumeSlider.SetValueWithoutNotify(_settings.MasterVolume);
        bgmVolumeSlider.SetValueWithoutNotify(_settings.BGMVolume);
        sfxVolumeSlider.SetValueWithoutNotify(_settings.SFXVolume);

        masterVolumeSlider.onValueChanged.AddListener(v => _settings.MasterVolume = v);
        bgmVolumeSlider.onValueChanged.AddListener(v => _settings.BGMVolume = v);
        sfxVolumeSlider.onValueChanged.AddListener(v => _settings.SFXVolume = v);

        InitCycleEvents();
    }
    public void ChangeTab(int n)
    {
        if(n < taps.Length && taps[n] != null)
        {
            taps[curtap].SetActive(false);
            taps[n].SetActive(true);
            curtap = n;
        }
    }
    private void Update()
    {
        for (int i = 0; i < cycles.Length; i++)
        {
            var remained = cycles[i].GetRemained();
            cycleUis[i].remainedText.text = Mathf.FloorToInt(remained).ToString();
            cycleUis[i].remainedSlider.SetValueWithoutNotify(remained / (cycles[i].IsFirstEvent ? cycles[i].data.elapseTimeNeed : cycles[i].data.termTimeNeed));
        }
    }
    void InitCycleEvents()
    {
        cycles = FindObjectsByType<CycleEvent>();
        cycleUis = new DemoCycleEventUI[cycles.Length];
        for (int i = 0; i < cycles.Length; i++)
        {
            cycleUis[i] = Instantiate(cycleEventDemoUIPrefab, cycleEventDemoUIParent).GetComponent<DemoCycleEventUI>();
            cycleUis[i].title.text = cycles[i].data.id;
            cycleUis[i].remainedText.text = Mathf.FloorToInt(cycles[i].GetRemained()).ToString();
            cycleUis[i].rate.text = Mathf.FloorToInt(cycles[i].data.rate*100).ToString();
            cycleUis[i].condition.text = cycles[i].data.requires.Length > 0 ? "있음" : "없음";
            int captured = i;
            cycleUis[i].remainedSlider.onValueChanged.AddListener((v) =>
            {
                cycleUis[captured].remainedText.text = v.ToString();
                cycles[captured].SetRemained(v);
            });
        }
    }
    public void OnApplyButton() => _settings.Save();
    public void OnCancelButton() => _settings.Load(); // 저장 안 하고 닫으면 롤백
}