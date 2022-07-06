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

        dictationRecognizer = new DictationRecognizer(ConfidenceLevel.High, DictationTopicConstraint.Dictation);

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation hypothesis: {0} with confidence: {1}", text, confidence);
            Guess(text[0]);
            for (int i = 0; i < wordParent.childCount; i++)
            {
                wordParent.GetChild(i).GetComponent<Character>().Guess(text);
            }
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) 
        {   
            //Restart
            if (leftCharacters == 0) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        else if (Input.anyKeyDown) 
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
            
            if (!win) 
            {
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

    void GameOver()
    {
        for (int i = 0; i < wordParent.childCount; i++)
        {
            wordParent.GetChild(i).GetComponent<Character>().ShowCharacter();
        }
        leftCharacters = 0;
    }
}
