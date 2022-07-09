using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class LeaderBoardHook : MonoBehaviour
{
    public TextMeshProUGUI Rank, Pseudo, Score;

    public void SetRank(int value)
    {
        Rank.text = (value + 1).ToString();
    }
    public void SetPseudo(string name)
    {
        Pseudo.text = name;
    }
    public void SetScore(int value)
    {
        Score.text = value.ToString();
    }

}
