using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopMenu : MonoBehaviour
{
    [Header("Player Sprites to Sell")]
    [SerializeField] private List<Sprite> playerSpritesToSell;

    [Header("UI Elements")]
    [SerializeField] private GameObject spriteButtonPrefab;
    [SerializeField] private Transform spriteButtonContainer;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button useButton;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private TextMeshProUGUI coinText;

    private Sprite selectedSprite;
    private HashSet<Sprite> purchasedSprites = new HashSet<Sprite>();

    private void Start()
    {
        PopulateShop();
        UpdateCoinText();

        if (playerSpritesToSell.Count > 0)
        {
            Sprite firstSprite = playerSpritesToSell[0];
            purchasedSprites.Add(firstSprite);
            selectedSprite = firstSprite;

            buyButton.interactable = false;
            useButton.interactable = true;
        }
        else
        {
            buyButton.interactable = false;
            useButton.interactable = false;
        }
    }

    private void PopulateShop()
    {
        foreach (Sprite sprite in playerSpritesToSell)
        {
            GameObject buttonObject = Instantiate(spriteButtonPrefab, spriteButtonContainer);
            Button button = buttonObject.GetComponent<Button>();

            Image imgSprite = buttonObject.transform.Find("IMG_Sprite").GetComponent<Image>();
            if (imgSprite != null)
            {
                imgSprite.sprite = sprite;
            }

            TextMeshProUGUI txtName = buttonObject.transform.Find("TXT_Name").GetComponent<TextMeshProUGUI>();
            if (txtName != null)
            {
                txtName.text = sprite.name;
            }

            button.onClick.AddListener(() => BTN_Choose(sprite));
        }
    }

    public void BTN_Choose(Sprite sprite)
    {
        selectedSprite = sprite;

        if (purchasedSprites.Contains(sprite))
        {
            buyButton.interactable = false;
            useButton.interactable = true;
        }
        else
        {
            buyButton.interactable = true;
            useButton.interactable = false;
        }
    }

    public void Buy()
    {
        int price = 100;

        if (gameManager.coinCount >= price)
        {
            gameManager.coinCount -= price;
            purchasedSprites.Add(selectedSprite);
            UpdateCoinText();

            buyButton.interactable = false;
            useButton.interactable = true;
        }
        else
        {
            Debug.Log("Moedas insuficientes!");
        }
    }

    public void Use()
    {
        playerManager.UpdatePlayerSprite(selectedSprite);
        Debug.Log("Sprite atualizado!");

        buyButton.interactable = false;
        useButton.interactable = false;
    }

    public void BTN_BuyMoveSpeed()
    {
        int speedCost = 50;
        if (gameManager.coinCount >= speedCost)
        {
            gameManager.coinCount -= speedCost;
            playerManager.currentMovementSpeed += 0.5f; // Aumenta a velocidade de movimento
            UpdateCoinText();
            Debug.Log("Velocidade de movimento aumentada!");
        }
        else
        {
            Debug.Log("Moedas insuficientes para aumentar a velocidade!");
        }
    }

    public void UpdateCoinText()
    {
        coinText.text = "$" + gameManager.coinCount;
    }
}
