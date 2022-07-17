using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using GleyDailyRewards;
using UnityEngine.UI;

public enum ChestType {Common, Rare, Epic, Legendary}

public class ChestLootManager : MonoBehaviour
{
    public static ChestLootManager instance;
    ChestLootManager()
    {
        instance = this;
    }

    public int GemAmountPerHour = 10;

    public TimerButtonScript firstLootButton;
    public TimerButtonScript secondLootButton;
    public TimerButtonScript thirdLootButton;
    public TimerButtonScript fourthLootButton;
    [Space(20)]
    public GameObject confirmPaidWindow;
    public Button confirmPaidButton;
    public Button cancelPaidButton;
    [Space(20)]
    [ShowInInspector]
    public Dictionary<string, LootChestProperty> lootChest = new Dictionary<string, LootChestProperty>();
    public List<LootChestProperty> ChestData = new List<LootChestProperty>();

    void Awake()
    {
        GleyDailyRewards.TimerButton.AddClickListener(RewardButtonClicked);
        Initiate();
    }

    void Initiate()
    {
        if (PlayFabManager.Instance.Player_ChestInventory != null)
        {
            string[] chestInventoryString = PlayFabManager.Instance.Player_ChestInventory.Split('/');

            for (int i = 0; i < 4; i++)
            {
                ChestData.Add(DataHolder.Instance.lootChestData[i]);
                ChestData[i].chestAmount = int.Parse(chestInventoryString[i]);
            }
        }

        if (PlayFabManager.Instance.Player_FirstLootSavedTime != null)
        {
            string[] firstLootString = PlayFabManager.Instance.Player_FirstLootSavedTime.Split('%');
            if (PlayFabManager.Instance.Player_FirstLootSavedTime.Contains("Common"))
                AddChestToLootButton(firstLootButton, ChestData[0], firstLootString[0]);
            if (PlayFabManager.Instance.Player_FirstLootSavedTime.Contains("Rare"))
                AddChestToLootButton(firstLootButton, ChestData[1], firstLootString[0]);
            if (PlayFabManager.Instance.Player_FirstLootSavedTime.Contains("Epic"))
                AddChestToLootButton(firstLootButton, ChestData[2], firstLootString[0]);
            if (PlayFabManager.Instance.Player_FirstLootSavedTime.Contains("Legendary"))
                AddChestToLootButton(firstLootButton, ChestData[3], firstLootString[0]);

        }
        if (PlayFabManager.Instance.Player_SecondLootSavedTime != null)
        {
            string[] secondLootString = PlayFabManager.Instance.Player_SecondLootSavedTime.Split('%');
            if (PlayFabManager.Instance.Player_SecondLootSavedTime.Contains("Common"))
                AddChestToLootButton(secondLootButton, ChestData[0], secondLootString[0]);
            if (PlayFabManager.Instance.Player_SecondLootSavedTime.Contains("Rare"))
                AddChestToLootButton(secondLootButton, ChestData[1], secondLootString[0]);
            if (PlayFabManager.Instance.Player_SecondLootSavedTime.Contains("Epic"))
                AddChestToLootButton(secondLootButton, ChestData[2], secondLootString[0]);
            if (PlayFabManager.Instance.Player_SecondLootSavedTime.Contains("Legendary"))
                AddChestToLootButton(secondLootButton, ChestData[3], secondLootString[0]);
        }
        if (PlayFabManager.Instance.Player_ThirdLootSavedTime != null)
        {
            string[] thirdLootString = PlayFabManager.Instance.Player_ThirdLootSavedTime.Split('%');
            if (PlayFabManager.Instance.Player_ThirdLootSavedTime.Contains("Common"))
               AddChestToLootButton(thirdLootButton, ChestData[0], thirdLootString[0]);
            if (PlayFabManager.Instance.Player_ThirdLootSavedTime.Contains("Rare"))
                AddChestToLootButton(thirdLootButton, ChestData[1], thirdLootString[0]);
            if (PlayFabManager.Instance.Player_ThirdLootSavedTime.Contains("Epic"))
                AddChestToLootButton(thirdLootButton, ChestData[2], thirdLootString[0]);
            if (PlayFabManager.Instance.Player_ThirdLootSavedTime.Contains("Legendary"))
                AddChestToLootButton(thirdLootButton, ChestData[3], thirdLootString[0]);
        }
        if (PlayFabManager.Instance.Player_FourthLootSavedTime != null)
        {
            string[] fourthLootString = PlayFabManager.Instance.Player_FourthLootSavedTime.Split('%');
            if (PlayFabManager.Instance.Player_FourthLootSavedTime.Contains("Common"))
                AddChestToLootButton(fourthLootButton, ChestData[0], fourthLootString[0]);
            if (PlayFabManager.Instance.Player_FourthLootSavedTime.Contains("Rare"))
               AddChestToLootButton(fourthLootButton, ChestData[1], fourthLootString[0]);
            if (PlayFabManager.Instance.Player_FourthLootSavedTime.Contains("Epic"))
               AddChestToLootButton(fourthLootButton, ChestData[2], fourthLootString[0]);
            if (PlayFabManager.Instance.Player_FourthLootSavedTime.Contains("Legendary"))
                AddChestToLootButton(fourthLootButton, ChestData[3], fourthLootString[0]);
        }
    }


    public void AddChestToLootButton(TimerButtonScript lootButton, LootChestProperty chestData, string savedTime = null)
    {
        lootButton.InitializeLootButton(chestData, savedTime);

        if (lootChest.ContainsKey(lootButton.buttonID.ToString()))
            lootChest[lootButton.buttonID.ToString()] = chestData;
        else
            lootChest.Add(lootButton.buttonID.ToString(), chestData);

        TimeMethods.SaveTime(lootButton.buttonID.ToString());

        PlayFabManager.Instance.CreateChestFENData();
    }

    public void RewardButtonClicked(TimerButtonIDs buttonID, bool timeExpired)
    {
        Debug.Log(buttonID + " Clicked -> Timer expired: " + timeExpired);
        if (timeExpired)
        {
            OpenLootChest(buttonID);
        }
        else
        {
            confirmPaidWindow.SetActive(true);
            Debug.Log("Wait " + GleyDailyRewards.TimerButton.GetRemainingTime(buttonID));
        }
    }

    void OpenPaidForOpen()
    {

    }
    void ConfirmPaidOpenLoot()
    {

    }
    void CancelPaidOpenLoot()
    {

    }

    public void OpenLootChest(TimerButtonIDs ButtonID)
    {
        if (lootChest.ContainsKey(ButtonID.ToString()))
        {
            Debug.Log("OpenChest ! " + lootChest[ButtonID.ToString()].chestType.ToString());
            lootChest[ButtonID.ToString()].GetReward();
            lootChest.Remove(ButtonID.ToString());

            switch (ButtonID)
            {
                case TimerButtonIDs.LootOne:
                    PlayFabManager.Instance.Player_FirstLootSavedTime = null;
                    firstLootButton.chest = null;
                    ChestData[0].chestAmount--;
                    break;
                case TimerButtonIDs.LootTwo:
                    PlayFabManager.Instance.Player_SecondLootSavedTime = null;
                    secondLootButton.chest = null;
                    ChestData[1].chestAmount--;
                    break;
                case TimerButtonIDs.LootThree:
                    PlayFabManager.Instance.Player_ThirdLootSavedTime = null;
                    thirdLootButton.chest = null;
                    ChestData[2].chestAmount--;
                    break;
                case TimerButtonIDs.LootFour:
                    PlayFabManager.Instance.Player_FourthLootSavedTime = null;
                    fourthLootButton.chest = null;
                    ChestData[3].chestAmount--;
                    break;
                default:
                    break;
            }

            PlayFabManager.Instance.CreateChestFENData();
        }
    }
}

[System.Serializable]
public class LootChestProperty
{
    public int chestAmount;

    public Sprite chestSprite;

    [PropertySpace(0, 20)]
    public ChestType chestType;


    [Range(0,100)]
    public int chanceToLootCoin;
    public List<LootData> coinLoots;

    [PropertySpace(20, 0)]
    public int oneMinonAmount = 30;
    public int twoMinonAmount = 15;
    public int threeMinonAmount = 10;
    [PropertySpace(0, 20)]
    public List<LootData> minionLoot;

    [BoxGroup("Time")]
    public int Hour, Minute, Second;

    public void GetReward()
    {

        LootData coinLoot = null;
        if (chanceToLootCoin < UnityEngine.Random.Range(0, 100))
        {
            coinLoot = OutCoinLoot();
            Debug.Log("Win Coins : " + coinLoot.value);
            PlayFabManager.Instance.AddCoin(coinLoot.value);
        }
           

        List<LootData> minionLoot = OutMinionLoot(UnityEngine.Random.Range(1, 4));

        int amountMinion = 0;
        switch (minionLoot.Count)
        {
            case 1: amountMinion = oneMinonAmount; break;
            case 2: amountMinion = twoMinonAmount; break;
            case 3: amountMinion = threeMinonAmount; break;
            default: break;
        }
        foreach (var item in minionLoot)
        {
            for (int i = 0; i < MinionInventory.Instance.minionInventory.Count; i++)
            {
                if (MinionInventory.Instance.minionInventory[i].minionKey == item.minionKey)
                {
                    MinionInventory.Instance.minionInventory[i].minionAmount += amountMinion;
                    MinionInventory.Instance.minionInventory[i].inventoryCard.UpdateCard();
                    for (int j = 0; j < 3; j++)
                    {
                        if (MinionInventory.Instance.minionInventory[i].deckCard[j] != null)
                            MinionInventory.Instance.minionInventory[i].deckCard[j].UpdateCard();
                    }

                    Debug.Log("Win Minon : " + item.minionKey);
                }
            }
             
        }

        PlayFabManager.Instance.CreateMinionFENData();
    }


    LootData OutCoinLoot()
    {
        int totalWeight = 0;
        foreach (var item in coinLoots)
            totalWeight += item.chance;

        float randomInt = UnityEngine.Random.Range(0, totalWeight);
        foreach (var item in coinLoots)
        {
            if (item.chance >= randomInt)
                return item;

            randomInt -= item.chance;
        }
        
        throw new SystemException("Loot Failed !");
    }
    List<LootData> OutMinionLoot(int amount)
    {
        List<LootData> minionLootRemaining = new List<LootData>();
        foreach (var item in minionLoot)
            minionLootRemaining.Add(item);

        List<LootData> minionLooted = new List<LootData>();

        for (int i = 0; i < amount; i++)
        {
            LootData chossenMinion = GetMinionLoot(minionLootRemaining);
            minionLooted.Add(chossenMinion);
            minionLootRemaining.Remove(chossenMinion);
        }

        return minionLooted;
    }
    LootData GetMinionLoot(List<LootData> minoinList) 
    {
        int totalWeight = 0;
        foreach (var item in minoinList)
            totalWeight += item.chance;

        float randomInt = UnityEngine.Random.Range(0, totalWeight);
        foreach (var item in minionLoot)
        {
            if (item.chance >= randomInt)
                return item;

            randomInt -= item.chance;
        }

        throw new SystemException("Loot Failed !");
    }


    [System.Serializable]
    public class LootData
    {
        public string minionKey;

        [Space(10)]
        public int value;
        public int chance;
    }

}