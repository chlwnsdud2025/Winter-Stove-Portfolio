
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CycleEvent_QuestGiver : CycleEvent
{
    [Header("퀘스트 데이터")]
    [SerializeField] QuestData[] questDatas;

    [Header("사운드")]
    [SerializeField] AudioClip SFX_Accepted;
    [SerializeField] AudioClip SFX_Completed;

    [Header("UI 요소")]
    [SerializeField] Transform parent;
    [SerializeField] GameObject prefab;
    [SerializeField] int MAX_VIEWED_QUEST;
    QuestUIElement[] cachedTmpros;
    QuestData[] showedQuest;
    int curQuestNum;
    int clearedNum;
    private void Awake()
    {
        showedQuest = new QuestData[MAX_VIEWED_QUEST];
        cachedTmpros = new QuestUIElement[MAX_VIEWED_QUEST];
        curQuestNum = 0;
        clearedNum = 0;
    }
    private void Start()
    {
        for(int i = 0; i < MAX_VIEWED_QUEST; i++)
        {
            var obj = Instantiate(prefab, parent);
            cachedTmpros[i] = obj.GetComponent<QuestUIElement>();
            obj.SetActive(false);
        }
    }
    protected override void Raise()
    {
        // 퀘스트 완료 여부 확인
        for (int i = 0; i < MAX_VIEWED_QUEST; i++)
        {
            if (showedQuest[i] == null) continue;
            if (showedQuest[i].state == QuestStates.Running && showedQuest[i].condition.achived)
            {
                SoundManager.Instance?.PlaySFX(SFX_Completed);
                CycleEvent_Nuclear.Postpone();
                curQuestNum--;

                cachedTmpros[i].gameObject.SetActive(false);
                showedQuest[i].state = QuestStates.Cleared;
                clearedNum++;
                if(clearedNum == questDatas.Length)
                {
                    PopupManager.Instance?.Popup("모든 퀘스트 클리어!");
                    CycleEvent_Nuclear.Disable();
                }
                else
                {
                    PopupManager.Instance?.Popup(showedQuest[i].text + " 완료!");
                }
                showedQuest[i] = null;
            }
        }

        // 한 번에 사운드 여러개 재생되는거 방지
        bool questAcceptedFlag = false;
        // 새 퀘스트 추가
        for (int i = 0; i < questDatas.Length && curQuestNum < MAX_VIEWED_QUEST; i++)
        {
            if(questDatas[i].state == QuestStates.Sleep && IsPreQuestCleaered(questDatas[i]))
            {
                curQuestNum++;
                AddQuest(questDatas[i]);
                questAcceptedFlag = true;
                questDatas[i].state = QuestStates.Running;
            }
        }
        if(questAcceptedFlag)
            SoundManager.Instance?.PlaySFX(SFX_Accepted);
    }
    bool AddQuest(QuestData data)
    {
        for(int i = 0; i < MAX_VIEWED_QUEST; i++)
        {
            if (showedQuest[i] == null)
            {
                showedQuest[i] = data;
                cachedTmpros[i].tmpro.text = data.text;
                cachedTmpros[i].gameObject.SetActive(true);
                return true;
            }
        }
        return false;
    }
    bool IsPreQuestCleaered(QuestData data)
    {
        for(int i = 0; i < data.preQuests.Length; i++)
        {
            if (data.preQuests[i].state != QuestStates.Cleared) return false;
        }
        return true;
    }
}
