using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionPathFinding
{
    List<Tile> PossibleDestinationTiles = new List<Tile>();
    List<Tile> PossibleDestinationAfterTackleTiles = new List<Tile>();
    [HideInInspector] public List<Tile> PathTile = new List<Tile>();
    Tile destinationTile;


    public List<Tile> GetPossibleDestinationTile(Minion minion)
    {
        PossibleDestinationTiles = new List<Tile>();
        List<Tile> LastIterationTile = new List<Tile>();

        LastIterationTile.Add(minion.currentTile);

        for (int a = 0; a < minion.movementAmountLeft; a++)
        {
            List<Tile> newTile = new List<Tile>();

            foreach (Tile LastTile in LastIterationTile)
            {
                foreach (var tile in LastTile.lineTilesList)
                    if (PossibleDestinationTiles.Contains(tile) == false && tile.filled == false && tile.haveObstacle == false)
                    {
                        PossibleDestinationTiles.Add(tile);
                        newTile.Add(tile);
                    }
            }


            LastIterationTile.Clear();
            foreach (Tile tile in newTile)
                LastIterationTile.Add(tile);
        }

        return PossibleDestinationTiles;
    }
    public List<Tile> GetPossibleDestinationAfterTackle(Minion minion)
    {
        PossibleDestinationAfterTackleTiles = new List<Tile>();
        List<Tile> LastIterationTile = new List<Tile>();

        LastIterationTile.Add(minion.currentTile);

        int moveAmountAfterTackle = minion.movementAmountLeft;

        if(OnTackle(minion))
            moveAmountAfterTackle = Mathf.CeilToInt(minion.movementAmountLeft / 2);

         
        for (int a = 0; a < moveAmountAfterTackle; a++)
        {
            List<Tile> newTile = new List<Tile>();

            foreach (Tile LastTile in LastIterationTile)
            {
                int totalPathTackles = 0;

                if (LastTile != minion.currentTile)
                {                 
                    List<Minion> tacklePathMinions = new List<Minion>();

                    foreach (var tile in LastTile.lineTilesList)
                        if (tile.filled)
                            if (minion.myMinion != tile.currentMinion.myMinion)
                                totalPathTackles += tile.currentMinion.tackle;
                }
                             
                if(totalPathTackles - minion.leak <= 0)
                    foreach (var tile in LastTile.lineTilesList)
                        if (PossibleDestinationAfterTackleTiles.Contains(tile) == false && tile.filled == false && tile.haveObstacle == false)
                        {
                            tile.ActivePossiblePathTarget(true);
                            PossibleDestinationAfterTackleTiles.Add(tile);
                            newTile.Add(tile);
                        }
            }

            LastIterationTile.Clear();
            foreach (Tile tile in newTile)
                LastIterationTile.Add(tile);
        }

        return PossibleDestinationAfterTackleTiles;
    }

    public bool OnTackle(Minion minion)
    {
        bool tackle = false;
        int totalTackles = 0;
        foreach (var tile in minion.currentTile.lineTilesList)
            if (tile.filled)
                if (tile.currentMinion.myMinion != minion.myMinion)
                    totalTackles += tile.currentMinion.tackle;

        if (totalTackles - minion.leak > 0)
            tackle = true;

        return tackle;
    }

    public List<Tile> GetPossibleTileDirection (Minion minion, List<Tile> searchingTiles, List<Tile> tilesSearched, Tile destination)
    {
        List<Tile> nextTilesSearch = new List<Tile>();

        foreach (Tile searchTile in searchingTiles)
        {
            foreach (Tile tileNeighbor in searchTile.lineTilesList)
            {
                int totalPathTackles = 0;
                  
                foreach (var secondTileNeighbor in tileNeighbor.lineTilesList)
                    if (secondTileNeighbor.filled && secondTileNeighbor != minion.currentTile)
                        if (secondTileNeighbor.currentMinion.myMinion != minion.myMinion)
                            totalPathTackles += minion.tackle;

                if (tileNeighbor.filled == false && tileNeighbor.haveObstacle == false
                    &&  PossibleDestinationAfterTackleTiles.Contains(tileNeighbor)
                    && !nextTilesSearch.Contains(tileNeighbor)
                    && !tilesSearched.Contains(tileNeighbor))
                {

                    if (totalPathTackles - minion.leak <= 0 || tileNeighbor == destination)
                    {
                        nextTilesSearch.Add(tileNeighbor);
                        tileNeighbor.lastTile = searchTile;
                    }
                }
            }

            tilesSearched.Add(searchTile);
        }

        return nextTilesSearch;
    }
    public List<Tile> ReconstitutePath(Tile destination, int pathCost)
    {
        PathTile = new List<Tile>();
        destinationTile = destination;
        Tile pathTile = destination;
        for (int i = 0; i < pathCost; i++)
        {
            PathTile.Add(pathTile);
            pathTile = pathTile.lastTile;
        }
        PathTile.Reverse();

        return PathTile;
    }
    public List<Tile> PathAfterTrapCheck(Minion minion, List<Tile> originalPath)
    {
        List<Tile> path = new List<Tile>();

        int tileBeforeTrap = -1;
        for (int i = 0; i < originalPath.Count; i++)
            if (originalPath[i].haveTrap)
                if (minion.myMinion != originalPath[i].currentTrap.myTrap)
                {
                    tileBeforeTrap = i;
                    break;
                }
                  

        for (int i = 0; i < originalPath.Count; i++)
        {
            path.Add(originalPath[i]);

            if(tileBeforeTrap != -1)
                if (i >= tileBeforeTrap)
                    break;
        }

        return path;
    }


    public List<Vector2> GetPathCorner(Minion minion, List<Tile> path)
    {
        List<Vector2> cornerCoord = new List<Vector2>();

        //trouve tout les coins dans le chemin 
        bool onXLine = (path[0].coords.x == minion.currentTile.coords.x);
        bool onYLine = (path[0].coords.y == minion.currentTile.coords.y);
        int lastX = Mathf.FloorToInt(path[0].coords.x);
        int lastY = Mathf.FloorToInt(path[0].coords.y);
        //int vectorPathIndex = 0;


        for (int i = 0; i < path.Count ; i++)
        {
            if (onXLine)
            {
                if (path[i].coords.x != lastX)
                {
                    onYLine = true;
                    onXLine = false;
                    lastX = Mathf.FloorToInt(path[i].coords.x);
                    lastY = Mathf.FloorToInt(path[i].coords.y);
                    cornerCoord.Add(path[i - 1].coords);
                }
            }

            if (onYLine)
            {
                if (path[i].coords.y != lastY)
                {
                    onYLine = false;
                    onXLine = true;
                    lastX = Mathf.FloorToInt(path[i].coords.x);
                    lastY = Mathf.FloorToInt(path[i].coords.y);
                    cornerCoord.Add(path[i - 1].coords);
                }
            }
        }

        cornerCoord.Add(path[path.Count - 1].coords);
      

        return cornerCoord;
    }
}
