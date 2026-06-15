using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CycleEvent_Radio : CycleEvent
{
    [Serializable]
    public struct SubtitleEntry
    {
        public List<string> strs;
    }
    [SerializeField] List<SubtitleEntry> subtitles;
    [SerializeField] ConditionData allSubtitlesPlayed;
    [SerializeField] SubtitlePlayer subtitlePlayer;
    [SerializeField] float subtitleTerm = 2.5f;
    public AudioClip radioAudio;

    // 자막 뭉치 인덱스
    int curSubtitleEntryIndex = 0;
    // 자막 뭉치의 개별 자막 인덱스
    int curSubtitleIndex = 0;


    float lastT;
    protected override void Raise()
    {
        if (allSubtitlesPlayed.achived) return;

        SoundManager.Instance.PlaySFX(radioAudio, transform.position);

        IsPlaying = true;
        lastT = Time.time;
        curSubtitleIndex = 0;
    }

    protected override void Update()
    {
        base.Update();

        if (IsPlaying == false) return;

        if(Time.time - lastT > subtitleTerm)
        {
            lastT = Time.time;
            PlayNext();
        }
    }
    void PlayNext()
    {
        if (curSubtitleIndex < subtitles[curSubtitleEntryIndex].strs.Count)
            subtitlePlayer.Play(subtitles[curSubtitleEntryIndex].strs[curSubtitleIndex++]);
        else
        {
            IsPlaying = false;
            if (curSubtitleEntryIndex < subtitles.Count - 1)
                curSubtitleEntryIndex++;
            else
                allSubtitlesPlayed.achived = true;
        }

    }
}
