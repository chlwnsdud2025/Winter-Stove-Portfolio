
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CycleEvent_QuestGiver : CycleEvent
{
    [Header("퀘스트 데이터")]
    [SerializeField] QuestData[] questDatas;

    [Header("UI 요소")]
    [SerializeField] Transform parent;
    [SerializeField] GameObject prefab;
    [SerializeField] int MAX_VIEWED_QUEST;
    QuestUIElement[] cachedTmpros;
    QuestData[] showedQuest;
    private void Awake()
    {
        showedQuest = new QuestData[MAX_VIEWED_QUEST];
        cachedTmpros = new QuestUIElement[MAX_VIEWED_QUEST];
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
                RmvQuest(showedQuest[i]);
                questDatas[i].state = QuestStates.Cleared;
            }
        }

        // 새 퀘스트 추가
        for (int i = 0; i < questDatas.Length; i++)
        {
            if(questDatas[i].state == QuestStates.Sleep && IsPreQuestCleaered(questDatas[i]))
            {
                AddQuest(questDatas[i]);
                questDatas[i].state = QuestStates.Running;
            }
        }
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
    bool RmvQuest(QuestData data)
    {
        for (int i = 0; i < MAX_VIEWED_QUEST; i++)
        {
            if (showedQuest[i] != null)
            {
                showedQuest[i] = null;
                cachedTmpros[i].gameObject.SetActive(false);
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
