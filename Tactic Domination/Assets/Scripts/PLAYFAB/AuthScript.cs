
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;
using TMPro;
using Sirenix.OdinInspector;
using System;

public class AuthScript : MonoBehaviour
{
    [SerializeField]
    TMP_InputField ifEmail, ifPassword, ifPseudo;

    [SerializeField]
    PlayFabManager playFabManager;
    public void RegisterPlayer()
    {
        playFabManager.LoadingMessage("Connecting to server...");

        var request = new RegisterPlayFabUserRequest()
        {
            Email = ifEmail.text,
            Password = ifPassword.text,
            DisplayName = ifPseudo.text,
            Username = ifPseudo.text
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, Success, Failed);
    }
    private void Failed(PlayFabError error)
    {
        playFabManager.LoadingMessage(error.ErrorMessage);
        playFabManager.HideLoading();
    }
    private void Success(RegisterPlayFabUserResult success)
    {
        playFabManager.LoadingMessage("Initialize Data");
        InitData();
    }


    private void InitData()
    {
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate{StatisticName = "Exp", Value = 0},
                new StatisticUpdate{StatisticName = "Level", Value = 1},             
                new StatisticUpdate{StatisticName = "Rank", Value = UnityEngine.Random.Range(100,1000)},
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, InitDataSuccess, InitDataFailed);
    }
    private void InitDataFailed(PlayFabError error)
    {
        playFabManager.LoadingMessage(error.ErrorMessage);
        playFabManager.HideLoading();
    }
    private void InitDataSuccess(UpdatePlayerStatisticsResult result)
    {
        playFabManager.LoadingMessage("Initialize Inventory...");
        InitInventory();
    }



    private void InitInventory()
    { 
    
        playFabManager.Player_MinionInventory = "1/AAA.1.1.DECK1/BBB.1.1.DECK1/CCC.1.1/DDD.1.1.DECK1/EEE.1.1/FFF.1.1.DECK2/HHH.1.1.DECK2/III.1.1/JJJ.1.1.DECK2";
        playFabManager.Player_ChestInventory = "2/2/1/1";
        playFabManager.Player_FirstLootSavedTime = "";
        playFabManager.Player_SecondLootSavedTime = "";
        playFabManager.Player_ThirdLootSavedTime = "";
        playFabManager.Player_FourthLootSavedTime = "";

        var requestInventory = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>
            {
                {"Minion",playFabManager.Player_MinionInventory},
                {"Chest",playFabManager.Player_ChestInventory},
                {"LootOne",playFabManager.Player_FirstLootSavedTime},
                {"LootTwo",playFabManager.Player_SecondLootSavedTime},
                {"LootThree",playFabManager.Player_ThirdLootSavedTime},
                {"LootFour",playFabManager.Player_FourthLootSavedTime}
            }
        };

        PlayFabClientAPI.UpdateUserData(requestInventory, InitInventorySuccess, InitInventoryFailed);
    }
    private void InitInventoryFailed(PlayFabError error)
    {
        playFabManager.LoadingMessage(error.ErrorMessage);
        playFabManager.HideLoading();
    }
    private void InitInventorySuccess(UpdateUserDataResult result)
    {
        playFabManager.LoadingMessage("Register Successfull");
        playFabManager.HideLoading();
        LoginPlayer();
    }





    public void LoginPlayer()
    {
        playFabManager.LoadingMessage("Connecting to server...");

        var request = new LoginWithEmailAddressRequest()
        {
            Password = ifPassword.text,
            Email = ifEmail.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, LoginSuccess, LoginFailed);
    }
    private void LoginFailed(PlayFabError error)
    {
        playFabManager.LoadingMessage(error.ErrorMessage);
        playFabManager.HideLoading();
    }
    private void LoginSuccess(LoginResult success)
    {
        playFabManager.Player_Id = success.PlayFabId;
        playFabManager.LoadingMessage("Loading DiplayName...");
        getPlayerName();
    }



    void getPlayerName()
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, InfoSucces, InfoFailed);
    }
    private void InfoFailed(PlayFabError error)
    {
        playFabManager.LoadingMessage(error.ErrorMessage);
        playFabManager.HideLoading();
    }
    private void InfoSucces(GetAccountInfoResult result)
    {
        playFabManager.Player_DisplayName = result.AccountInfo.Username;
        playFabManager.LoadingMessage("Loading Data...");
        ReadData();
    }



    private void ReadData()
    {
        var request = new GetPlayerStatisticsRequest();
        PlayFabClientAPI.GetPlayerStatistics(request, SuccesData, FailedData);
    }
    private void FailedData(PlayFabError error)
    {
        playFabManager.LoadingMessage(error.ErrorMessage);
        playFabManager.HideLoading();
    }
    private void SuccesData(GetPlayerStatisticsResult result)
    {
        playFabManager.Player_Exp = result.Statistics[0].Value;
        playFabManager.Player_Level = result.Statistics[1].Value;
        playFabManager.Player_Rank = result.Statistics[2].Value;

        playFabManager.maxExp = playFabManager.initialMaxExp;
        for (int i = 1; i < playFabManager.Player_Level; i++)
        {
            playFabManager.maxExp += Mathf.FloorToInt(playFabManager.maxExp * 0.1f);
        }

        playFabManager.LoadingMessage("Loading Currency...");
        GetBalanceCurrency();
    }



    void GetBalanceCurrency()
    {
        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, currencySuccess, currencyFailed);
   
    }
    private void currencyFailed(PlayFabError error)
    {
        playFabManager.LoadingMessage(error.ErrorMessage);
        playFabManager.HideLoading();
    }
    private void currencySuccess(GetUserInventoryResult result)
    {
        foreach (var item in result.VirtualCurrency)
        {
            if (item.Key == "CO")
            {
                playFabManager.Player_Coin = item.Value;
            }
            if (item.Key == "GM")
            {
                playFabManager.Player_Gem = item.Value;
            }
        }
        GetInventory();
    }




    void GetInventory()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, successInventory, failedInventory);
    }
    private void failedInventory(PlayFabError error)
    {
        playFabManager.LoadingMessage(error.ErrorMessage);
        playFabManager.HideLoading();
    }
    private void successInventory(GetUserDataResult result)
    {
        if (result.Data.ContainsKey("Minion"))
            playFabManager.Player_MinionInventory = result.Data["Minion"].Value;

        if (result.Data.ContainsKey("Chest"))
            playFabManager.Player_ChestInventory = result.Data["Chest"].Value;

        if (result.Data.ContainsKey("LootOne"))
            playFabManager.Player_FirstLootSavedTime = result.Data["LootOne"].Value;

        if (result.Data.ContainsKey("LootTwo"))
            playFabManager.Player_SecondLootSavedTime = result.Data["LootTwo"].Value;

        if (result.Data.ContainsKey("LootThree"))
            playFabManager.Player_ThirdLootSavedTime = result.Data["LootThree"].Value;

        if (result.Data.ContainsKey("LootFour"))
            playFabManager.Player_FourthLootSavedTime = result.Data["LootFour"].Value;

        playFabManager.LoadingMessage("Login Succesfull");
        playFabManager.HideLoading();
        LoadMenu();
    }
    

    void LoadMenu()
    {
        playFabManager.HideLoading();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
