using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    public DialogueDataSO myDialogue;                          // npc 만의
    private DialogueManager dialogueManager;                    // 대화 매니

    void Start()
    {
        dialogueManager = FindAnyObjectByType<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogError("다이얼 로그 매니저가 없습니다.");
        }
    }

    private void OnMouseDown()
    {
        if (dialogueManager == null) return;                    // 대
        if (dialogueManager.IsDialogueActive()) return;          // 대
        if (myDialogue == null) return;                         // 대

        dialogueManager.StartDialogue(myDialogue);              // 대
    }
}