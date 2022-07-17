using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MinionInventory : MonoBehaviour
{
    public static MinionInventory Instance;
    MinionInventory()
    {
        Instance = this;
    }

    public MinionCard commonMinionCard, rareMinionCard, epicMinionCard;

    public Transform OwnedCardHolder, notOwnedCardHolder;

    public Button firstDeckTabsButton, secondDeckTabsButton, thirdDeckTabsButton;

    public Transform deckHolderOne, deckHolderTwo, deckHolderThree;

    public Color selectedTabColor, notSelectedTabColor;

    public EquipMinionSelector equipMinionSelector;



    public List<MinionData> minionInventory = new List<MinionData>();
    [ShowInInspector]
    public List<string[]> allDecks = new List<string[]>();
    string[] firstDeck = new string[3];
    string[] secondDeck = new string[3];
    string[] thirdDeck = new string[3];

    public int Player_Deck;


    PlayFabManager playFabManager;
    MinionCard currentMinionCard;

    private void Awake()
    {
        playFabManager = GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>();
        firstDeckTabsButton.onClick.AddListener(SelectFirstDeck);
        secondDeckTabsButton.onClick.AddListener(SelectSecondDeck);
        thirdDeckTabsButton.onClick.AddListener(SelectThirdDeck);

        UpdateMinionInventoryFromFEN();
        SetupInventory();
    }

    public void UpdateMinionInventoryFromFEN()
    {
        if (PlayFabManager.Instance.Player_MinionInventory != null)
        {
            minionInventory = new List<MinionData>();
            string[] minionsInventoryString = PlayFabManager.Instance.Player_MinionInventory.Split('/');

            Player_Deck = int.Parse(minionsInventoryString[0]);

            List<string> deckOneList = new List<string>();
            List<string> deckTwoList = new List<string>();
            List<string> deckThirdList = new List<string>();

            foreach (var item in DataHolder.Instance.allMinions)
            {
                MinionData newMinionData = new MinionData();
                newMinionData.minion = item;
                newMinionData.minionKey = item.minionKey;
                newMinionData.level = 1;

                for (int i = 0; i < minionsInventoryString.Length; i++)
                {
                    if (minionsInventoryString[i].Contains(item.minionKey))
                    {
                        string[] minionDataString = minionsInventoryString[i].Split('.');
                        newMinionData.minionAmount = int.Parse(minionDataString[1]);
                        newMinionData.level = int.Parse(minionDataString[2]);

                        if (minionsInventoryString[i].Contains("DECK"))
                        {
                            if (minionsInventoryString[i].Contains("DECK1"))
                            {
                                newMinionData.deckIndex.Add(1);
                                deckOneList.Add(newMinionData.minionKey);
                            }

                            if (minionsInventoryString[i].Contains("DECK2"))
                            {
                                newMinionData.deckIndex.Add(2);
                                deckTwoList.Add(newMinionData.minionKey);
                            }

                            if (minionsInventoryString[i].Contains("DECK3"))
                            {
                                newMinionData.deckIndex.Add(3);
                                deckThirdList.Add(newMinionData.minionKey);
                            }

                        }
                    }
                }

                minionInventory.Add(newMinionData);
            }

            for (int i = 0; i < 3; i++)
            {
                if (deckOneList.Count > i)
                    firstDeck[i] = deckOneList[i];

                if (deckTwoList.Count > i)
                    secondDeck[i] = deckTwoList[i];

                if (deckThirdList.Count > i)
                    thirdDeck[i] = deckThirdList[i];
            }

            allDecks.Add(firstDeck);
            allDecks.Add(secondDeck);
            allDecks.Add(thirdDeck);
        }
    }

    public void SetupInventory()
    {
        switch (Player_Deck)
        {
            case 1:
                deckHolderOne.gameObject.SetActive(true);
                deckHolderTwo.gameObject.SetActive(false);
                deckHolderThree.gameObject.SetActive(false);
                break;
            case 2:
                deckHolderOne.gameObject.SetActive(false);
                deckHolderTwo.gameObject.SetActive(true);
                deckHolderThree.gameObject.SetActive(false);
                break;
            case 3:
                deckHolderOne.gameObject.SetActive(false);
                deckHolderTwo.gameObject.SetActive(false);
                deckHolderThree.gameObject.SetActive(true);
                break;
            default:
                break;
        }
        foreach (var item in minionInventory)
        {
            MinionCard newMinionCardInventory = null;
            List<MinionCard> newMinionCardDeck = new List<MinionCard>();

            switch (item.minion.rarety)
            {
                case Minion.MinonRarety.Common:

                    newMinionCardInventory = Instantiate(commonMinionCard);
                    for (int i = 0; i < item.deckIndex.Count; i++)
                        newMinionCardDeck.Add(Instantiate(commonMinionCard));

                    break;
                case Minion.MinonRarety.Rare:

                    newMinionCardInventory = Instantiate(rareMinionCard);
                    for (int i = 0; i < item.deckIndex.Count; i++)
                        newMinionCardDeck.Add(Instantiate(rareMinionCard));

                    break;
                case Minion.MinonRarety.Epic:

                    newMinionCardInventory = Instantiate(epicMinionCard);
                    for (int i = 0; i < item.deckIndex.Count; i++)
                        newMinionCardDeck.Add(Instantiate(epicMinionCard));

                    break;
                default:
                    break;
            }

            if (item.minionAmount > 0)
                newMinionCardInventory.InitiateCard(item, OwnedCardHolder.transform, MinionCard.CardType.Owned);
            else
                newMinionCardInventory.InitiateCard(item, notOwnedCardHolder.transform, MinionCard.CardType.NotOwned);

            for (int i = 0; i < item.deckIndex.Count; i++)
            {
                if (item.deckIndex[i] == 1)
                    newMinionCardDeck[i].InitiateCard(item, deckHolderOne, MinionCard.CardType.Deck, 1);
                if (item.deckIndex[i] == 2)
                    newMinionCardDeck[i].InitiateCard(item, deckHolderTwo, MinionCard.CardType.Deck, 2);
                if (item.deckIndex[i] == 3)
                    newMinionCardDeck[i].InitiateCard(item, deckHolderThree, MinionCard.CardType.Deck, 3);

                item.deckCard[Player_Deck - 1] = newMinionCardDeck[i];
            }

            item.inventoryCard = newMinionCardInventory;     
        }

        if (Player_Deck == 1)
            SelectFirstDeck();
        if (Player_Deck == 2)
            SelectSecondDeck();
        if (Player_Deck == 3)
            SelectThirdDeck();
    }

    public void SelectFirstDeck()
    {
        Player_Deck = 1;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if(i != 0)
                {
                    MinionData newData = null;
                    foreach (var data in minionInventory)
                        if (allDecks[i][j] == data.minionKey)
                        {
                            newData = data;
                            break;
                        }

                    if (newData != null)
                        if (!newData.deckIndex.Contains(Player_Deck))
                            newData.inventoryCard.ActiveEquipedBanner(false);
                }
                else
                {
                    MinionData newData = null;
                    foreach (var data in minionInventory)
                        if (allDecks[i][j] == data.minionKey)
                        {
                            newData = data;
                            break;
                        }

                    if (newData != null)
                        newData.inventoryCard.ActiveEquipedBanner(true);
                }
            }
        }

        if (currentMinionCard != null)
            currentMinionCard.CloseClickMenu();

        firstDeckTabsButton.GetComponent<Image>().color = selectedTabColor;
        secondDeckTabsButton.GetComponent<Image>().color = notSelectedTabColor;
        thirdDeckTabsButton.GetComponent<Image>().color = notSelectedTabColor;

        firstDeckTabsButton.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        secondDeckTabsButton.transform.localScale = Vector3.one;
        thirdDeckTabsButton.transform.localScale = Vector3.one;

        deckHolderOne.gameObject.SetActive(true);
        deckHolderTwo.gameObject.SetActive(false);
        deckHolderThree.gameObject.SetActive(false);

        currentMinionCard = null;
    }
    public void SelectSecondDeck()
    {
        Player_Deck = 2;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i != 1)
                {
                    MinionData newData = null;
                    foreach (var data in minionInventory)
                        if (allDecks[i][j] == data.minionKey)
                        {
                            newData = data;
                            break;
                        }

                    if (newData != null)
                      if (!newData.deckIndex.Contains(Player_Deck))
                            newData.inventoryCard.ActiveEquipedBanner(false);
                }
                else
                {
                    MinionData newData = null;
                    foreach (var data in minionInventory)
                        if (allDecks[i][j] == data.minionKey)
                        {
                            newData = data;
                            break;
                        }

                    if (allDecks[i][j] != null)
                        newData.inventoryCard.ActiveEquipedBanner(true);
                }
            }
        }

        if(currentMinionCard != null)
            currentMinionCard.CloseClickMenu();

        firstDeckTabsButton.GetComponent<Image>().color = notSelectedTabColor;
        secondDeckTabsButton.GetComponent<Image>().color = selectedTabColor;
        thirdDeckTabsButton.GetComponent<Image>().color = notSelectedTabColor;

        firstDeckTabsButton.transform.localScale = Vector3.one;
        secondDeckTabsButton.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        thirdDeckTabsButton.transform.localScale = Vector3.one;

        deckHolderOne.gameObject.SetActive(false);
        deckHolderTwo.gameObject.SetActive(true);
        deckHolderThree.gameObject.SetActive(false);

        currentMinionCard = null;
    }
    public void SelectThirdDeck()
    {
        Player_Deck = 3;


        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i != 2)
                {
                    MinionData newData = null;
                    foreach (var data in minionInventory)
                        if (allDecks[i][j] == data.minionKey)
                        {
                            newData = data;
                            break;
                        }

                    if (newData != null)
                        if (!newData.deckIndex.Contains(Player_Deck))
                            newData.inventoryCard.ActiveEquipedBanner(false);

                }
                else
                {
                    MinionData newData = null;
                    foreach (var data in minionInventory)
                        if (allDecks[i][j] == data.minionKey)
                        {
                            newData = data;
                            break;
                        }

                    if (newData != null)
                        newData.inventoryCard.ActiveEquipedBanner(true);
                }
            }
        }

        if (currentMinionCard != null)
            currentMinionCard.CloseClickMenu();

        firstDeckTabsButton.GetComponent<Image>().color = notSelectedTabColor;
        secondDeckTabsButton.GetComponent<Image>().color = notSelectedTabColor;
        thirdDeckTabsButton.GetComponent<Image>().color = selectedTabColor;

        firstDeckTabsButton.transform.localScale = Vector3.one;
        secondDeckTabsButton.transform.localScale = Vector3.one;
        thirdDeckTabsButton.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

        deckHolderOne.gameObject.SetActive(false);
        deckHolderTwo.gameObject.SetActive(false);
        deckHolderThree.gameObject.SetActive(true);

        currentMinionCard = null;
    }


    public void ChangeCurrentCard(MinionCard newCard)
    {
        if (currentMinionCard != null)
            currentMinionCard.CloseClickMenu();

        currentMinionCard = newCard;
    }


    public void OpenEquipMinionSelector(MinionData minionData)
    {
        equipMinionSelector.gameObject.SetActive(true);
        equipMinionSelector.SetUpCardCurrentDeck(minionData);
    }
    public void ChangeMinionInDeck(MinionData minionToChange)
    {
        equipMinionSelector.ChangeMinionInDeck(minionToChange);
    }
}
