using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAttackRange
{
    public List<Tile> attackRange = new List<Tile>();
    List<Tile> tempAttackRange = new List<Tile>();

    public List<Tile> attackZone = new List<Tile>();
    List<Tile> tempAttackZone = new List<Tile>();

    public List<Tile> GetPossibleAttackRange(Tile currentMinionTile, MinionAttack attack)
    {
        attackRange = new List<Tile>();

        switch (attack.rangeType)
        {
            case RangeType.Arround:

                attackRange = ArroundRange(currentMinionTile, attack);

                break;
            case RangeType.X:

                attackRange = XRange(currentMinionTile, attack);

                break;
            case RangeType.Line:

                attackRange = LineRange(currentMinionTile, attack);

                break;
            case RangeType.X_And_Line:

                attackRange = XLineRange(currentMinionTile, attack);

                break;
            case RangeType.onSelf:

                attackRange.Add(currentMinionTile);

                break;
            default:
                break;
        }

        if (attack.attackRange.x != 0)
        {
            currentMinionTile.ActivePossibleAttackTarget(true);
            if (attackRange.Contains(currentMinionTile))
                attackRange.Remove(currentMinionTile);
        }
        else
        {
            if (!attackRange.Contains(currentMinionTile))
                attackRange.Add(currentMinionTile);
        }

        return attackRange;
    }
    public List<Tile> GetAttackRangeWithVision(Tile currentMinionTile, MinionAttack attack)
    {
        List<Tile> attackRangeWithVision = new List<Tile>();

        foreach (Tile tile in attackRange)
        {
            bool haveNoVision = false;

            Vector3 origin = currentMinionTile.worldPos + Vector3.up;
            Vector3 direction = (tile.worldPos + Vector3.up) - (currentMinionTile.worldPos + Vector3.up);
            float distance = Vector3.Distance(currentMinionTile.worldPos, tile.worldPos);

            RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance, LayerMask.GetMask("AttackRangeCheck"));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponentInParent<Tile>() != null)
                {
                    Tile hitTile = hit.collider.GetComponentInParent<Tile>();

                    if (hitTile != tile)
                    {
                        if (hitTile.filled)
                            haveNoVision = true;
                        else if (hitTile.haveObstacle)
                        {
                            if (hitTile.waterObstacle && attack.canAttackOverWaterObstale)
                                continue;
                            else
                                haveNoVision = true;
                        }
                    }
                }               
            }

            if (haveNoVision == false)
                if (!tile.haveObstacle || attack.canSelectObstacle)
                    attackRangeWithVision.Add(tile);
        }

        return attackRangeWithVision;
    }

    List<Tile> ArroundRange(Tile currentMinionTile, MinionAttack attack)
    {
        List<Tile> possibleTiles = new List<Tile>();
        tempAttackRange = new List<Tile>();

        foreach (Tile tile in currentMinionTile.lineTilesList)
        {
            tempAttackRange.Add(tile);

            if (attack.attackRange.x <= 1 && attack.attackRange.y > 0)
            {
                if(tile.haveObstacle == false || attack.canSelectObstacle)
                {
                    if (!attack.cantSelectMinion)
                        possibleTiles.Add(tile);
                    else
                    {
                        if (!tile.filled)
                            possibleTiles.Add(tile);
                    }
                }
            }
        }

        for (int a = 1; a < attack.attackRange.y; a++)
        {
            int amount = tempAttackRange.Count;
            for (int b = 0; b < amount; b++)
            {
                for (int c = 0; c < tempAttackRange[b].lineTilesList.Count; c++)
                {
                    if (!tempAttackRange.Contains(tempAttackRange[b].lineTilesList[c]))
                    {
                        tempAttackRange.Add(tempAttackRange[b].lineTilesList[c]);

                        if (a + 1 >= attack.attackRange.x)
                        {
                            if (tempAttackRange[b].lineTilesList[c].haveObstacle == false || attack.canSelectObstacle)
                            {
                                if (!attack.cantSelectMinion)
                                    possibleTiles.Add(tempAttackRange[b].lineTilesList[c]);
                                else
                                {
                                    if (!tempAttackRange[b].lineTilesList[c].filled)
                                        possibleTiles.Add(tempAttackRange[b].lineTilesList[c]);
                                }
                            }
                     
                        }
                    }
                }
            }
        }

        return possibleTiles;
    }
    List<Tile> XRange(Tile currentMinionTile, MinionAttack attack)
    {
        List<Tile> possibleTiles = new List<Tile>();

        Tile otherXTile = null;
        for (int i = 0; i < attack.attackRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(currentMinionTile.coords.x - (i + 1), currentMinionTile.coords.y + (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!attack.cantSelectMinion)
                            possibleTiles.Add(otherXTile);
                        else
                        {
                            if (!otherXTile.filled)
                                possibleTiles.Add(otherXTile);
                        }
                    }

                }
            }
            else break;
        }

        for (int i = 0; i < attack.attackRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(currentMinionTile.coords.x + (i + 1), currentMinionTile.coords.y + (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!attack.cantSelectMinion)
                            possibleTiles.Add(otherXTile);
                        else
                        {
                            if (!otherXTile.filled)
                                possibleTiles.Add(otherXTile);
                        }
                    }

                }
            }
            else break;
        }

        for (int i = 0; i < attack.attackRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(currentMinionTile.coords.x - (i + 1), currentMinionTile.coords.y - (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!attack.cantSelectMinion)
                            possibleTiles.Add(otherXTile);
                        else
                        {
                            if (!otherXTile.filled)
                                possibleTiles.Add(otherXTile);
                        }
                    }
                }
            }
            else break;
        }

        for (int i = 0; i < attack.attackRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(currentMinionTile.coords.x + (i + 1), currentMinionTile.coords.y - (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!attack.cantSelectMinion)
                            possibleTiles.Add(otherXTile);
                        else
                        {
                            if (!otherXTile.filled)
                                possibleTiles.Add(otherXTile);
                        }
                    }
                }
            }
            else break;
        }


        return possibleTiles;
    }
    List<Tile> LineRange(Tile currentMinionTile, MinionAttack attack)
    {
        List<Tile> possibleTiles = new List<Tile>();

        Tile otherLineTile = currentMinionTile;
        for (int i = 0; i < attack.attackRange.y; i++)
        {
            if (otherLineTile.YUp != null)
            {
                otherLineTile = otherLineTile.YUp;

                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!attack.cantSelectMinion)
                            possibleTiles.Add(otherLineTile);
                        else
                        {
                            if (!otherLineTile.filled)
                                possibleTiles.Add(otherLineTile);
                        }
                    }
                }
            }
            else break;
        }

        otherLineTile = currentMinionTile;
        for (int i = 0; i < attack.attackRange.y; i++)
        {
            if (otherLineTile.YDown != null)
            {
                otherLineTile = otherLineTile.YDown;

                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!attack.cantSelectMinion)
                            possibleTiles.Add(otherLineTile);
                        else
                        {
                            if (!otherLineTile.filled)
                                possibleTiles.Add(otherLineTile);
                        }
                    }
                }
            }
            else break;
        }

        otherLineTile = currentMinionTile;
        for (int i = 0; i < attack.attackRange.y; i++)
        {
            if (otherLineTile.XLeft != null)
            {
                otherLineTile = otherLineTile.XLeft;

                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!attack.cantSelectMinion)
                            possibleTiles.Add(otherLineTile);
                        else
                        {
                            if (!otherLineTile.filled)
                                possibleTiles.Add(otherLineTile);
                        }
                    }
                }
            }
            else break;
        }

        otherLineTile = currentMinionTile;
        for (int i = 0; i < attack.attackRange.y; i++)
        {
            if (otherLineTile.XRight != null)
            {
                otherLineTile = otherLineTile.XRight;

                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!attack.cantSelectMinion)
                            possibleTiles.Add(otherLineTile);
                        else
                        {
                            if (!otherLineTile.filled)
                                possibleTiles.Add(otherLineTile);
                        }
                    }
                }
            }
            else break;
        }

        return possibleTiles;
    }
    List<Tile> XLineRange(Tile currentMinionTile, MinionAttack attack)
    {
        List<Tile> possibleTiles = new List<Tile>();

        Tile otherXLineTile = null;
        for (int i = 0; i < attack.attackRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(currentMinionTile.coords.x - i, currentMinionTile.coords.y + i);
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXLineTile);

            if (otherXLineTile != null)
            {
                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!attack.cantSelectMinion)
                            possibleTiles.Add(otherXLineTile);
                        else
                        {
                            if (!otherXLineTile.filled)
                                possibleTiles.Add(otherXLineTile);
                        }
                    }
                }
            }
            else break;
        }

        for (int i = 0; i < attack.attackRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(currentMinionTile.coords.x + i, currentMinionTile.coords.y + i);
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXLineTile);

            if (otherXLineTile != null)
            {
                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!attack.cantSelectMinion)
                            possibleTiles.Add(otherXLineTile);
                        else
                        {
                            if (!otherXLineTile.filled)
                                possibleTiles.Add(otherXLineTile);
                        }
                    }
                }
            }
            else break;
        }

        for (int i = 0; i < attack.attackRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(currentMinionTile.coords.x - i, currentMinionTile.coords.y - i);
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXLineTile);

            if (otherXLineTile != null)
            {
                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                        {
                            if (!attack.cantSelectMinion)
                                possibleTiles.Add(otherXLineTile);
                            else
                            {
                                if (!otherXLineTile.filled)
                                    possibleTiles.Add(otherXLineTile);
                            }
                        }
                    }
                }
            }
            else break;
        }

        for (int i = 0; i < attack.attackRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(currentMinionTile.coords.x + i, currentMinionTile.coords.y - i);
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXLineTile);

            if (otherXLineTile != null)
            {
                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                        {
                            if (!attack.cantSelectMinion)
                                possibleTiles.Add(otherXLineTile);
                            else
                            {
                                if (!otherXLineTile.filled)
                                    possibleTiles.Add(otherXLineTile);
                            }
                        }
                    }
                }
            }
            else break;
        }


        otherXLineTile = currentMinionTile;
        for (int i = 0; i < attack.attackRange.y; i++)
        {
            if (otherXLineTile.YUp != null)
            {
                otherXLineTile = otherXLineTile.YUp;

                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                        {
                            if (!attack.cantSelectMinion)
                                possibleTiles.Add(otherXLineTile);
                            else
                            {
                                if (!otherXLineTile.filled)
                                    possibleTiles.Add(otherXLineTile);
                            }
                        }
                    }
                }
            }
            else break;
        }

        otherXLineTile = currentMinionTile;
        for (int i = 0; i < attack.attackRange.y; i++)
        {
            if (otherXLineTile.YDown != null)
            {
                otherXLineTile = otherXLineTile.YDown;

                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                        {
                            if (!attack.cantSelectMinion)
                                possibleTiles.Add(otherXLineTile);
                            else
                            {
                                if (!otherXLineTile.filled)
                                    possibleTiles.Add(otherXLineTile);
                            }
                        }
                    }
                }
            }
            else break;
        }

        otherXLineTile = currentMinionTile;
        for (int i = 0; i < attack.attackRange.y; i++)
        {
            if (otherXLineTile.XLeft != null)
            {
                otherXLineTile = otherXLineTile.XLeft;

                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                        {
                            if (!attack.cantSelectMinion)
                                possibleTiles.Add(otherXLineTile);
                            else
                            {
                                if (!otherXLineTile.filled)
                                    possibleTiles.Add(otherXLineTile);
                            }
                        }
                    }
                }
            }
            else break;
        }

        otherXLineTile = currentMinionTile;
        for (int i = 0; i < attack.attackRange.y; i++)
        {
            if (otherXLineTile.XRight != null)
            {
                otherXLineTile = otherXLineTile.XRight;

                if (i + 1 >= attack.attackRange.x)
                {
                    if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                    {
                        if (!otherXLineTile.haveObstacle || attack.canSelectObstacle)
                        {
                            if (!attack.cantSelectMinion)
                                possibleTiles.Add(otherXLineTile);
                            else
                            {
                                if (!otherXLineTile.filled)
                                    possibleTiles.Add(otherXLineTile);
                            }
                        }
                    }
                }
            }
            else break;
        }

        return possibleTiles;
    }




    public List<Tile> GetAttackZone(Tile currentMinionTile, Tile targetTile, MinionAttack attack)
    {
        attackZone = new List<Tile>();
        tempAttackZone = new List<Tile>();

        switch (attack.zoneType)
        {
            case ZoneType.Arround:

                attackZone = ArroundZone(targetTile, attack);

                break;
            case ZoneType.Custom:

                attackZone = CustomZone(targetTile, attack);

                break;
            case ZoneType.Oriented_Custom:

                attackZone = CustomOrientedZone(currentMinionTile, targetTile, attack);

                break;
            case ZoneType.X:

                attackZone = XZone(targetTile, attack);

                break;
            case ZoneType.Vertical_Line:

                attackZone = VerticalLineZone(currentMinionTile, targetTile, attack);

                break;
            case ZoneType.Horizontal_Line:

                attackZone = HorizontalLineZone(currentMinionTile, targetTile, attack);

                break;

            case ZoneType.Cross:

                attackZone = CrossZone(targetTile, attack);

                break;
            case ZoneType.Cross_And_X:

                attackZone = CrossXZone(targetTile, attack);

                break;
            case ZoneType.Single_Tile:

                attackZone.Add(targetTile);

                break;
            default:
                break;
        }

        return attackZone;
    }

    List<Tile> ArroundZone(Tile targetTile, MinionAttack attack)
    {
        List<Tile> zoneTiles = new List<Tile>();
        tempAttackZone.Add(targetTile);

        foreach (Tile tile in targetTile.lineTilesList)
        {
            tempAttackZone.Add(tile);

            if (attack.zoneRange.x <= 1 && attack.zoneRange.y > 0)
                zoneTiles.Add(tile);
        }

        for (int a = 1; a < attack.zoneRange.y; a++)
        {
            int amount = tempAttackZone.Count;
            for (int b = 0; b < amount; b++)
            {
                for (int c = 0; c < tempAttackZone[b].lineTilesList.Count; c++)
                {
                    if (!tempAttackZone.Contains(tempAttackZone[b].lineTilesList[c]))
                    {
                        tempAttackZone.Add(tempAttackZone[b].lineTilesList[c]);

                        if (a + 1 >= attack.zoneRange.x && tempAttackZone[b].lineTilesList[c] != targetTile)
                        {
                            zoneTiles.Add(tempAttackZone[b].lineTilesList[c]);
                        }
                    }
                }
            }
        }

        if (attack.zoneRange.x != 0)
        {
            if (zoneTiles.Contains(targetTile))
                zoneTiles.Remove(targetTile);
        }
        else
        {
            if (!zoneTiles.Contains(targetTile))
                zoneTiles.Add(targetTile);
        }

        return zoneTiles;
    }
    List<Tile> CustomZone(Tile targetTile, MinionAttack attack)
    {
        List<Tile> zoneTiles = new List<Tile>();

        foreach (var coord in attack.customZone)
        {
            Tile otherTile = null;
            Vector2 otherTileCoords = new Vector2(targetTile.coords.x + coord.x, targetTile.coords.y + coord.y);
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherTile);

            if (otherTile != null)
                zoneTiles.Add(otherTile);
        }


        return zoneTiles;
    }

    List<Tile>CustomOrientedZone(Tile currentMinionTile, Tile targetTile, MinionAttack attack)
    {
        List<Tile> zoneTiles = new List<Tile>();


        if(targetTile.coords.x > currentMinionTile.coords.x)
        {
            foreach (var coord in attack.orientedCustomZone)
            {
                Tile otherTile = null;
                Vector2 otherTileCoords = new Vector2(targetTile.coords.x + coord.y, targetTile.coords.y + coord.x);
                GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherTile);

                if (otherTile != null)
                    zoneTiles.Add(otherTile);
            }
        }
        else if (targetTile.coords.x < currentMinionTile.coords.x)
        {
            foreach (var coord in attack.orientedCustomZone)
            {
                Tile otherTile = null;
                Vector2 otherTileCoords = new Vector2(targetTile.coords.x - coord.y, targetTile.coords.y - coord.x);
                GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherTile);

                if (otherTile != null)
                    zoneTiles.Add(otherTile);
            }

        }

        if (targetTile.coords.y > currentMinionTile.coords.y)
        {
            foreach (var coord in attack.orientedCustomZone)
            {
                Tile otherTile = null;
                Vector2 otherTileCoords = new Vector2(targetTile.coords.x + coord.x, targetTile.coords.y + coord.y);
                GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherTile);

                if (otherTile != null)
                    zoneTiles.Add(otherTile);
            }
        }
        else if (targetTile.coords.y < currentMinionTile.coords.y)
        {
            foreach (var coord in attack.orientedCustomZone)
            {
                Tile otherTile = null;
                Vector2 otherTileCoords = new Vector2(targetTile.coords.x - coord.x, targetTile.coords.y - coord.y);
                GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherTile);

                if (otherTile != null)
                    zoneTiles.Add(otherTile);
            }
        }


 


        return zoneTiles;
    }
    List<Tile> XZone(Tile targetTile, MinionAttack attack)
    {
        List<Tile> zoneTiles = new List<Tile>();

        Tile otherXTile;

        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(targetTile.coords.x - (i + 1), targetTile.coords.y + (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherXTile);
            }
            else break;
        }
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(targetTile.coords.x + (i + 1), targetTile.coords.y + (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherXTile);
            }
            else break;
        }
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(targetTile.coords.x - (i + 1), targetTile.coords.y - (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherXTile);
            }
            else break;
        }
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(targetTile.coords.x + (i + 1), targetTile.coords.y - (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherXTile);
            }
            else break;
        }


        if (attack.zoneRange.x != 0)
        {
            if (zoneTiles.Contains(targetTile))
                zoneTiles.Remove(targetTile);
        }
        else
        {
            if (!zoneTiles.Contains(targetTile))
                zoneTiles.Add(targetTile);
        }

        return zoneTiles;
    }
    List<Tile> VerticalLineZone(Tile currentMinionTile, Tile targetTile, MinionAttack attack)
    {
        List<Tile> zoneTiles = new List<Tile>();
        Tile otherLineTile;

        if (targetTile.coords.y > currentMinionTile.coords.y)
        {
            otherLineTile = targetTile;
            if (targetTile.coords.x == currentMinionTile.coords.x)
            {
                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.YUp != null)
                    {
                        otherLineTile = otherLineTile.YUp;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
            }
            else if (targetTile.coords.x > currentMinionTile.coords.x)
            {
                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYUpRight != null)
                    {
                        otherLineTile = otherLineTile.XYUpRight;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
            }
            else if (targetTile.coords.x < currentMinionTile.coords.x)
            {
                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYUpLeft != null)
                    {
                        otherLineTile = otherLineTile.XYUpLeft;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
            }
        }
        else if (targetTile.coords.y < currentMinionTile.coords.y)
        {
            otherLineTile = targetTile;
            if (targetTile.coords.x == currentMinionTile.coords.x)
            {
                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.YDown != null)
                    {
                        otherLineTile = otherLineTile.YDown;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
            }
            else if (targetTile.coords.x > currentMinionTile.coords.x)
            {
                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYDownRight != null)
                    {
                        otherLineTile = otherLineTile.XYDownRight;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
            }
            else if (targetTile.coords.x < currentMinionTile.coords.x)
            {
                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYDownLeft != null)
                    {
                        otherLineTile = otherLineTile.XYDownLeft;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
            }
        }
        else if (targetTile.coords.x > currentMinionTile.coords.x)
        {
            otherLineTile = targetTile;
            for (int i = 0; i < attack.zoneRange.y; i++)
            {
                if (otherLineTile.XRight != null)
                {
                    otherLineTile = otherLineTile.XRight;
                    zoneTiles.Add(otherLineTile);

                }
                else break;
            }
        }
        else if (targetTile.coords.x < currentMinionTile.coords.x)
        {
            otherLineTile = targetTile;
            for (int i = 0; i < attack.zoneRange.y; i++)
            {
                if (otherLineTile.XLeft != null)
                {
                    otherLineTile = otherLineTile.XLeft;
                    zoneTiles.Add(otherLineTile);
                }
                else break;
            }
        }


        if (attack.zoneRange.x != 0)
        {
            if (zoneTiles.Contains(targetTile))
                zoneTiles.Remove(targetTile);
        }
        else
        {
            if (!zoneTiles.Contains(targetTile))
                zoneTiles.Add(targetTile);
        }

        return zoneTiles;
    }
    List<Tile> HorizontalLineZone(Tile currentMinionTile, Tile targetTile, MinionAttack attack)
    {
        List<Tile> zoneTiles = new List<Tile>();
        Tile otherLineTile;

        if (targetTile.coords.y > currentMinionTile.coords.y)
        {
            if (targetTile.coords.x > currentMinionTile.coords.x)
            {
                otherLineTile = targetTile;
                zoneTiles.Add(otherLineTile);

                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYUpLeft != null)
                    {
                        otherLineTile = otherLineTile.XYUpLeft;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
                otherLineTile = targetTile;
                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYDownRight != null)
                    {
                        otherLineTile = otherLineTile.XYDownRight;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
            }
            else if (targetTile.coords.x < currentMinionTile.coords.x)
            {
                otherLineTile = targetTile;
                zoneTiles.Add(otherLineTile);

                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYUpRight != null)
                    {
                        otherLineTile = otherLineTile.XYUpRight;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
                otherLineTile = targetTile;
                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYDownLeft != null)
                    {
                        otherLineTile = otherLineTile.XYDownLeft;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
            }
        }
        else if (targetTile.coords.y < currentMinionTile.coords.y)
        {
            if (targetTile.coords.x > currentMinionTile.coords.x)
            {
                otherLineTile = targetTile;
                zoneTiles.Add(otherLineTile);

                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYUpRight != null)
                    {
                        otherLineTile = otherLineTile.XYUpRight;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
                otherLineTile = targetTile;
                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYDownLeft != null)
                    {
                        otherLineTile = otherLineTile.XYDownLeft;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
            }
            else if (targetTile.coords.x < currentMinionTile.coords.x)
            {
                otherLineTile = targetTile;
                zoneTiles.Add(otherLineTile);

                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYUpLeft != null)
                    {
                        otherLineTile = otherLineTile.XYUpLeft;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
                otherLineTile = targetTile;
                for (int i = 0; i < attack.zoneRange.y; i++)
                {
                    if (otherLineTile.XYDownRight != null)
                    {
                        otherLineTile = otherLineTile.XYDownRight;
                        zoneTiles.Add(otherLineTile);
                    }
                    else break;
                }
            }
        }
        if (targetTile.coords.y == currentMinionTile.coords.y)
        {
            otherLineTile = targetTile;
            zoneTiles.Add(otherLineTile);

            for (int i = 0; i < attack.zoneRange.y; i++)
            {
                if (otherLineTile.YUp != null)
                {
                    otherLineTile = otherLineTile.YUp;
                    zoneTiles.Add(otherLineTile);
                }
                else break;
            }
            otherLineTile = targetTile;
            for (int i = 0; i < attack.zoneRange.y; i++)
            {
                if (otherLineTile.YDown != null)
                {
                    otherLineTile = otherLineTile.YDown;
                    zoneTiles.Add(otherLineTile);
                }
                else break;
            }
        }
        if (targetTile.coords.x == currentMinionTile.coords.x)
        {
            otherLineTile = targetTile;
            zoneTiles.Add(otherLineTile);

            for (int i = 0; i < attack.zoneRange.y; i++)
            {
                if (otherLineTile.XLeft != null)
                {
                    otherLineTile = otherLineTile.XLeft;
                    zoneTiles.Add(otherLineTile);
                }
                else break;
            }
            otherLineTile = targetTile;
            for (int i = 0; i < attack.zoneRange.y; i++)
            {
                if (otherLineTile.XRight != null)
                {
                    otherLineTile = otherLineTile.XRight;
                    zoneTiles.Add(otherLineTile);
                }
                else break;
            }
        }

        if (attack.zoneRange.x != 0)
        {
            if (zoneTiles.Contains(targetTile))
                zoneTiles.Remove(targetTile);
        }
        else
        {
            if (!zoneTiles.Contains(targetTile))
                zoneTiles.Add(targetTile);
        }

        return zoneTiles;
    }
    List<Tile> CrossZone(Tile targetTile, MinionAttack attack)
    {
        List<Tile> zoneTiles = new List<Tile>();
        Tile otherLineTile;

        otherLineTile = targetTile;
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            if (otherLineTile.YUp != null)
            {
                otherLineTile = otherLineTile.YUp;
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherLineTile);
            }
            else break;
        }
        otherLineTile = targetTile;
        for (int i = 0; i < attack.zoneRange.y; i++)
        {

            if (otherLineTile.YDown != null)
            {
                otherLineTile = otherLineTile.YDown;
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherLineTile);
            }
            else break;
        }
        otherLineTile = targetTile;
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            if (otherLineTile.XLeft != null)
            {
                otherLineTile = otherLineTile.XLeft;
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherLineTile);
            }
            else break;
        }
        otherLineTile = targetTile;
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            if (otherLineTile.XRight != null)
            {
                otherLineTile = otherLineTile.XRight;
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherLineTile);
            }
            else break;
        }

        if (attack.zoneRange.x != 0)
        {
            if (zoneTiles.Contains(targetTile))
                zoneTiles.Remove(targetTile);
        }
        else
        {
            if (!zoneTiles.Contains(targetTile))
                zoneTiles.Add(targetTile);
        }

        return zoneTiles;
    }
    List<Tile> CrossXZone(Tile targetTile, MinionAttack attack)
    {
        List<Tile> zoneTiles = new List<Tile>();
        Tile otherLineTile;

        otherLineTile = targetTile;
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            if (otherLineTile.YUp != null)
            {
                otherLineTile = otherLineTile.YUp;
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherLineTile);
            }
            else break;
        }
        otherLineTile = targetTile;
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            if (otherLineTile.YDown != null)
            {
                otherLineTile = otherLineTile.YDown;
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherLineTile);
            }
            else break;
        }
        otherLineTile = targetTile;
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            if (otherLineTile.XLeft != null)
            {
                otherLineTile = otherLineTile.XLeft;
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherLineTile);
            }
            else break;
        }
        otherLineTile = targetTile;
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            if (otherLineTile.XRight != null)
            {
                otherLineTile = otherLineTile.XRight;
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherLineTile);
            }
            else break;
        }

        Tile otherXTile;
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(targetTile.coords.x - (i + 1), targetTile.coords.y + (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherXTile);
            }
            else break;
        }
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(targetTile.coords.x + (i + 1), targetTile.coords.y + (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherXTile);
            }
            else break;
        }
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(targetTile.coords.x - (i + 1), targetTile.coords.y - (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherXTile);
            }
            else break;
        }
        for (int i = 0; i < attack.zoneRange.y; i++)
        {
            Vector2 otherTileCoords = new Vector2(targetTile.coords.x + (i + 1), targetTile.coords.y - (i + 1));
            GameManager.Instance.tilesDictionary.TryGetValue(otherTileCoords, out otherXTile);

            if (otherXTile != null)
            {
                if (i + 1 >= attack.zoneRange.x)
                    zoneTiles.Add(otherXTile);
            }
            else break;
        }

        if (attack.zoneRange.x != 0)
        {
            if (zoneTiles.Contains(targetTile))
                zoneTiles.Remove(targetTile);
        }
        else
        {
            if (!zoneTiles.Contains(targetTile))
                zoneTiles.Add(targetTile);
        }

        return zoneTiles;
    }
}
