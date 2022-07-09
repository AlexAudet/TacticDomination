using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Obstacle : MonoBehaviour
{
    public PhotonView view;
    public int turnLeft;
    public Tile currentTile;
    public bool myObstacle;


    public void InitialteObstacle(Vector2 tileCoords, int activeTime)
    {
        view = GetComponent<PhotonView>();
        view.RPC("UpdateInfoForAll", RpcTarget.All, tileCoords, activeTime);
    }
    
    [PunRPC]
    void UpdateInfoForAll(Vector2 tileCoords, int activeTime)
    {
        turnLeft = activeTime;
        GameManager.Instance.tilesDictionary.TryGetValue(tileCoords, out currentTile);
        currentTile.haveObstacle = true;
    }

    public void NewTurn()
    {
        turnLeft--;
        if (turnLeft == 0)
            view.RPC("DestroyObstacle", RpcTarget.All);
    }


    [PunRPC]
    void DestroyObstacle()
    {
        currentTile.haveObstacle = false;
        currentTile.UpdateInfo();

        Destroy(gameObject);
    }
}

