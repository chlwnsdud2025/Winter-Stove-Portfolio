using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitlePlayer : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI tmpro;
    [SerializeField] float subtitleDuration = 3f;
    [SerializeField] float fadeoutDuration = .5f;
    float showRemained;
    private void Awake()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        tmpro.color = new Color(tmpro.color.r, tmpro.color.g, tmpro.color.b, 0);
    }
    public void Play(string str)
    {
        tmpro.gameObject.SetActive(true);
        tmpro.text = str;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        tmpro.color = new Color(tmpro.color.r, tmpro.color.g, tmpro.color.b, 1);
        showRemained = subtitleDuration;
    }
    private void Update()
    {
        if (showRemained > 0)
        {
            showRemained -= Time.deltaTime;

            if (showRemained < fadeoutDuration)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, showRemained / fadeoutDuration);
                tmpro.color = new Color(tmpro.color.r, tmpro.color.g, tmpro.color.b, showRemained / fadeoutDuration);
            }
        }
    }
}
