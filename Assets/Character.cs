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
            RevealCharacter();
        }
    }

    public void Guess(string c)
    {
        if (c.ToLower()[0] == character.ToString().ToLower()[0]) 
        {
            RevealCharacter();
        }
    }

    // Correct Guess
    void RevealCharacter()
    {
        if (text.text == "")
        {
            text.text = character.ToString().ToUpper();
            manager.leftCharacters--;
            manager.correctGuess = true;
        }
    }

    // After Loosing
    public void ShowCharacter()
    {
        if (text.text == "")
        {
            text.text = "<i>" + character.ToString().ToUpper() + "</i>";
            text.alpha = 0.5f;
        }
    }
    public void ShowCharacterOral()
    {
        if (text.text == "")
        {
            text.text = character.ToString().ToUpper();
            text.alpha = 1f;
        }
    }
}
