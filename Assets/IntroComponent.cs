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

    public string[] dialogue;

    private int currentLine = 0;
    private float currentMessageProgress = 0;

    public float charTime;

    private void Start()
    {
        Assert.AreEqual(charList.Length, charNames.Length);
        currentLine = 0;
        ShowCurrentLine();
    }

    public void ShowCurrentLine()
    {
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
        if (currentMessageProgress < currentMessage.Length)
        {
            currentMessageProgress += charTime;
        }
        textBox.text = currentMessage.Substring(0, Math.Min((int) currentMessageProgress, currentMessage.Length));
    }
}
