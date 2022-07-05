using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Windows.Speech;


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

    //Voice
    public DictationRecognizer dictationRecognizer;

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
                input_comparison(input);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartDictationEngine();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CloseDictationEngine();
        }

    }
    public void input_comparison(char input)
    {
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
    void GameOver()
    {
        for (int i = 0; i < wordParent.childCount; i++)
        {
            wordParent.GetChild(i).GetComponent<Character>().ShowCharacter();
        }
        leftCharacters = 0;
    }

    private void StartDictationEngine()
    {
        dictationRecognizer = new DictationRecognizer();
        //dictationRecognizer.DictationHypothesis += DictationRecognizer_OnDictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_OnDictationResult;
        //dictationRecognizer.DictationComplete += DictationRecognizer_OnDictationComplete;
        //dictationRecognizer.DictationError += DictationRecognizer_OnDictationError;
        dictationRecognizer.Start();
    }

    private void CloseDictationEngine()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationResult -= DictationRecognizer_OnDictationResult;

            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                dictationRecognizer.Stop();

            dictationRecognizer.Dispose();
        }
    }

    private void DictationRecognizer_OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Dictation result: " + text);

        char input = text[0];
        bool amongSolu = CheckChars(input);
        if (amongSolu == false)
        {
            input_comparison(input); //shows in tried chars
        }
    }

    bool CheckChars(char input)
    {
        bool isitinSolu = false;
        for (int i = 0; i < wordParent.childCount; i++)
        {
            Character letter = wordParent.GetChild(i).GetComponent<Character>();
            //Debug.Log("letters char is: " + letter.character);
            if (letter.character.ToString().ToLower()[0] == input)
            {
                letter.ShowVoiceLetter();
                isitinSolu = true;
            } 
        }
        return isitinSolu;
    }
}
