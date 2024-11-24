using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class Intro : MonoBehaviour
{
    public TextMeshProUGUI ChatName;
    public TextMeshProUGUI text;
    public InputAction SkipAction;
    public int CoroutineId = 0;
    public SleepControl Control;
    public List<string> textList = new();
    public List<Chat> chatList = new();
    public List<Chat> failList = new();
    public int ChatId = 0;
    public bool IsFailMode = false;
    public int failId = 0;
    public void Start()
    {
        ChatName.text = chatList[ChatId].name;
        text.text = chatList[ChatId].context;
        StartCoroutine(ShowText());
    }
    public void OnEnable()
    {
        SkipAction.Enable();
        SkipAction.performed += OnSkip;
    }
    public void OnDisable()
    {
        SkipAction.Disable();
        SkipAction.performed -= OnSkip;
    }

    IEnumerator ShowText()
    {
        text.maxVisibleCharacters = 0;
        yield return null;

        CoroutineId++;
        int id = CoroutineId;

        while(text.maxVisibleCharacters < text.textInfo.characterCount
            && id == CoroutineId)
        {
            yield return new WaitForSeconds(0.03f);
            text.maxVisibleCharacters++;
        }

        text.maxVisibleCharacters = text.textInfo.characterCount;
    }

    public void OnSkip(InputAction.CallbackContext context)
    {
        CoroutineId++;
        AuidoManager.instance.PlaySfxById(0);
        if (text.maxVisibleCharacters >= text.textInfo.characterCount)
        {
            if(IsFailMode == false)
            {
                ChatId++;
                if (ChatId < chatList.Count)
                {
                    ChatName.text = chatList[ChatId].name;
                    text.text = chatList[ChatId].context;
                    StartCoroutine(ShowText());
                }
                else
                {
                    Control.GameStart();
                    gameObject.SetActive(false);
                }
            }
            else
            {
                failId++;
                if (failId < failList.Count)
                {
                    ChatName.text = failList[failId].name;
                    text.text = failList[failId].context;
                    StartCoroutine(ShowText());
                }
                else
                {
                    failId = 0;
                    Control.EnableAction();
                    gameObject.SetActive(false);
                }
            }

        }
    }

    public void ShowFailChat()
    {
        gameObject.SetActive(true);
        Control.DisableAction();
        ChatName.text = failList[0].name;
        text.text = failList[0].context;
        StartCoroutine(ShowText());
        IsFailMode = true;
    }
}

[Serializable]
public class Chat
{
    public string name;
    public string context;
}
