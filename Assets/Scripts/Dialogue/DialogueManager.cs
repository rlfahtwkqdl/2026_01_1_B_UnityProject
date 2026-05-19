using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.XR.OpenVR;

public class DialogueManager : MonoBehaviour
{
    [Header("UI 요소 - 인스펙터 창에서 연결")]
    public GameObject DialoguePanel;
    public Image characterImage;
    public TextMeshProUGUI characternameText;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;

    [Header("기본 설정")]
    public Sprite defaultCharacterImage;

    [Header("타이핑 효과 설정")]
    public float typingSpeed = 0.05f;
    public bool skipTypingOnClick = true;

    //내부 변수들
    private DialogueDataSO currentDialogue;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DialoguePanel.SetActive(false);
        nextButton.onClick.AddListener(HandleNextInput);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            HandleNextInput();
        }
    }

    IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        dialogueText.text = "";

        for(int i = 0; i < textToType.Length; i++)
        {
            dialogueText.text += textToType[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void CompleteTyping()
    {
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        isTyping = false;

        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            dialogueText.text = currentDialogue.dialogueLines[currentLineIndex];
        }
    }


    void ShowCurrentLine()
    {
        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            if(typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            string currentText = currentDialogue.dialogueLines[currentLineIndex];
            typingCoroutine = StartCoroutine(TypeText(currentText));
        }
    }


    public void ShowNextLine()
    {
        currentLineIndex++;


        if (currentLineIndex >= currentDialogue.dialogueLines.Count)
        {

        }
        else
        {
            ShowCurrentLine();
        }
    }

    void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isDialogueActive = false;
        isTyping = false;
        DialoguePanel.SetActive(false);
        currentLineIndex = 0;
    }

    public void HandleNextInput()
    {
        if (isTyping && skipTypingOnClick)
        {
            CompleteTyping();
        }
        else if (!isTyping)
        {
            ShowCurrentLine();
        }
        
    }

    public void SkipDialogue()
    {
        EndDialogue();
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    public void StartDialogue(DialogueDataSO dialogue)     //새로운 대화를
    {
        if (dialogue == null || dialogue.dialogueLines.Count == 0) return;

        //대화 시작 준비
        currentDialogue = dialogue;                       //현재 대화 데이터 설정
        currentLineIndex = 0;                             //첫 번째 대화 부터 시작
        isDialogueActive = true;                          //대화 활성 화 플래그 On

        //UI 업데이트
        DialoguePanel.SetActive(true);                    //대화창 보이기
        characternameText.text = dialogue.characterName;  //캐릭터 이름 표

        if (characterImage != null)
        {
            if (dialogue.characterImage != null)
            {
                characterImage.sprite = dialogue.characterImage;
            }
            else
            {
                characterImage.sprite = defaultCharacterImage;
            }
        }

        ShowCurrentLine();
    }
}
