using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Photon.Pun;

public class Tile : MonoBehaviour
{
    public GameObject tileModel;
    

    [FoldoutGroup("Target"), Indent]
    public GameObject target_AttackRange, target_PossiblePath, target_AttackZone, target_Destination, target_ImpossiblePath;
    Vector3 possible_Attack_Target_scale, possible_path_Target_scale, AttackZone_Target_scale, path_destination_target_scale, path_impossible_target_scale;

    [FoldoutGroup("Info")]
    [ReadOnly, Indent]
    public Vector2 coords;
    [FoldoutGroup("Info")]
    [ReadOnly, Indent]
    public Vector3 worldPos;
    [FoldoutGroup("Info")]
    [ReadOnly, Indent]
    public bool haveObstacle;
    [FoldoutGroup("Info")]
    [ReadOnly, Indent]
    public bool waterObstacle;
    [FoldoutGroup("Info")]
    [ReadOnly, Indent]
    public bool filled;
    [FoldoutGroup("Info")]
    [ReadOnly, Indent]
    public bool haveTrap;
    [FoldoutGroup("Info")]
    [ReadOnly, Indent]
    public bool haveGlyph;

    [HideInInspector] public bool cantHaveObstacle;
    [HideInInspector] public Tile lastTile;
    [HideInInspector] public Minion currentMinion;
    [HideInInspector] public Trap currentTrap;


    GameObject obstacleInstance;
    [HideInInspector] public PhotonView view;
    [HideInInspector] public Glyph glyph;
    [HideInInspector] public List<Tile> allTilesList = new List<Tile>();
    [HideInInspector] public List<Tile> lineTilesList = new List<Tile>();
    [HideInInspector] public Tile XLeft;
    [HideInInspector] public Tile XRight;
    [HideInInspector] public Tile YUp;
    [HideInInspector] public Tile YDown;
    [HideInInspector] public Tile XYUpLeft;
    [HideInInspector] public Tile XYUpRight;
    [HideInInspector] public Tile XYDownLeft;
    [HideInInspector] public Tile XYDownRight;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        possible_path_Target_scale = target_PossiblePath.transform.localScale;
        possible_Attack_Target_scale = target_AttackRange.transform.localScale;
        AttackZone_Target_scale = target_AttackZone.transform.localScale;
        path_destination_target_scale = target_Destination.transform.localScale;
        path_impossible_target_scale = target_ImpossiblePath.transform.localScale;

        glyph = GetComponentInChildren<Glyph>();

        ActiveAttackCheckCollider(false);
    }

    public void CreateObstacle(bool isWater)
    {
        view.RPC("CreateObstacleForAllPlayer", RpcTarget.All, isWater);

    }
    [PunRPC]
    void CreateObstacleForAllPlayer(bool isWater)
    {
        haveObstacle = true;
        waterObstacle = isWater;
   
        if (!isWater)
        {
            Vector3[] rotations = { new Vector3(0, 0, 0), new Vector3(0, 90, 0), new Vector3(0, -90, 0), new Vector3(0, 180, 0) };
            obstacleInstance = Instantiate(GameManager.Instance.obstacle, transform);
            obstacleInstance.transform.localScale = Vector3.one;
            obstacleInstance.transform.eulerAngles = rotations[Random.Range(0, 4)];
            obstacleInstance.transform.GetChild(Random.Range(0, obstacleInstance.transform.childCount)).gameObject.SetActive(true);
        }
        else
        {
            tileModel.SetActive(false);
        }
    }

    public void DeleteObstacle()
    {
        view.RPC("DeleteObstacleForAllPlayer", RpcTarget.All);
    }
    [PunRPC]
    void DeleteObstacleForAllPlayer()
    {
        haveObstacle = false;
        waterObstacle = false;
        Destroy(obstacleInstance);
        transform.GetChild(0).gameObject.SetActive(true);
      //  collider.enabled = false;
    }

    public void CloseGlyph(bool forAll)
    {
        if(forAll)
            view.RPC("CloseGlyphForAll", RpcTarget.All);
        else
        {
            if (currentMinion != null)
                currentMinion.CloseAllGlyphEffect();
            glyph.glyphObject.SetActive(false);
            haveGlyph = false;
            UpdateInfo();
        }

    }
    [PunRPC]
    public void CloseGlyphForAll()
    {
        if (currentMinion != null)
        {
            currentMinion.CloseAllGlyphEffect();
            currentMinion.UpdateEffectVfx();
        }
           
        glyph.glyphObject.SetActive(false);
        haveGlyph = false;
        UpdateInfo();
    }

    public void DestroyTrap(bool forAll)
    {
        if(forAll)
            view.RPC("DestroyTrapForAll", RpcTarget.All);
        else
        {
            haveTrap = false;

            Destroy(currentTrap.gameObject, 2);

            currentTrap = null;
            UpdateInfo();
        }
    }
    [PunRPC]
    void DestroyTrapForAll()
    {
        haveTrap = false;

        Destroy(currentTrap.gameObject, 2);

        currentTrap = null;
        UpdateInfo();
    }

    //movement de drop de la tile
    public void DropEffect()
    {
       
        Tween t = transform.DOMoveY(-1.5f, 0.4f)
            .SetEase(Ease.OutSine)
            .SetRelative(true);

        t.OnComplete(() =>
        {
            transform.DOMoveY(1.5f, 0.6f)
            .SetEase(Ease.OutElastic)
            .SetRelative(true);
        });
    }

    //Cherche les tile voisine a la tile
    public void SearchNeightbors()
    {
        Vector2 neighborsCoords = coords;
        Dictionary<Vector2, Tile> tilesDictionary;
        tilesDictionary = GameManager.Instance.tilesDictionary;

        //Set la tile a gauche
        neighborsCoords.x--;
        if (tilesDictionary.ContainsKey(neighborsCoords))
        {
            Tile t;
            tilesDictionary.TryGetValue(neighborsCoords, out t);
            allTilesList.Add(t);
            lineTilesList.Add(t);
            XLeft = t;
        }
        neighborsCoords = coords;

        //Set la tile a droite
        neighborsCoords.x++;
        if (tilesDictionary.ContainsKey(neighborsCoords))
        {
            Tile t;
            tilesDictionary.TryGetValue(neighborsCoords, out t);
            allTilesList.Add(t);
            lineTilesList.Add(t);
            XRight = t;
        }
        neighborsCoords = coords;

        //Set la tile en bas
        neighborsCoords.y--;
        if (tilesDictionary.ContainsKey(neighborsCoords))
        {
            Tile t;
            tilesDictionary.TryGetValue(neighborsCoords, out t);
            allTilesList.Add(t);
            lineTilesList.Add(t);
            YDown = t;
        }
        neighborsCoords = coords;

        //Set la tile au dessus
        neighborsCoords.y++;
        if (tilesDictionary.ContainsKey(neighborsCoords))
        {
            Tile t;
            tilesDictionary.TryGetValue(neighborsCoords, out t);
            allTilesList.Add(t);
            lineTilesList.Add(t);
            YUp = t;
        }
        neighborsCoords = coords;

        //Set la tile diagonal haut gauche
        neighborsCoords.x--;
        neighborsCoords.y++;
        if (tilesDictionary.ContainsKey(neighborsCoords))
        {
            Tile t;
            tilesDictionary.TryGetValue(neighborsCoords, out t);
            allTilesList.Add(t);
            XYUpLeft = t;
        }
        neighborsCoords = coords;

        //Set la tile diagonal haut droite
        neighborsCoords.x++;
        neighborsCoords.y++;
        if (tilesDictionary.ContainsKey(neighborsCoords))
        {
            Tile t;
            tilesDictionary.TryGetValue(neighborsCoords, out t);
            allTilesList.Add(t);
            XYUpRight = t;
        }
        neighborsCoords = coords;

        //Set la tile diagonal bas gauche
        neighborsCoords.x--;
        neighborsCoords.y--;
        if (tilesDictionary.ContainsKey(neighborsCoords))
        {
            Tile t;
            tilesDictionary.TryGetValue(neighborsCoords, out t);
            allTilesList.Add(t);
            XYDownLeft = t;
        }
        neighborsCoords = coords;

        //Set la tile diagonal bas droite
        neighborsCoords.x++;
        neighborsCoords.y--;
        if (tilesDictionary.ContainsKey(neighborsCoords))
        {
            Tile t;
            tilesDictionary.TryGetValue(neighborsCoords, out t);
            allTilesList.Add(t);
            XYDownRight = t;
        }
        neighborsCoords = coords;
    }

    public void UpdateInfo()
    {
        view.RPC("UpdateForAllPlayer", RpcTarget.Others, coords, worldPos, haveTrap, haveGlyph, haveObstacle, waterObstacle, filled);
    }

    [PunRPC]
    void UpdateForAllPlayer(Vector2 coords_Transfer, Vector3 worldPos_Transfer,bool haveTrap_Transfer, bool haveGlyph_Transfer,
        bool haveObstacle_Transfer, bool waterObstacle_Transfer, bool filled_Transfer)
    {
        coords = coords_Transfer;
        worldPos = worldPos_Transfer;
        haveTrap = haveTrap_Transfer;
        haveGlyph = haveGlyph_Transfer;
        haveObstacle = haveObstacle_Transfer;
        waterObstacle = waterObstacle_Transfer;
        filled = filled_Transfer;
    }






    public void ActiveAttackCheckCollider(bool value)
    {
       // attackCheckCollider.gameObject.SetActive(value);
    }

    public void ActivePossibleAttackTarget(bool value)
    {
        target_AttackRange.SetActive(value);

      //  target_AttackRange.transform.DOKill();
      //  target_AttackRange.transform.localScale = Vector3.zero;
      //  target_AttackRange.transform.DOScale(possible_Attack_Target_scale, 0.2f)
      //      .SetEase(Ease.OutBack);
      //  target_AttackRange.SetActive(value);

    }

    public void ActiveAttackZonetarget(bool value)
    {
        target_AttackZone.SetActive(value);

      //  target_AttackZone.transform.DOKill();
      //  target_AttackZone.transform.localScale = Vector3.zero;
      //  target_AttackZone.transform.DOScale(AttackZone_Target_scale, 0.2f)
      //      .SetEase(Ease.OutBack);
      //  target_AttackZone.SetActive(value);

    }

    public void ActivePossiblePathTarget(bool value)
    {
        target_PossiblePath.SetActive(value);

      //  .transform.DOKill();
      //  target_PossiblePath.transform.localScale = Vector3.zero;
      //  target_PossiblePath.transform.DOScale(possible_path_Target_scale, 0.2f)
      //      .SetEase(Ease.OutBack);
      //  target_PossiblePath.SetActive(value);
    }

    public void ActivePathDestinationTarget(bool value)
    {
        target_Destination.SetActive(value);

        //target_Destination.transform.DOKill();
        //target_Destination.transform.localScale = Vector3.zero;
        //target_Destination.transform.DOScale(path_destination_target_scale, 0.2f)
        //    .SetEase(Ease.OutBack);
        //target_Destination.SetActive(value);
    }

    public void ActiveImpossiblePathTarget(bool value)
    {
        target_ImpossiblePath.SetActive(value);

       // target_ImpossiblePath.transform.DOKill();
       // target_ImpossiblePath.transform.localScale = Vector3.zero;
       // target_ImpossiblePath.transform.DOScale(path_impossible_target_scale, 0.2f)
       //     .SetEase(Ease.OutBack);
       // target_ImpossiblePath.SetActive(value);
    }

    public void DeactivateAllTarget()
    {
        target_AttackRange.SetActive(false);
        target_AttackZone.SetActive(false);
        target_PossiblePath.SetActive(false);
      //  target_Destination.SetActive(false);
        target_ImpossiblePath.SetActive(false);

    }
}
