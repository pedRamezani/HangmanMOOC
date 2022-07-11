using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using TMPro;


public class Hangman : MonoBehaviour
{
    public char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUWVXYZ".ToCharArray();

    public string[] words;
    public List<string> restartWords =  new List<string> {"reset", "restart", "neustart"};
    public Transform characterTemplate;
    public Transform wordParent;

    public int leftCharacters;
    public bool correctGuess, gameOver = false, win = false, restart = false, wantsToRestart = false;
    public GameObject restartPanel;
    public List<char> triedCharacters;
    public TextMeshProUGUI triedText;
    public TextMeshProUGUI scoreText;
    public Image hangmanImage;
    public Sprite[] hangmanSprite;
    public Sprite winSprite;
    public Sprite looseSprite;
    public int hangmanFrame;

    public static int MAX_FRAMES = 9; 

    private DictationRecognizer dictationRecognizer;

    void Start()
    {
        words = GetComponent<EnglishWords>().all.Where(s => s.Length >= 5 && s.Length < 15).ToArray();
        char[] word = words[UnityEngine.Random.Range(0, words.Length)].ToCharArray();
        leftCharacters = word.Length;

        foreach (char character in word)
        {
            if (char.IsLetter(character)) 
            {
                Character characterScript = Instantiate(characterTemplate, wordParent).GetComponent<Character>();
                characterScript.character = character;
                characterScript.manager = this;
            }
        }

        triedText.richText = true;
        triedText.text = string.Join(", ", alphabet);

        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation hypothesis: {0} with confidence: {1}", text, confidence);
            if (restartWords.Contains(text.ToLower()))
            {
                wantsToRestart = true;
                restartPanel.SetActive(true);
            }
            else 
            {
                Guess(text[0]);
                for (int i = 0; i < wordParent.childCount; i++)
                {
                    wordParent.GetChild(i).GetComponent<Character>().Guess(text);
                }
            }
            
        };

        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Won: " + Score.winCounter + "\nLoose: " + Score.looseCounter;
    }

    void Update()
    {
        if (wantsToRestart)
        {
            if (dictationRecognizer.Status == SpeechSystemStatus.Running) 
            {
                dictationRecognizer.Stop();
            }
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Reset");
                restart = true;
                wantsToRestart = false;
            }
            else if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                Debug.Log("Escape");
                wantsToRestart = false;
                restartPanel.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return) || restart)
        {   
            if (dictationRecognizer.Status == SpeechSystemStatus.Running) 
            {
                dictationRecognizer.Stop();
            }
            Debug.Log("Restart Game");
            //Restart
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } 
        else if (Input.GetKeyDown(KeyCode.Space)) 
        {   
            if (dictationRecognizer.Status == SpeechSystemStatus.Stopped) 
            {
                dictationRecognizer.Start();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space)) 
        {
            if (dictationRecognizer.Status == SpeechSystemStatus.Running) 
            {
                dictationRecognizer.Stop();
            }
        }
        else if (Input.anyKeyDown && !gameOver) 
        {
            if (Input.inputString.ToCharArray().Length > 0)
            {
                char input = Input.inputString.ToCharArray()[0];
                if (char.IsLetter(input))
                {
                    Guess(input);
                }
            }
        }
    }

    void Guess(char input) 
    {
        if (!triedCharacters.Contains(char.ToLower(input)))
        {
            triedCharacters.Add(char.ToLower(input));
            triedText.text = "";
            foreach (char letter in alphabet) 
            {
                bool strikethrough = triedCharacters.Contains(char.ToLower(letter));
                if (triedText.text != "") 
                {
                        triedText.text += ", ";
                }
                if (strikethrough) 
                {
                    triedText.text += "<color=#f44336>";
                }
                triedText.text += letter.ToString();
                if (strikethrough) 
                {
                    triedText.text += "</color>";
                }
            }
            
            if (!correctGuess) 
            {
                hangmanImage.sprite = hangmanSprite[hangmanFrame];
                hangmanFrame++;
                if (hangmanFrame >= MAX_FRAMES) 
                {
                    hangmanFrame = MAX_FRAMES;
                    // Verloren
                    GameOver(false);
                }
            } 
            else if (leftCharacters == 0) 
            {
                GameOver(true);
            }
            correctGuess = false;
        }
    }

    void GameOver(bool hasWon)
    {

        if (!hasWon) {
            for (int i = 0; i < wordParent.childCount; i++)
            {
                wordParent.GetChild(i).GetComponent<Character>().ShowCharacter();
            }
            Score.looseCounter += 1;
            hangmanImage.sprite = looseSprite;
        }
        else
        {
            Score.winCounter += 1;
            hangmanImage.sprite = winSprite;
        }

        UpdateScore();
        
        win = hasWon;
        gameOver = true;
    }
}
