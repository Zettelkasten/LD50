using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class IntroComponent : MonoBehaviour
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

    private void Start()
    {
        dialogue = dialogueText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        Assert.AreEqual(charList.Length, charNames.Length);
        currentLine = 0;
        ShowCurrentLine();
    }

    public void ShowCurrentLine()
    {
        // disable others
        foreach (var c in this.charList)
            c.SetActive(false);
        
        var fullLine = dialogue[currentLine];
        var splitBy = new char[] { ':' };
        var split = fullLine.Split(splitBy, 2);
        currentSpeaker = split[0].Trim();
        currentMessage = split[1].Trim();
        
        // find char
        if (currentChar != null)
            currentChar.SetActive(false);
        currentChar = charList[Array.IndexOf(this.charNames, this.currentSpeaker)];
        currentChar.SetActive(true);

        textBox.text = "";
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
            // todo, start game.
        }
    }
}
