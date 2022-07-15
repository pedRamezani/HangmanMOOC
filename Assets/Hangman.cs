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
    public int hangmanFrame = -1;

    private int MAX_FRAMES; 
    private DictationRecognizer dictationRecognizer;
    private char[] word;

    void Start()
    {
        MAX_FRAMES = hangmanSprite.Length;
        words = GetComponent<EnglishWords>().all.Where(s => s.Length >= 5 && s.Length < 15).ToArray();
        word = words[UnityEngine.Random.Range(0, words.Length)].ToCharArray();
        Debug.LogFormat("Word is {0}", new string(word));
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
            Debug.LogFormat("Dictation Result: {0} with confidence: {1}", text, confidence);
            string searchingWord = new string(word); //char[] conv to string

            if (restartWords.Contains(text.ToLower()))                                  //checks if want to get nect word
            {
                wantsToRestart = true;
                restartPanel.SetActive(true);
            }
            else if (text.ToLower().Equals(searchingWord.ToLower())) {                  //checks if oral word ist the solution
                Debug.LogFormat("Es ist gleich");
                for (int i = 0; i < wordParent.childCount; i++)
                {
                    wordParent.GetChild(i).GetComponent<Character>().ShowCharacterOral();
                }
                GameOver(true);
            }
            else                                                                        //checks the letter
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
                hangmanFrame++;
                if (hangmanFrame >= MAX_FRAMES) 
                {
                    hangmanFrame = MAX_FRAMES;
                    // Verloren
                    GameOver(false);
                }
                else 
                {
                    hangmanImage.sprite = hangmanSprite[hangmanFrame];
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
