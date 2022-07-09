using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks , IMatchmakingCallbacks
{
    public bool testSolo;

    public static NetworkManager Instance;
    NetworkManager()
    {
        Instance = this;
    }

    public string roomName;

    private LoadBalancingClient loadBalancingClient = new LoadBalancingClient();




    public GameObject playerPrefab;
    public GameObject gameManager;

    public Text TextConnexionState;
    public Text TextIsMaster;

    public Text TextPlayerOneName;
    public Text TextPlayerTwoName;

    public Transform SpawnPointPlayerOne;
    public Transform SpawnPointPlayerTwo;

    public GameObject PanelAttenteJoueur;

    bool inRoom;

    PhotonView view;


    private void Awake()
    {
        view = GetComponent<PhotonView>();


        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "1";

    }

    public override void OnConnected()
    {
        base.OnConnected();

    
    }

    //void Start()
    //{
    //    PhotonNetwork.ConnectUsingSettings("proto1");
    //}

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to the Photon Master Server");
        PhotonNetwork.AutomaticallySyncScene = true;

       
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError(22222);

        base.OnJoinRandomFailed(returnCode, message);

        RoomOptions myRoomOption = new RoomOptions();
        myRoomOption.MaxPlayers = 2;
        PhotonNetwork.NickName = "Player" + Random.Range(1, 500);

        PhotonNetwork.CreateRoom(null, myRoomOption);
    }

    public override void OnJoinedRoom()
    {
        Debug.LogError(33333);

        base.OnJoinedRoom();

        if (!testSolo)
        {
            if (PhotonNetwork.PlayerList.Length < 2)
                return;
        }


        Debug.Log("Joined Room : " + PhotonNetwork.CurrentRoom.Name);

        inRoom = true;
        Vector3 sp = Vector3.zero;

        //Affiche des infos
        TextPlayerOneName.text = null;
        TextPlayerTwoName.text = null;
        TextConnexionState.text = "Room : " + PhotonNetwork.CurrentRoom.Name;
        if (PhotonNetwork.IsMasterClient)
        {
            TextPlayerOneName.text = PhotonNetwork.NickName;
            sp = SpawnPointPlayerOne.transform.position;
        }
        else
        {
            TextPlayerTwoName.text = PhotonNetwork.NickName;
            sp = SpawnPointPlayerTwo.transform.position;
        }


        //Instaciation de mon player
        GameObject MyPlayerInstance;
        GameObject MyTestPlayerInstance;// pour les test solo
        GameObject gameManagerInstance = null;

        if (!testSolo)
        {
            if (PhotonNetwork.IsMasterClient)
                MyPlayerInstance = PhotonNetwork.Instantiate
                    (playerPrefab.name, new Vector3(16, 27.8f, -2f), Quaternion.identity, 0);
            else
                MyPlayerInstance = PhotonNetwork.Instantiate
                (playerPrefab.name, new Vector3(16, 27.8f, -2f), Quaternion.identity, 0);
        }
        else
        {
            MyPlayerInstance = PhotonNetwork.Instantiate
             (playerPrefab.name, new Vector3(16, 27.8f, -2f), Quaternion.identity, 0);

            MyTestPlayerInstance = PhotonNetwork.Instantiate
             (playerPrefab.name, new Vector3(16, 27.8f, -2f), Quaternion.identity, 0);
        }


        MyPlayerInstance.GetComponentInChildren<Camera>().enabled = true;
        MyPlayerInstance.GetComponentInChildren<Camera>().GetComponent<AudioListener>().enabled = true;
        //UpdateListOfPlayers();

        if (PhotonNetwork.IsMasterClient)
        {
            gameManagerInstance = PhotonNetwork.Instantiate
                      (gameManager.name, Vector3.zero, Quaternion.identity, 0);
        }


        TextIsMaster.text = PhotonNetwork.IsMasterClient.ToString();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        Debug.Log("I Created the room");
    }


}
