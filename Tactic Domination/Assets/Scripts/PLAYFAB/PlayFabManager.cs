using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance = null;

    TextMeshProUGUI textMessage;
    GameObject panel;

    [SerializeField]
    int loadingTimeOut = 3;
    public int initialMaxExp = 1000;
    [ReadOnly]
    public int maxExp = 10;

    [PropertySpace(20,0)]
    public string Player_Id, Player_DisplayName;
    public int Player_Level, Player_Exp, Player_Rank, Player_Coin, Player_Gem;
    public string Player_MinionInventory, Player_ChestInventory;
    public string Player_FirstLootSavedTime, Player_SecondLootSavedTime, Player_ThirdLootSavedTime, Player_FourthLootSavedTime;


    MinionData currentMinonToUpgrade;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        panel = transform.Find("Panel").gameObject;
        textMessage = panel.GetComponentInChildren<TextMeshProUGUI>();
    }


    public void ShowLoading()
    {
        if (!panel.activeInHierarchy)
        {
            panel.SetActive(true);
        }
    }
    public void HideLoading()
    {
        StartCoroutine(Timer());
    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(loadingTimeOut);
        panel.SetActive(false);
    }
    public void LoadingMessage(string msg)
    {
        ShowLoading();
        textMessage.text = msg;
    }





    [Button]
    public void AddExperience(int experienceAmount = 0)
    {
        int totalExp = Player_Exp + experienceAmount;
        int totalLevel = Player_Level;
        if (totalExp >= maxExp)
        {
            while (totalExp >= maxExp)
            {
                totalLevel++;

                totalExp -= maxExp;

                float floatMaxExp = initialMaxExp;
                maxExp = initialMaxExp;

                for (int i = 1; i < Player_Level; i++)
                    maxExp += Mathf.FloorToInt(floatMaxExp * 0.1f);
            }
        }

        var requestExp = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {StatisticName = "Exp", Value = totalExp},
                new StatisticUpdate {StatisticName = "Level", Value = totalLevel}
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(requestExp, ExpSuccessUpdate, ExpFailedUpdate);
    }
    private void ExpFailedUpdate(PlayFabError error)
    {
        LoadingMessage(error.ErrorMessage);
        HideLoading();
    }
    private void ExpSuccessUpdate(UpdatePlayerStatisticsResult success)
    {
        GetExperienceData();
    }
    private void GetExperienceData()
    {
        var request = new GetPlayerStatisticsRequest();
        PlayFabClientAPI.GetPlayerStatistics(request, SuccesGetExpData, FailedGetExpData);
    }
    private void FailedGetExpData(PlayFabError error)
    {
        LoadingMessage(error.ErrorMessage);
        HideLoading();
    }
    private void SuccesGetExpData(GetPlayerStatisticsResult result)
    {
        Player_Exp = result.Statistics[0].Value;
        Player_Level = result.Statistics[1].Value;

        MenuManager.Instance.UpdateStat();
    }


    [Button]
    public void UpdateRank(int rankPointsToAdd = 0)
    {
       
        var requestExp = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {StatisticName = "Rank", Value =  Player_Rank += rankPointsToAdd}
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(requestExp, RankUpdateSuccess, RankUpdateFailed);
    }
    private void RankUpdateFailed(PlayFabError error)
    {
        LoadingMessage(error.ErrorMessage);
        HideLoading();
    }
    private void RankUpdateSuccess(UpdatePlayerStatisticsResult obj)
    {
        MenuManager.Instance.UpdateStat();
    }



    public void UpgradeMinion(MinionData minionData)
    {
        currentMinonToUpgrade = minionData;

        var request = new SubtractUserVirtualCurrencyRequest()
        {
            VirtualCurrency = "CO",
            Amount = DataHolder.Instance.minionAmountToLevelUp[currentMinonToUpgrade.level - 1].coinCost
        };

        PlayFabClientAPI.SubtractUserVirtualCurrency(request, SubstractCoinForMinionUpdateSuccess, SubstractCoinForMinionUpdateFailed);
    }
    private void SubstractCoinForMinionUpdateFailed(PlayFabError error)
    {
        LoadingMessage(error.ErrorMessage);
        HideLoading();
    }
    private void SubstractCoinForMinionUpdateSuccess(ModifyUserVirtualCurrencyResult result)
    {
        Player_Coin = result.Balance;
        currentMinonToUpgrade.minionAmount -= DataHolder.Instance.minionAmountToLevelUp[currentMinonToUpgrade.level - 1].minionAmount;
        currentMinonToUpgrade.level++;

        currentMinonToUpgrade.inventoryCard.UpdateCard();
        for (int i = 0; i < 3; i++) 
            if (currentMinonToUpgrade.deckCard[i] != null)
                currentMinonToUpgrade.deckCard[i].UpdateCard();

        MenuManager.Instance.UpdateStat();

        CreateMinionFENData();
    }



    public void AddCoin(int coinAmount)
    {
        var requestCoin = new AddUserVirtualCurrencyRequest()
        {
            VirtualCurrency = "CO",
            Amount = coinAmount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(requestCoin, UpdateCoinSucces, UpdateCoinFailed);
    }
    private void UpdateCoinFailed(PlayFabError error)
    {
        LoadingMessage(error.ErrorMessage);
        HideLoading();
    }
    private void UpdateCoinSucces(ModifyUserVirtualCurrencyResult result)
    {
        Player_Coin = result.Balance;
        HideLoading();
        MenuManager.Instance.UpdateStat();
    }


    public void AddGem(int gemAmount)
    {
        var requestCoin = new AddUserVirtualCurrencyRequest()
        {
            VirtualCurrency = "GM",
            Amount = gemAmount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(requestCoin, UpdateGemSucces, UpdateGemFailed);
    }
    private void UpdateGemFailed(PlayFabError error)
    {
        LoadingMessage(error.ErrorMessage);
        HideLoading();
    }
    private void UpdateGemSucces(ModifyUserVirtualCurrencyResult result)
    {
        Player_Gem = result.Balance;
        HideLoading();
        MenuManager.Instance.UpdateStat();
    }




    public void CreateMinionFENData()
    {
        string newMinionInventoryData = MinionInventory.Instance.Player_Deck.ToString() + "/";

        foreach (var item in MinionInventory.Instance.minionInventory)
        {
            newMinionInventoryData += item.minionKey;
            newMinionInventoryData += ("." + item.minionAmount.ToString());
            newMinionInventoryData += ("." + item.level.ToString());

            if (item.deckIndex.Count > 0)
                foreach (var deck in item.deckIndex)
                {
                    newMinionInventoryData += ".DECK" + deck.ToString();
                }


            newMinionInventoryData += "/";
        }

        newMinionInventoryData.Remove(newMinionInventoryData.Length - 1);
        WriteMinionData(newMinionInventoryData);   
    }
    public void CreateChestFENData()
    {

        string chestData = ChestLootManager.instance.ChestData[0].chestAmount.ToString() 
            + "/" + ChestLootManager.instance.ChestData[1].chestAmount.ToString() 
            + "/" + ChestLootManager.instance.ChestData[2].chestAmount.ToString() 
            + "/" + ChestLootManager.instance.ChestData[3].chestAmount.ToString();


        if (ChestLootManager.instance.firstLootButton.chest != null)
            Player_FirstLootSavedTime = GleyDailyRewards.TimeMethods.LoadTime
                (ChestLootManager.instance.firstLootButton.buttonID.ToString()).ToBinary() + "%"
                + ChestLootManager.instance.firstLootButton.chest.chestType.ToString();
        //  else
        //      Player_FirstLootSavedTime = null;

        if (ChestLootManager.instance.secondLootButton.chest != null)
            Player_SecondLootSavedTime = GleyDailyRewards.TimeMethods.LoadTime
                (ChestLootManager.instance.secondLootButton.buttonID.ToString()).ToBinary() + "%"
                + ChestLootManager.instance.secondLootButton.chest.chestType.ToString();
        //  else
        //      Player_SecondLootSavedTime = null;

        if (ChestLootManager.instance.thirdLootButton.chest != null)
            Player_ThirdLootSavedTime = GleyDailyRewards.TimeMethods.LoadTime
                (ChestLootManager.instance.thirdLootButton.buttonID.ToString()).ToBinary() + "%"
                + ChestLootManager.instance.thirdLootButton.chest.chestType.ToString();
        //  else
        //      Player_ThirdLootSavedTime = null;

        if (ChestLootManager.instance.fourthLootButton.chest != null)
            Player_FourthLootSavedTime = GleyDailyRewards.TimeMethods.LoadTime
                (ChestLootManager.instance.fourthLootButton.buttonID.ToString()).ToBinary() + "%"
                + ChestLootManager.instance.fourthLootButton.chest.chestType.ToString();
        // else
        //     Player_FourthLootSavedTime = null;


        WriteChestData(chestData);

    }

    void WriteMinionData(string fen)
    {
        if (fen != null)
            Player_MinionInventory = fen;

        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
                {
                    {"Minion",Player_MinionInventory},
                }
        };

        PlayFabClientAPI.UpdateUserData(request, WriteDataSuccess, WriteDataFailed);


    }

    public void WriteChestData(string fen)
    {
        if (fen != null)
            Player_ChestInventory = fen;

        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"Chest",Player_ChestInventory},
                {"LootOne",Player_FirstLootSavedTime},
                {"LootTwo",Player_SecondLootSavedTime},
                {"LootThree",Player_ThirdLootSavedTime},
                {"LootFour",Player_FourthLootSavedTime}
            }
        };

        PlayFabClientAPI.UpdateUserData(request, WriteDataSuccess, WriteDataFailed);
    }
    private void WriteDataFailed(PlayFabError error)
    {
        LoadingMessage(error.ErrorMessage);
        HideLoading();
    }
    private void WriteDataSuccess(UpdateUserDataResult result)
    {

        LoadingMessage("Updateting Data...");
        HideLoading();
    }
}

[System.Serializable]
public class MinionData 
{
    public Minion minion;
    public MinionCard[] deckCard = new MinionCard[3];
    public MinionCard inventoryCard;
    public string minionKey;
    public int level;
    public int minionAmount;
    public List<int> deckIndex = new List<int>();
}


