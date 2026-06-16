using UnityEngine;

public class AudioClipPlayer : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    public void PlaySFX()
    {
        if (clip == null) return;

        SoundManager.Instance?.PlaySFX(clip, transform.position);
    }
}
