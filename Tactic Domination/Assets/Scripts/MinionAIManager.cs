using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class MinionActions
{

    public float value;
    public int whichAttack;
    public Minion minion;
    public Tile destinationTile;
    public Tile targetAttackTile;
    public string description;
    public int cost;

    public MinionActions(Minion minion, Tile destiantion, Tile Target, float value, int cost, int whichAttack, string description)
    {
        this.minion = minion;
        this.value = value;
        this.cost = cost;
        this.destinationTile = destiantion;
        this.targetAttackTile = Target;  
        this.whichAttack = whichAttack;
        this.description = description ;
    }


}
public class MinionAIManager : MonoBehaviour
{
    public static MinionAIManager Instance;
    MinionAIManager()
    {
        Instance = this;
    }


    public List<MinionActions> buffActions = new List<MinionActions>();
    public List<MinionActions> attackActions = new List<MinionActions>();
    public List<MinionActions> movementActions = new List<MinionActions>();

    List<Minion> EnemyMinion()
    {
        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
        List<Minion> enemyMinion = new List<Minion>();

        foreach (var minion in allMinions)
        {
            Minion minionScript = minion.GetComponent<Minion>();
            if (minionScript.myMinion == false)
                enemyMinion.Add(minionScript);
        }

        return enemyMinion;
    }
    List<Minion> AllyMinion()
    {
        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
        List<Minion> allyMinions = new List<Minion>();

        foreach (var minion in allMinions)
        {
            Minion minionScript = minion.GetComponent<Minion>();
            if (minionScript.myMinion == true)
                allyMinions.Add(minionScript);
        }

        return allyMinions;
    }
    List<MinionAI> minionAI(List<Minion> minions)
    {
        List<MinionAI> allMinionAI = new List<MinionAI>();

        foreach (var minion in minions)
        {
            MinionAI minionAI = minion.GetComponent<MinionAI>();
            if (minionAI != null)
            {
                allMinionAI.Add(minionAI);
            }
        }

        return allMinionAI;
    }

    Coroutine searchActionCoroutine;

    [Button]
    public void CalculBestAction()
    {
        if(GameManager.Instance.yourTurn == true)
        {
          // if(GameManager.Instance.allyAI == true)
          // {
          //     foreach (var minion in AllyMinion())
          //     {
          //         MinionAI minionAI = minion.GetComponent<MinionAI>();
          //         minionAI.SetActionsValues();
          //     }
          // }
        }
        else
        {
            if (GameManager.Instance.enemyAI == true)
            {
                if(searchActionCoroutine == null)
                    searchActionCoroutine = StartCoroutine(AIProcess());
                else
                {
                    StopCoroutine(searchActionCoroutine);
                    searchActionCoroutine = StartCoroutine(AIProcess());
                }
                     
            }
        }
    }
    IEnumerator AIProcess()
    {
        //Check les buffs
        for (int t = 0; t < 3; t++)
        {
            float bestActionValue = 0;
            MinionActions bestAction = null;

            buffActions.Clear();
            List<MinionAI> minionAi = new List<MinionAI>();
            foreach (var minionAI in minionAI(EnemyMinion()))
            {
                minionAI.SetBuffAction();
                minionAi.Add(minionAI);
            }

            if (buffActions.Count > 0)
            {
                bestActionValue = 0;
                bestAction = null;
                for (int i = 0; i < buffActions.Count; i++)
                {
                    if (buffActions[i].value >= bestActionValue &&
                        buffActions[i].cost <= GameManager.Instance.actionPointLeft)
                    {
                        if (buffActions[i].destinationTile != null && buffActions[i].minion.movementAmountLeft <= 0)
                            continue;

                        bestActionValue = buffActions[i].value;
                        bestAction = buffActions[i];
                    }
                }               
            }
            else break;

            if (bestAction != null)
            {
                Minion minion = bestAction.minion;
                //Debug.Log(minion.MinionName + "  :  " + bestAction.description + "     Attack   : " + bestAction.whichAttack + "        Position   : " + bestAction.destinationTile + "        Target   : " + bestAction.targetAttackTile + "      Value :   " + bestAction.value + "     Cost :   " + bestAction.cost);

                Movement(bestAction);

                yield return new WaitForSeconds(1);
                if (bestAction.destinationTile != null)
                    yield return new WaitWhile(() => bestAction.minion.onMove);

                Attack(bestAction);
            }
            else break;

            yield return new WaitForSeconds(1);
            yield return new WaitWhile(() => bestAction.minion.onAttack);
        }

        //Check les attaques
        for (int t = 0; t < 20; t++)
        {
            float bestActionValue = 0;
            MinionActions bestAction = null;

            attackActions.Clear();
            List<MinionAI> minionAi = new List<MinionAI>();
            foreach (var minionAI in minionAI(EnemyMinion()))
            {
                minionAI.SetAttackAction();
                minionAi.Add(minionAI);
            }

            if (attackActions.Count > 0)
            {
                bestActionValue = 0;
                bestAction = null;
                for (int i = 0; i < attackActions.Count; i++)
                {
                    if (attackActions[i].value >= bestActionValue &&
                        attackActions[i].cost <= GameManager.Instance.actionPointLeft)
                    {
                        if (attackActions[i].destinationTile != null && attackActions[i].minion.movementAmountLeft <= 0)
                            continue;

                        bestActionValue = attackActions[i].value;
                        bestAction = attackActions[i];
                    }
                }
            }
            else break;

            if (bestAction != null)
            {
                Minion minion = bestAction.minion;
               // Debug.Log(minion.MinionName + "  :     " + bestAction.description + "        Attack   : " + bestAction.whichAttack + "        Position   : " + bestAction.destinationTile + "        Target   : " + bestAction.targetAttackTile + "       Value :   " + bestAction.value + "       Cost :   " + bestAction.cost);

                Movement(bestAction);

                yield return new WaitForSeconds(1);
                if (bestAction.destinationTile != null)
                    yield return new WaitWhile(() => bestAction.minion.onMove);

                Attack(bestAction);
            }
            else break;

            yield return new WaitForSeconds(1);
            yield return new WaitWhile(() => bestAction.minion.onAttack);
        }

        List<MinionAI> minionMoved = new List<MinionAI>();

        // Check les movements
        for (int t = 0; t < 3; t++)
        {
            float bestActionValue = 0;
            MinionActions bestAction = null;

            movementActions.Clear();
            List<MinionAI> minionAi = new List<MinionAI>();
            foreach (var minionAI in minionAI(EnemyMinion()))
            {
                if (!minionMoved.Contains(minionAI))
                {
                    minionAI.SetMovementAction();
                    minionAi.Add(minionAI);
                }
            }

            if (movementActions.Count > 0)
            {
                bestActionValue = 0;
                bestAction = null;
                for (int i = 0; i < movementActions.Count; i++)
                {
                    if (movementActions[i].value >= bestActionValue &&
                        movementActions[i].cost <= GameManager.Instance.actionPointLeft)
                    {
                        if (movementActions[i].destinationTile != null && movementActions[i].minion.movementAmountLeft <= 0)
                            continue;

                        bestActionValue = movementActions[i].value;
                        bestAction = movementActions[i];
                    }
                }
            }
            else break;

            if (bestAction != null)
            {
                Minion minion = bestAction.minion;
               // Debug.Log(minion.MinionName + "  :  " + bestAction.description + "  Value :   " + bestAction.value + "   Cost :   " + bestAction.cost);
                minionMoved.Add(bestAction.minion.GetComponent<MinionAI>());
                Movement(bestAction);
                yield return new WaitForSeconds(1);
                yield return new WaitWhile(() => bestAction.minion.onMove);
            }
            else break;

          
        }

        yield return new WaitForSeconds(1);

        Debug.Log("Action point Left :   " + GameManager.Instance.actionPointLeft);

        GameManager.Instance.NextTurn();

        StopCoroutine(AIProcess());

    }

    void Attack(MinionActions action)
    {
        if (action.minion.silenceTurnLeft > 0 || action.minion.currentGlyph == Glyph.GlyphType.Silence)
            return;

        Tile minionTile = action.minion.currentTile;
        if (action.destinationTile != null)
            if (minionTile != action.destinationTile)
                return;

        if (action.targetAttackTile != null && action.minion.onAttack == false)
        {
            switch (action.whichAttack)
            {
                case 1:
                    if(action.minion.firstAttack.rangeType != RangeType.onSelf)
                    {
                        action.minion.firstAttack.InitiateAttack();
                        action.minion.firstAttack.CheckAttackZone(action.targetAttackTile);
                        action.minion.firstAttack.ConfirmAttack(action.targetAttackTile);
                    }
                    else if(action.minion.currentTile == action.targetAttackTile)
                    {
                        action.minion.firstAttack.InitiateAttack();
                        action.minion.firstAttack.CheckAttackZone(action.targetAttackTile);
                        action.minion.firstAttack.ConfirmAttack(action.targetAttackTile);
                    }
                 
                    break;
                case 2:
                    if (action.minion.secondAttack.rangeType != RangeType.onSelf)
                    {
                        action.minion.secondAttack.InitiateAttack();
                        action.minion.secondAttack.CheckAttackZone(action.targetAttackTile);
                        action.minion.secondAttack.ConfirmAttack(action.targetAttackTile);
                    }
                    else if (action.minion.currentTile == action.targetAttackTile)
                    {
                        Debug.Log("AGDYUBEWYJHABCHND");

                        action.minion.secondAttack.InitiateAttack();
                        action.minion.secondAttack.CheckAttackZone(action.targetAttackTile);
                        action.minion.secondAttack.ConfirmAttack(action.targetAttackTile);
                    }
                    break;
                case 3:
                    if (action.minion.specialAttack.rangeType != RangeType.onSelf)
                    {
                        action.minion.specialAttack.InitiateAttack();
                        action.minion.specialAttack.CheckAttackZone(action.targetAttackTile);
                        action.minion.specialAttack.ConfirmAttack(action.targetAttackTile);
                    }
                    else if (action.minion.currentTile == action.targetAttackTile)
                    {
                        action.minion.specialAttack.InitiateAttack();
                        action.minion.specialAttack.CheckAttackZone(action.targetAttackTile);
                        action.minion.specialAttack.ConfirmAttack(action.targetAttackTile);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    void Movement(MinionActions action)
    {
        if (action.minion.stundTurnLeft > 0)
            return;

        if (action.destinationTile != null && action.destinationTile != action.minion.currentTile && action.minion.onMove == false)
        {
            action.minion.FindPossiblePath();
            if (action.minion.currentTile != action.destinationTile)
                action.minion.FindShortestPath(action.minion.currentTile, action.destinationTile);
        }
    }
}
