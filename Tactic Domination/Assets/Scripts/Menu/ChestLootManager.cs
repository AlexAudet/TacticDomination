using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using GleyDailyRewards;

public enum ChestType {Common, Rare, Epic, Legendary}

public class ChestLootManager : MonoBehaviour
{
    public static ChestLootManager instance;
    ChestLootManager()
    {
        instance = this;
    }

    public TimerButtonScript firstLootButton;
    public TimerButtonScript secondLootButton;
    public TimerButtonScript thirdLootButton;
    public TimerButtonScript fourthLootButton;


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


    [Button]
    public void AddChestToLootButton(TimerButtonScript lootButton, LootChestProperty chestData, string savedTime = null)
    {
        lootButton.InitializeLootButton(chestData, savedTime);

        if (lootChest.ContainsKey(lootButton.buttonID.ToString()))
            lootChest[lootButton.buttonID.ToString()] = chestData;
        else
            lootChest.Add(lootButton.buttonID.ToString(), chestData);

        if(savedTime == null)
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
             Debug.Log("Wait " + GleyDailyRewards.TimerButton.GetRemainingTime(buttonID));
        }
    }

    public void OpenLootChest(TimerButtonIDs ButtonID)
    {
        if (lootChest.ContainsKey(ButtonID.ToString()))
        {
            Debug.Log("OpenChest ! " + lootChest[ButtonID.ToString()].chestType.ToString());
            lootChest.Remove(ButtonID.ToString());

            switch (ButtonID)
            {
                case TimerButtonIDs.LootOne:
                    PlayFabManager.Instance.Player_FirstLootSavedTime = null;
                    firstLootButton.chest = null;
                    break;
                case TimerButtonIDs.LootTwo:
                    PlayFabManager.Instance.Player_SecondLootSavedTime = null;
                    secondLootButton.chest = null;
                    break;
                case TimerButtonIDs.LootThree:
                    PlayFabManager.Instance.Player_ThirdLootSavedTime = null;
                    thirdLootButton.chest = null;
                    break;
                case TimerButtonIDs.LootFour:
                    PlayFabManager.Instance.Player_FourthLootSavedTime = null;
                    fourthLootButton.chest = null;
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
    public ChestType chestType;

    [PropertySpace(0, 10)]
    public int Hour, Minute, Second;

    public void GetReward()
    {
        Debug.Log("You looted a " + chestType.ToString() + " chest");


    }
}