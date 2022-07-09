using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    MenuManager()
    {
        Instance = this;
    }



    public Button LeaderBoardButton;
    PlayFabManager playFabManager;


    public TextMeshProUGUI coinAmountText, gemAmountText, rankAmountText, expAmountText, levelAmountText, PseudoText;
    public Slider expSlider;


    private void Awake()
    {
        LeaderBoardButton.onClick.AddListener(OpenLeaderBoard);
        playFabManager = GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>();

         

        UpdateStat();
    }

    public void OpenLeaderBoard()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LeaderBoard");
    }

    public void UpdateStat()
    {
        coinAmountText.text = playFabManager.Player_Coin.ToString();
        gemAmountText.text = playFabManager.Player_Gem.ToString();
        PseudoText.text = playFabManager.Player_DisplayName.ToString();
        expAmountText.text = playFabManager.Player_Exp.ToString() + " / " + playFabManager.maxExp.ToString();
        levelAmountText.text = playFabManager.Player_Level.ToString();
        rankAmountText.text = playFabManager.Player_Rank.ToString();

        float expAmountFloat = playFabManager.Player_Exp;
        float maxExpAmountFloat = playFabManager.maxExp;
        expSlider.value = expAmountFloat / maxExpAmountFloat;
    }
}

