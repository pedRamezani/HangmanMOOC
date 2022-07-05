using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Windows.Speech;

public class Character : MonoBehaviour
{
    public TextMeshProUGUI text;
    public char character;
    public Hangman manager;

    void Start()
    {
        text.text = "";
        //manager.dictationRecognizer.DictationResult += ShowVoiceLetter;
    }

    void Update()
    {
        if (Input.GetKeyDown(character.ToString().ToLower())) 
        {
            if (text.text == "") 
            {
                text.text = character.ToString().ToUpper();
                manager.leftCharacters--;
                manager.win = true;
                
            }
        }
    }

    public void ShowCharacter()
    {
        if (text.text == "")
        {
            text.text = character.ToString().ToUpper();
            text.alpha = 0.5f;
        }
    }

    public void ShowVoiceLetter()
    {
        if (text.text == "")
        {
            text.text = character.ToString().ToUpper();
            text.alpha = 1f;
            manager.leftCharacters--;
            manager.win = true;
        }
    }
}
