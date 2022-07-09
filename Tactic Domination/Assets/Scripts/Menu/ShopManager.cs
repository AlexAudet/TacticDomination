using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    ShopManager()
    {
        Instance = this;
    }
    PlayFabManager playFabManager;
    DataHolder data;

    public List<ShopItemData> coinItemShop = new List<ShopItemData>();



    [Space(30)]
    public List<ShopItemData> minionItemShop = new List<ShopItemData>();


    [HideInInspector]
    public List<MinionData> commonMinionAvailable = new List<MinionData>();
    [HideInInspector]
    public List<MinionData> rareMinionAvailable = new List<MinionData>();
    [HideInInspector]
    public List<MinionData> epicMinionAvailable = new List<MinionData>();

    void Start()
    {
        playFabManager = GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>();
        data = playFabManager.GetComponent<DataHolder>();
        InitiateShop();
    }

    public void InitiateShop()
    {
        commonMinionAvailable = new List<MinionData>();
        rareMinionAvailable = new List<MinionData>();
        epicMinionAvailable = new List<MinionData>();

        foreach (var item in MinionInventory.Instance.minionInventory)
        {
            if (item.minionAmount > 0)
            {
                switch (item.minion.rarety)
                {
                    case Minion.MinonRarety.Common:
                        commonMinionAvailable.Add(item);
                        break;
                    case Minion.MinonRarety.Rare:
                        rareMinionAvailable.Add(item);
                        break;
                    case Minion.MinonRarety.Epic:
                        epicMinionAvailable.Add(item);
                        break;
                    default:
                        break;
                }
            }
        }

        foreach (var item in coinItemShop)
        {
            item.Initiate();
        }

        foreach (var item in minionItemShop)
        {
            item.Initiate(true);
        }         
    }
}

[System.Serializable]
public class ShopItemData
{
    Button button;
    public TextMeshProUGUI costText;
    public Transform cardHolder;

    [PropertySpace(10,0)]
    public enum CurrencyType {Coin,Gem,Money,Ads}
    public CurrencyType currencyType;
    public enum ItemType { Coin, Gem, Minion}
    public ItemType itemType;

    [ShowIf("@this.currencyType == CurrencyType.Money")]
    public float moneyCost;
    [ShowIf("@this.currencyType == CurrencyType.Coin")]
    public int coinCost;
    [ShowIf("@this.currencyType == CurrencyType.Gem")]
    public int gemCost;

    [HideInInspector]
    public string key;
    public int amount;

    [Space(10)]
    [ShowIf("@this.itemType == ItemType.Minion")]
    public int commonMinionChance = 50;
    [ShowIf("@this.itemType == ItemType.Minion")]
    public List<MinionOffer> commonMinionOffer = new List<MinionOffer>();

    [Space(10)]
    [ShowIf("@this.itemType == ItemType.Minion")]
    public int rareMinionChance = 35;
    [ShowIf("@this.itemType == ItemType.Minion")]
    public List<MinionOffer> rareMinionOffer = new List<MinionOffer>();

    [Space(10)]
    [ShowIf("@this.itemType == ItemType.Minion")]
    public int epicMinionChance = 15;
    [ShowIf("@this.itemType == ItemType.Minion")]
    public List<MinionOffer> epicMinionOffer = new List<MinionOffer>();


    public void Initiate(bool minion = false)
    {
        if(minion == true)
        {
            foreach (Transform child in cardHolder.transform)
                GameObject.Destroy(child.gameObject);

            MinionCard newCard = null;

            MinionData data;
            int random = UnityEngine.Random.Range(0, 100);

            if (random > 100 - epicMinionChance && ShopManager.Instance.epicMinionAvailable.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, ShopManager.Instance.epicMinionAvailable.Count);
                data = ShopManager.Instance.epicMinionAvailable[randomIndex];
                newCard = GameObject.Instantiate(MinionInventory.Instance.epicMinionCard, cardHolder);
                int whichOffer = UnityEngine.Random.Range(0, epicMinionOffer.Count);
                coinCost = epicMinionOffer[whichOffer].cost;
                amount = epicMinionOffer[whichOffer].amount;

                ShopManager.Instance.epicMinionAvailable.RemoveAt(randomIndex);
            }
            else if (random > 100 - rareMinionChance + epicMinionChance && ShopManager.Instance.rareMinionAvailable.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, ShopManager.Instance.rareMinionAvailable.Count);
                data = ShopManager.Instance.rareMinionAvailable[randomIndex];
                newCard = GameObject.Instantiate(MinionInventory.Instance.rareMinionCard, cardHolder);
                int whichOffer = UnityEngine.Random.Range(0, rareMinionOffer.Count);
                coinCost = rareMinionOffer[whichOffer].cost;
                amount = rareMinionOffer[whichOffer].amount;

                ShopManager.Instance.rareMinionAvailable.RemoveAt(randomIndex);
            }
            else if (ShopManager.Instance.commonMinionAvailable.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, ShopManager.Instance.commonMinionAvailable.Count);
                data = ShopManager.Instance.commonMinionAvailable[randomIndex];
                newCard = GameObject.Instantiate(MinionInventory.Instance.commonMinionCard, cardHolder);
                int whichOffer = UnityEngine.Random.Range(0, commonMinionOffer.Count);
                coinCost = commonMinionOffer[whichOffer].cost;
                amount = commonMinionOffer[whichOffer].amount;

                ShopManager.Instance.commonMinionAvailable.RemoveAt(randomIndex);
            }
            else
            {
                Initiate(true);
                return;
            }

            key = data.minionKey;

            newCard.InitiateCard(data, cardHolder, MinionCard.CardType.Shop, shopAmount:amount);


            newCard.GetComponent<Button>().onClick.AddListener(BuyItem);
        }

        switch (currencyType)
        {
            case CurrencyType.Coin:
                costText.text = coinCost.ToString();
                break;
            case CurrencyType.Gem:
                costText.text = gemCost.ToString();
                break;
            case CurrencyType.Money:
                costText.text = moneyCost.ToString();
                break;
            case CurrencyType.Ads:
                costText.text = "Free!";
                break;
            default:
                break;
        }
    }

    public void BuyItem()
    {
        if (itemType == ShopItemData.ItemType.Minion && !DataHolder.Instance.minonPrefabDictionary.ContainsKey(key))
            Debug.Log("There is no minion with this key : " + key);


        switch (currencyType)
        {
            case ShopItemData.CurrencyType.Coin:

                if (PlayFabManager.Instance.Player_Coin < coinCost)
                    return;
                else
                {
                    var request = new SubtractUserVirtualCurrencyRequest()
                    {
                        VirtualCurrency = "CO",
                        Amount = coinCost
                    };

                    PlayFabClientAPI.SubtractUserVirtualCurrency(request, UpdateSubtractCoinSuccess, UpdateSubtractCoinFailed);

                }

                break;
            case ShopItemData.CurrencyType.Gem:

                if (PlayFabManager.Instance.Player_Gem < gemCost)
                    return;
                else
                {
                    var request = new SubtractUserVirtualCurrencyRequest()
                    {
                        VirtualCurrency = "GM",
                        Amount = gemCost
                    };

                    PlayFabClientAPI.SubtractUserVirtualCurrency(request, UpdateSubtractGemSuccess, UpdateSubtractGemFailed);

                }

                break;
            case ShopItemData.CurrencyType.Money:


                break;
            default:
                break;
        }
    }

    private void UpdateSubtractGemFailed(PlayFabError error)
    {
        PlayFabManager.Instance.LoadingMessage(error.ErrorMessage);
        PlayFabManager.Instance.HideLoading();
    }
    private void UpdateSubtractGemSuccess(ModifyUserVirtualCurrencyResult result)
    {
        PlayFabManager.Instance.Player_Gem = result.Balance;
        ConfirmBuy();
        MenuManager.Instance.UpdateStat();
    }
    private void UpdateSubtractCoinFailed(PlayFabError error)
    {
        PlayFabManager.Instance.LoadingMessage(error.ErrorMessage);
        PlayFabManager.Instance.HideLoading();
    }
    private void UpdateSubtractCoinSuccess(ModifyUserVirtualCurrencyResult result)
    {
        PlayFabManager.Instance.Player_Coin = result.Balance;
        ConfirmBuy();
        MenuManager.Instance.UpdateStat();
    }

    void ConfirmBuy()
    {
        switch (itemType)
        {
            case ShopItemData.ItemType.Coin:

                var requestCoin = new AddUserVirtualCurrencyRequest()
                {
                    VirtualCurrency = "CO",
                    Amount = amount
                };

                PlayFabClientAPI.AddUserVirtualCurrency(requestCoin, UpdateAddCoinSuccess, UpdateAddCoinFailed);

                break;
            case ShopItemData.ItemType.Gem:

                var requestGem = new AddUserVirtualCurrencyRequest()
                {
                    VirtualCurrency = "GM",
                    Amount = amount
                };

                PlayFabClientAPI.AddUserVirtualCurrency(requestGem, UpdateAddGemSuccess, UpdateAddGemFailed);

                break;
            case ShopItemData.ItemType.Minion:

                for (int i = 0; i < MinionInventory.Instance.minionInventory.Count; i++)
                    if (MinionInventory.Instance.minionInventory[i].minionKey == key)
                    {
                        MinionInventory.Instance.minionInventory[i].minionAmount += amount;
                        MinionInventory.Instance.minionInventory[i].inventoryCard.UpdateCard();
                        for (int j = 0; j < 3; j++)
                        {
                            if (MinionInventory.Instance.minionInventory[i].deckCard[j] != null)
                                MinionInventory.Instance.minionInventory[i].deckCard[j].UpdateCard();
                        }

                    }


                break;
            default:
                break;
        }

        PlayFabManager.Instance.CreateMinionFENData();
    }

    private void UpdateAddCoinFailed(PlayFabError error)
    {
        PlayFabManager.Instance.LoadingMessage(error.ErrorMessage);
        PlayFabManager.Instance.HideLoading();
    }
    private void UpdateAddCoinSuccess(ModifyUserVirtualCurrencyResult obj)
    {
        PlayFabManager.Instance.Player_Coin += amount;
        MenuManager.Instance.UpdateStat();
    }
    private void UpdateAddGemFailed(PlayFabError error)
    {
        PlayFabManager.Instance.LoadingMessage(error.ErrorMessage);
        PlayFabManager.Instance.HideLoading();
    }
    private void UpdateAddGemSuccess(ModifyUserVirtualCurrencyResult obj)
    {
        PlayFabManager.Instance.Player_Gem += amount;
        MenuManager.Instance.UpdateStat();
    }
}


[System.Serializable]
public class MinionOffer
{
    public int amount;
    public int cost;
}
