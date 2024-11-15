using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Canvas Settings")]
    [SerializeField] private Canvas CVS_Menu;
    [SerializeField] private Button BTN_StartGame;
    [SerializeField] private Button BTN_Quit;
    [SerializeField] private Button BTN_Shop;
    [SerializeField] private Button BTN_Credits;

    [Header("Screens")]
    [SerializeField] private GameObject screenShop;
    [SerializeField] private GameObject screenCredits;

    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        BTN_StartGame.onClick.AddListener(StartGame);
        BTN_Quit.onClick.AddListener(QuitGame);
        BTN_Shop.onClick.AddListener(OpenShop);
        BTN_Credits.onClick.AddListener(OpenCredits);

        BackToMenu(); 
    }

    private void StartGame()
    {
        BTN_StartGame.interactable = false;
        StartCoroutine(InitializeGame());
    }
    IEnumerator InitializeGame()
    {
        yield return gameManager.StartCoroutine(gameManager.FadeIn());
        gameManager.ResetGame();
        CVS_Menu.enabled = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OpenShop()
    {
        ShopMenu shopMenu = FindAnyObjectByType<ShopMenu>();
        shopMenu.UpdateCoinText();
        screenShop.SetActive(true);
        CVS_Menu.enabled = false;
    }

    private void OpenCredits()
    {
        screenCredits.SetActive(true);
        CVS_Menu.enabled = false;
    }

    public void BackToMenu()
    {
        screenShop.SetActive(false);
        screenCredits.SetActive(false);
        BTN_StartGame.interactable = true;
        CVS_Menu.enabled = true;
    }
}
