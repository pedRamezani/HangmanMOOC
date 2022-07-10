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
        // TODO: Fix Fission => not working correctly
        if (Input.GetKeyDown(character.ToString().ToLower()) && !Input.GetKeyDown(KeyCode.Space)) 
        {
            if (text.text == "") 
            {
                text.text = character.ToString().ToUpper();
                manager.leftCharacters--;
                manager.correctGuess = true;
            }
        }
    }

    public void Guess(string c)
    {
        if (text.text == "" && c.ToLower()[0] == character.ToString().ToLower()[0]) 
            {
                text.text = character.ToString().ToUpper();
                manager.leftCharacters--;
                manager.win = true;
            }
    }

    public void ShowCharacter()
    {
        if (text.text == "")
        {
            text.text = "<i>" + character.ToString().ToUpper() + "</i>";
            text.alpha = 0.5f;
        }
    }
}
