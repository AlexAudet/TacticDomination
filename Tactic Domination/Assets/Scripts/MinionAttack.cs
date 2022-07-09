using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Photon.Pun;


public enum RangeType { Arround, X, Line, X_And_Line, onSelf };
public enum ZoneType { Arround, X, Vertical_Line, Horizontal_Line, Cross, Cross_And_X, Single_Tile, Custom, Oriented_Custom };
public enum DamageType { Physical, Magic, Push };
public enum EffectType { None, Stun, Silence, SuperForce, Freeze, Overcharge, Burn, FlowerSkin, Weakened, Invulnerable };
public enum ProjectilType { Linear, Semi_Circle };
public enum AttackMovementType { None, Push, Pull, Teleport, Change_Place };
public enum AttackMovementReferenceType { Minion, Target_Tile };
public enum AffectWhichMinon { Enemy, Ally, Both, None };

public class MinionAttack : MonoBehaviour
{
    public Minion currentMinion;

    [Indent, HideLabel, Title("Attack Name", TitleAlignment = TitleAlignments.Centered)]
    public string attackName;
    [Indent]
    public Sprite attackIcon;
    [Indent, PropertySpace(10, 0)]
    public float attackDuration;
    [Indent]
    public int actionPointCost;
    [Indent, PropertySpace(0, 10)]
    public int cooldown;
    [HideInInspector] public int cooldownLeft;


    public enum MovementType { Jump, Slide, Teleportation, With_Anim };
    [FoldoutGroup("Special Attribute")]
    [PropertySpace(10, 10), Indent]
    public bool moveToTargetTile;
    [FoldoutGroup("Special Attribute")]
    [Indent, PropertySpace(-10, 10), ShowIf("@this.moveToTargetTile == true")]
    public MovementType movemenTtype;

    #region Damage
    [FoldoutGroup("Damage"), Indent, PropertySpace(10, 0)]
    public DamageType damageType;
    [FoldoutGroup("Damage"), Indent, PropertySpace(0, 10)]
    public bool canDamageAllyMinions;
    [FoldoutGroup("Damage"), Indent, PropertySpace(-10, 10), ShowIf("@this.canDamageAllyMinions == true")]
    public bool canDamageSelf;

    [FoldoutGroup("Damage"), Indent, PropertySpace(10, 0), LabelText("Base Damage"), OnValueChanged("ClalculeAttackStat")]
    public Vector2 baseDamageRange = new Vector2(10, 20);
    [FoldoutGroup("Damage"), Indent, LabelText("% Per Level"), OnValueChanged("ClalculeAttackStat"), Range(0, 100)]
    public float damagePercentagePerLevel;

    [FoldoutGroup("Damage"), Indent, PropertySpace(10, 0), LabelText("Current Damage"),ReadOnly]
    public Vector2 damageRange;
    [FoldoutGroup("Damage"), Indent, PropertySpace(0, 10), LabelText("Max Damage"), ReadOnly]
    public Vector2 maxLevelDamage;

    #endregion

    #region Heal
    [FoldoutGroup("Heal"), Indent, PropertySpace(10,10)]
    public bool canHeal;
    [FoldoutGroup("Heal"), Indent, ShowIf("@this.canHeal == true")]
    public AffectWhichMinon healTarget;

    [FoldoutGroup("Heal"), Indent, PropertySpace(10, 0), LabelText("Base"), OnValueChanged("ClalculeAttackStat"), ShowIf("@this.canHeal == true")]
    public Vector2 baseHealRange = new Vector2(10, 20);
    [FoldoutGroup("Heal"), Indent, PropertySpace(0, 5), LabelText("% Per Level"), OnValueChanged("ClalculeAttackStat"), Range(0, 100), ShowIf("@this.canHeal == true")]
    public float healPercentagePerLevel;

    [FoldoutGroup("Heal"), Indent, PropertySpace(10, 0), ShowIf("@this.canHeal == true"), ReadOnly]
    public Vector2 healRange;
    [FoldoutGroup("Heal"), Indent, PropertySpace(0, 10), LabelText("Max"), ShowIf("@this.canHeal == true"), ReadOnly]
    public Vector2 maxLevelHeal;
    #endregion

    #region Effect
    [FoldoutGroup("Effect"), Indent, PropertySpace(10, 0)]
    public bool silence;
    [FoldoutGroup("Effect"), Indent(2), PropertySpace(5, 5), ShowIf("@this.silence == true")]
    public AffectWhichMinon snearTarget;

    [FoldoutGroup("Effect"), Indent]
    public bool stun;
    [FoldoutGroup("Effect"), Indent(2), PropertySpace(5, 5), ShowIf("@this.stun == true")]
    public AffectWhichMinon stunTarget;

    [FoldoutGroup("Effect"), Indent]
    public bool superForce;
    [FoldoutGroup("Effect"), Indent(2), PropertySpace(5, 0), ShowIf("@this.superForce == true")]
    public AffectWhichMinon superForceTarget;

    [FoldoutGroup("Effect"), Indent]
    public bool freeze;
    [FoldoutGroup("Effect"), Indent(2), PropertySpace(5, 5), ShowIf("@this.freeze == true")]
    public AffectWhichMinon freezeTarget;

    [FoldoutGroup("Effect"), Indent]
    public bool overcharge;
    [FoldoutGroup("Effect"), Indent(2), PropertySpace(5, 5), ShowIf("@this.overcharge == true")]
    public AffectWhichMinon overchargeTarget;

    [FoldoutGroup("Effect"), Indent]
    public bool burn;
    [FoldoutGroup("Effect"), Indent(2), PropertySpace(5, 0), ShowIf("@this.burn == true")]
    public AffectWhichMinon burnTarget;
    [FoldoutGroup("Effect"), Indent(2), PropertySpace(0, 5), ShowIf("@this.burn == true")]
    public int burnDamageAmount;

    [FoldoutGroup("Effect"), Indent]
    public bool flowerSkin;
    [FoldoutGroup("Effect"), Indent(2), PropertySpace(5, 5), ShowIf("@this.flowerSkin == true")]
    public AffectWhichMinon flowerSkinTarget;

    [FoldoutGroup("Effect"), Indent]
    public bool weakened;
    [FoldoutGroup("Effect"), Indent(2), PropertySpace(5, 5), ShowIf("@this.weakened == true")]
    public AffectWhichMinon weakenedTarget;

    [FoldoutGroup("Effect"), Indent]
    public bool invulnerable;
    [FoldoutGroup("Effect"), Indent(2), PropertySpace(5, 5), ShowIf("@this.invulnerable == true")]
    public AffectWhichMinon invulnerableTarget;


    [FoldoutGroup("Effect"), Indent, PropertySpace(10, 0), Range(0, 10), LabelText("Base Turn Amount"), OnValueChanged("ClalculeAttackStat")]
    public int baseTurnAmountEffect = 1;
    [FoldoutGroup("Effect"), Indent, PropertySpace(0, 5), Range(0, 100), LabelText("% Per Level"), OnValueChanged("ClalculeAttackStat")]
    public float turnAmountEffectPercentagePerLevel;

    [FoldoutGroup("Effect"), Indent, PropertySpace(10, 0), ReadOnly, LabelText("Current Turn Amount")]
    public int turnAmountEffect;
    [FoldoutGroup("Effect"), Indent, PropertySpace(0, 10), LabelText("Max Turn Amount"), ReadOnly]
    public int maxLevelTurnAmountEffect;
    #endregion

    #region Range
    [FoldoutGroup("Range"), Indent, PropertySpace(10, 10)]
    public RangeType rangeType;

    [FoldoutGroup("Range"), Indent, OnValueChanged("ClalculeAttackStat"), ShowIf("@this.rangeType != RangeType.onSelf"), LabelText("Base Range")]
    public Vector2 baseAttackRange;
    [FoldoutGroup("Range"), Indent, PropertySpace(0, 10), OnValueChanged("ClalculeAttackStat"), ShowIf("@this.rangeType != RangeType.onSelf"), Range(0,100), LabelText("% Per Level")]
    public float rangePourcentagePerLevel;

    [FoldoutGroup("Range"), Indent, ShowIf("@this.rangeType != RangeType.onSelf"), ReadOnly, LabelText("Current Range")]
    public Vector2 attackRange;
    [FoldoutGroup("Range"), Indent, PropertySpace(0, 10), ShowIf("@this.rangeType != RangeType.onSelf"), ReadOnly, LabelText("Max Range")]
    public Vector2 maxLevelAttackRange;

    [FoldoutGroup("Range"), Indent, ShowIf("@this.rangeType != RangeType.onSelf")]
    public bool noLineOfSight;
    [FoldoutGroup("Range"), Indent, ShowIf("@this.noLineOfSight == false && this.rangeType != RangeType.onSelf")]
    public bool canAttackOverWaterObstale;
    [FoldoutGroup("Range"), Indent, ShowIf("@this.rangeType != RangeType.onSelf")]
    public bool canSelectObstacle;
    [FoldoutGroup("Range"), Indent, PropertySpace(0, 10), ShowIf("@this.rangeType != RangeType.onSelf")]
    public bool cantSelectMinion;
    #endregion

    #region Zone
    [FoldoutGroup("Zone"), Indent, PropertySpace(10, 10), OnValueChanged("PreventIncoherenceInMovement")]
    public ZoneType zoneType;
    [FoldoutGroup("Zone"), Indent, OnValueChanged("PreventIncoherenceInMovement"), HideIf("@this.zoneType == ZoneType.Custom || this.zoneType == ZoneType.Single_Tile || this.zoneType == ZoneType.Custom || this.zoneType == ZoneType.Oriented_Custom")]
    public Vector2 zoneRange;
    [FoldoutGroup("Zone"), Indent, ShowIf("@this.zoneType == ZoneType.Custom")]
    public List<Vector2> customZone;
    [FoldoutGroup("Zone"), Indent, ShowIf("@this.zoneType == ZoneType.Oriented_Custom")]
    public List<Vector2> orientedCustomZone;
    #endregion

    #region Movement
    [FoldoutGroup("Movement"), Indent, PropertySpace(10, 10), OnValueChanged("PreventIncoherenceInMovement")]
    public AttackMovementType attackMovementType;
    [FoldoutGroup("Movement"), Indent, ShowIf("@this.attackMovementType == AttackMovementType.Push || this.attackMovementType == AttackMovementType.Pull")]
    public AttackMovementReferenceType referenceType;
    [FoldoutGroup("Movement"), Indent, ShowIf("@this.attackMovementType == AttackMovementType.Push || this.attackMovementType == AttackMovementType.Pull")]
    public float tileMovementSpeed = 10;
    [FoldoutGroup("Movement"), Indent, PropertySpace(0, 10), ShowIf("@this.attackMovementType == AttackMovementType.Push || this.attackMovementType == AttackMovementType.Pull")]
    public bool canSelfPushOrPull;

    [FoldoutGroup("Movement"), Indent, PropertySpace(10, 0), Range(0, 10), LabelText("Base Distance"), OnValueChanged("ClalculeAttackStat")]
    [ShowIf("@this.attackMovementType == AttackMovementType.Push || this.attackMovementType == AttackMovementType.Pull")]
    public int baseTileMovement = 1;
    [FoldoutGroup("Movement"), Indent, PropertySpace(0, 5), Range(0, 100), LabelText("% Per Level"), OnValueChanged("ClalculeAttackStat")]
    [ShowIf("@this.attackMovementType == AttackMovementType.Push || this.attackMovementType == AttackMovementType.Pull")]
    public float tileMovementPercentagePerLevel;

    [FoldoutGroup("Movement"), Indent, PropertySpace(10, 0), ReadOnly, LabelText("Current Distance")]
    [ShowIf("@this.attackMovementType == AttackMovementType.Push || this.attackMovementType == AttackMovementType.Pull")]
    public int tileMovementAmount;
    [FoldoutGroup("Movement"), Indent, PropertySpace(0, 10), LabelText("Max Distance"), ReadOnly]
    [ShowIf("@this.attackMovementType == AttackMovementType.Push || this.attackMovementType == AttackMovementType.Pull")]
    public int maxLeveltileMovementAmount;

    #endregion



    #region Projectils
    [FoldoutGroup("Projectil"), Indent, PropertySpace(10,10)]
    public bool ThrowProjectil;
    [FoldoutGroup("Projectil"), Indent, PropertySpace(0, 10), ShowIf("@this.ThrowProjectil == true")]
    public float spawnDelay;
    [FoldoutGroup("Projectil"), Indent, ShowIf("@this.ThrowProjectil == true")]
    public Projectil projectilPrefab;
    [FoldoutGroup("Projectil"), Indent, PropertySpace(0, 10), ShowIf("@this.ThrowProjectil == true")]
    public GameObject projectilMuzzle;
    [FoldoutGroup("Projectil"), Indent, PropertySpace(0, 5), ShowIf("@this.ThrowProjectil == true")]
    public ProjectilType projectilType;
    [FoldoutGroup("Projectil"), Indent, PropertySpace(0, 10), ShowIf("@this.ThrowProjectil == true")]
    public float projectilSpeed = 10;
    [FoldoutGroup("Projectil"), Indent, PropertySpace(-10, 10), ShowIf("@this.ThrowProjectil == true && this.projectilType == ProjectilType.Semi_Circle")]
    public float projectilHeight = 10;
    #endregion

    #region Trap
    [FoldoutGroup("Trap"), Indent, PropertySpace(10, 10)]
    public bool makeTrap;
    [FoldoutGroup("Trap"), Indent, ShowIf("@this.makeTrap == true")]
    public GameObject trapModel;

    [FoldoutGroup("Trap"), Indent, PropertySpace(10, 0), Range(0, 10), LabelText("Base Turn Amount"), OnValueChanged("ClalculeAttackStat"), ShowIf("@this.makeTrap == true")]
    public int baseTrapActiveTime = 1;
    [FoldoutGroup("Trap"), Indent, PropertySpace(0, 5), Range(0, 100), LabelText("% Per Level"), OnValueChanged("ClalculeAttackStat"), ShowIf("@this.makeTrap == true")]
    public float trapActiveTimePercentagePerLevel;

    [FoldoutGroup("Trap"), Indent, PropertySpace(10, 0), ReadOnly, LabelText("Current Turn Amount"), ShowIf("@this.makeTrap == true")]
    public int trapActiveTime;
    [FoldoutGroup("Trap"), Indent, PropertySpace(0, 10), LabelText("Max Turn AMount"), ReadOnly, ShowIf("@this.makeTrap == true")] 
    public int maxLevelTrapActiveTime;
    #endregion

    #region Obstacles
    [FoldoutGroup("Obstacle"), Indent, PropertySpace(10, 10)]
    public bool makeObstacle;
    [FoldoutGroup("Obstacle"), Indent, ShowIf("@this.makeObstacle == true")]
    public GameObject ObstacleModel;

    [FoldoutGroup("Obstacle"), Indent, PropertySpace(10, 0), Range(0, 10), LabelText("Base Turn Amount"), OnValueChanged("ClalculeAttackStat"), ShowIf("@this.makeObstacle == true")]
    public int baseObstacleActiveTime = 1;
    [FoldoutGroup("Obstacle"), Indent, PropertySpace(0, 5), Range(0, 100), LabelText("% Per Level"), OnValueChanged("ClalculeAttackStat"), ShowIf("@this.makeObstacle == true")]
    public float obstacleActiveTimePercentagePerLevel;

    [FoldoutGroup("Obstacle"), Indent, PropertySpace(10, 0), ReadOnly, LabelText("Current Turn Amount"), ShowIf("@this.makeObstacle == true")]
    public int obstacleActiveTime;
    [FoldoutGroup("Obstacle"), Indent, PropertySpace(0, 10), LabelText("Max Turn AMount"), ReadOnly, ShowIf("@this.makeObstacle == true")]
    public int maxLevelObstacleActiveTime;
    #endregion

    #region Glyph
    [FoldoutGroup("Glyph"), Indent, PropertySpace(10, 10)]
    public bool makeGlyph;
    [FoldoutGroup("Glyph"), Indent, ShowIf("@this.makeGlyph == true")]
    public Glyph.GlyphTarget glyphTarget;
    [FoldoutGroup("Glyph"), Indent, ShowIf("@this.makeGlyph == true")]
    public Glyph.GlyphType glyphType;

    [FoldoutGroup("Glyph"), Indent, PropertySpace(20, 0), LabelText("Base Damage"), OnValueChanged("ClalculeAttackStat"), ShowIf("@this.makeGlyph == true")]
    public Vector2 baseGlyphDamageRange = new Vector2(10, 20);
    [FoldoutGroup("Glyph"), Indent, LabelText("% Per Level"), OnValueChanged("ClalculeAttackStat"), Range(0, 100), ShowIf("@this.makeGlyph == true")]
    public float glyphDamageRangePercentagePerLevel;

    [FoldoutGroup("Glyph"), Indent, PropertySpace(10, 0), LabelText("Current Damage"), ReadOnly, ShowIf("@this.makeGlyph == true")]
    public Vector2 glyphDamageRange;
    [FoldoutGroup("Glyph"), Indent, PropertySpace(0, 10), LabelText("Max Damage"), ReadOnly, ShowIf("@this.makeGlyph == true")]
    public Vector2 maxLevelGlyphDamageRange;

    [FoldoutGroup("Glyph"), Indent, PropertySpace(20, 0), Range(0, 10), LabelText("Base Turn Amount"), OnValueChanged("ClalculeAttackStat"), ShowIf("@this.makeGlyph == true")]
    public int baseGlyphTurnAmount = 1;
    [FoldoutGroup("Glyph"), Indent, PropertySpace(0, 5), Range(0, 100), LabelText("% Per Level"), OnValueChanged("ClalculeAttackStat"), ShowIf("@this.makeGlyph == true")]
    public float glyphTurnAmountPercentagePerLevel;

    [FoldoutGroup("Glyph"), Indent, PropertySpace(10, 0), ReadOnly, LabelText("Current Turn Amount"), ShowIf("@this.makeGlyph == true")]
    public int glyphTurnAmount;
    [FoldoutGroup("Glyph"), Indent, PropertySpace(0, 10), LabelText("Max Turn AMount"), ReadOnly, ShowIf("@this.makeGlyph == true")]
    public int maxLevelGlyphTurnAmount;
    #endregion

    #region FeedBack
    public enum FeedBackHappening { On_Execute_Attack, OnAnim };
    [FoldoutGroup("Feedback")]
    [PropertySpace(10, 10), Indent]
    public bool CameraShake;
    [FoldoutGroup("Feedback")]
    [Indent(2), ShowIf("@this.CameraShake == true")]
    public FeedBackHappening cameraShakeType;
    [FoldoutGroup("Feedback")]
    [Indent(2), ShowIf("@this.CameraShake == true")]
    public float Strenght = 2;
    [FoldoutGroup("Feedback")]
    [Indent(2), ShowIf("@this.CameraShake == true")]
    public int vibrationAmount = 10;
    [FoldoutGroup("Feedback")]
    [Indent(2), PropertySpace(0, 10), ShowIf("@this.CameraShake == true")]
    public float shakeDuration = 0.3f;

    [FoldoutGroup("Feedback")]
    [PropertySpace(0, 10), Indent]
    public bool slowMotion;
    [FoldoutGroup("Feedback")]
    [Indent(2), ShowIf("@this.slowMotion == true")]
    public FeedBackHappening slowMotionType;
    [FoldoutGroup("Feedback")]
    [Indent(2), ShowIf("@this.slowMotion == true")]
    public float slowMotionAmount;
    [FoldoutGroup("Feedback")]
    [Indent(2), PropertySpace(0, 10), ShowIf("@this.slowMotion == true")]
    public float slowMotionDuration;


    [FoldoutGroup("Feedback")]
    [PropertySpace(0, 10), Indent]
    public bool camZoom;
    [FoldoutGroup("Feedback")]
    [Indent(2), ShowIf("@this.camZoom == true")]
    public FeedBackHappening camZoomType;
    [FoldoutGroup("Feedback")]
    [Indent(2), ShowIf("@this.camZoom == true")]
    public float camZoomAmount = 7;
    [FoldoutGroup("Feedback")]
    [Indent(2), ShowIf("@this.camZoom == true")]
    public float camZoomSpeed = 2;
    [FoldoutGroup("Feedback")]
    [Indent(2), PropertySpace(0, 10), ShowIf("@this.camZoom == true")]
    public float camZoomDuration = 1;
    #endregion

    #region VFX
    public enum VFXSpawnType { OnCast, On_Execute_Attack, OnTime };
    [FoldoutGroup("VFX")]
    [Indent()]
    public bool VfxOnTargetTile;
    [FoldoutGroup("VFX")]
    [Indent(2), PropertySpace(10, 0), ShowIf("@this.VfxOnTargetTile == true")]
    public ParticleSystem vfxOnTargetTilePrefab;
    [FoldoutGroup("VFX")]
    [Indent(2), PropertySpace(0, 10), ShowIf("@this.VfxOnTargetTile == true")]
    public VFXSpawnType vfxOnTargetTileSpawnType;
    [FoldoutGroup("VFX")]
    [Indent(2), PropertySpace(-10, 10), ShowIf("@this.VfxOnTargetTile == true")]
    public float vfxOnTargetTileSpawnTime;

    [FoldoutGroup("VFX")]
    [Indent()]
    public bool VfxOnMinionTile;
    [FoldoutGroup("VFX")]
    [Indent(2), PropertySpace(10, 0), ShowIf("@this.VfxOnMinionTile == true")]
    public ParticleSystem vfxOnMinionPrefab;
    [FoldoutGroup("VFX")]
    [Indent(2), PropertySpace(0, 10), ShowIf("@this.VfxOnMinionTile == true")]
    public VFXSpawnType vfxOnMinionTileSpawnType;
    [FoldoutGroup("VFX")]
    [Indent(2), PropertySpace(-10, 10), ShowIf("@this.VfxOnMinionTile == true")]
    public float vfxOnMinionTileSpawnTime;
    #endregion

    private void Awake()
    {
        currentMinion = GetComponent<Minion>();
    }

    public void ClalculeAttackStat()
    {
        if (currentMinion == null)
            currentMinion = GetComponent<Minion>();

        //Damage
        float processMinDamage = baseDamageRange.x;
        float processMaxDamage = baseDamageRange.y;
        for (int i = 1; i < 10; i++)
        {
            if (i == currentMinion.minionLevel)
            {
                damageRange.x = Mathf.RoundToInt(processMinDamage);
                damageRange.y = Mathf.RoundToInt(processMaxDamage);
            }

            processMinDamage += ((processMinDamage * damagePercentagePerLevel) / 100);
            processMaxDamage += ((processMaxDamage * damagePercentagePerLevel) / 100);


        }
        maxLevelDamage.x = Mathf.RoundToInt(processMinDamage);
        maxLevelDamage.y = Mathf.RoundToInt(processMaxDamage);

        if (currentMinion.minionLevel == 10)
        {
            damageRange.x = Mathf.RoundToInt(maxLevelDamage.x);
            damageRange.y = Mathf.RoundToInt(maxLevelDamage.y);
        }

        //Heal
        float processMinHeal = baseHealRange.x;
        float processMaxHeal = baseHealRange.y;
        for (int i = 1; i < 10; i++)
        {
            if (i == currentMinion.minionLevel)
            {
                healRange.x = Mathf.RoundToInt(processMinHeal);
                healRange.y = Mathf.RoundToInt(processMaxHeal);
            }

            processMinHeal += ((processMinHeal * healPercentagePerLevel) / 100);
            processMaxHeal += ((processMaxHeal * healPercentagePerLevel) / 100);


        }
        maxLevelHeal.x = Mathf.RoundToInt(processMinHeal);
        maxLevelHeal.y = Mathf.RoundToInt(processMaxHeal);

        if (currentMinion.minionLevel == 10)
        {
            healRange.x = Mathf.RoundToInt(maxLevelHeal.x);
            healRange.y = Mathf.RoundToInt(maxLevelHeal.y);
        }

        //AttackRange
        float processMaxRange = baseAttackRange.y;
        for (int i = 1; i < 10; i++)
        {
            if (i == currentMinion.minionLevel)
                attackRange.y = Mathf.RoundToInt(processMaxRange);

            processMaxRange += ((processMaxRange * rangePourcentagePerLevel) / 100);
        }
        attackRange.x = baseAttackRange.x;
        maxLevelAttackRange.x = baseAttackRange.x;
        maxLevelAttackRange.y = Mathf.RoundToInt(processMaxRange);

        if (currentMinion.minionLevel == 10)
            attackRange.y = Mathf.RoundToInt(maxLevelAttackRange.y);


        //Effect Turn Amount
        float processTurnEffect = baseTurnAmountEffect;
        for (int i = 1; i < 10; i++)
        {
            if (i == currentMinion.minionLevel)
                turnAmountEffect = Mathf.RoundToInt(processTurnEffect);

            processTurnEffect += ((processTurnEffect * turnAmountEffectPercentagePerLevel) / 100);
        }
        maxLevelTurnAmountEffect = Mathf.RoundToInt(processTurnEffect);

        if (currentMinion.minionLevel == 10)
            turnAmountEffect = Mathf.RoundToInt(maxLevelTurnAmountEffect);

        //push distance
        float processPushDistt = baseTileMovement;
        for (int i = 1; i < 10; i++)
        {
            if (i == currentMinion.minionLevel)
                tileMovementAmount = Mathf.RoundToInt(processPushDistt);

            processPushDistt += ((processPushDistt * tileMovementPercentagePerLevel) / 100);
        }
        maxLeveltileMovementAmount = Mathf.RoundToInt(processPushDistt);

        if (currentMinion.minionLevel == 10)
            tileMovementAmount = Mathf.RoundToInt(maxLeveltileMovementAmount);

        //trap active time
        float processTrapActiveTime = baseTrapActiveTime;
        for (int i = 1; i < 10; i++)
        {
            if (i == currentMinion.minionLevel)
                trapActiveTime = Mathf.RoundToInt(processTrapActiveTime);

            processTrapActiveTime += ((processTrapActiveTime * trapActiveTimePercentagePerLevel) / 100);
        }
        maxLevelTrapActiveTime = Mathf.RoundToInt(processTrapActiveTime);

        if (currentMinion.minionLevel == 10)
            trapActiveTime = Mathf.RoundToInt(maxLevelTrapActiveTime);

        //obstacle active time
        float processObstacleActiveTime = baseObstacleActiveTime;
        for (int i = 1; i < 10; i++)
        {
            if (i == currentMinion.minionLevel)
                obstacleActiveTime = Mathf.RoundToInt(processObstacleActiveTime);

            processObstacleActiveTime += ((processObstacleActiveTime * obstacleActiveTimePercentagePerLevel) / 100);
        }
        maxLevelObstacleActiveTime = Mathf.RoundToInt(processObstacleActiveTime);

        if (currentMinion.minionLevel == 10)
            obstacleActiveTime = Mathf.RoundToInt(maxLevelObstacleActiveTime);

        //glyph active time
        float processGlyphActiveTime = baseGlyphTurnAmount;
        for (int i = 1; i < 10; i++)
        {
            if (i == currentMinion.minionLevel)
                glyphTurnAmount = Mathf.RoundToInt(processGlyphActiveTime);

            processGlyphActiveTime += ((processGlyphActiveTime * glyphTurnAmountPercentagePerLevel) / 100);
        }
        maxLevelGlyphTurnAmount = Mathf.RoundToInt(processGlyphActiveTime);

        if (currentMinion.minionLevel == 10)
            glyphTurnAmount = Mathf.RoundToInt(maxLevelGlyphTurnAmount);

        //Glyph Damage
        float processMinGlyphDamage = baseGlyphDamageRange.x;
        float processMaxGlyphDamage = baseGlyphDamageRange.y;
        for (int i = 1; i < 10; i++)
        {
            if (i == currentMinion.minionLevel)
            {
                glyphDamageRange.x = Mathf.RoundToInt(processMinGlyphDamage);
                glyphDamageRange.y = Mathf.RoundToInt(processMaxGlyphDamage);
            }

            processMinGlyphDamage += ((processMinGlyphDamage * glyphDamageRangePercentagePerLevel) / 100);
            processMaxGlyphDamage += ((processMaxGlyphDamage * glyphDamageRangePercentagePerLevel) / 100);


        }
        maxLevelGlyphDamageRange.x = Mathf.RoundToInt(processMinGlyphDamage);
        maxLevelGlyphDamageRange.y = Mathf.RoundToInt(processMaxGlyphDamage);

        if (currentMinion.minionLevel == 10)
        {
            glyphDamageRange.x = Mathf.RoundToInt(maxLevelGlyphDamageRange.x);
            glyphDamageRange.y = Mathf.RoundToInt(maxLevelGlyphDamageRange.y);
        }
    }
    void PreventIncoherenceInMovement()
    {
        if (attackMovementType == AttackMovementType.Teleport)
        {
            canSelectObstacle = false;
            cantSelectMinion = false;
        }
        // if(rangeType == RangeType.Arround && fromTargetTile == false)
        // {
        //     if (attackMovementType == AttackMovementType.Push || attackMovementType == AttackMovementType.Pull)
        //         if(attackRange.y > 2)
        //             attackMovementType = AttackMovementType.None;
        //
        // }
        //
        // if (zoneType == ZoneType.Arround)
        // {
        //     if (attackMovementType == AttackMovementType.Push || attackMovementType == AttackMovementType.Pull)
        //         if (zoneRange.y > 2)
        //             attackMovementType = AttackMovementType.None;
        //
        // }
        //
        // if (attackMovementType == AttackMovementType.Push || attackMovementType == AttackMovementType.Pull)
        // {
        //     if (fromTargetTile == false)
        //     {
        //         if(rangeType != RangeType.Arround)
        //         {
        //             if (zoneType != ZoneType.Vertical_Line && zoneType != ZoneType.Horizontal_Line)
        //             {
        //                 attackMovementType = AttackMovementType.None;
        //             }
        //         }
        //         else
        //         {
        //             attackMovementType = AttackMovementType.None;
        //         }
        //        
        //     }
        // }
    }


    // Tile targetTile;
    List<Tile> attackRangeTile = new List<Tile>();
    List<Tile> zoneAttackTile = new List<Tile>();
    MinionAttackRange attackRangeManager = null;

    //Regarde la zone que le minion peu attacker
    public void InitiateAttack()
    {
        attackRangeManager = new MinionAttackRange();
        List<Tile> possibleAttackRangeTile = new List<Tile>();
        attackRangeTile = new List<Tile>();
        GameManager.Instance.DeactivateAllTargetOnTile();
        UIManager.Instance.ActiveAllMinionHealth(true);

        SoundManager.Instance.PlayUISound("AttackSelection");

        currentMinion.ResetPathFinder();
        currentMinion.ResetAttacks();
        currentMinion.onAttackInitiation = true;
        currentMinion.currentAttack = this;

        foreach (var tile in attackRangeManager.GetPossibleAttackRange(currentMinion.currentTile, this))
            possibleAttackRangeTile.Add(tile);

        if (rangeType == RangeType.onSelf)
            CheckAttackZone(currentMinion.currentTile);

        if (!noLineOfSight)
        {
            foreach (var tile in attackRangeManager.GetAttackRangeWithVision(currentMinion.currentTile, this))
                attackRangeTile.Add(tile);
        }
        else
        {
            foreach (var tile in possibleAttackRangeTile)
                attackRangeTile.Add(tile);
        }

        foreach (var tile in possibleAttackRangeTile)
        {
            if (attackRangeTile.Contains(tile))
                tile.ActivePossibleAttackTarget(true);
            else
                tile.ActiveImpossiblePathTarget(true);
        }

    }

    // regarde la zone deffect de l'attack lorsque une tile est selectionner
    public void CheckAttackZone(Tile attackTile)
    {
        ResetAttackConfirmation();

        // if (!attackRangeTile.Contains(attackTile))
        //     return;

        currentMinion.onAttackConfirmation = true;
        currentMinion.onAttackInitiation = false;
        currentMinion.targetTile = attackTile;

        bool haveMinionInZone = false;
        foreach (var tile in attackRangeManager.GetAttackZone(currentMinion.currentTile, attackTile, this))
        {
            zoneAttackTile.Add(tile);
            tile.ActiveAttackZonetarget(true);
            if (tile.currentMinion != null)
                haveMinionInZone = true;
        }

        if (haveMinionInZone)
        {
            CalculDamageInfoPosition(attackTile);
            SoundManager.Instance.PlayUISound("SetAttackZoneWithMinion");
        }
        else
            SoundManager.Instance.PlayUISound("SetAttackZone");
    }

    //confirme l'attaque lorsque le joueur reappui sur la zone deffet dattaque
    public void ConfirmAttack(Tile targetTile)
    {
        currentMinion.StartCoroutine(AttackProcess());
        IEnumerator AttackProcess()
        {
            SoundManager.Instance.PlayUISound("AttackConfirmation");

            yield return new WaitWhile(() => currentMinion.onMove);
            yield return new WaitWhile(() => currentMinion.onAttack);

            if (VfxOnMinionTile && vfxOnMinionTileSpawnType == VFXSpawnType.OnCast)
            {
                if (this == currentMinion.firstAttack)
                    currentMinion.PlayOnMinionVFX(1);

                if (this == currentMinion.secondAttack)
                    currentMinion.PlayOnMinionVFX(2);

                if (this == currentMinion.specialAttack)
                    currentMinion.PlayOnMinionVFX(3);
            }

            Debug.Log(currentMinion.MinionName + "   " + targetTile);

            cooldownLeft = cooldown;
            currentMinion.onAttackConfirmation = false;
            currentMinion.SetOnAttack(true);
            currentMinion.SetRootMotion(true);
            currentMinion.RotateWithLerp(Quaternion.LookRotation
                ((targetTile.worldPos + Vector3.up * 3) - (currentMinion.currentTile.worldPos + Vector3.up * 3)));

            if (moveToTargetTile)
                currentMinion.ChangeCurrentTile(targetTile);


            GameManager.Instance.DeactivateAllTargetOnTile();
            GameManager.Instance.ConsumeActionPoint(actionPointCost * (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1));

            UIManager.Instance.TurnOffAllActionHightLight();
            UIManager.Instance.ActiveAllMinionHealth(false);
            UIManager.Instance.UpdateActionPanel(currentMinion);



            foreach (var tile in zoneAttackTile)
            {
                if (tile.filled)
                {
                    tile.currentMinion.healthBar.HideDamageInfo();
                    tile.currentMinion.healthBar.ShowHealthBar(true);
                }
            }

            yield return new WaitForSeconds(0.2f);

            currentMinion.onAttackAnim = true;

            if (this == currentMinion.firstAttack)
                currentMinion.PlayAnimation("Attack_1");
            else if (this == currentMinion.secondAttack)
                currentMinion.PlayAnimation("Attack_2");
            else if (this == currentMinion.specialAttack)
                currentMinion.PlayAnimation("Attack_3");


            if (ThrowProjectil)
            {
                yield return new WaitForSeconds(spawnDelay);

                projectilPrefab.transform.position = currentMinion.spawnProjectilTransform.position;

                projectilPrefab.SetAttack(this, currentMinion, targetTile);
                projectilPrefab.MoveProjectil(currentMinion.spawnProjectilTransform.position, targetTile.worldPos + Vector3.up * 3);
            }

            yield return new WaitForSeconds(Mathf.Clamp(attackDuration - spawnDelay, 0, attackDuration));

            if (!ThrowProjectil)
            {
                ExecuteAttack(targetTile);
                ResetAttack();
            }

            currentMinion.onAttackAnim = false;
            currentMinion.SetOnAttack(false);
            currentMinion.SetRootMotion(false);

            if (moveToTargetTile)
                currentMinion.SetRootMotion(false);

            if (moveToTargetTile)
                currentMinion.RecenterMinion();
        }
    }

    //applique les damage et les effect sur les minion dans la zone
    public void ExecuteAttack(Tile targetTile)
    {

        if (VfxOnMinionTile && vfxOnMinionTileSpawnType == VFXSpawnType.On_Execute_Attack)
        {
            if (this == currentMinion.firstAttack)
                currentMinion.PlayOnMinionVFX(1);

            if (this == currentMinion.secondAttack)
                currentMinion.PlayOnMinionVFX(2);

            if (this == currentMinion.specialAttack)
                currentMinion.PlayOnMinionVFX(3);
        }

        if (CameraShake && cameraShakeType == FeedBackHappening.On_Execute_Attack)
            GameManager.Instance.CameraShake(Strenght, vibrationAmount, shakeDuration);

        if (slowMotion && slowMotionType == FeedBackHappening.On_Execute_Attack)
            GameManager.Instance.SlowMotion(slowMotionAmount, slowMotionDuration);

        if (camZoom && camZoomType == FeedBackHappening.On_Execute_Attack)
            GameManager.Instance.CamZoom(targetTile, camZoomSpeed, camZoomAmount, camZoomDuration);


        bool critical = Random.Range(0, 100) <= currentMinion.criticalHitChance;

        Debug.Log(currentMinion.MinionName + "     zone attack    " + zoneAttackTile.Count);

        foreach (Tile tile in zoneAttackTile)
        {
            if (tile.filled)
            {
                Minion targetMinion = tile.currentMinion;

                int damage = Mathf.FloorToInt(Random.Range(damageRange.x, damageRange.y) *
                    (currentMinion.weakenedTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Weakened ? 0.5f : 1 * (critical ? 1.5f : 1)) *
                    (currentMinion.superForceTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.SuperForce ? 2 : 1));

                if (canHeal)
                {
                    int heal = Mathf.FloorToInt(Random.Range(healRange.x, healRange.y));

                    switch (healTarget)
                    {

                        case AffectWhichMinon.Enemy:

                            if (tile.currentMinion.myMinion != currentMinion.myMinion)
                                tile.currentMinion.GetHeal(heal);

                            break;
                        case AffectWhichMinon.Ally:

                            if (tile.currentMinion.myMinion == currentMinion.myMinion)
                                tile.currentMinion.GetHeal(heal);

                            break;
                        case AffectWhichMinon.Both:

                            tile.currentMinion.GetHeal(heal);

                            break;
                        default:
                            break;
                    }
                }

                ApplyEffects(tile);

                if (damageRange.x != 0 && damageRange.y != 0)
                    if (canDamageAllyMinions || targetMinion.myMinion != currentMinion.myMinion)
                        targetMinion.GetDamage(damage, damageType, critical);

            }
            else
            {

                if (makeTrap)
                {
                    if (tile.haveObstacle == false && tile.filled == false)
                    {
                        if (attackMovementType != AttackMovementType.Teleport &&
                            attackMovementType != AttackMovementType.Change_Place)
                        {
                            if (this == currentMinion.firstAttack)
                                currentMinion.InitiateTrap(1, tile);

                            if (this == currentMinion.secondAttack)
                                currentMinion.InitiateTrap(2, tile);

                            if (this == currentMinion.specialAttack)
                                currentMinion.InitiateTrap(3, tile);
                        }
                    }
                }

                if (makeObstacle)
                {
                    if (tile.haveObstacle == false)
                        InstanciateObstacle(tile);
                }
            }


            if (makeGlyph)
            {
                if (tile.haveObstacle == false)
                {
                    if (this == currentMinion.firstAttack)
                        currentMinion.InitiateGlyph(1, tile);

                    if (this == currentMinion.secondAttack)
                        currentMinion.InitiateGlyph(2, tile);

                    if (this == currentMinion.specialAttack)
                        currentMinion.InitiateGlyph(3, tile);
                }
            }
        }

        if (attackMovementType == AttackMovementType.Push)
            PushMinions(targetTile);

        if (attackMovementType == AttackMovementType.Pull)
            PullMinions(targetTile);

        if (attackMovementType == AttackMovementType.Teleport)
            TeleportMinion(currentMinion, targetTile);

        if (attackMovementType == AttackMovementType.Change_Place)
        {
            TeleportMinion(targetTile.currentMinion, currentMinion.currentTile);
            TeleportMinion(currentMinion, targetTile);
        }
    }



    void ApplyEffects(Tile tile)
    {
        Minion minionOnTile = tile.currentMinion;

        EffectType effectType = EffectType.None;
        AffectWhichMinon target = AffectWhichMinon.None;

        if (silence)
        {
            effectType = EffectType.Silence;
            target = burnTarget;
        }
        if (stun)
        {
            effectType = EffectType.Stun;
            target = stunTarget;
        }
        if (superForce)
        {
            effectType = EffectType.SuperForce;
            target = superForceTarget;
        }
        if (freeze)
        {
            effectType = EffectType.Freeze;
            target = freezeTarget;
        }
        if (overcharge)
        {
            effectType = EffectType.Overcharge;
            target = overchargeTarget;
        }
        if (burn)
        {
            effectType = EffectType.Burn;
            target = burnTarget;
        }
        if (flowerSkin)
        {
            effectType = EffectType.FlowerSkin;
            target = flowerSkinTarget;
        }
        if (weakened)
        {
            effectType = EffectType.Weakened;
            target = weakenedTarget;
        }
        if (invulnerable)
        {
            effectType = EffectType.Invulnerable;
            target = invulnerableTarget;
        }

        switch (target)
        {
            case AffectWhichMinon.Enemy:
                if (minionOnTile.myMinion != currentMinion.myMinion)
                    minionOnTile.GetEffect(effectType, turnAmountEffect, burnDamageAmount);
                break;
            case AffectWhichMinon.Ally:
                if (minionOnTile.myMinion == currentMinion.myMinion)
                    minionOnTile.GetEffect(effectType, turnAmountEffect, burnDamageAmount);
                break;
            case AffectWhichMinon.Both:
                minionOnTile.GetEffect(effectType, turnAmountEffect, burnDamageAmount);
                break;
            default:
                break;
        }
    }
    void PushMinions(Tile targetTile)
    {
        bool needPushDamage = false;
        Tile destinationTile = null;

        List<Tile> tileWithMinion = new List<Tile>();
        foreach (var tile in zoneAttackTile)
        {
            if (tile.filled)
            {
                if (tile.currentMinion != currentMinion || canSelfPushOrPull)
                {
                    tileWithMinion.Add(tile);
                }

            }
        }

        if (referenceType == AttackMovementReferenceType.Minion)
        {
            bool onSameX = targetTile.coords.x == currentMinion.currentTile.coords.x;
            bool onSameY = targetTile.coords.y == currentMinion.currentTile.coords.y;
            bool upSide = targetTile.coords.y > currentMinion.currentTile.coords.y;
            bool leftSide = targetTile.coords.x < currentMinion.currentTile.coords.x;

            //regarde si il y a pusieur minion sur la meme ligne
            List<List<Tile>> enemiesInSameLine = new List<List<Tile>>();


            if (onSameX || onSameY)
            {
                //determine combien de large est la pousser
                List<int> zoneRange = new List<int>();

                foreach (var tile in zoneAttackTile)
                    if (!zoneRange.Contains(Mathf.FloorToInt(onSameX ? tile.coords.x : tile.coords.y)) && tile.filled)
                        zoneRange.Add(Mathf.FloorToInt(onSameX ? tile.coords.x : tile.coords.y));

                for (int i = 0; i < zoneRange.Count; i++)
                {
                    List<Tile> sameLineTile = new List<Tile>();
                    Tile checkTile = null;

                    if (onSameX)
                        GameManager.Instance.tilesDictionary.TryGetValue(new Vector2(zoneRange[i], currentMinion.currentTile.coords.y), out checkTile);
                    else if (onSameY)
                        GameManager.Instance.tilesDictionary.TryGetValue(new Vector2(currentMinion.currentTile.coords.x, zoneRange[i]), out checkTile);

                    //par du minion attaquant et regarde en ligne les minion a pousser
                    for (int j = 0; j < 20; j++)
                    {
                        if (onSameX)
                        {
                            if ((upSide ? checkTile.YUp : checkTile.YDown) != null)
                            {
                                if (tileWithMinion.Contains((upSide ? checkTile.YUp : checkTile.YDown)))
                                    sameLineTile.Add((upSide ? checkTile.YUp : checkTile.YDown));

                                checkTile = (upSide ? checkTile.YUp : checkTile.YDown);
                            }
                            else break;
                        }
                        else if (onSameY)
                        {
                            if ((leftSide ? checkTile.XLeft : checkTile.XRight) != null)
                            {
                                if (tileWithMinion.Contains((leftSide ? checkTile.XLeft : checkTile.XRight)))
                                    sameLineTile.Add((leftSide ? checkTile.XLeft : checkTile.XRight));

                                checkTile = (leftSide ? checkTile.XLeft : checkTile.XRight);
                            }
                            else break;
                        }
                    }

                    sameLineTile.Reverse();

                    enemiesInSameLine.Add(sameLineTile);
                }
                foreach (var enemyTileList in enemiesInSameLine)
                {
                    foreach (var enemyTile in enemyTileList)
                    {
                        Tile checkTile = enemyTile;

                        for (int i = 0; i < tileMovementAmount; i++)
                        {
                            if (onSameX ? (upSide ? checkTile.YUp : checkTile.YDown) : (leftSide ? checkTile.XLeft : checkTile.XRight) != null)
                            {
                                Tile tempCheckTile = onSameX ? (upSide ? checkTile.YUp : checkTile.YDown) : (leftSide ? checkTile.XLeft : checkTile.XRight);
                                if (tempCheckTile.filled == false && tempCheckTile.haveObstacle == false)
                                {
                                    checkTile = tempCheckTile;
                                    destinationTile = checkTile;
                                }
                                else
                                {
                                    destinationTile = checkTile;

                                    if (i < tileMovementAmount - 1)
                                    {
                                        needPushDamage = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                destinationTile = checkTile;

                                if (i < tileMovementAmount - 1)
                                {
                                    needPushDamage = true;
                                    break;
                                }
                            }
                        }

                        destinationTile.filled = true;
                        enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
                    }
                }
            }
            else
            {
                if (upSide)
                {
                    if (leftSide)
                    {
                        foreach (var tile in tileWithMinion)
                        {
                            if (tile.filled)
                            {
                                Tile checkTile = tile;

                                for (int i = 0; i < tileMovementAmount; i++)
                                {
                                    if (checkTile.XLeft != null && checkTile.YUp != null)
                                    {
                                        if (checkTile.XLeft.filled == false && checkTile.XLeft.haveObstacle == false
                                            && checkTile.YUp.filled == false && checkTile.YUp.haveObstacle == false
                                            && checkTile.XLeft.YUp.filled == false && checkTile.XLeft.YUp.haveObstacle == false)
                                        {
                                            checkTile = checkTile.XLeft.YUp;
                                            destinationTile = checkTile;
                                        }
                                        else
                                        {
                                            destinationTile = checkTile;

                                            if (i < tileMovementAmount - 1)
                                            {
                                                needPushDamage = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        destinationTile = checkTile;

                                        if (i < tileMovementAmount - 1)
                                        {
                                            needPushDamage = true;
                                            break;
                                        }
                                    }
                                }

                                destinationTile.filled = true;
                                tile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
                            }
                        }
                    }
                    else
                    {
                        foreach (var tile in tileWithMinion)
                        {
                            Tile checkTile = tile;

                            for (int i = 0; i < tileMovementAmount; i++)
                            {
                                if (checkTile.XRight != null && checkTile.YUp != null)
                                {
                                    if (checkTile.XRight.filled == false && checkTile.XRight.haveObstacle == false
                                        && checkTile.YUp.filled == false && checkTile.YUp.haveObstacle == false
                                        && checkTile.XRight.YUp.filled == false && checkTile.XRight.YUp.haveObstacle == false)
                                    {
                                        checkTile = checkTile.XRight.YUp;
                                        destinationTile = checkTile;
                                    }
                                    else
                                    {
                                        destinationTile = checkTile;

                                        if (i < tileMovementAmount - 1)
                                        {
                                            needPushDamage = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    destinationTile = checkTile;

                                    if (i < tileMovementAmount - 1)
                                    {
                                        needPushDamage = true;
                                        break;
                                    }
                                }
                            }

                            destinationTile.filled = true;
                            tile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
                        }
                    }
                }
                else
                {
                    if (leftSide)
                    {
                        foreach (var tile in tileWithMinion)
                        {
                            Tile checkTile = tile;

                            for (int i = 0; i < tileMovementAmount; i++)
                            {
                                if (checkTile.XLeft != null && checkTile.YDown != null)
                                {
                                    if (checkTile.XLeft.filled == false && checkTile.XLeft.haveObstacle == false
                                        && checkTile.YDown.filled == false && checkTile.YDown.haveObstacle == false
                                        && checkTile.XLeft.YDown.filled == false && checkTile.XLeft.YDown.haveObstacle == false)
                                    {
                                        checkTile = checkTile.XLeft.YDown;
                                        destinationTile = checkTile;
                                    }
                                    else
                                    {
                                        destinationTile = checkTile;

                                        if (i < tileMovementAmount - 1)
                                        {
                                            needPushDamage = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    destinationTile = checkTile;

                                    if (i < tileMovementAmount - 1)
                                    {
                                        needPushDamage = true;
                                        break;
                                    }
                                }
                            }

                            destinationTile.filled = true;
                            tile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
                        }
                    }
                    else
                    {
                        foreach (var tile in tileWithMinion)
                        {
                            Tile checkTile = tile;

                            for (int i = 0; i < tileMovementAmount; i++)
                            {
                                if (checkTile.XRight != null && checkTile.YDown != null)
                                {
                                    if (checkTile.XRight.filled == false && checkTile.XRight.haveObstacle == false
                                        && checkTile.YDown.filled == false && checkTile.YDown.haveObstacle == false
                                        && checkTile.XRight.YDown.filled == false && checkTile.XRight.YDown.haveObstacle == false)
                                    {
                                        checkTile = checkTile.XRight.YDown;
                                        destinationTile = checkTile;
                                    }
                                    else
                                    {
                                        destinationTile = checkTile;

                                        if (i < tileMovementAmount - 1)
                                        {
                                            needPushDamage = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    destinationTile = checkTile;

                                    if (i < tileMovementAmount - 1)
                                    {
                                        needPushDamage = true;
                                        break;
                                    }
                                }
                            }

                            destinationTile.filled = true;
                            tile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
                        }
                    }
                }
            }
        }
        else if (referenceType == AttackMovementReferenceType.Target_Tile)
        {
            //vers le haut
            List<Tile> minionLineTile = new List<Tile>();
            Tile otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.YUp != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.YUp.filled)
                        if (tileWithMinion.Contains(otherLineTile.YUp))
                            if (otherLineTile.YUp.currentMinion != currentMinion || canSelfPushOrPull)
                                minionLineTile.Add(otherLineTile.YUp);

                    otherLineTile = otherLineTile.YUp;
                }
                else break;
            }
            minionLineTile.Reverse();
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.YUp != null)
                    {
                        if (checkTile.YUp.filled == false && checkTile.YUp.haveObstacle == false)
                        {
                            checkTile = checkTile.YUp;
                            destinationTile = checkTile;
                        }
                        else
                        {
                            destinationTile = checkTile;

                            if (i < tileMovementAmount - 1)
                            {
                                needPushDamage = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;

                        if (i < tileMovementAmount - 1)
                        {
                            needPushDamage = true;
                            break;
                        }
                    }
                }
                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //vers le bas
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.YDown != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.YDown.filled)
                        if (tileWithMinion.Contains(otherLineTile.YDown))
                            if (otherLineTile.YDown.currentMinion != currentMinion || canSelfPushOrPull)
                                minionLineTile.Add(otherLineTile.YDown);

                    otherLineTile = otherLineTile.YDown;
                }
                else break;
            }
            minionLineTile.Reverse();
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.YDown != null)
                    {
                        if (checkTile.YDown.filled == false && checkTile.YDown.haveObstacle == false)
                        {
                            checkTile = checkTile.YDown;
                            destinationTile = checkTile;
                        }
                        else
                        {
                            destinationTile = checkTile;

                            if (i < tileMovementAmount - 1)
                            {
                                needPushDamage = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;

                        if (i < tileMovementAmount - 1)
                        {
                            needPushDamage = true;
                            break;
                        }
                    }
                }

                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //vers la gauche
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.XLeft != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.XLeft.filled)
                        if (tileWithMinion.Contains(otherLineTile.XLeft))
                            if (otherLineTile.XLeft.currentMinion != currentMinion || canSelfPushOrPull)
                                minionLineTile.Add(otherLineTile.XLeft);

                    otherLineTile = otherLineTile.XLeft;
                }
                else break;
            }
            minionLineTile.Reverse();
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.XLeft != null)
                    {
                        if (checkTile.XLeft.filled == false && checkTile.XLeft.haveObstacle == false)
                        {
                            checkTile = checkTile.XLeft;
                            destinationTile = checkTile;
                        }
                        else
                        {
                            destinationTile = checkTile;

                            if (i < tileMovementAmount - 1)
                            {
                                needPushDamage = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;

                        if (i < tileMovementAmount - 1)
                        {
                            needPushDamage = true;
                            break;
                        }
                    }
                }

                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //vers la droite
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.XRight != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.XRight.filled)
                        if (tileWithMinion.Contains(otherLineTile.XRight))
                            if (otherLineTile.XRight.currentMinion != currentMinion || canSelfPushOrPull)
                                minionLineTile.Add(otherLineTile.XRight);

                    otherLineTile = otherLineTile.XRight;
                }
                else break;
            }
            minionLineTile.Reverse();
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.XRight != null)
                    {
                        if (checkTile.XRight.filled == false && checkTile.XRight.haveObstacle == false)
                        {
                            checkTile = checkTile.XRight;
                            destinationTile = checkTile;
                        }
                        else
                        {
                            destinationTile = checkTile;

                            if (i < tileMovementAmount - 1)
                            {
                                needPushDamage = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;

                        if (i < tileMovementAmount - 1)
                        {
                            needPushDamage = true;
                            break;
                        }
                    }
                }

                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //diagonal droite haut
            /* minionLineTile = new List<Tile>();
             otherLineTile = targetTile;
             for (int i = 0; i < zoneRange.y; i++)
             {
                 if (otherLineTile.XRight != null && otherLineTile.YUp != null)
                 {
                     if (i + 1 >= zoneRange.x && otherLineTile.XRight.YUp.filled)
                         if (tileWithMinion.Contains(otherLineTile.XRight.YUp))
                             if (otherLineTile.XRight.YUp.currentMinion != currentMinion || canSelfPushOrPull)
                                 minionLineTile.Add(otherLineTile.XRight.YUp);

                     otherLineTile = otherLineTile.XRight.YUp;
                 }
                 else break;
             }
             minionLineTile.Reverse();
             foreach (var enemyTile in minionLineTile)
             {
                 Tile checkTile = enemyTile;

                 for (int i = 0; i < tileMovementAmount; i++)
                 {
                     if (checkTile.XRight != null && checkTile.YUp != null)
                     {
                         if (checkTile.XRight.filled == false && checkTile.XRight.haveObstacle == false
                             && checkTile.YUp.filled == false && checkTile.YUp.haveObstacle == false
                             && checkTile.XRight.YUp.filled == false && checkTile.XRight.YUp.haveObstacle == false)
                         {
                             checkTile = checkTile.XRight.YUp;
                             destinationTile = checkTile;
                         }
                         else
                         {
                             destinationTile = checkTile;

                             if (i < tileMovementAmount - 1)
                             {
                                 needPushDamage = true;
                                 break;
                             }
                         }
                     }
                     else
                     {
                         destinationTile = checkTile;

                         if (i < tileMovementAmount - 1)
                         {
                             needPushDamage = true;
                             break;
                         }
                     }
                 }

                 destinationTile.filled = true;
                 enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
             }

             //diagonal droite Bas
             minionLineTile = new List<Tile>();
             otherLineTile = targetTile;
             for (int i = 0; i < zoneRange.y; i++)
             {
                 if (otherLineTile.XRight != null && otherLineTile.YDown != null)
                 {
                     if (i + 1 >= zoneRange.x && otherLineTile.XRight.YDown.filled)
                         if (tileWithMinion.Contains(otherLineTile.XRight.YDown))
                             if (otherLineTile.XRight.YDown.currentMinion != currentMinion || canSelfPushOrPull)
                                 minionLineTile.Add(otherLineTile.XRight.YDown);
                 }
                 else break;
             }
             minionLineTile.Reverse();
             foreach (var enemyTile in minionLineTile)
             {
                 Tile checkTile = enemyTile;

                 for (int i = 0; i < tileMovementAmount; i++)
                 {
                     if (checkTile.XRight != null && checkTile.YDown != null)
                     {
                         if (checkTile.XRight.filled == false && checkTile.XRight.haveObstacle == false
                             && checkTile.YDown.filled == false && checkTile.YDown.haveObstacle == false
                             && checkTile.XRight.YDown.filled == false && checkTile.XRight.YDown.haveObstacle == false)
                         {
                             checkTile = checkTile.XRight.YDown;
                             destinationTile = checkTile;
                         }
                         else
                         {
                             destinationTile = checkTile;

                             if (i < tileMovementAmount - 1)
                             {
                                 needPushDamage = true;
                                 break;
                             }
                         }
                     }
                     else
                     {
                         destinationTile = checkTile;

                         if (i < tileMovementAmount - 1)
                         {
                             needPushDamage = true;
                             break;
                         }
                     }
                 }

                 destinationTile.filled = true;
                 enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
             }

             //diagonal gauche haut
             minionLineTile = new List<Tile>();
             otherLineTile = targetTile;
             for (int i = 0; i < zoneRange.y; i++)
             {
                 if (otherLineTile.XLeft != null && otherLineTile.YUp != null)
                 {
                     if (i + 1 >= zoneRange.x && otherLineTile.XLeft.YUp.filled)
                         if (tileWithMinion.Contains(otherLineTile.XLeft.YUp))
                             if (otherLineTile.XLeft.YUp.currentMinion != currentMinion || canSelfPushOrPull)
                                 minionLineTile.Add(otherLineTile.XLeft.YUp); minionLineTile.Add(otherLineTile.XLeft.YUp);

                 }
                 else break;
             }
             minionLineTile.Reverse();
             foreach (var enemyTile in minionLineTile)
             {
                 Tile checkTile = enemyTile;

                 for (int i = 0; i < tileMovementAmount; i++)
                 {
                     if (checkTile.XLeft != null && checkTile.YUp != null)
                     {
                         if (checkTile.XLeft.filled == false && checkTile.XLeft.haveObstacle == false
                             && checkTile.YUp.filled == false && checkTile.YUp.haveObstacle == false
                             && checkTile.XLeft.YUp.filled == false && checkTile.XLeft.YUp.haveObstacle == false)
                         {
                             checkTile = checkTile.XLeft.YUp;
                             destinationTile = checkTile;
                         }
                         else
                         {
                             destinationTile = checkTile;

                             if (i < tileMovementAmount - 1)
                             {
                                 needPushDamage = true;
                                 break;
                             }
                         }
                     }
                     else
                     {
                         destinationTile = checkTile;

                         if (i < tileMovementAmount - 1)
                         {
                             needPushDamage = true;
                             break;
                         }
                     }
                 }

                 destinationTile.filled = true;
                 enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
             }*/

            //diagonal gauche bas
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.XLeft != null && otherLineTile.YDown != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.XLeft.YDown.filled)
                        if (tileWithMinion.Contains(otherLineTile.XLeft.YDown))
                            if (otherLineTile.XLeft.YDown.currentMinion != currentMinion || canSelfPushOrPull)
                                minionLineTile.Add(otherLineTile.XLeft.YDown);
                }
                else break;
            }
            minionLineTile.Reverse();
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.XLeft != null && checkTile.YDown != null)
                    {
                        if (checkTile.XLeft.filled == false && checkTile.XLeft.haveObstacle == false
                            && checkTile.YDown.filled == false && checkTile.YDown.haveObstacle == false
                            && checkTile.XLeft.YDown.filled == false && checkTile.XLeft.YDown.haveObstacle == false)
                        {
                            checkTile = checkTile.XLeft.YDown;
                            destinationTile = checkTile;
                        }
                        else
                        {
                            destinationTile = checkTile;

                            if (i < tileMovementAmount - 1)
                            {
                                needPushDamage = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;

                        if (i < tileMovementAmount - 1)
                        {
                            needPushDamage = true;
                            break;
                        }
                    }
                }

                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }
        }
    }
    void PullMinions(Tile targetTile)
    {
        bool needPushDamage = false;
        Tile destinationTile = null;

        if (referenceType == AttackMovementReferenceType.Minion)
        {
            bool onSameX = targetTile.coords.x == currentMinion.currentTile.coords.x;
            bool onSameY = targetTile.coords.y == currentMinion.currentTile.coords.y;
            bool upSide = targetTile.coords.y > currentMinion.currentTile.coords.y;
            bool leftSide = targetTile.coords.x < currentMinion.currentTile.coords.x;

            //regarde si il y a pusieur minion sur la meme ligne
            List<List<Tile>> enemiesInSameLine = new List<List<Tile>>();
            List<Tile> tileWithMinion = new List<Tile>();
            foreach (var tile in zoneAttackTile)
            {
                if (tile.filled)
                {
                    if (tile.currentMinion != currentMinion)
                        tileWithMinion.Add(tile);
                }
            }

            if (onSameX || onSameY)
            {
                //determine combien de large est la pousser
                List<int> zoneRange = new List<int>();

                foreach (var tile in zoneAttackTile)
                    if (!zoneRange.Contains(Mathf.FloorToInt(onSameX ? tile.coords.x : tile.coords.y)) && tile.filled)
                        zoneRange.Add(Mathf.FloorToInt(onSameX ? tile.coords.x : tile.coords.y));

                for (int i = 0; i < zoneRange.Count; i++)
                {
                    List<Tile> sameLineTile = new List<Tile>();
                    Tile checkTile = null;

                    if (onSameX)
                        GameManager.Instance.tilesDictionary.TryGetValue(new Vector2(zoneRange[i], currentMinion.currentTile.coords.y), out checkTile);
                    else if (onSameY)
                        GameManager.Instance.tilesDictionary.TryGetValue(new Vector2(currentMinion.currentTile.coords.x, zoneRange[i]), out checkTile);

                    //par du minion attaquant et regarde en ligne les minion a pousser
                    for (int j = 0; j < 20; j++)
                    {
                        if (onSameX)
                        {
                            if ((upSide ? checkTile.YUp : checkTile.YDown) != null)
                            {
                                if (tileWithMinion.Contains((upSide ? checkTile.YUp : checkTile.YDown)))
                                    sameLineTile.Add((upSide ? checkTile.YUp : checkTile.YDown));

                                checkTile = (upSide ? checkTile.YUp : checkTile.YDown);
                            }
                            else break;
                        }
                        else if (onSameY)
                        {
                            if ((leftSide ? checkTile.XLeft : checkTile.XRight) != null)
                            {
                                if (tileWithMinion.Contains((leftSide ? checkTile.XLeft : checkTile.XRight)))
                                    sameLineTile.Add((leftSide ? checkTile.XLeft : checkTile.XRight));

                                checkTile = (leftSide ? checkTile.XLeft : checkTile.XRight);
                            }
                            else break;
                        }
                    }

                    enemiesInSameLine.Add(sameLineTile);
                }
                foreach (var enemyTileList in enemiesInSameLine)
                {
                    foreach (var enemyTile in enemyTileList)
                    {
                        Tile checkTile = enemyTile;

                        for (int i = 0; i < tileMovementAmount; i++)
                        {
                            if (onSameX ? (upSide ? checkTile.YUp : checkTile.YDown) : (leftSide ? checkTile.XLeft : checkTile.XRight) != null)
                            {
                                Tile tempCheckTile = onSameX ? (upSide ? checkTile.YDown : checkTile.YUp) : (leftSide ? checkTile.XRight : checkTile.XLeft);
                                if (tempCheckTile.filled == false && tempCheckTile.haveObstacle == false)
                                {
                                    checkTile = tempCheckTile;
                                    destinationTile = checkTile;
                                }
                                else
                                {
                                    destinationTile = checkTile;
                                    break;
                                }
                            }
                            else
                            {
                                destinationTile = checkTile;
                                break;
                            }
                        }

                        destinationTile.filled = true;
                        enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
                    }
                }
            }
            else
            {
                if (upSide)
                {
                    if (leftSide)
                    {
                        foreach (var tile in tileWithMinion)
                        {
                            if (tile.filled)
                            {
                                Tile checkTile = tile;

                                for (int i = 0; i < tileMovementAmount; i++)
                                {
                                    if (checkTile.XRight != null && checkTile.YDown != null)
                                    {
                                        if (checkTile.XRight.filled == false && checkTile.XRight.haveObstacle == false
                                            && checkTile.YDown.filled == false && checkTile.YDown.haveObstacle == false
                                            && checkTile.XRight.YDown.filled == false && checkTile.XRight.YDown.haveObstacle == false)
                                        {
                                            checkTile = checkTile.XRight.YDown;
                                            destinationTile = checkTile;
                                        }
                                        else
                                        {
                                            destinationTile = checkTile;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        destinationTile = checkTile;
                                        break;
                                    }
                                }

                                destinationTile.filled = true;
                                tile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
                            }
                        }
                    }
                    else
                    {
                        foreach (var tile in tileWithMinion)
                        {
                            Tile checkTile = tile;

                            for (int i = 0; i < tileMovementAmount; i++)
                            {
                                if (checkTile.XLeft != null && checkTile.YDown != null)
                                {
                                    if (checkTile.XLeft.filled == false && checkTile.XLeft.haveObstacle == false
                                        && checkTile.YDown.filled == false && checkTile.YDown.haveObstacle == false
                                        && checkTile.XLeft.YDown.filled == false && checkTile.XLeft.YDown.haveObstacle == false)
                                    {
                                        checkTile = checkTile.XLeft.YDown;
                                        destinationTile = checkTile;
                                    }
                                    else
                                    {
                                        destinationTile = checkTile;

                                        if (i < tileMovementAmount - 1)
                                        {
                                            needPushDamage = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    destinationTile = checkTile;

                                    if (i < tileMovementAmount - 1)
                                    {
                                        needPushDamage = true;
                                        break;
                                    }
                                }

                                checkTile.ActiveImpossiblePathTarget(true);
                            }

                            destinationTile.ActiveImpossiblePathTarget(true);
                            destinationTile.filled = true;
                            tile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
                        }
                    }
                }
                else
                {
                    if (leftSide)
                    {
                        foreach (var tile in tileWithMinion)
                        {
                            Tile checkTile = tile;

                            for (int i = 0; i < tileMovementAmount; i++)
                            {
                                if (checkTile.XRight != null && checkTile.YUp != null)
                                {
                                    if (checkTile.XRight.filled == false && checkTile.XRight.haveObstacle == false
                                        && checkTile.YUp.filled == false && checkTile.YUp.haveObstacle == false
                                        && checkTile.XRight.YUp.filled == false && checkTile.XRight.YUp.haveObstacle == false)
                                    {
                                        checkTile = checkTile.XRight.YUp;
                                        destinationTile = checkTile;
                                    }
                                    else
                                    {
                                        destinationTile = checkTile;
                                        break;
                                    }
                                }
                                else
                                {
                                    destinationTile = checkTile;
                                    break;
                                }
                            }

                            destinationTile.filled = true;
                            tile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
                        }
                    }
                    else
                    {
                        foreach (var tile in tileWithMinion)
                        {
                            Tile checkTile = tile;

                            for (int i = 0; i < tileMovementAmount; i++)
                            {
                                if (checkTile.XLeft != null && checkTile.YUp != null)
                                {
                                    if (checkTile.XLeft.filled == false && checkTile.XLeft.haveObstacle == false
                                        && checkTile.YUp.filled == false && checkTile.YUp.haveObstacle == false
                                        && checkTile.XLeft.YUp.filled == false && checkTile.XLeft.YUp.haveObstacle == false)
                                    {
                                        checkTile = checkTile.XLeft.YUp;
                                        destinationTile = checkTile;
                                    }
                                    else
                                    {
                                        destinationTile = checkTile;
                                        break;
                                    }
                                }
                                else
                                {
                                    destinationTile = checkTile;
                                    break;
                                }
                            }

                            destinationTile.filled = true;
                            tile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
                        }
                    }
                }
            }
        }
        else if (referenceType == AttackMovementReferenceType.Target_Tile)
        {
            //est en haut
            List<Tile> minionLineTile = new List<Tile>();
            Tile otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.YUp != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.YUp.filled)
                        if (otherLineTile.YUp.currentMinion != currentMinion || canSelfPushOrPull)
                            minionLineTile.Add(otherLineTile.YUp);

                    otherLineTile = otherLineTile.YUp;

                }
                else break;
            }
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.YDown != null)
                    {
                        if (checkTile.YDown.filled == false && checkTile.YDown.haveObstacle == false)
                        {
                            if (checkTile.YDown != targetTile)
                            {
                                checkTile = checkTile.YDown;
                                destinationTile = checkTile;
                            }
                            else
                            {
                                destinationTile = checkTile;
                                break;
                            }
                        }
                        else
                        {
                            destinationTile = checkTile;
                            break;
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;
                        break;
                    }
                }
                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //est en bas
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.YDown != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.YDown.filled)
                        if (otherLineTile.YDown.currentMinion != currentMinion || canSelfPushOrPull)
                            minionLineTile.Add(otherLineTile.YDown);

                    otherLineTile = otherLineTile.YDown;
                }
                else break;
            }
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.YUp != null)
                    {
                        if (checkTile.YUp.filled == false && checkTile.YUp.haveObstacle == false)
                        {
                            if (checkTile.YUp != targetTile)
                            {
                                checkTile = checkTile.YUp;
                                destinationTile = checkTile;
                            }
                            else
                            {
                                destinationTile = checkTile;
                                break;
                            }
                        }
                        else
                        {
                            destinationTile = checkTile;
                            break;
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;
                        break;
                    }
                }

                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //est en gauche
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.XLeft != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.XLeft.filled)
                        if (otherLineTile.XLeft.currentMinion != currentMinion || canSelfPushOrPull)
                            minionLineTile.Add(otherLineTile.XLeft);

                    otherLineTile = otherLineTile.XLeft;
                }
                else break;
            }
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.XRight != null)
                    {
                        if (checkTile.XRight.filled == false && checkTile.XRight.haveObstacle == false)
                        {
                            if (checkTile.XRight != targetTile)
                            {
                                checkTile = checkTile.XRight;
                                destinationTile = checkTile;
                            }
                            else
                            {
                                destinationTile = checkTile;
                                break;
                            }
                        }
                        else
                        {
                            destinationTile = checkTile;
                            break;
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;
                        break;
                    }
                }

                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //est en droite
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.XRight != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.XRight.filled)
                        if (otherLineTile.XRight.currentMinion != currentMinion || canSelfPushOrPull)
                            minionLineTile.Add(otherLineTile.XRight);

                    otherLineTile = otherLineTile.XRight;
                }
                else break;
            }
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.XLeft != null)
                    {
                        if (checkTile.XLeft.filled == false && checkTile.XLeft.haveObstacle == false)
                        {
                            if (checkTile.XLeft != targetTile)
                            {
                                checkTile = checkTile.XLeft;
                                destinationTile = checkTile;
                            }
                            else
                            {
                                destinationTile = checkTile;
                                break;
                            }
                        }
                        else
                        {
                            destinationTile = checkTile;
                            break;
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;
                        break;
                    }
                }

                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //est en diagonal droite haut
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.XRight != null && otherLineTile.YUp != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.XRight.YUp.filled)
                        if (otherLineTile.XRight.YUp.currentMinion != currentMinion || canSelfPushOrPull)
                            minionLineTile.Add(otherLineTile.XRight.YUp);

                    otherLineTile = otherLineTile.XRight.YUp;
                }
                else break;
            }
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.XLeft != null && checkTile.YDown != null)
                    {
                        if (checkTile.XLeft.filled == false && checkTile.XLeft.haveObstacle == false
                            && checkTile.YDown.filled == false && checkTile.YDown.haveObstacle == false
                            && checkTile.XLeft.YDown.filled == false && checkTile.XLeft.YDown.haveObstacle == false)
                        {
                            if (checkTile.XLeft.YDown != targetTile)
                            {
                                checkTile = checkTile.XLeft.YDown;
                                destinationTile = checkTile;
                            }
                            else
                            {
                                destinationTile = checkTile;
                                break;
                            }
                        }
                        else
                        {
                            destinationTile = checkTile;
                            break;
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;
                        break;
                    }
                }

                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //est en diagonal droite Bas
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.XRight != null && otherLineTile.YDown != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.XRight.YDown.filled)
                        if (otherLineTile.XRight.YDown.currentMinion != currentMinion || canSelfPushOrPull)
                            minionLineTile.Add(otherLineTile.XRight.YDown);

                    otherLineTile = otherLineTile.XRight.YDown;
                }
                else break;
            }
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.XLeft != null && checkTile.YUp != null)
                    {
                        if (checkTile.XLeft.filled == false && checkTile.XLeft.haveObstacle == false
                            && checkTile.YUp.filled == false && checkTile.YUp.haveObstacle == false
                            && checkTile.XLeft.YUp.filled == false && checkTile.XLeft.YUp.haveObstacle == false)
                        {
                            if (checkTile.XLeft.YUp != targetTile)
                            {
                                checkTile = checkTile.XLeft.YUp;
                                destinationTile = checkTile;
                            }
                            else
                            {
                                destinationTile = checkTile;
                                break;
                            }
                        }
                        else
                        {
                            destinationTile = checkTile;
                            break;
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;
                        break;
                    }
                }

                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //est en diagonal gauche haut
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.XLeft != null && otherLineTile.YUp != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.XLeft.YUp.filled)
                        if (otherLineTile.XLeft.YUp.currentMinion != currentMinion || canSelfPushOrPull)
                            minionLineTile.Add(otherLineTile.XLeft.YUp);

                    otherLineTile = otherLineTile.XLeft.YUp;
                }
                else break;
            }
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.XRight != null && checkTile.YDown != null)
                    {
                        if (checkTile.XRight.filled == false && checkTile.XRight.haveObstacle == false
                            && checkTile.YDown.filled == false && checkTile.YDown.haveObstacle == false
                            && checkTile.XRight.YDown.filled == false && checkTile.XRight.YDown.haveObstacle == false)
                        {
                            if (checkTile.XRight.YDown != targetTile)
                            {
                                checkTile = checkTile.XRight.YDown;
                                destinationTile = checkTile;
                            }
                            else
                            {
                                destinationTile = checkTile;
                                break;
                            }

                        }
                        else
                        {
                            destinationTile = checkTile;
                            break;
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;
                        break;
                    }
                }

                destinationTile.ActiveImpossiblePathTarget(true);
                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }

            //est en diagonal gauche bas
            minionLineTile = new List<Tile>();
            otherLineTile = targetTile;
            for (int i = 0; i < zoneRange.y; i++)
            {
                if (otherLineTile.XLeft != null && otherLineTile.YDown != null)
                {
                    if (i + 1 >= zoneRange.x && otherLineTile.XLeft.YDown.filled)
                        if (otherLineTile.XLeft.YDown.currentMinion != currentMinion || canSelfPushOrPull)
                            minionLineTile.Add(otherLineTile.XLeft.YDown);

                    otherLineTile = otherLineTile.XLeft.YDown;
                }
                else break;
            }
            foreach (var enemyTile in minionLineTile)
            {
                Tile checkTile = enemyTile;

                for (int i = 0; i < tileMovementAmount; i++)
                {
                    if (checkTile.XRight != null && checkTile.YUp != null)
                    {
                        if (checkTile.XRight.filled == false && checkTile.XRight.haveObstacle == false
                            && checkTile.YUp.filled == false && checkTile.YUp.haveObstacle == false
                            && checkTile.XRight.YUp.filled == false && checkTile.XRight.YUp.haveObstacle == false)
                        {
                            if (checkTile.XRight.YUp != targetTile)
                            {
                                checkTile = checkTile.XRight.YUp;
                                destinationTile = checkTile;
                            }
                            else
                            {
                                destinationTile = checkTile;
                                break;
                            }
                        }
                        else
                        {
                            destinationTile = checkTile;
                            break;
                        }
                    }
                    else
                    {
                        destinationTile = checkTile;
                        break;
                    }
                }

                destinationTile.filled = true;
                enemyTile.currentMinion.MoveMinionByAttack(destinationTile.coords, 0, tileMovementSpeed);
            }
        }
    }
    void TeleportMinion(Minion minionToTeleport, Tile destinationTile)
    {
        minionToTeleport.TeleportMinion(destinationTile);
    }


    void CalculDamageInfoPosition(Tile targetTile)
    {
        List<Tile> TileWithMinion = new List<Tile>();
        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");

        for (int i = 0; i < allMinions.Length; i++)
        {
            Minion minion = allMinions[i].GetComponent<Minion>();

            if (!zoneAttackTile.Contains(minion.currentTile))
                minion.healthBar.ShowHealthBar(true);
            else
                TileWithMinion.Add(minion.currentTile);
        }

        int tileUpAmount = Mathf.RoundToInt(Mathf.Abs(targetTile.coords.y - 13));
        int tileDownAmount = Mathf.RoundToInt(Mathf.Abs(targetTile.coords.y));
        float repartition = (TileWithMinion.Count + 1) / 2;

        if (tileUpAmount > Mathf.CeilToInt(repartition))
        {
            if (tileDownAmount > Mathf.CeilToInt(repartition))
            {
                for (int i = 0; i < TileWithMinion.Count; i++)
                {
                    Tile tile = TileWithMinion[i];
                    Vector3 infoPos = Vector3.zero;
                    switch (i)
                    {
                        case 0:
                            infoPos = targetTile.worldPos + Vector3.up * 10;
                            break;
                        case 1:
                            infoPos = targetTile.YUp.worldPos + Vector3.up * 10 - (targetTile.worldPos - targetTile.YUp.worldPos).normalized;
                            break;
                        case 2:
                            infoPos = targetTile.YDown.worldPos + Vector3.up * 10 + (targetTile.worldPos - targetTile.YUp.worldPos).normalized;
                            break;
                        case 3:
                            infoPos = targetTile.YUp.YUp.worldPos + Vector3.up * 10 - (targetTile.worldPos - targetTile.YUp.worldPos).normalized * 2;
                            break;
                        case 4:
                            infoPos = targetTile.YDown.YDown.worldPos + Vector3.up * 10 + (targetTile.worldPos - targetTile.YUp.worldPos).normalized * 2;
                            break;
                        case 5:
                            infoPos = targetTile.YUp.YUp.YUp.worldPos + Vector3.up * 10 - (targetTile.worldPos - targetTile.YUp.worldPos).normalized * 3;
                            break;
                        default:
                            break;
                    }

                    tile.currentMinion.healthBar.ShowDamageInfo(currentMinion, damageRange, damageType, infoPos);
                }
            }
            else
            {
                for (int i = 0; i < TileWithMinion.Count; i++)
                {
                    Tile tile = TileWithMinion[i];
                    Vector3 infoPos = Vector3.zero;

                    Tile tt = null;
                    GameManager.Instance.tilesDictionary.TryGetValue(new Vector2(targetTile.coords.x, (targetTile.coords.y + i)), out tt);

                    infoPos = tt.worldPos + Vector3.up * 10 - (targetTile.worldPos - targetTile.YUp.worldPos).normalized * i;

                    tile.currentMinion.healthBar.ShowDamageInfo(currentMinion, damageRange, damageType, infoPos);
                }
            }
        }
        else
        {
            for (int i = 0; i < TileWithMinion.Count; i++)
            {
                Tile tile = TileWithMinion[i];
                Vector3 infoPos = Vector3.zero;

                Tile tt = null;
                GameManager.Instance.tilesDictionary.TryGetValue(new Vector2(targetTile.coords.x, (targetTile.coords.y - (i + 1))), out tt);

                infoPos = tt.worldPos + Vector3.up * 10 - (targetTile.worldPos - targetTile.YDown.worldPos).normalized * (i + 1);

                tile.currentMinion.healthBar.ShowDamageInfo(currentMinion, damageRange, damageType, infoPos);
            }
        }
    }


    void InstanciateObstacle(Tile tile)
    {
        GameObject newObstacle = PhotonNetwork.Instantiate(ObstacleModel.name, new Vector3(tile.worldPos.x, 0, tile.worldPos.z), Quaternion.identity, 0);
        Obstacle obstacleComponent = newObstacle.GetComponent<Obstacle>();
        if (GameManager.Instance.yourTurn)
            obstacleComponent.myObstacle = true;

        obstacleComponent.InitialteObstacle(tile.coords, obstacleActiveTime);
    }


    //reset toute les list et variable pour l'attaque range
    public void ResetAttack()
    {
        currentMinion.onAttackInitiation = false;

        foreach (var tile in attackRangeTile)
            tile.ActivePossibleAttackTarget(false);

        attackRangeTile.Clear();
 
    }

    public void ResetAttackTargetTile()
    {
        foreach (var tile in attackRangeTile)
            tile.ActivePossibleAttackTarget(false);

    }

    //reset toute les listes et variable pour la zone deffect de l'attaque uniquement
    public void ResetAttackConfirmation()
    {
        //Debug.Log("reset attack confirmation");
        currentMinion.onAttackConfirmation = false;

        foreach (var tile in zoneAttackTile)
        {
            if (tile.currentMinion != null)
                tile.currentMinion.healthBar.HideDamageInfo();

            tile.ActiveAttackZonetarget(false);
        }

        zoneAttackTile.Clear();
    }



    //regarde si la tile selectionner est dans lattaque range
    public bool IsTileOnAttackRange(Tile tile)
    {
        bool value = false;

        if (attackRangeTile.Contains(tile))
            value = true;

        return value;
    }
}
