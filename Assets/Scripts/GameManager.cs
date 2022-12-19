using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum EGameState
{
    Menu,
    Running,
    Paused,
    Finished
}
public enum EAudioClip
{
    Jump,
    Death
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public EGameState GameState { get; private set; }
    public Action e_GameStarted;

    // UI panel
    [Header("UI Panel")]
    [SerializeField] private GameObject menuObj;
    [SerializeField] private GameObject pauseObj;
    [SerializeField] private GameObject gameOverObj;

    // Individual ui objects
    [Header("UI Objects")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private GameObject colorWheelObj;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Animator blackFade;

    [Header("Audio")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Image speakerImg;
    [SerializeField] private Sprite speakerEnabledSpr;
    [SerializeField] private Sprite speakerDisabledSpr;
    [SerializeField] private Image musicImg;
    [SerializeField] private Sprite musicEnabledSpr;
    [SerializeField] private Sprite musicDisabledSpr;
    [SerializeField] private AudioSource audioSource;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip DieSound;

    private float timeScaleSave = 1;
    private int score = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        GameState = EGameState.Menu;
    }

    private void Start()
    {
        #region Adjust Sound Settings
        mixer.GetFloat("SFX", out float sfxvolume);
        mixer.GetFloat("Music", out float musicvolume);

        if (sfxvolume == -80)
            speakerImg.sprite = speakerDisabledSpr;

        else if (sfxvolume == 0)
            speakerImg.sprite = speakerEnabledSpr;

        if (musicvolume == -80)
            musicImg.sprite = musicDisabledSpr;
        else if (musicvolume == -35)
            musicImg.sprite = musicEnabledSpr;
        #endregion

        Time.timeScale = 1;
    }

    private void Update()
    {
        switch (GameState)
        {
            case EGameState.Menu:
                break;

            case EGameState.Running:
                if (Input.GetKeyDown(KeyCode.Escape))
                    PauseGame();

                if (Time.timeScale <= 2)
                    Time.timeScale += Time.deltaTime * Time.deltaTime / 2f;
                break;

            case EGameState.Paused:
                if (Input.GetKeyDown(KeyCode.Escape))
                    ResumeGame();
                break;

            case EGameState.Finished:
                break;
        }

        scoreText.text = $"Score: {score}";
        gameOverScoreText.text = $"Score:\n{score}";

        if (!Application.isFocused && GameState == EGameState.Running)
            PauseGame();
    }

    public void PlayJumpSound(EAudioClip _clip)
    {
        switch (_clip)
        {
            case EAudioClip.Jump:
                audioSource.clip = jumpSound;
                break;
            case EAudioClip.Death:
                audioSource.clip = DieSound;
                break;
        }

        audioSource.pitch = UnityEngine.Random.Range(.95f, 1.05f);

        audioSource.Play();
    }
    public void ToggleSound(string _mixerName)
    {
        mixer.GetFloat(_mixerName, out float volume);

        if (volume == -80)
        {
            if (_mixerName == "SFX")
            {
                mixer.SetFloat(_mixerName, 0);
                speakerImg.sprite = speakerEnabledSpr;
            }
            if (_mixerName == "Music")
            {
                mixer.SetFloat(_mixerName, -35);
                musicImg.sprite = musicEnabledSpr;
            }
        }

        else if (volume == 0)
        {
            speakerImg.sprite = speakerDisabledSpr;
            mixer.SetFloat(_mixerName, -80);
        }
        else if (volume == -35)
        {
            musicImg.sprite = musicDisabledSpr;
            mixer.SetFloat(_mixerName, -80);
        }
    }

    public void IncreaseScore(int _amount)
    {
        if (GameState != EGameState.Running) return;

        score += _amount;
    }

    public void StartGame()
    {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        menuObj.SetActive(false);
        countdownText.gameObject.SetActive(true);

        float time = 3;

        while (time > 0)
        {
            countdownText.text = "" + (int)time;

            time -= Time.deltaTime;

            yield return null;
        }
        GameState = EGameState.Running;

        e_GameStarted?.Invoke();

        countdownText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        colorWheelObj.SetActive(true);
    }

    public void GameOver()
    {
        GameState = EGameState.Finished;

        Time.timeScale = 0;

        pauseObj.SetActive(false);
        scoreText.gameObject.SetActive(false);

        gameOverObj.SetActive(true);
        gameOverScoreText.gameObject.SetActive(true);
        colorWheelObj.SetActive(false);
    }

    public void RestartGame()
    {
        StartCoroutine(LoadGameOver());
    }

    private IEnumerator LoadGameOver()
    {
        blackFade.SetTrigger("FadeIn");

        float counter = 0;
        float waitTime = blackFade.GetCurrentAnimatorStateInfo(0).length;

        while (counter < waitTime)
        {
            counter += Time.unscaledDeltaTime;
            yield return null;
        }

        SceneManager.LoadScene(1);
    }

    public void PauseGame()
    {
        GameState = EGameState.Paused;

        timeScaleSave = Time.timeScale;
        Time.timeScale = 0;

        pauseObj.SetActive(true);
    }
    public void ResumeGame()
    {
        GameState = EGameState.Running;

        Time.timeScale = timeScaleSave;
        pauseObj.SetActive(false);
    }
}
