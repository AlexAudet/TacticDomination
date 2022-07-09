using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class MinionAI : MonoBehaviour
{
    public enum MinionTarget {Ally, Enemy, self}
    public enum AttackType { Damage, Buff }

    public bool debugCheck;

    MinionAttackRange attackManager;
    Minion currentMinion;

    //  public float 
    [Space(20)]
    [Tooltip("X = valeur de fuite si notre vie est a 0% Y = valeur de fuite si notre vie est a 100%")]
    public Vector2 fleeHealthRange;
    public int proximityFleeingTreshold = 4;

    [Space(20)]
    [Tooltip("X = valeur de fuite si notre vie est a 0% Y = valeur de Chase si notre vie est a 100%")]
    public Vector2 chaseHealthRange;
    public int chaseDistanceTarget = 2;

    [Space(20)]
    [Tooltip("X = valeur si la vie du minion a protege est a 0% Y = valeur  si la vie du minion a protege est a 100%")]
    public Vector2 allyHealthForProtectionRange;



    [Space(20)]
    public MinionTarget firstAttackTarget;
    public AttackType firstAttackType;
    [Tooltip("X = valeur si l'autre minion est coller  Y = valeur si l'autre minion a 15 tile de distance")]
    public Vector2 firstAttackDistanceFactorRange;
    [Tooltip("X = valeur si l'autre minion a 0% de vie  Y = valeur si l'autre minion a 100% de vie")]
    public Vector2 firstAttackHealthFactorRange;
    [Tooltip("X = valeur si l'autre minion a -100% de Resistance dans son type d'attaque  Y = valeur si l'autre minion a +100% de Resistance dans son type d'attaque")]
    public Vector2 firstAttackResistanceFactorRange;
    [Tooltip("X = valeur si le nombre d'action point est a 3  Y = valeur si le nombre d'action point est a 10")]
    public Vector2 firstAttackActionPointFactorRange;
    public float firstAttackMinActionPoint = 3;


    [Space(20)]
    public MinionTarget secondAttackTarget;
    public AttackType secondAttackType;
    [Tooltip("X = valeur si l'autre minion est coller  Y = valeur si l'autre minion a 15 tile de distance")]
    public Vector2 secondAttackDistanceFactorRange;
    [Tooltip("X = valeur si l'autre minion a 0% de vie  Y = valeur si l'autre minion a 100% de vie")]
    public Vector2 secondAttackHealthFactorRange;
    [Tooltip("X = valeur si l'autre minion a -100% de Resistance dans son type d'attaque  Y = valeur si l'autre minion a +100% de Resistance dans son type d'attaque")]
    public Vector2 secondAttackResistanceFactorRange;
    [Tooltip("X = valeur si le nombre d'action point est a 3  Y = valeur si le nombre d'action point est a 10")]
    public Vector2 secondAttackActionPointFactorRange;
    public float secondAttackMinActionPoint = 3;


    [Space(20)]
    public MinionTarget specialAttackTarget;
    public AttackType specialAttackType;
    [Tooltip("X = valeur si l'autre minion est coller  Y = valeur si l'autre minion a 15 tile de distance")]
    public Vector2 specialAttackDistanceFactorRange;
    [Tooltip("X = valeur si l'autre minion a 0% de vie  Y = valeur si l'autre minion a 100% de vie")]
    public Vector2 specialAttackHealthFactorRange;
    [Tooltip("X = valeur si l'autre minion a -100% de Resistance dans son type d'attaque  Y = valeur si l'autre minion a +100% de Resistance dans son type d'attaque")]
    public Vector2 specialAttackResistanceFactorRange;
    [Tooltip("X = valeur si le nombre d'action point est a 3  Y = valeur si le nombre d'action point est a 10")]
    public Vector2 specialAttackActionPointFactorRange;
    public float specialAttackMinActionPoint = 3;



    Dictionary<Vector2, float> enemyDistancefield = new Dictionary<Vector2, float>();

    List<GameObject> text = new List<GameObject>();


    List<Minion> EnemyMinion()
    {
        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
        List<Minion> enemyMinion = new List<Minion>();

        foreach (var minion in allMinions)
        {
            Minion minionScript = minion.GetComponent<Minion>();
            if (minionScript.myMinion != currentMinion.myMinion)
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
            if (minionScript.myMinion == currentMinion.myMinion)
                allyMinions.Add(minionScript);
        }

        return allyMinions;
    }
    List<Tile> TacklableEnemy()
    {
        List<Tile> tacklableMinion = new List<Tile>();
        foreach (var minion in EnemyMinion())
        {
            if (minion.leak < currentMinion.tackle)
            {
                tacklableMinion.Add(minion.currentTile);
            }
        }
        return tacklableMinion;
    }
    List<Tile> EnemyThatCanTackle()
    {
        List<Tile> tackleEnemy = new List<Tile>();
        foreach (var minion in EnemyMinion())
        {
            if (minion.tackle > currentMinion.leak)
            {
                tackleEnemy.Add(minion.currentTile);
            }
        }
        return tackleEnemy;
    }

    private void Awake()
    {
        currentMinion = GetComponent<Minion>();
    }

    void ResetAll()
    {
        foreach (var item in text)
        {
            Destroy(item, 0.1f);
        }

        enemyDistancefield.Clear();
    }

    public void SetBuffAction()
    {
        ResetAll();

        if (currentMinion.silenceTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Silence)
            return;

        SetEnemyDistanceField();

        //calcule le cout de l'action
        MinionPathFinding pathFinding = new MinionPathFinding();
        int movementCost = (pathFinding.OnTackle(currentMinion) ? 3 : (currentMinion.haveMoved ? 0 : 1));
        if (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge)
            movementCost *= 2;

        //calcule le cout de l'action
        int firstAttackCost = currentMinion.firstAttack.actionPointCost;
        if (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge)
            firstAttackCost *= 2;

        //calcule le cout de l'action
        int secondAttackCost = currentMinion.secondAttack.actionPointCost;
        if (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge)
            secondAttackCost *= 2;

        //calcule le cout de l'action
        int specialAttackCost = currentMinion.specialAttack.actionPointCost;
        if (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge)
            specialAttackCost *= 2;

        if (firstAttackType == AttackType.Buff &&
            GameManager.Instance.actionPointThisTurn >= firstAttackMinActionPoint &&
            firstAttackCost <= GameManager.Instance.actionPointLeft &&
            currentMinion.firstAttack.cooldownLeft <= 0)
        {
            if(currentMinion.firstAttack.rangeType == RangeType.onSelf && currentMinion.firstAttack.zoneType == ZoneType.Single_Tile)
            {
                float currentHealth = currentMinion.currentHealth;
                float maxHealth = currentMinion.maxHealth;

                float healthFactorValue = Remap(currentHealth / maxHealth, 0, 1, firstAttackHealthFactorRange.x, firstAttackHealthFactorRange.y);
                float actionPointFactorValue = Remap(GameManager.Instance.actionPointThisTurn, 3, 10, firstAttackActionPointFactorRange.x, firstAttackActionPointFactorRange.y);

                float finaleValue = healthFactorValue + actionPointFactorValue;

                CreateBuffAction(null, currentMinion.currentTile, finaleValue, firstAttackCost, 1, "Buff");
                return;
            }

               
            //Toute les tile qui peuvent etre un target d'attaque potentielle
            Dictionary<Vector2, float> AttackTargetsValue = new Dictionary<Vector2, float>();
            foreach (KeyValuePair<Vector2, float> coord in AttackTargetTilesValue(1, firstAttackTarget))
                AttackTargetsValue.Add(coord.Key, coord.Value);

            //Regarde toute les target tile pour determiner qu'elle est la meilleur accessible
            Tile bestTarget = BestAccessibleAttackTarget(AttackTargetsValue, currentMinion.firstAttack);


            //Si les meilleure target accessible n'est pas égale a null 
            if (bestTarget != null)
            {
                float bestTargetValue = AttackTargetsValue[bestTarget.coords];

                //Calcule la valeur des position possible pour etre dans le range de la meilleure target accessible possible
                Dictionary<Vector2, float> AttackPosForInRange = new Dictionary<Vector2, float>();
                foreach (KeyValuePair<Vector2, float> coord in PositionTilesValue(bestTargetValue, PossiblePositionTileForAttack(bestTarget, currentMinion.firstAttack)))
                    AttackPosForInRange.Add(coord.Key, coord.Value);

                Tile position = BestPosition(AttackPosForInRange);
                if (position == null)
                    CreateBuffAction(null, bestTarget, bestTargetValue, firstAttackCost, 1, "Buff");
                else if (firstAttackCost + movementCost <= GameManager.Instance.actionPointLeft && currentMinion.stundTurnLeft <= 0)
                    CreateBuffAction(position, bestTarget, bestTargetValue, firstAttackCost + movementCost, 1, "Movement and Buff");
            }
        }

        if (secondAttackType == AttackType.Buff &&
            GameManager.Instance.actionPointThisTurn >= secondAttackMinActionPoint &&
            secondAttackCost <= GameManager.Instance.actionPointLeft &&
            currentMinion.secondAttack.cooldownLeft <= 0)
        {

            if (currentMinion.secondAttack.rangeType == RangeType.onSelf && currentMinion.secondAttack.zoneType == ZoneType.Single_Tile)
            {
                float currentHealth = currentMinion.currentHealth;
                float maxHealth = currentMinion.maxHealth;

                float healthFactorValue = Remap(currentHealth / maxHealth, 0, 1, secondAttackHealthFactorRange.x, secondAttackHealthFactorRange.y);
                float actionPointFactorValue = Remap(GameManager.Instance.actionPointThisTurn, 3, 10, secondAttackActionPointFactorRange.x, secondAttackActionPointFactorRange.y);

                float finaleValue = healthFactorValue + actionPointFactorValue;

                CreateBuffAction(null, currentMinion.currentTile, finaleValue, secondAttackCost, 2, "Buff");
                return;
            }

            //Toute les tile qui peuvent etre un target d'attaque potentielle
            Dictionary<Vector2, float> AttackTargetsValue = new Dictionary<Vector2, float>();
            foreach (KeyValuePair<Vector2, float> coord in AttackTargetTilesValue(2, secondAttackTarget))
                AttackTargetsValue.Add(coord.Key, coord.Value);

            //Regarde toute les target tile pour determiner qu'elle est la meilleur accessible
            Tile bestTarget = BestAccessibleAttackTarget(AttackTargetsValue, currentMinion.secondAttack);


            //Si les meilleure target accessible n'est pas égale a null 
            if (bestTarget != null)
            {
                float bestTargetValue = AttackTargetsValue[bestTarget.coords];

                //Calcule la valeur des position possible pour etre dans le range de la meilleure target accessible possible
                Dictionary<Vector2, float> AttackPosForInRange = new Dictionary<Vector2, float>();
                foreach (KeyValuePair<Vector2, float> coord in PositionTilesValue(bestTargetValue, PossiblePositionTileForAttack(bestTarget, currentMinion.secondAttack)))
                    AttackPosForInRange.Add(coord.Key, coord.Value);

                Tile position = BestPosition(AttackPosForInRange);
                if (position == null)
                    CreateBuffAction(null, bestTarget, bestTargetValue, secondAttackCost, 2, "Buff");
                else if (secondAttackCost + movementCost <= GameManager.Instance.actionPointLeft && currentMinion.stundTurnLeft <= 0)
                    CreateBuffAction(position, bestTarget, bestTargetValue, secondAttackCost + movementCost, 2, "Movement and Buff");
            }
        }

        if (specialAttackType == AttackType.Buff &&
            GameManager.Instance.actionPointThisTurn >= specialAttackMinActionPoint &&
            specialAttackCost <= GameManager.Instance.actionPointLeft &&
            currentMinion.specialAttack.cooldownLeft <= 0)
        {

            if (currentMinion.specialAttack.rangeType == RangeType.onSelf && currentMinion.specialAttack.zoneType == ZoneType.Single_Tile)
            {
                float currentHealth = currentMinion.currentHealth;
                float maxHealth = currentMinion.maxHealth;

                float healthFactorValue = Remap(currentHealth / maxHealth, 0, 1, specialAttackHealthFactorRange.x, specialAttackHealthFactorRange.y);
                float actionPointFactorValue = Remap(GameManager.Instance.actionPointThisTurn, 3, 10, specialAttackActionPointFactorRange.x, secondAttackActionPointFactorRange.y);

                float finaleValue = healthFactorValue + actionPointFactorValue;

                CreateBuffAction(null, currentMinion.currentTile, finaleValue, specialAttackCost, 3, "Buff");
                return;
            }

            //Toute les tile qui peuvent etre un target d'attaque potentielle
            Dictionary<Vector2, float> AttackTargetsValue = new Dictionary<Vector2, float>();
            foreach (KeyValuePair<Vector2, float> coord in AttackTargetTilesValue(3, specialAttackTarget))
                AttackTargetsValue.Add(coord.Key, coord.Value);

            //Regarde toute les target tile pour determiner qu'elle est la meilleur accessible
            Tile bestTarget = BestAccessibleAttackTarget(AttackTargetsValue, currentMinion.specialAttack);


            //Si les meilleure target accessible n'est pas égale a null 
            if (bestTarget != null)
            {
                float bestTargetValue = AttackTargetsValue[bestTarget.coords];

                //Calcule la valeur des position possible pour etre dans le range de la meilleure target accessible possible
                Dictionary<Vector2, float> AttackPosForInRange = new Dictionary<Vector2, float>();
                foreach (KeyValuePair<Vector2, float> coord in PositionTilesValue(bestTargetValue, PossiblePositionTileForAttack(bestTarget, currentMinion.specialAttack)))
                    AttackPosForInRange.Add(coord.Key, coord.Value);

                Tile position = BestPosition(AttackPosForInRange);
                if (position == null)
                    CreateBuffAction(null, bestTarget, bestTargetValue, specialAttackCost, 3, "Buff");
                else if (specialAttackCost + movementCost <= GameManager.Instance.actionPointLeft && currentMinion.stundTurnLeft <= 0)
                    CreateBuffAction(position, bestTarget, bestTargetValue,specialAttackCost + movementCost, 3, "Movement and Buff");
            }
        }
    }

    public void SetAttackAction()
    {
        ResetAll();

        if(currentMinion.silenceTurnLeft > 0)
            return;

        SetEnemyDistanceField();

        //calcule le cout de l'action
        MinionPathFinding pathFinding = new MinionPathFinding();
        int movementCost = (pathFinding.OnTackle(currentMinion) ? 3 : (currentMinion.haveMoved ? 0 : 1));
        if (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge)
            movementCost *= 2;

        //calcule le cout de l'action
        int firstAttackCost = currentMinion.firstAttack.actionPointCost;
        if (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge)
            firstAttackCost *= 2;

        //calcule le cout de l'action
        int secondAttackCost = currentMinion.secondAttack.actionPointCost;
        if (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge)
            secondAttackCost *= 2;

        //calcule le cout de l'action
        int specialAttackCost = currentMinion.specialAttack.actionPointCost;
        if (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge)
            specialAttackCost *= 2;

        if (firstAttackType == AttackType.Damage &&
            GameManager.Instance.actionPointThisTurn >= firstAttackMinActionPoint && 
            firstAttackCost <= GameManager.Instance.actionPointLeft && 
            currentMinion.firstAttack.cooldownLeft <= 0)
        {
            //Toute les tile qui peuvent etre un target d'attaque potentielle
            Dictionary<Vector2, float> AttackTargetsValue = new Dictionary<Vector2, float>();
            foreach (KeyValuePair<Vector2, float> coord in AttackTargetTilesValue(1, firstAttackTarget))
                AttackTargetsValue.Add(coord.Key, coord.Value);

            //Regarde toute les target tile pour determiner qu'elle est la meilleur accessible
            Tile bestTarget = BestAccessibleAttackTarget(AttackTargetsValue, currentMinion.firstAttack);
       

            //Si les meilleure target accessible n'est pas égale a null 
            if (bestTarget != null)
            {
                float bestTargetValue = AttackTargetsValue[bestTarget.coords];

                //Calcule la valeur des position possible pour etre dans le range de la meilleure target accessible possible
                Dictionary<Vector2, float> AttackPosForInRange = new Dictionary<Vector2, float>();
                foreach (KeyValuePair<Vector2, float> coord in PositionTilesValue(bestTargetValue, PossiblePositionTileForAttack(bestTarget, currentMinion.firstAttack)))
                    AttackPosForInRange.Add(coord.Key, coord.Value);

                Tile position = BestPosition(AttackPosForInRange);
                if (position == null)
                {
                    if (currentMinion.secondAttack.rangeType == RangeType.onSelf)
                    {
                        if (bestTarget == currentMinion.currentTile)
                            CreateAttackAction(null, bestTarget, bestTargetValue, firstAttackCost, 1, "Attack");
                    }
                    else
                        CreateAttackAction(null, bestTarget, bestTargetValue, firstAttackCost, 1, "Attack");
                }
                else if (firstAttackCost + movementCost <= GameManager.Instance.actionPointLeft && currentMinion.stundTurnLeft <= 0)
                    CreateAttackAction(position, bestTarget, bestTargetValue, firstAttackCost + movementCost, 1, "Movement and Attack");
            }
        }
        
        if (secondAttackType == AttackType.Damage &&
            GameManager.Instance.actionPointThisTurn >= secondAttackMinActionPoint && 
            secondAttackCost <= GameManager.Instance.actionPointLeft && 
            currentMinion.secondAttack.cooldownLeft <= 0)
        {
            //Toute les tile qui peuvent etre un target d'attaque potentielle
            Dictionary<Vector2, float> AttackTargetsValue = new Dictionary<Vector2, float>();
            foreach (KeyValuePair<Vector2, float> coord in AttackTargetTilesValue(2, secondAttackTarget))
                AttackTargetsValue.Add(coord.Key, coord.Value);

            //Regarde toute les target tile pour determiner qu'elle est la meilleur accessible
            Tile bestTarget = BestAccessibleAttackTarget(AttackTargetsValue, currentMinion.secondAttack);
          


            //Si les meilleure target accessible n'est pas égale a null 
            if (bestTarget != null)
            {
                float bestTargetValue = AttackTargetsValue[bestTarget.coords];

                //Calcule la valeur des position possible pour etre dans le range de la meilleure target accessible possible
                Dictionary<Vector2, float> AttackPosForInRange = new Dictionary<Vector2, float>();
                foreach (KeyValuePair<Vector2, float> coord in PositionTilesValue(bestTargetValue, PossiblePositionTileForAttack(bestTarget, currentMinion.secondAttack)))
                    AttackPosForInRange.Add(coord.Key, coord.Value);

                Tile position = BestPosition(AttackPosForInRange);
                if (position == null)
                {
                    if(currentMinion.secondAttack.rangeType == RangeType.onSelf)
                    {
                        if(bestTarget == currentMinion.currentTile)
                            CreateAttackAction(null, bestTarget, bestTargetValue, secondAttackCost, 2, "Attack");
                    }
                    else
                        CreateAttackAction(null, bestTarget, bestTargetValue, secondAttackCost, 2, "Attack");
                }
                else if (secondAttackCost + movementCost <= GameManager.Instance.actionPointLeft && currentMinion.stundTurnLeft <= 0)
                    CreateAttackAction(position, bestTarget, bestTargetValue, secondAttackCost + movementCost, 2, "Movement and Attack");
            }
        }

        if (specialAttackType == AttackType.Damage &&
            GameManager.Instance.actionPointThisTurn >= specialAttackMinActionPoint &&
            specialAttackCost <= GameManager.Instance.actionPointLeft &&
            currentMinion.specialAttack.cooldownLeft <= 0)
        {
            //Toute les tile qui peuvent etre un target d'attaque potentielle
            Dictionary<Vector2, float> AttackTargetsValue = new Dictionary<Vector2, float>();
            foreach (KeyValuePair<Vector2, float> coord in AttackTargetTilesValue(3, specialAttackTarget))
                AttackTargetsValue.Add(coord.Key, coord.Value);

            //Regarde toute les target tile pour determiner qu'elle est la meilleur accessible
            Tile bestTarget = BestAccessibleAttackTarget(AttackTargetsValue, currentMinion.specialAttack);



            //Si les meilleure target accessible n'est pas égale a null 
            if (bestTarget != null)
            {
                float bestTargetValue = AttackTargetsValue[bestTarget.coords];

                //Calcule la valeur des position possible pour etre dans le range de la meilleure target accessible possible
                Dictionary<Vector2, float> AttackPosForInRange = new Dictionary<Vector2, float>();
                foreach (KeyValuePair<Vector2, float> coord in PositionTilesValue(bestTargetValue, PossiblePositionTileForAttack(bestTarget, currentMinion.specialAttack)))
                    AttackPosForInRange.Add(coord.Key, coord.Value);

                Tile position = BestPosition(AttackPosForInRange);
                if (position == null)
                {
                    if (currentMinion.secondAttack.rangeType == RangeType.onSelf)
                    {
                        if (bestTarget == currentMinion.currentTile)
                            CreateAttackAction(null, bestTarget, bestTargetValue, specialAttackCost, 3, "Attack");
                    }
                    else
                        CreateAttackAction(null, bestTarget, bestTargetValue, specialAttackCost, 3, "Attack");
                }
                else if (specialAttackCost + movementCost <= GameManager.Instance.actionPointLeft && currentMinion.stundTurnLeft <= 0)
                    CreateAttackAction(position, bestTarget, bestTargetValue, specialAttackCost + movementCost, 3, "Movement and Attack");
            }
        }
    }

    public void SetMovementAction()
    {
        //calcule le cout de l'action
        MinionPathFinding pathFinding = new MinionPathFinding();
        int movementCost = (pathFinding.OnTackle(currentMinion) ? 3 : (currentMinion.haveMoved ? 0 : 1));
        if (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge)
            movementCost *= 2;

        //Si le minion a besoin de fuire
        if (movementCost <= GameManager.Instance.actionPointLeft)
        {
            if (currentMinion.stundTurnLeft <= 0)
            {
                if (GetChaseValue() > 0)
                {
                    Tile chasePosition = null;
                    float bestValue = enemyDistancefield[currentMinion.currentTile.coords];
                    foreach (var tile in pathFinding.GetPossibleDestinationAfterTackle(currentMinion))
                    {
                        if (enemyDistancefield[tile.coords] < bestValue)
                        {
                            if(enemyDistancefield[tile.coords] >= chaseDistanceTarget)
                            {
                                bestValue = enemyDistancefield[tile.coords];
                                chasePosition = tile;
                            }
                        }                      
                    }

                    if (chasePosition != null && chasePosition != currentMinion.currentTile)
                        CreateMovementAction(chasePosition, null, GetChaseValue(), movementCost, 0, "Get Closer");
                }
            
                //regarde pour fuire
                if (GetFleeValue() > 0)
                {
                    Tile destination = FarthestTileFromEnemy();
                    if (destination != null && destination != currentMinion.currentTile)
                        CreateMovementAction(destination, null, GetFleeValue(), movementCost, 0, "Fleeing");
                }

                //Regarde pour la protection
                Dictionary<Vector2, float> positionProtectionValue = new Dictionary<Vector2, float>();
                foreach (KeyValuePair<Vector2, float> coord in ProtectionTileValue())
                    positionProtectionValue.Add(coord.Key, coord.Value);

                if (positionProtectionValue.Count > 0)
                {
                    Tile protectionPosition = BestPosition(positionProtectionValue);

                    if (protectionPosition != null && protectionPosition != currentMinion.currentTile)
                    {
                        float protectionPositionValue = positionProtectionValue[protectionPosition.coords];
                        CreateMovementAction(protectionPosition, null, protectionPositionValue, movementCost, 0, "Protection");
                    }
                }
            }        
        }
    }



    //Cherche les tiles si possible pour etre dans le range d'attaque de la tile et l'attaque donné
    List<Tile> PossiblePositionTileForAttack(Tile targetTile, MinionAttack attack)
    {
        MinionPathFinding pathFinder = new MinionPathFinding();
        attackManager = new MinionAttackRange();

        List<Tile> possibleDestination = new List<Tile>();

        if (attack.rangeType != RangeType.onSelf)
        {
            if (!attack.noLineOfSight)
            {
                attackManager.GetPossibleAttackRange(targetTile, attack);
                foreach (var tile in attackManager.GetAttackRangeWithVision(targetTile, attack))
                    if (pathFinder.GetPossibleDestinationAfterTackle(currentMinion).Contains(tile) || tile == currentMinion.currentTile)
                        possibleDestination.Add(tile);
            }
            else
                foreach (var tile in attackManager.GetPossibleAttackRange(targetTile, attack))
                    if (pathFinder.GetPossibleDestinationAfterTackle(currentMinion).Contains(tile) || tile == currentMinion.currentTile)
                        possibleDestination.Add(tile);
        }
        else
        {
            if (pathFinder.GetPossibleDestinationAfterTackle(currentMinion).Contains(targetTile) || targetTile == currentMinion.currentTile)
                possibleDestination.Add(targetTile);
        }


        return possibleDestination;
    }

    //Cherche toute les targets tiles possible pour attaquer avec l'attaque donné et leur donne une valeur
    Dictionary<Vector2, float> AttackTargetTilesValue(int whichAttack, MinionTarget target)
    {
        MinionAttackRange attackRangeManager = new MinionAttackRange();
        List<Tile> attackTarget = new List<Tile>();
        MinionAttack attack = null;

        switch (whichAttack)
        {
            case 1: attack = currentMinion.firstAttack;
                break;
            case 2: attack = currentMinion.secondAttack;
                break;
            case 3: attack = currentMinion.specialAttack;
                break;
        }

        if (attack.rangeType != RangeType.onSelf)
        {
            if (target == MinionTarget.Enemy)
            {
                foreach (var minion in EnemyMinion())
                {
                    attackTarget.Add(minion.currentTile);

                    foreach (var tile in attackRangeManager.GetAttackZone(currentMinion.currentTile, minion.currentTile, attack))
                    {
                        if (!attackTarget.Contains(tile))
                            if(tile.haveObstacle == false)
                                attackTarget.Add(tile);
                    }
                }
            }

            if (target == MinionTarget.Ally)
            {
                foreach (var minion in AllyMinion())
                {
                    attackTarget.Add(minion.currentTile);

                    foreach (var tile in attackRangeManager.GetAttackZone(currentMinion.currentTile, minion.currentTile, attack))
                    {
                        if (!attackTarget.Contains(tile))
                            if (tile.haveObstacle == false)
                                attackTarget.Add(tile);
                    }
                }
            }
        }
        else
        {
            if (target == MinionTarget.Enemy)
            {
                foreach (var minion in EnemyMinion())
                    foreach (var tile in attackRangeManager.GetAttackZone(currentMinion.currentTile, minion.currentTile, attack))
                        if (!attackTarget.Contains(tile))
                            if (tile.haveObstacle == false)
                                attackTarget.Add(tile);
            }

            if (target == MinionTarget.Ally)
            {
                foreach (var minion in AllyMinion())
                    foreach (var tile in attackRangeManager.GetAttackZone(currentMinion.currentTile, minion.currentTile, attack))
                        if (!attackTarget.Contains(tile))
                            if (tile.haveObstacle == false)
                                attackTarget.Add(tile);
            }
        }
       

        Dictionary<Vector2, float> targetTiles = new Dictionary<Vector2, float>();
        foreach (var tile in attackTarget)
        {
            if (!targetTiles.ContainsKey(tile.coords))
            {
                float targetTileValue = 0;
                if (target == MinionTarget.Enemy)
                {
                    targetTileValue = AttackTargetCalculValue(tile, whichAttack);
                    if (tile.currentMinion != null)
                        if (currentMinion.myMinion != tile.currentMinion.myMinion)
                            targetTileValue += 1;
                }
                if (target == MinionTarget.Ally)
                {
                    targetTileValue = AttackTargetCalculValue(tile, whichAttack);
                    if (tile.currentMinion != null)
                        if (currentMinion.myMinion == tile.currentMinion.myMinion)
                            targetTileValue += 1;
                }


                targetTiles.Add(tile.coords, targetTileValue);
                              
                if (debugCheck && whichAttack == 3)
                {
                    TextMesh t = CreatWorldText(tile.transform, targetTileValue.ToString("F2"), Vector3.up * 0.5f, 5, Color.red, TextAnchor.MiddleCenter, 3);
                    text.Add(t.gameObject);
                }
            }          
        }

        return targetTiles;
    }

    //Determine la valeur des position possible parmis la liste et parmis la valeur de la target tile associé
    Dictionary<Vector2, float> PositionTilesValue(float bestTargetValue, List<Tile> tiles)
    {
        Dictionary<Vector2, float> position = new Dictionary<Vector2, float>();

        foreach (var posTile in tiles)
        {
            float positionValue = bestTargetValue;
        
            positionValue -= DistanceBetweenTile(currentMinion.currentTile, posTile) / 15;
  

            if (!position.ContainsKey(posTile.coords))
                position.Add(posTile.coords, positionValue);
        }

        return position;
    }

    //Cherche tout le tile qui permettre de bloquer la ligne de vue sur les minion allier pour les proteger
    Dictionary<Vector2, float> ProtectionTileValue()
    {
        MinionPathFinding pathFinder = new MinionPathFinding();
        List<Tile> reachableTile = new List<Tile>();
        foreach (var tile in pathFinder.GetPossibleDestinationAfterTackle(currentMinion))
            reachableTile.Add(tile);

        reachableTile.Add(currentMinion.currentTile);

        Dictionary<Vector2, float> protectionPositionValue = new Dictionary<Vector2, float>();

        foreach (var enemy in EnemyMinion())
        {
            foreach (var ally in AllyMinion())
            {
                if (ally == currentMinion)
                    continue;
                else
                    if (ally.GetComponent<MinionAI>().GetProtectionValue(currentMinion) < GetProtectionValue(ally))
                        continue;

                List<Tile> tileInLine = new List<Tile>();

                bool haveNoVision = false;

                Vector3 origin = enemy.currentTile.worldPos + Vector3.up;
                Vector3 direction = (ally.currentTile.worldPos + Vector3.up) - (enemy.currentTile.worldPos + Vector3.up);
                float distance = Vector3.Distance(enemy.currentTile.worldPos, ally.currentTile.worldPos);

                RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance, LayerMask.GetMask("AttackRangeCheck"));
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.GetComponentInParent<Tile>() != null)
                    {
                        Tile hitTile = hit.collider.GetComponentInParent<Tile>();

                        if (hitTile != enemy.currentTile && hitTile != ally.currentTile)
                        {
                            if (hitTile.filled && hitTile != currentMinion.currentTile)
                                haveNoVision = true;
                            else if (hitTile.haveObstacle && !hitTile.waterObstacle)
                                haveNoVision = true;

                            tileInLine.Add(hitTile);
                        }
                    }
                }

                float healthAmount = ally.currentHealth / ally.maxHealth;
                float protectionNeededFactor = GetProtectionValue(ally); ;

                if (haveNoVision == false)
                {
                    foreach (var tile in tileInLine)
                    {
                        if (reachableTile.Contains(tile))
                        {

                            if (!protectionPositionValue.ContainsKey(tile.coords))
                                protectionPositionValue.Add(tile.coords, protectionNeededFactor - (DistanceBetweenTile(currentMinion.currentTile, tile) / 15));
                            else
                                protectionPositionValue[tile.coords] += protectionNeededFactor - (DistanceBetweenTile(currentMinion.currentTile, tile) / 15);
                        }
                    }
                }
            }
        }

        return protectionPositionValue;
    }

    //Cherche toute les tile qui Tacklerais un enemy
    List<Tile> TackleTile()
    {
        List<Tile> tacklePosition = new List<Tile>();
        MinionPathFinding pathFinder = new MinionPathFinding();
        List<Tile> reachableTile = new List<Tile>();
        foreach (var tile in pathFinder.GetPossibleDestinationAfterTackle(currentMinion))
            reachableTile.Add(tile);  

        foreach (var tacklableTile in TacklableEnemy())
        {
            foreach (var lineTile in tacklableTile.lineTilesList)
                if (reachableTile.Contains(lineTile))
                    tacklePosition.Add(lineTile);        
        }
              
        return tacklePosition;
    }



    //Renvoie la Tile la plus roche possible pour le deplacement du minion de la target Tile
    Tile ClosestPositionFromTile(Dictionary<Vector2, float> targetTileDictionary)
    {
        MinionPathFinding pathFinder = new MinionPathFinding();

        //Si ne peut etre dans le range de aucune attaque, se dirige vers la meilleure sible selon la valeur
        Tile bestTargetToReash = null;
        float bestPositionValue = 0;
        foreach (KeyValuePair<Vector2, float> coord in targetTileDictionary)
        {
            if (coord.Value > bestPositionValue)
            {
                bestPositionValue = coord.Value;
                bestTargetToReash = GameManager.Instance.tilesDictionary[coord.Key];             
            }
        }


        Tile closestTile = null;
        int bestDistanceValue = Mathf.RoundToInt(Mathf.Abs(currentMinion.currentTile.coords.x - bestTargetToReash.coords.x) + Mathf.Abs(currentMinion.currentTile.coords.y - bestTargetToReash.coords.y));
        foreach (var tile in pathFinder.GetPossibleDestinationAfterTackle(currentMinion))
        {
            int distanceFromTarget = Mathf.RoundToInt(Mathf.Abs(bestTargetToReash.coords.x - tile.coords.x) + Mathf.Abs(bestTargetToReash.coords.y - tile.coords.y));

            if (distanceFromTarget < bestDistanceValue)
            {
                bestDistanceValue = distanceFromTarget;
                closestTile = tile;
            }            
        }

        return closestTile;
    }

    //Cherche la tile la plus éloigner d'un enemy pour la fuite
    Tile FarthestTileFromEnemy()
    {
        MinionPathFinding pathFinder = new MinionPathFinding();

        Tile bestTile = currentMinion.currentTile;
        float bestValue = enemyDistancefield[currentMinion.currentTile.coords];
        foreach (var tile in pathFinder.GetPossibleDestinationAfterTackle(currentMinion))
            if(bestValue < enemyDistancefield[tile.coords])
            {
                bestValue = enemyDistancefield[tile.coords];
                bestTile = tile;
            }

        return bestTile;
    }

    //Regarde toute les possible position tile  du dictionnaire pour determiner qu'elle est la meilleur accessible
    Tile BestPosition(Dictionary<Vector2, float> positionDictionary)
    {
        float bestPositionValue = 0;
        Tile bestPosition = null;
        foreach (KeyValuePair<Vector2, float> coord in positionDictionary)
        {
            if (bestPositionValue < coord.Value)
            {
                bestPositionValue = coord.Value;
                bestPosition = (coord.Key == currentMinion.currentTile.coords ? null : GameManager.Instance.tilesDictionary[coord.Key]);
            }
        }


        return bestPosition;
    }

    //Regarde toute les target tile pour determiner qu'elle est la meilleur accessible pour attaquer
    Tile BestAccessibleAttackTarget(Dictionary<Vector2, float> attackTargetDictionary, MinionAttack attack)
    {
        float bestTargetValue = 0;
        Tile bestTarget = null;
        foreach (KeyValuePair<Vector2, float> coord in attackTargetDictionary)
        {
            if (coord.Value > bestTargetValue)
            {
                Tile targetTile = GameManager.Instance.tilesDictionary[coord.Key];
                if (PossiblePositionTileForAttack(targetTile, attack).Count > 0)
                {
                    bestTargetValue = coord.Value;
                    bestTarget = GameManager.Instance.tilesDictionary[coord.Key];
                }
            }
        }

        return bestTarget;
    }

    float BestAttackTargetValue(Dictionary<Vector2, float> attackTargetDictionary)
    {
        //Si ne peut etre dans le range de aucune attaque, se dirige vers la meilleure sible selon la valeur
        float bestPositionValue = 0;
        foreach (KeyValuePair<Vector2, float> coord in attackTargetDictionary)
            if (coord.Value > bestPositionValue)
                bestPositionValue = coord.Value;

        return bestPositionValue;
    }




    //Renvoie la distance en coordonné d'un tile a l'autre
    float DistanceBetweenTile(Tile posTile, Tile targetTile)
    {
        return Mathf.Abs(posTile.coords.x - targetTile.coords.x) + Mathf.Abs(posTile.coords.y - targetTile.coords.y);
    }

    //Cherche la valeur de fuite ou de chase
    float GetFleeValue()
    {
        float healthAmount = currentMinion.currentHealth / currentMinion.maxHealth;
        float effectFactor = (currentMinion.flowerSkinTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.SkinFlower ? 2 : 1);
        float fleeValue = Mathf.Lerp(fleeHealthRange.x, fleeHealthRange.y, healthAmount) * effectFactor;

        if (enemyDistancefield[currentMinion.currentTile.coords] <= proximityFleeingTreshold)
            return fleeValue;
        else
            return 0;
    }
    float GetProtectionValue(Minion allyMinion)
    {
        float healthAmount = allyMinion.currentHealth / allyMinion.maxHealth;
        float effectFactor = (currentMinion.flowerSkinTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.SkinFlower ? 0.5f : 1);
        float protectionValue = Mathf.Lerp(allyHealthForProtectionRange.x, allyHealthForProtectionRange.y, healthAmount) * effectFactor;
        if(currentMinion.invulnerableTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Invulnerable)
            if (GetFleeValue() <= protectionValue)
                protectionValue *= 2;

        return protectionValue;
    }

    float GetChaseValue()
    {
        float healthAmount = currentMinion.currentHealth / currentMinion.maxHealth;
        return Mathf.Lerp(chaseHealthRange.x, chaseHealthRange.y, healthAmount) * (currentMinion.flowerSkinTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.SkinFlower ? 0.5f : 1);
    }

    //Regarde la valeur de la target tile donné celon les minions qui se trouvent dessus ou dans la zone d'attaque
    float AttackTargetCalculValue(Tile targetTile, int whichAttack)
    {
        float finalValue = 0;
        attackManager = new MinionAttackRange();
        List<Minion> minionInZone = new List<Minion>();

        MinionAttack attack = null;
        Vector2 distanceFactor = Vector2.zero;
        Vector2 healthFactor = Vector2.zero;
        Vector2 resistanceFactor = Vector2.zero;
        Vector2 actionPointFactor = Vector2.zero;
        MinionTarget target = MinionTarget.Ally;

        switch (whichAttack)
        {
            case 1:
                attack = currentMinion.firstAttack;
                target = firstAttackTarget;
                distanceFactor = firstAttackDistanceFactorRange;
                healthFactor = firstAttackHealthFactorRange;
                resistanceFactor = firstAttackResistanceFactorRange;
                actionPointFactor = firstAttackActionPointFactorRange;
                break;
            case 2:
                attack = currentMinion.secondAttack;
                target = secondAttackTarget;
                distanceFactor = secondAttackDistanceFactorRange;
                healthFactor = secondAttackHealthFactorRange;
                resistanceFactor = secondAttackResistanceFactorRange;
                actionPointFactor = secondAttackActionPointFactorRange;
                break;
            case 3:
                attack = currentMinion.specialAttack;
                target = specialAttackTarget;
                distanceFactor = specialAttackDistanceFactorRange;
                healthFactor = specialAttackHealthFactorRange;
                resistanceFactor = specialAttackResistanceFactorRange;
                actionPointFactor = specialAttackActionPointFactorRange;
                break;
        }

        bool isPhysiscalDamage = attack.damageType == DamageType.Physical;


        foreach (var zoneTile in attackManager.GetAttackZone(currentMinion.currentTile, targetTile, attack))
            if (zoneTile.currentMinion != null)
                minionInZone.Add(zoneTile.currentMinion);

        foreach (var minion in minionInZone)
        {
            if(target == MinionTarget.Enemy)
            {
                if (minion.myMinion != currentMinion.myMinion)
                {
                    float currentHealth = minion.currentHealth;
                    float maxHealth = minion.maxHealth;

                    float distanceFactorValue = Remap(DistanceBetweenTile(currentMinion.currentTile, targetTile), 0, 15, distanceFactor.x, distanceFactor.y);
                    float healthFactorValue = Remap(currentHealth / maxHealth, 0, 1, healthFactor.x, healthFactor.y);
                    float resistanceFactorValue = Remap(isPhysiscalDamage ? minion.physicalResist : minion.magicalResist, -100, 100, resistanceFactor.x, resistanceFactor.y);
                    float actionPointFactorValue = Remap(GameManager.Instance.actionPointThisTurn, 3, 10, actionPointFactor.x, actionPointFactor.y);

                    float effectFactorValue = (currentMinion.superForceTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.SuperForce ? 2 : (currentMinion.weakenedTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Weakened ? 0.5f : 1));

                    finalValue += (distanceFactorValue + healthFactorValue + resistanceFactorValue + actionPointFactorValue) * effectFactorValue;
                }
                else if (attack.canDamageAllyMinions)
                {
                    float distanceFactorValue = (distanceFactor.x + distanceFactor.y) / 2;
                    float healthFactorValue = (healthFactor.x + healthFactor.y) / 2;
                    float resistanceFactorValue = (resistanceFactor.x + resistanceFactor.y) / 2;
                    float actionPointFactorValue = (actionPointFactor.x + actionPointFactor.y) / 2;

                    float effectFactorValue = (currentMinion.superForceTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.SuperForce ? 2 : (currentMinion.weakenedTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Weakened ? 0.5f : 1));

                    finalValue -= (distanceFactorValue + healthFactorValue + resistanceFactorValue + actionPointFactorValue) * effectFactorValue;
                }
            }
            
            if (target == MinionTarget.Ally)
            {
                if (minion.myMinion == currentMinion.myMinion)
                {
                    float currentHealth = minion.currentHealth;
                    float maxHealth = minion.maxHealth;

                    float distanceFactorValue = Remap(DistanceBetweenTile(currentMinion.currentTile, targetTile), 0, 15, distanceFactor.x, distanceFactor.y);
                    float healthFactorValue = Remap(currentHealth / maxHealth, 0, 1, healthFactor.x, healthFactor.y);
                    float resistanceFactorValue = Remap(isPhysiscalDamage ? minion.physicalResist : minion.magicalResist, -100, 100, resistanceFactor.x, resistanceFactor.y);
                    float actionPointFactorValue = Remap(GameManager.Instance.actionPointThisTurn, 3, 10, actionPointFactor.x, actionPointFactor.y);

                    float effectFactorValue = (currentMinion.superForceTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.SuperForce ? 2 : (currentMinion.weakenedTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Weakened ? 0.5f : 1));

                    finalValue += (distanceFactorValue + healthFactorValue + resistanceFactorValue + actionPointFactorValue) * effectFactorValue;
                }
                else if (attack.canDamageAllyMinions)
                {
                    float distanceFactorValue = (distanceFactor.x + distanceFactor.y) / 2;
                    float healthFactorValue = (healthFactor.x + healthFactor.y) / 2;
                    float resistanceFactorValue = (resistanceFactor.x + resistanceFactor.y) / 2;
                    float actionPointFactorValue = (actionPointFactor.x + actionPointFactor.y) / 2;

                    float effectFactorValue = (currentMinion.superForceTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.SuperForce ? 2 : (currentMinion.weakenedTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Weakened ? 0.5f : 1));

                    finalValue -= (distanceFactorValue + healthFactorValue + resistanceFactorValue + actionPointFactorValue) * effectFactorValue;
                }
            }

            //  if (debugCheck && targetTile.currentMinion != null)
            //  {
            //      Debug.Log("Target Minion :   " + targetTile.currentMinion.MinionName +
            //              "   Heatlth Factor :   " + healthFactorMultiplier +
            //              "   Resistance Factor :   " + resistanceFactorMultiplier +
            //              "   Action Point Factor :   " + actionPointFactorMultiplier +
            //              "   FinalValue :   " + targetTileValue);
            //  }
        }

        return finalValue;
    }

  

    //Set pour chaque Tile la distance de l'enemy le plus proche
    void SetEnemyDistanceField()
    {
        enemyDistancefield.Clear();

        List<Tile> tileWithMinion = new List<Tile>();
        foreach (var minion in EnemyMinion())
        {
            tileWithMinion.Add(minion.currentTile);
        }

        foreach (var tile in GameManager.Instance.allTiles)
        {
            float closestDistance = 100;
            foreach (var minionTile in tileWithMinion)
            {
                float distance = DistanceBetweenTile(tile, minionTile);
                if (closestDistance > distance)              
                    closestDistance = distance;              
            }

            enemyDistancefield.Add(tile.coords, closestDistance);
        }
    }

    void CreateBuffAction(Tile position, Tile target, float value, int cost, int whichAttack, string description)
    {
        MinionActions action = new MinionActions(currentMinion, position, target, value, cost, whichAttack, description);

        MinionAIManager.Instance.buffActions.Add(action);
    }

    void CreateAttackAction(Tile position, Tile target, float value, int cost, int whichAttack, string description)
    {

        MinionActions action = new MinionActions(currentMinion, position, target, value, cost, whichAttack, description);

        MinionAIManager.Instance.attackActions.Add(action);
    }

    void CreateMovementAction(Tile position, Tile target, float value, int cost, int whichAttack, string description)
    {
        MinionActions action = new MinionActions(currentMinion, position, target, value, cost, whichAttack, description);

        MinionAIManager.Instance.movementActions.Add(action);
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public TextMesh CreatWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, int sortingOrder)
    {
        GameObject textObject = new GameObject("World_text", typeof(TextMesh));
        Transform transform = textObject.transform;
        transform.LookAt(Vector3.down);
        transform.localScale = Vector3.one * 0.1f;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = textObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.text = text;
        textMesh.fontSize = fontSize * 10;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
