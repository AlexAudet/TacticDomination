using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinionCard : MonoBehaviour
{
    public MinionData data;
    public Minion minionPrefab;
    public enum CardType { Deck, Owned, NotOwned , ChangeMinion, Shop}
    public CardType type;

    [Space(20)]    
    public Image icon;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI minionAmountText;
    public Slider minionAmountSlider;
    public GameObject Fade;
    public GameObject GlowFrame;
    public GameObject EquipedBanner;
    public GameObject clickMenu;
    public Button equipButon;
    public Button upgradeButon;
    public Button InfoButton;
    public TextMeshProUGUI shopMinionAmountText;

    Button cardButton;

    private void Awake()
    {
        cardButton = GetComponent<Button>();
        cardButton.onClick.AddListener(OpenClickCardMenu);
        equipButon.onClick.AddListener(EquipMinion);
        InfoButton.onClick.AddListener(InfoMinion);
        upgradeButon.onClick.AddListener(UpgradeMinion);
    }

    void OpenClickCardMenu()
    {
        if (type == CardType.Shop)
            return;

        if(type != CardType.ChangeMinion)
        {
            if(type != CardType.NotOwned)
            {
                float floatMinionAmount = data.minionAmount;
                float floatMinionNeeded = DataHolder.Instance.minionAmountToLevelUp[data.level - 1].minionAmount;

                equipButon.gameObject.SetActive(!data.deckIndex.Contains(MinionInventory.Instance.Player_Deck) && type != CardType.NotOwned);
           
                if (data.minionAmount >= floatMinionNeeded && PlayFabManager.Instance.Player_Coin >= DataHolder.Instance.minionAmountToLevelUp[data.level - 1].coinCost )
                    upgradeButon.gameObject.SetActive(true);
                else
                    upgradeButon.gameObject.SetActive(false);

                MinionInventory.Instance.ChangeCurrentCard(this);

                clickMenu.SetActive(true);
            }
            else
            {
                InfoMinion();
            }
       
        }
        else
        {
            MinionInventory.Instance.ChangeMinionInDeck(data);
        }    
    }

    public void CloseClickMenu()
    {
        clickMenu.SetActive(false);
    }

    void EquipMinion()
    {
        MinionInventory.Instance.OpenEquipMinionSelector(data);
    }

    void InfoMinion()
    {
      
    }

    void UpgradeMinion()
    {
        int minionNeeded = DataHolder.Instance.minionAmountToLevelUp[data.level - 1].minionAmount;
        int coinNeeded = DataHolder.Instance.minionAmountToLevelUp[data.level - 1].coinCost;

        if (data.minionAmount >= minionNeeded && PlayFabManager.Instance.Player_Coin >= coinNeeded)
            PlayFabManager.Instance.UpgradeMinion(data);
        else
            Debug.LogError("Not enought");

        CloseClickMenu();
    }

    public void InitiateCard(MinionData minonData, Transform parent, CardType cardType, int whichDeck = 0, int shopAmount = 0)
    {
        if(cardType != MinionCard.CardType.NotOwned)
        {
            transform.parent = parent;
            
            type = cardType;

            Fade.SetActive(false);
            GlowFrame.SetActive(true);
            data = minonData;

            if (cardType == CardType.Deck)
                minonData.deckCard[whichDeck - 1] = this;
            else if (cardType == CardType.Owned)
                minonData.inventoryCard = this;
           

            icon.sprite = minonData.minion.minionIconSprite;
            levelText.text = "Level " + minonData.level.ToString();
            float floatMinionAmount = minonData.minionAmount;
            float floatMinionNeeded = DataHolder.Instance.minionAmountToLevelUp[minonData.level - 1].minionAmount;
            minionAmountText.text = floatMinionAmount.ToString() + "/" + floatMinionNeeded.ToString();
            equipButon.gameObject.SetActive(floatMinionAmount >= floatMinionNeeded);
          
            if (type != CardType.Shop)
            {
                shopMinionAmountText.transform.parent.gameObject.SetActive(false);
                levelText.transform.parent.gameObject.SetActive(true);
                minionAmountSlider.gameObject.SetActive(true);
                minionAmountSlider.value = floatMinionAmount / floatMinionNeeded;
            }
            else
            {
                shopMinionAmountText.transform.parent.gameObject.SetActive(true);
                levelText.transform.parent.gameObject.SetActive(false);
                minionAmountSlider.gameObject.SetActive(false);
                shopMinionAmountText.text = "x" + shopAmount.ToString();
            }
              

            if (data.deckIndex.Contains(MinionInventory.Instance.Player_Deck) && type != CardType.Deck && type != CardType.ChangeMinion && type != CardType.Shop)
                EquipedBanner.SetActive(true);
            else
                EquipedBanner.SetActive(false);

            minionPrefab = minonData.minion;
        }
        else
        {
            Fade.SetActive(true);
            GlowFrame.SetActive(false);
            minionPrefab = minonData.minion;
            data = minonData;
            transform.parent = parent;
            type = CardType.NotOwned;
            icon.sprite = minonData.minion.minionIconSprite;
            levelText.text = "Level 1";
            minionAmountText.text = 0.ToString() + "/" + 0.ToString();
            minionAmountSlider.value = 0;
        }

        transform.localScale = Vector3.one;

        if(parent.gameObject.GetComponent<ContentSizeFitter>() != null)
        {
            parent.gameObject.GetComponent<ContentSizeFitter>().enabled = false;
            parent.gameObject.GetComponent<ContentSizeFitter>().enabled = true;
        }
    }

    public void UpdateCard()
    {

        if (type != MinionCard.CardType.NotOwned)
        {
            Fade.SetActive(false);
            GlowFrame.SetActive(true);

            levelText.text = "Level " + data.level.ToString();
            float floatMinionAmount = data.minionAmount;
            float floatMinionNeeded = DataHolder.Instance.minionAmountToLevelUp[data.level - 1].minionAmount;
            minionAmountText.text = floatMinionAmount.ToString() + "/" + floatMinionNeeded.ToString();
            equipButon.gameObject.SetActive(floatMinionAmount >= floatMinionNeeded);
            minionAmountSlider.value = floatMinionAmount / floatMinionNeeded;
            EquipedBanner.SetActive(data.deckIndex.Contains(MinionInventory.Instance.Player_Deck) && type != CardType.Deck && type != CardType.ChangeMinion);
        }
        else
        {
            MinionCard newMinionCardInventory = null;

            switch (minionPrefab.rarety)
            {
                case Minion.MinonRarety.Common:

                    newMinionCardInventory = Instantiate(MinionInventory.Instance.commonMinionCard);

                    break;
                case Minion.MinonRarety.Rare:

                    newMinionCardInventory = Instantiate(MinionInventory.Instance.rareMinionCard);

                    break;
                case Minion.MinonRarety.Epic:

                    newMinionCardInventory = Instantiate(MinionInventory.Instance.epicMinionCard);

                    break;
                default:
                    break;
            }
            newMinionCardInventory.InitiateCard(data, MinionInventory.Instance.OwnedCardHolder, CardType.Owned);
            Destroy(gameObject);
        }     
    }

    public void ActiveEquipedBanner(bool value)
    {
        EquipedBanner.SetActive(value);
    }
}
