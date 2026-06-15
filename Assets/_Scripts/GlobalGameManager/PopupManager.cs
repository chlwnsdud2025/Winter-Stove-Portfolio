using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RootMotion.Demos.Turret;

public interface IPopupManager
{
    public void Popup(string msg);
}
public class PopupManager : MonoBehaviour, IPopupManager
{
    static IPopupManager instance;
    public static IPopupManager Instance { get => instance; }
    [Header("Bind")]
    [SerializeField] GameObject popup;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI tmpro;

    [Header("Properties")]
    [SerializeField] float duration;

    float lastT;
    bool isPlaying;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        popup.SetActive(false);
    }

    public void Popup(string msg)
    {
        tmpro.text = msg;
        popup.SetActive(true);
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        tmpro.color = new Color(tmpro.color.r, tmpro.color.g, tmpro.color.b, 1f);
        lastT = Time.time;
        isPlaying = true;

    }
    private void Update()
    {
        if (isPlaying == false) return;

        if(Time.time - lastT < duration)
        {
            var val = Mathf.Clamp01((Time.time - lastT) / duration);
            tmpro.color = new Color(tmpro.color.r, tmpro.color.g, tmpro.color.b, val);
            image.color = new Color(image.color.r, image.color.g, image.color.b, val);
        }
        else
        {
            isPlaying = false;
            popup.SetActive(false);
        }
    }
}
