using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;

public class GameManager : MonoBehaviour
{
    #region singleton
    public static GameManager Instance;
    GameManager()
    {
        Instance = this;
    }
    PlayFabManager playFabManager;

    [BoxGroup("User Data")]
    [PropertySpace(10, 0), Indent]
    public int InitCoins;
    [BoxGroup("User Data")]
    [Indent]
    public int coins;
    [BoxGroup("User Data")]
    [Indent]
    public int Initial_exp;
    [BoxGroup("User Data")]
    [PropertySpace(0, 10), Indent]
    public int exp;


    #endregion
    [BoxGroup("AI")]
    [PropertySpace(10, 0), Indent]
    public bool enemyAI;
    [BoxGroup("AI")]
    [PropertySpace(0, 10), Indent]
    public bool allyAI;

    [BoxGroup("Tile")]
    [PropertySpace(10, 0), Indent]
    public Tile tile;
    [BoxGroup("Tile")]
    [Indent]
    public Material LightTileMaterial;
    [BoxGroup("Tile")]
    [PropertySpace(0, 10), Indent]
    public Material DarkTileMaterial;



    [PropertySpace(20, 0)]
    public float timeByTurn = 30;


    [BoxGroup("Obstacle")]
    [PropertySpace(10, 0), Indent]
    public GameObject obstacle;
    [BoxGroup("Obstacle")]
    [Range(0, 100), Indent]
    public float chanceToBeWater;
    [BoxGroup("Obstacle")]
    [Range(1, 5), Indent]
    public int chunkObstacleAmount;
    [BoxGroup("Obstacle")]
    [PropertySpace(0, 10), Indent]
    public Vector2 chunkSizeRange;


    [FoldoutGroup("Infos", false), ReadOnly]
    public bool yourTurn;
    [FoldoutGroup("Infos"), ReadOnly]
    public int actionPointLeft;
    [HideInInspector]public int actionPointThisTurn;
    [FoldoutGroup("Infos"), ReadOnly]
    public Minion[] allMinions = new Minion[6];
    [FoldoutGroup("Infos"), ReadOnly]
    public Minion[] myMinions = new Minion[3];
    [FoldoutGroup("Infos"), ReadOnly]
    public Minion[] otherMinions = new Minion[3];
    [FoldoutGroup("Infos"), ReadOnly]
    public Tile[] allTiles;
    [FoldoutGroup("Infos"), ReadOnly]
    public int currentTurn = 1;

    public Dictionary<Vector2, Tile> tilesDictionary = new Dictionary<Vector2, Tile>();
    Vector2 gridSize = new Vector2(7, 13);
    float spaceBetween = 0;
    float tileSize = 4;
    int currentTileCheck = 0;
    bool haveAllMinion = false;
    bool minionIsAttacking;
    bool minionIsMoving;
    PhotonView view;
    Player playerData;
    Minion currentMinion; 
    Camera cam;
    Vector3 camOriginalPos;
    Quaternion camOriginalRot;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        playerData = Resources.Load("PlayerData") as Player;

        playFabManager = GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>();

        cam = Camera.main;
        camOriginalPos = cam.transform.position;
        camOriginalRot = cam.transform.rotation;

        InitCoins = playFabManager.Player_Coin;
        Initial_exp = playFabManager.Player_Exp;
    }

    private void Start()
    {
        allTiles = new Tile[Mathf.RoundToInt(gridSize.x * gridSize.y)];
        InitiateGame();
    }

    private void Update()
    {
        OnMapClickManagemer();
    }
    void OnMapClickManagemer()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount == 1)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
                if (hit.collider.GetComponent<Tile>() != null)
                {
                    Tile tile = hit.collider.GetComponent<Tile>();

                    bool onPathCheck = false;
                    bool onAttackCheck = false;
                    if(currentMinion != null)
                    {
                        if (currentMinion.onAttackInitiation || currentMinion.onAttackConfirmation)
                            onAttackCheck = true;

                        onPathCheck = currentMinion.onPathSelection;
                    }


                    if (tile.filled)
                    {
                        if (!onAttackCheck)
                        {
                            ChangeCurrentMinion(tile.currentMinion);
                            SoundManager.Instance.PlayUISound("MinionSelection");
                        }
                        else if (currentMinion.currentAttack.IsTileOnAttackRange(tile))
                            currentMinion.currentAttack.CheckAttackZone(tile);
                    }
                    else 
                    {
                        if (onAttackCheck)
                        {
                            if (currentMinion.currentAttack.IsTileOnAttackRange(tile))
                                currentMinion.currentAttack.CheckAttackZone(tile);
                            else
                            {
                                ClearMap();
                                UIManager.Instance.CurrentMinionMovement();
                            }
                        

                            return;
                        }
                        if (onPathCheck && !UIManager.minionInfoOpen)
                        {
                            if (currentMinion.PossiblePathTile(tile))
                                currentMinion.FindShortestPath(currentMinion.currentTile, tile);
                            else
                            {
                                ClearMap();
                                ChangeCurrentMinion(null);
                                UIManager.Instance.ActiveAllMinionHealth(false);
                            }
                              

                            return;
                        }
                    }
                }
            }
        }
    }


    #region Initial function
    public void InitiateGame()
    {
        tilesDictionary = new Dictionary<Vector2, Tile>();

        StartCoroutine(initiate());

        IEnumerator initiate()
        {
            //Créé le grid

            if (PhotonNetwork.IsMasterClient)
            {
                yourTurn = true;

                CreateGrid();

                if (!NetworkManager.Instance.testSolo)
                    view.RPC("GetTileForOtherPlayer", RpcTarget.Others);

                view.RPC("CreateMinion", RpcTarget.All);
            }

            yield return new WaitForSeconds(0.5f);

            GetAllMinon();
            SetLightDirection();

            //Créé tout les chunkDatas
            while (!haveAllMinion)
            {
                GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");

                if (allMinions.Length == 6)
                    haveAllMinion = true;

                yield return null;
            }

            if (PhotonNetwork.IsMasterClient)
                CreateChunkObstacle();

            yield return new WaitForSeconds(0.5f);

            ChangeCurrentMinion(myMinions[0], false);
            UIManager.Instance.PlayIntro();
        }
    }

    void CreateGrid()
    {
        //regarde si la grid size est paire, si oui rajoute 1 pour la rendre impaire
        if (gridSize.x % 2 == 0)
            gridSize.x++;
        if (gridSize.y % 2 == 0)
            gridSize.y++;

        //reset les listes


        currentTileCheck = 0;

        //créée tout les tile en rapport avec la dimenssion du grid
        for (int x = 1; x < gridSize.x + 1; x++)
        {
            for (int y = 1; y < gridSize.y + 1; y++)
            {

                //set la position des tiles
                Vector3 newTilePosition = Vector3.zero;
                newTilePosition.x += (x * tileSize) + (spaceBetween * x);
                newTilePosition.z += (y * tileSize) + (spaceBetween * y);
                newTilePosition.y -= 0.5f * tileSize;

                //set les scale des tiles
                Vector3 newTileScale = Vector3.one * tileSize;


                //créé la nouvelle tile
                Tile newTile = PhotonNetwork.Instantiate(tile.name, Vector3.zero, Quaternion.identity, 0).GetComponent<Tile>();

                //applique les datas aux tiles
                newTile.transform.localScale = newTileScale;
                newTile.transform.position = newTilePosition;
                newTile.coords = new Vector2(x, y);
                newTile.worldPos = newTile.transform.position + Vector3.up * (0.5f * tileSize);
                newTile.name = "Tile :" + x.ToString() + " , " + y.ToString();

              //  newTile.tileModel = newTile.tileModels[0];

                if (y % 2 == 0)
                {
                    if (x % 2 == 0)
                        newTile.tileModel.GetComponent<MeshRenderer>().sharedMaterial = LightTileMaterial;
                    else
                        newTile.tileModel.GetComponent<MeshRenderer>().sharedMaterial = DarkTileMaterial;
                }
                else
                {
                    if (x % 2 != 0)
                        newTile.tileModel.GetComponent<MeshRenderer>().sharedMaterial = LightTileMaterial;
                    else
                        newTile.tileModel.GetComponent<MeshRenderer>().sharedMaterial = DarkTileMaterial;
                }
                 

                //rajoute les tiles dans la list et le dictionnaire
                allTiles[currentTileCheck] = newTile;
                tilesDictionary.Add(newTile.coords, newTile);

                //Prepare la tile pour se faire dropper du ciel au moment venue
               // newTile.transform.position += Vector3.up * 200;
              //  newTile.gameObject.SetActive(false);

                currentTileCheck++;
            }
        }


        //regarde chaque tile et les mets dans la bonne liste
        for (int i = 0; i < allTiles.Length; i++)
        {
            Tile tile = allTiles[i];

            //va chercher les tiles voisines
            tile.SearchNeightbors();
           // tile.ChoseModel();
        }
    }


    [PunRPC]
    void GetTileForOtherPlayer()
    {

        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        currentTileCheck = 0;

        for (int x = 1; x < gridSize.x + 1; x++)
        {
            for (int y = 1; y < gridSize.y + 1; y++)
            {
                Tile tile = tiles[currentTileCheck].GetComponent<Tile>();

                Vector3 newTileScale = Vector3.one * tileSize;

                //set la position des tiles
                Vector3 newTilePosition = Vector3.zero;
                newTilePosition.x += (x * tileSize) + (spaceBetween * x);
                newTilePosition.z += (y * tileSize) + (spaceBetween * y);
                newTilePosition.y -= 0.5f * tileSize;

                //applique les datas aux tiles
                tile.transform.localScale = newTileScale;
                tile.transform.position = newTilePosition;
                tile.coords = new Vector2(x, y);
                tile.worldPos = tile.transform.position + Vector3.up * (0.5f * tileSize);
                tile.name = "Tile :" + x.ToString() + " , " + y.ToString();

                //Prepare la tile pour se faire dropper du ciel au moment venue
               // tile.transform.position += Vector3.up * 200;
                //tile.gameObject.SetActive(false);

                tilesDictionary.Add(tile.coords, tile);

                currentTileCheck++;
            }
        }

        //regarde chaque tile et les mets dans la bonne liste
        for (int i = 0; i < tiles.Length; i++)
        {

            tiles[i].GetComponent<Tile>().SearchNeightbors();

            allTiles[i] = tiles[i].GetComponent<Tile>();
        }

        tilesDictionary.Clear();
        for (int i = 0; i < allTiles.Length; i++)
        {
            tilesDictionary.Add(allTiles[i].coords, allTiles[i]);
        }
    }

    [PunRPC]
    void CreateMinion()
    {
        Tile[] MinionSpawnTiles = new Tile[3];

        //Si c'est un test solo
        Tile[] OtherMinionSpawnTiles = new Tile[3];

        float halfY = Mathf.FloorToInt((gridSize.y - 1) / 2) + 1;

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 randomCoord = new Vector2(Mathf.FloorToInt(Random.Range(1, gridSize.x)), Mathf.FloorToInt(Random.Range(2 * (i + 1), 2 * (i + 1))));

                Tile choosenTile;
                tilesDictionary.TryGetValue(randomCoord, out choosenTile);
                MinionSpawnTiles[i] = choosenTile;
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 randomCoord = new Vector2(Mathf.FloorToInt(Random.Range(1, gridSize.x)), Mathf.FloorToInt(Random.Range(2 * (i + 1), 2 * (i + 1))) + gridSize.x);

                Tile choosenTile;
                tilesDictionary.TryGetValue(randomCoord, out choosenTile);
                MinionSpawnTiles[i] = choosenTile;

            }
        }

        if (NetworkManager.Instance.testSolo)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 randomCoord = new Vector2(Mathf.FloorToInt(Random.Range(1, gridSize.x)), Mathf.FloorToInt(Random.Range(2 * (i + 1), 2 * (i + 1))) + gridSize.x);

                Tile choosenTile;
                tilesDictionary.TryGetValue(randomCoord, out choosenTile);
                OtherMinionSpawnTiles[i] = choosenTile;
            }
        }


        int rand = Random.Range(0, 3);

       /* for (int i = 0; i < 3; i++)
        {
            //Instantie le character sur notre Instance
            MinionDeck minionClass = playerData.minionsDeck[rand];
            Minion newMinion = PhotonNetwork.Instantiate(minionClass.minion.name,
                MinionSpawnTiles[i].worldPos, Quaternion.identity, 0).GetComponent<Minion>();


            newMinion.ChangeCurrentTile(MinionSpawnTiles[i]);
            MinionSpawnTiles[i].filled = true;
            MinionSpawnTiles[i].UpdateInfo();
            myMinions[i] = newMinion;

            newMinion.minionLevel = minionClass.level;
            newMinion.myMinion = true;
            newMinion.playerMark.GetComponent<MeshRenderer>().sharedMaterial = newMinion.playerMarkAllyMat;

            newMinion.CalculStats();
            for (int j = 0; j < 3; j++)
            {
                if(minionClass.runes[j] != null)
                    newMinion.RuneStatsBonus(minionClass.runes[j].runeType, minionClass.runes[j].runeLevel, j );
            }
           
            newMinion.currentHealth = newMinion.maxHealth;
            newMinion.movementAmountLeft = newMinion.movementAmount;

            if (!PhotonNetwork.IsMasterClient)
            {
                newMinion.RotateMinion(-Vector3.forward);
            }


            if (NetworkManager.Instance.testSolo)
            {
                MinionDeck otherMinionClass = playerData.minionsDeck[rand];
                Minion newOtherMinion = PhotonNetwork.Instantiate(otherMinionClass.minion.name,
                    OtherMinionSpawnTiles[i].worldPos, Quaternion.identity, 0).GetComponent<Minion>();

                newOtherMinion.ChangeCurrentTile(OtherMinionSpawnTiles[i]);
                otherMinions[i] = newOtherMinion;
                OtherMinionSpawnTiles[i].filled = true;
                OtherMinionSpawnTiles[i].UpdateInfo();
                newOtherMinion.RotateMinion(-Vector3.forward);

                newOtherMinion.minionLevel = minionClass.level;
                newOtherMinion.CalculStats();
                for (int j = 0; j < 3; j++)
                {
                    if (otherMinionClass.runes[j] != null)
                        newOtherMinion.RuneStatsBonus(otherMinionClass.runes[j].runeType, otherMinionClass.runes[j].runeLevel, j);
                }
                newOtherMinion.currentHealth = newOtherMinion.maxHealth;
                newOtherMinion.movementAmountLeft = newOtherMinion.movementAmount;
            }

            rand++;
            if (rand > 2)
                rand = 0;
        }*/
    }
    void GetAllMinon()
    {
        GameObject[] findMinion = GameObject.FindGameObjectsWithTag("Minion");

        for (int i = 0; i < findMinion.Length; i++)
        {
            allMinions[i] = findMinion[i].GetComponent<Minion>();
        }
    }
    void SetLightDirection()
    {
        GameObject[] findMinion = GameObject.FindGameObjectsWithTag("Minion");
        for (int i = 0; i < findMinion.Length; i++)
        {
            if (PhotonNetwork.IsMasterClient)
                findMinion[i].GetComponent<Minion>().MinionMaterial.SetVector("_LightDirection", new Vector3(-1, 0, -1));
            else
                findMinion[i].GetComponent<Minion>().MinionMaterial.SetVector("_LightDirection", new Vector3(1, 0, 1));
        }
    }

    void CreateChunkObstacle()
    {
        for (int i = 0; i < chunkObstacleAmount; i++)
        {
            float halfY = Mathf.FloorToInt((gridSize.y - 1) / 2) + 1;

            for (int y = 0; y < 2; y++)
            {
                List<Tile> halfList = new List<Tile>();
                foreach (Tile tile in allTiles)
                {
                    if (tile.coords.y > (halfY * y) + (y == 0 ? 1 : 0) &&
                        tile.coords.y < (halfY * (y + 1)) - (y == 1 ? 1 : 0) &&
                        tile.coords.x < gridSize.x && tile.coords.x > 1 &&
                        !tile.filled && !tile.haveObstacle)
                        halfList.Add(tile);
                }

                Tile initialTile = halfList[Mathf.FloorToInt(Random.Range(0, halfList.Count))];
                StartCoroutine(FindAllChunkTile(initialTile, Random.Range(0, 100) > chanceToBeWater, halfList));
            }
        }


        IEnumerator FindAllChunkTile(Tile initialTile, bool isWater, List<Tile> halfList)
        {

            List<Tile> chunkTiles = new List<Tile>();
            int currentChunkSize = 0;
            int tryAmount = 0;
            int chunkSize = Mathf.FloorToInt(Random.Range(chunkSizeRange.x, chunkSizeRange.y));
            bool chunkFull = false;

            currentChunkSize++;
            chunkTiles.Add(initialTile);
            initialTile.CreateObstacle(isWater);

            while (!chunkFull)
            {
                Tile checkTile = chunkTiles[Mathf.RoundToInt(Random.Range(0, chunkTiles.Count))];

                foreach (Tile tile in checkTile.lineTilesList)
                {
                    if (!tile.filled && !tile.haveObstacle
                        && tile.coords.x < gridSize.x && tile.coords.x > 1 &&
                        tile.cantHaveObstacle == false &&
                        halfList.Contains(tile) &&
                        currentChunkSize < chunkSize)
                    {
                        currentChunkSize++;
                        chunkTiles.Add(tile);
                        tile.CreateObstacle(isWater);
                        if (currentChunkSize == chunkSize)
                            chunkFull = true;
                    }
                }

                tryAmount++;
                if (tryAmount > chunkSize * 2)
                    chunkFull = true;

                yield return null;
            }

            //regarde si il y a des tile tout seul entouré d'obstacle et si oui met un obstacle
            for (int i = 0; i < allMinions.Length; i++)
            {
                if (IsMinionStuck(allMinions[i]))
                {
                    Tile otherTile = allMinions[i].currentTile;
                    for (int a = 0; a < gridSize.x; a++)
                    {
                        otherTile.DeleteObstacle();
                        if (otherTile.XLeft != null)
                            otherTile = otherTile.XLeft;
                        else break;
                    }
                }
            }
        }
    }
    public bool IsMinionStuck(Minion minion)
    {
        bool value = true;

      // foreach (Tile tile in allTiles)
      // {
      //     tile.tileModel.GetComponent<MeshRenderer>().sharedMaterial = tile.baseMaterial;
      // }

        List<Tile> LastIterationTile = new List<Tile>();
        List<Tile> PossibleTiles = new List<Tile>();

        foreach (Tile tile in minion.currentTile.lineTilesList)
        {
            if (tile.filled == false &&
                tile.haveObstacle == false)
            {
                PossibleTiles.Add(tile);
                LastIterationTile.Add(tile);
            }
        }

        for (int a = 0; a < 20; a++)
        {
            foreach (Tile LastTile in LastIterationTile)
            {
                for (int c = 0; c < LastTile.lineTilesList.Count; c++)
                {
                    if (PossibleTiles.Contains(LastTile.lineTilesList[c]) == false &&
                        LastTile.lineTilesList[c].filled == false &&
                        LastTile.lineTilesList[c].haveObstacle == false)
                    {
                        PossibleTiles.Add(LastTile.lineTilesList[c]);
                    }
                }
            }

            LastIterationTile.Clear();
            foreach (Tile tile in PossibleTiles)
            {
                LastIterationTile.Add(tile);
            }
        }

        Tile t = null;
        float middleX = ((gridSize.x - 1) / 2) + 1;
        float middleY = ((gridSize.y - 1) / 2) + 1;
        Vector2 centerCoords = new Vector2(middleX, middleY);
        tilesDictionary.TryGetValue(centerCoords, out t);
        if (PossibleTiles.Contains(t))
            value = false;

        return value;
    }

    #endregion


    public void StartGame()
    {
        actionPointLeft = 3;
        UIManager.Instance.InitiateActionPointPanel();
        UIManager.Instance.SetActionPoint(actionPointLeft);

        if (yourTurn)
            UIManager.Instance.playYourTurn();
        else
            UIManager.Instance.playRivalTurn();

        TurnTimer();
    }

    Coroutine timerCoroutine;
    void TurnTimer()
    {
        timerCoroutine = StartCoroutine(timer());
        IEnumerator timer()
        {
            float t = timeByTurn;

            while (t > 0)
            {
                t -= Time.deltaTime;
                UIManager.Instance.SetTimer(t);

                yield return null;
            }

            //Regarde si on minion se deplace toujours
            GameObject[] findMinion = GameObject.FindGameObjectsWithTag("Minion");
            foreach (var m in findMinion)
            {
                Minion minion = m.GetComponent<Minion>();
                if (minion.onMove)
                    minionIsMoving = true;
            }
            while (minionIsMoving)
            {
                bool value = false;

                foreach (var m in findMinion)
                {
                    Minion minion = m.GetComponent<Minion>();
                    if (minion.onMove)
                        value = true;
                }

                minionIsMoving = value;


                yield return null;
            }

            //Regarde si les minion Attack toujours
            foreach (var m in findMinion)
            {
                Minion minion = m.GetComponent<Minion>();
                if (minion.onAttack)
                    minionIsAttacking = true;
            }
            while (minionIsAttacking)
            {
                bool value = false;

                foreach (var m in findMinion)
                {
                    Minion minion = m.GetComponent<Minion>();
                    if (minion.onAttack)
                        value = true;
                }

                minionIsAttacking = value;

                Debug.Log("A Minion is attacking !");

                yield return null;
            }

            if (yourTurn || NetworkManager.Instance.testSolo)
            {
                GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
                for (int i = 0; i < allMinions.Length; i++)
                {
                    Minion minion = allMinions[i].GetComponent<Minion>();

                    if (NetworkManager.Instance.testSolo)
                    {
                        if (yourTurn)
                        {
                            if (minion.myMinion)
                                minion.EndTurn();
                        }
                        else
                        {
                            if (!minion.myMinion)
                                minion.EndTurn();
                        }
                    }
                    else
                    {
                        if (yourTurn)
                        {
                            if (minion.view.IsMine)
                                minion.EndTurn();
                        }
                    }
                }

                view.RPC("NextTurnForAll", RpcTarget.All);
            }
        
        }
    }

    public void NextTurn()
    {
        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
        for (int i = 0; i < allMinions.Length; i++)
        {
            Minion minion = allMinions[i].GetComponent<Minion>();

            if (NetworkManager.Instance.testSolo)
            {
                if (yourTurn)
                {
                    if (minion.myMinion)
                        minion.EndTurn();
                }
                else
                {
                    if (!minion.myMinion)
                        minion.EndTurn();
                }
            }
            else
            {
                if (yourTurn)
                {
                    if (minion.view.IsMine)
                        minion.EndTurn();
                }
            }
        }

        view.RPC("NextTurnForAll", RpcTarget.All);
    }
    [PunRPC]
    void NextTurnForAll()
    {
        StartCoroutine(waitToNextTurn());
        IEnumerator waitToNextTurn()
        {
            GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
            GameObject[] allObstacles = GameObject.FindGameObjectsWithTag("TempObstacle");
            GameObject[] allTrap = GameObject.FindGameObjectsWithTag("Trap");

            //Regarde si un minion se deplace ou attack toujours
            bool canPass = false;
            while (canPass == false)
            {
                bool allMinionReady = true;

                for (int i = 0; i < allMinions.Length; i++)
                {
                    Minion minion = allMinions[i].GetComponent<Minion>();
                    if (minion.onMove || minion.onAttack)
                        allMinionReady = false;
                }

                if (allMinionReady == true)
                    canPass = true;

                yield return null;
            }

            if(timerCoroutine != null)
                StopCoroutine(timerCoroutine);


            yourTurn = !yourTurn;


            if (yourTurn || NetworkManager.Instance.testSolo)
            {
                actionPointThisTurn = Mathf.Clamp(3 + (currentTurn - 1), 0, 10);
                actionPointLeft = actionPointThisTurn;
                UIManager.Instance.SetActionPoint(actionPointLeft);

                //1 tour de plus a chaque fois que les deux player on jouer
                if (PhotonNetwork.IsMasterClient)
                    view.RPC("UpdateTurnCounterForAll", RpcTarget.All, currentTurn + 1);
            }

            if (yourTurn)
                UIManager.Instance.playYourTurn();
            else
                UIManager.Instance.playRivalTurn();



            for (int i = 0; i < allMinions.Length; i++)
            {
                Minion minion = allMinions[i].GetComponent<Minion>();

                if (yourTurn && minion.myMinion)
                    minion.NewTurn();

                if (!yourTurn && !minion.myMinion)
                    minion.NewTurn();
            }

            for (int i = 0; i < allTrap.Length; i++)
            {
                Trap trapScript = allTrap[i].GetComponent<Trap>();

                if (yourTurn && trapScript.myTrap)
                    trapScript.NewTurn();

                if (!yourTurn && !trapScript.myTrap)
                    trapScript.NewTurn();
            }

            for (int i = 0; i < allObstacles.Length; i++)
            {
                Obstacle obstacleComponent = allObstacles[i].GetComponent<Obstacle>();
              
                if (yourTurn && obstacleComponent.myObstacle)
                    obstacleComponent.NewTurn();

                if (!yourTurn && !obstacleComponent.myObstacle)
                    obstacleComponent.NewTurn();
            }

            foreach (var tile in allTiles)
            {
                if (tile.haveGlyph) 
                { 
                    if(yourTurn && tile.glyph.myGlyph)
                        tile.glyph.NewTurn();

                    if (!yourTurn && !tile.glyph.myGlyph)
                        tile.glyph.NewTurn();
                }                  
            }

            ClearMap();
            TurnTimer();

            if (!yourTurn && PhotonNetwork.CurrentRoom.PlayerCount == 1)
                GetComponent<MinionAIManager>().CalculBestAction();
        }     
    }

    [PunRPC]
    void UpdateTurnCounterForAll(int turn)
    {
        currentTurn = turn;
    }

    public void ConsumeActionPoint(int pointAmount)
    {
        actionPointLeft -= pointAmount;
        UIManager.Instance.SetActionPoint(actionPointLeft);
          
    }
    public void ChangeCurrentMinion(Minion newMinon, bool activeMovement = true)
    {
        if (currentMinion != null)
        {
            currentMinion.ResetAttacks();
        }

        UIManager.Instance.UpdateFastMinionInfo(newMinon);

        if (newMinon)
        {
            currentMinion = newMinon;
            UIManager.Instance.UpdateActionPanel(newMinon);

            UIManager.Instance.CloseMinionsInfo();

            if(activeMovement)
                UIManager.Instance.CurrentMinionMovement();         
        }
        else
        {
            SoundManager.Instance.PlayUISound("ChangeToMinionNull");
            currentMinion = null;
        }
       
    }
    public void CheckMinionInfo(Minion newMinon)
    {
        if (!UIManager.minionInfoOpen)
            UIManager.Instance.OpenMinionsInfo(newMinon);
    }

    public Minion GetCurrentMinion()
    {
        return currentMinion;
    }


    public void ClearMap()
    {
        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
      
        foreach (var tile in allTiles)
        {
            tile.DeactivateAllTarget();
        }

        foreach (var m in allMinions)
        {
            Minion minion = m.GetComponent<Minion>();
            if(yourTurn && minion.myMinion)
            {
                minion.ResetAttacks();
                minion.ResetPathFinder();
            }
            if (!yourTurn && !minion.myMinion)
            {
                minion.ResetAttacks();
                minion.ResetPathFinder();
            }
        } 
    }


    public void DeactivateAllTargetOnTile()
    {
        foreach (var tile in allTiles)
        {
            tile.DeactivateAllTarget();
        }
    }


    public void CameraShake(float strenght, int vibration, float duration)
    {
        view.RPC("CameraSakeForAll", RpcTarget.All, strenght, vibration, duration);
    }
    [PunRPC]
    public void CameraSakeForAll(float strenght, int vibration, float duration)
    {
        cam.transform.position = camOriginalPos;
        cam.DOShakePosition(duration, strenght, vibration);
    }

    public void SlowMotion(float slowAmount, float duration)
    {
        view.RPC("SlowMotionForAll", RpcTarget.All, slowAmount, duration);
    }
    [PunRPC]
    public void SlowMotionForAll(float slowAmount, float duration)
    {
        StartCoroutine(slowMotion());
        IEnumerator slowMotion()
        {
            float s = 1;
            while(s > slowAmount)
            {
                s -= Time.deltaTime * 2;
                Time.timeScale = s;
                yield return null;
            }
            Time.timeScale = slowAmount;

            yield return new WaitForSeconds(duration * slowAmount);

            s = slowAmount;
            while (s < 1)
            {
                s += Time.deltaTime * 2;
                Time.timeScale = s;
                yield return null;
            }
            Time.timeScale = 1;
        }
    }


    public void CamZoom(Tile targetTile, float zoomSpeed, float zoomAmount, float duration)
    {
        view.RPC("CamZoomForAll", RpcTarget.All, targetTile.worldPos, zoomSpeed, zoomAmount, duration);
    }
    [PunRPC]
    public void CamZoomForAll(Vector3 tileTargetPos, float zoomSpeed, float zoomAmount, float duration)
    {
        StartCoroutine(slowMotion());
        IEnumerator slowMotion()
        {
            Vector3 targetDir = tileTargetPos - cam.transform.position;
            Quaternion processRot = cam.transform.rotation;

            float s = 0;
            while (s < 1)
            {
                s += Time.deltaTime * zoomSpeed;
                processRot = Quaternion.Slerp(cam.transform.rotation, Quaternion.LookRotation(targetDir), s);
                cam.transform.rotation = processRot;

                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomAmount, s);

                yield return null;
            }

            cam.transform.rotation = Quaternion.LookRotation(targetDir);

            yield return new WaitForSeconds(duration);

            s = 0;
            while (s < 1)
            {
                s += Time.deltaTime * zoomSpeed;
                processRot = Quaternion.Slerp(cam.transform.rotation, camOriginalRot, s);
                cam.transform.rotation = processRot;
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 10, s);
                yield return null;
            }

            cam.transform.rotation = camOriginalRot;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       if (stream.IsWriting)
       {
           stream.SendNext(haveAllMinion);
       }
       else
       {
            haveAllMinion = (bool)stream.ReceiveNext();
       }
    }




    public void EndGame()
    {


    }




   
}