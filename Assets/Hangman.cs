using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Hangman : MonoBehaviour
{
    public string[] words;
    public Transform characterTemplate;
    public Transform wordParent;

    public int leftCharacters;
    public bool win;
    public List<char> triedCharacters;
    public TextMeshProUGUI triedText;
    public Image hangmanImage;
    public Sprite[] hangmanSprite;
    public int hangmanFrame;

    public static int MAX_FRAMES = 9; 

    void Start()
    {
        char[] word = words[UnityEngine.Random.Range(0, words.Length)].ToCharArray();
        leftCharacters = word.Length;

        foreach (char character in word)
        {
            if (char.IsLetterOrDigit(character)) 
            {
                Character characterScript = Instantiate(characterTemplate, wordParent).GetComponent<Character>();
                characterScript.character = character;
                characterScript.manager = this;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) 
        {   
            //Restart
            if (leftCharacters == 0) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } 
        else if (Input.anyKeyDown) 
        {
            if (Input.inputString.ToCharArray().Length > 0)
            {
                char input = Input.inputString.ToCharArray()[0];
                if (char.IsLetterOrDigit(input))
                {
                    if (!triedCharacters.Contains(input))
                    {
                        triedCharacters.Add(input);
                        if (!win) 
                        {
                            if (triedText.text != "") 
                            {
                            triedText.text += ", ";
                            }
                            triedText.text += input.ToString().ToUpper();

                            hangmanFrame++;
                            if (hangmanFrame >= MAX_FRAMES) 
                            {
                                hangmanFrame = MAX_FRAMES;
                                
                                // Verloren
                                GameOver();
                            }
                            hangmanImage.sprite = hangmanSprite[hangmanFrame];
                        }
                        win = false;
                    }
                }
            }
        }
    }

    void GameOver()
    {
        for (int i = 0; i < wordParent.childCount; i++)
        {
            wordParent.GetChild(i).GetComponent<Character>().ShowCharacter();
        }
        leftCharacters = 0;
    }
}
