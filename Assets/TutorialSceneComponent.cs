using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TutorialSceneComponent : MonoBehaviour
{
    public GameObject[] charList;
    private GameObject currentChar = null;
    public string[] charNames;

    public Text textBox;

    private string currentSpeaker;
    private string currentMessage;

    public TextAsset dialogueText;
    public string[] dialogue;

    private int currentLine = 0;
    private float currentMessageProgress = 0;

    public float charTime;

    public bool isDone = false;

    public bool autoContinueEnabled = false;
    public float autoContinue = -1;

    private void Start()
    {
        Assert.AreEqual(charList.Length, charNames.Length);
        dialogue = dialogueText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        
        // disable all at beginning
        foreach (var c in this.charList)
            c.SetActive(false);

        currentLine = 0;
        ShowCurrentLine();
    }

    public void ShowCurrentLine()
    {
        var fullLine = dialogue[currentLine];
        var splitBy = new char[] { ':' };
        var split = fullLine.Split(splitBy, 2);
        if (split.Length != 2)
        {
            Debug.Log("this is wrong!!!");
        }

        currentSpeaker = split[0].Trim();
        currentMessage = split[1].Trim();
        
        // find char
        if (currentChar != null)
            currentChar.SetActive(false);
        currentChar = charList[Array.IndexOf(this.charNames, this.currentSpeaker)];
        currentChar.SetActive(true);

        textBox.text = "";
        currentMessageProgress = 0;
    }

    private void FixedUpdate()
    {
        if (currentMessage == null)
            return;
        if (currentMessageProgress < currentMessage.Length)
        {
            currentMessageProgress += charTime;
        }
        textBox.text = currentMessage.Substring(0, Math.Min((int) currentMessageProgress, currentMessage.Length));
        
        if (Input.GetKeyDown("space"))
        {
            MouseDown();
        }

        if (autoContinueEnabled)
        {
            autoContinue -= Time.fixedDeltaTime;
            if (autoContinue < 0)
            {
                MouseDown();
                autoContinue = 300f;
            }
        }
    }

    public void MouseDown()
    {
         if (currentMessageProgress < currentMessage.Length)
        {
            currentMessageProgress = currentMessage.Length;
            return;
        }

        if (currentLine + 1 < dialogue.Length)
        {
            currentLine += 1;
            ShowCurrentLine();
        }
        else
        {
            isDone = true;
        }
    }

    public void ResetDialogue(string[] dialogue)
    {
        this.dialogue = dialogue;
        this.isDone = false;
        this.currentLine = 0;
        this.ShowCurrentLine();
    }
}
