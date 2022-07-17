using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipMinionSelector : MonoBehaviour
{
    public Button CancelButton;
    public Transform cardHolder;
    public GameObject fade;

    PlayFabManager playFabManager;
    MinionData minionData;
  
    private void Awake()
    {
        playFabManager = GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>();
        CancelButton.onClick.AddListener(CloseMinionSelector);
    }

    void CloseMinionSelector() 
    {
        fade.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SetUpCardCurrentDeck(MinionData newMinionData)
    {
        fade.SetActive(true);
        minionData = newMinionData;

        foreach (Transform item in cardHolder.transform)
            Destroy(item.gameObject);

        foreach (var item in MinionInventory.Instance.allDecks[MinionInventory.Instance.Player_Deck - 1])
        {
            MinionCard newCard = null;
            MinionData newData = null;

            foreach (var data in MinionInventory.Instance.minionInventory)
                if(item == data.minionKey)
                {
                    newData = data;
                    break;
                }
             
               
            switch (newData.minion.rarety)
            {
                case Minion.MinonRarety.Common:
                    newCard = Instantiate(MinionInventory.Instance.commonMinionCard);
                    break;
                case Minion.MinonRarety.Rare:
                    newCard = Instantiate(MinionInventory.Instance.rareMinionCard);
                    break;
                case Minion.MinonRarety.Epic:
                    newCard = Instantiate(MinionInventory.Instance.epicMinionCard);
                    break;
                default:
                    break;
            }

            newCard.InitiateCard(newData, cardHolder, MinionCard.CardType.ChangeMinion);
        }  
    }

    public void ChangeMinionInDeck(MinionData minionToChangeInDeck)
    {
        Debug.Log(minionToChangeInDeck);
        Debug.Log(minionToChangeInDeck.deckCard[MinionInventory.Instance.Player_Deck -1]);
        Transform parentTransform = minionToChangeInDeck.deckCard[MinionInventory.Instance.Player_Deck -1].transform.parent;
        int index = minionToChangeInDeck.deckCard[MinionInventory.Instance.Player_Deck - 1].transform.GetSiblingIndex();

        minionToChangeInDeck.deckIndex.Remove(MinionInventory.Instance.Player_Deck);     
        minionData.deckIndex.Add(MinionInventory.Instance.Player_Deck);

        //minionData.deckCard = minionToChangeInDeck.deckCard;

        minionData.inventoryCard.ActiveEquipedBanner(true);
        minionToChangeInDeck.inventoryCard.ActiveEquipedBanner(false);


        MinionInventory.Instance.allDecks[MinionInventory.Instance.Player_Deck - 1][index] = minionData.minionKey;

  
        Destroy(minionToChangeInDeck.deckCard[MinionInventory.Instance.Player_Deck - 1].gameObject);
      //  minionToChangeInDeck.deckCard = null;

        MinionCard newCard = Instantiate(minionData.inventoryCard);
        newCard.CloseClickMenu();
        minionData.inventoryCard.CloseClickMenu();
        newCard.InitiateCard(minionData, parentTransform, MinionCard.CardType.Deck, whichDeck : MinionInventory.Instance.Player_Deck);
        newCard.transform.SetSiblingIndex(index);

        CloseMinionSelector();

        PlayFabManager.Instance.CreateMinionFENData();
    }
}
