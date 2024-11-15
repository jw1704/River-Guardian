using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    #region Variables

    [Header("Player Settings")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Vector3 resetPosition;

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Dialogue Settings")]
    [SerializeField] private Canvas Canvas_Dialogue;
    [SerializeField] private TextMeshProUGUI Txt_Dialogue;
    [SerializeField] private List<string> Dialogues;
    [SerializeField] private float dialogueDelay = 3f;

    [Header("Timer Settings")]
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float countdownTime = 60f;

    [Header("Score Settings")]
    [SerializeField] private TextMeshProUGUI trashCountText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private int collectedTrashCount = 0;
    [SerializeField] private int maxTrashCount = 10;
    [SerializeField] public int coinCount = 0;

    [Header("Result Screen Settings")]
    [SerializeField] private Canvas CVS_Result;
    [SerializeField] private Image IMG_Background;
    [SerializeField] private TextMeshProUGUI TXT_Result;

    [Header("Pause Screen Settings")]
    [SerializeField] private Canvas CVS_Pause;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;

    private GameObject lastTrashSpawned;
    private bool isGameRunning = false;
    private bool hasGameEnded = false;
    private bool isPaused = false;
    #endregion

    private void Start()
    {
        StartCoroutine(FadeOut());
        CVS_Pause.enabled = false;
        PlayMenuMusic();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        CVS_Pause.enabled = isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResetGame()
    {
        collectedTrashCount = 0;
        UpdateTrashCountText();
        UpdateCoinText();

        playerManager.transform.position = resetPosition;
        timerSlider.value = countdownTime;
        hasGameEnded = false;
        InitializeGame();
    }

    #region Unity Callbacks

    void InitializeGame()
    {
        PlayGameMusic(); // Toca a música do jogo
        timerSlider.maxValue = countdownTime;
        timerSlider.value = countdownTime;
        UpdateTrashCountText();
        UpdateCoinText();
        CVS_Result.enabled = false;
        StartCoroutine(PlayDialogue());
    }

    #endregion

    #region Dialogue Coroutine

    private IEnumerator PlayDialogue()
    {
        yield return StartCoroutine(FadeOut());
        Canvas_Dialogue.enabled = true;
        Txt_Dialogue.text = "";

        foreach (string dialogue in Dialogues)
        {
            yield return StartCoroutine(TypeDialogue(dialogue));
            yield return new WaitForSeconds(dialogueDelay);
        }

        yield return StartCoroutine(FadeIn());
        Canvas_Dialogue.enabled = false;
        StartGame();
        yield return StartCoroutine(FadeOut());
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        Txt_Dialogue.text = "";
        foreach (char letter in dialogue)
        {
            Txt_Dialogue.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

    #endregion

    #region Game Start

    private void StartGame()
    {
        isGameRunning = true;
        StartCoroutine(Timer());
        StartCoroutine(FadeOut());
        StartCoroutine(SpawnTrash());
    }

    #endregion

    #region Fade Coroutines

    public IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1);
        fadeImage.enabled = true;
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            fadeImage.color = color;
            yield return null;
        }
        fadeImage.enabled = false;
    }

    public IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1);
        fadeImage.enabled = true;
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    #endregion

    #region Timer Coroutine

    public IEnumerator Timer()
    {
        float currentTime = countdownTime;

        while (currentTime > 0 && isGameRunning)
        {
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
            timerSlider.value = currentTime;
            currentTime -= Time.deltaTime;
            yield return null;
        }

        timerText.text = "0";
        timerSlider.value = 0;

        if (isGameRunning)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        if (hasGameEnded) return;
        hasGameEnded = true;

        isGameRunning = false;
        StopAllCoroutines();
        StartCoroutine(DisplayResult());
    }

    #endregion

    #region Result Screen Coroutine

    private IEnumerator DisplayResult()
    {
        yield return StartCoroutine(FadeIn());
        CVS_Result.enabled = true;

        TXT_Result.text = collectedTrashCount >= maxTrashCount ? "Vitória!" : "Derrota";

        yield return StartCoroutine(FadeOut());

        yield return new WaitForSeconds(5f);

        yield return StartCoroutine(FadeIn());
        CVS_Result.enabled = false;

        FindObjectOfType<MenuManager>().BackToMenu();

        yield return StartCoroutine(FadeOut());
        PlayMenuMusic();
    }

    #endregion

    #region Trash Spawning

    private IEnumerator SpawnTrash()
    {
        while (isGameRunning)
        {
            while (lastTrashSpawned != null && isGameRunning)
            {
                yield return null;
            }
            yield return new WaitForSeconds(2f);

            GameObject trashPrefab = GetRandomTrash();
            if (trashPrefab != null && isGameRunning)
            {
                Vector3 randomPosition = GetRandomPosition();
                lastTrashSpawned = Instantiate(trashPrefab, randomPosition, Quaternion.identity);
            }
        }
    }

    private GameObject GetRandomTrash()
    {
        GameObject[] trashPrefabs = Resources.LoadAll<GameObject>("Trash");
        if (trashPrefabs.Length == 0) return null;

        GameObject selectedTrash;
        do
        {
            selectedTrash = trashPrefabs[Random.Range(0, trashPrefabs.Length)];
        } while (selectedTrash == lastTrashSpawned);

        return selectedTrash;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-3.5f, 0.5f),
            0f
        );

        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(randomPosition.x, -10f, 10f),
            randomPosition.y,
            randomPosition.z
        );

        return clampedPosition;
    }

    #endregion

    #region Trash Collection

    public void OnTrashCollected()
    {
        if (hasGameEnded) return;

        collectedTrashCount++;
        coinCount += 5;
        UpdateTrashCountText();
        UpdateCoinText();

        if (collectedTrashCount >= maxTrashCount)
        {
            EndGame();
        }
    }

    private void UpdateTrashCountText()
    {
        trashCountText.text = collectedTrashCount + "/" + maxTrashCount;
    }

    private void UpdateCoinText()
    {
        coinText.text = "$" + coinCount;
    }

    #endregion

    #region Audio Management

    private void PlayMenuMusic()
    {
        if (audioSource != null && menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void PlayGameMusic()
    {
        if (audioSource != null && gameMusic != null)
        {
            audioSource.clip = gameMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    #endregion
}
