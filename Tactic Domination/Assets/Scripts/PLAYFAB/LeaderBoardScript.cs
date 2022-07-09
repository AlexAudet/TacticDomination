using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class LeaderBoardScript : MonoBehaviour
{

    [SerializeField]
    GameObject rankHolder;
    PlayFabManager playFabManager;

    private void Awake()
    {
        playFabManager = GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>();
        playFabManager.LoadingMessage("Loading LeaderBoard...");
        ReadLeaderBoard();
    }

    void ReadLeaderBoard() 
    { 
        var request = new GetLeaderboardRequest()
        {
            StatisticName = "Rank",
            StartPosition = 0,
            MaxResultsCount = 20
        };

        PlayFabClientAPI.GetLeaderboard(request, LeaderBoardSuccess, LeaderBoardFail);
    }

    private void LeaderBoardFail(PlayFabError error)
    {
        playFabManager.LoadingMessage(error.ErrorMessage);
        playFabManager.HideLoading();
    }

    private void LeaderBoardSuccess(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            GameObject goRank = Instantiate(rankHolder, this.transform);
            LeaderBoardHook leaderBoardHook = goRank.GetComponent<LeaderBoardHook>();
            leaderBoardHook.SetRank(item.Position);
            leaderBoardHook.SetPseudo(item.DisplayName);
            leaderBoardHook.SetScore(item.StatValue);       
        }

        playFabManager.HideLoading();
    }

    public void Home()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
