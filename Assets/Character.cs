using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Character : MonoBehaviour
{
    public TextMeshProUGUI text;
    public char character;
    public Hangman manager;

    void Start()
    {
        text.text = "";
        text.richText = true;
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
            text.text = "<i>" + character.ToString().ToUpper() + "</i>";
            text.alpha = 0.8f;
        }
    }
}
