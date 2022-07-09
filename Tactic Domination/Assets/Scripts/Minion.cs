using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Minion : MonoBehaviour
{

    [PropertySpace(20, 0)]
    public string MinionName;
    [OnValueChanged("CalculStatsForAll")]
    [Range(1,10)]
    public int minionLevel = 1;
    public Sprite minionIconSprite;
    public string minionKey;


    public enum MinonRarety { Common, Rare, Epic};
    public MinonRarety rarety;


    [HideInInspector] public int bonusMovementAmount;
    [HideInInspector] public int movementAmountLeft;


    [Title("Health"), FoldoutGroup("Stats"), Indent, LabelText("Base"), PropertySpace(10, 0), OnValueChanged("CalculStatsForAll")]
    public int baseMaxHealth = 100;
    [FoldoutGroup("Stats"), Indent, LabelText("% Per Level"), PropertySpace(0, 10), OnValueChanged("CalculStatsForAll"), Range(0, 100)]
    public float healthPercentagePerLevel;

    [FoldoutGroup("Stats"), Indent, LabelText("current"), ReadOnly]
    public int maxHealth = 100;
    [FoldoutGroup("Stats"), Indent, LabelText("Max"), ReadOnly]
    public int maxLevelHealth;
    [HideInInspector]
    public int currentHealth;



    [Title("Movement"), FoldoutGroup("Stats"), Indent, LabelText("Base"), PropertySpace(20, 0), OnValueChanged("CalculStatsForAll"), Range(0, 15)]
    public int baseMovement = 3;
    [FoldoutGroup("Stats"), Indent, LabelText("% Per Level"), PropertySpace(0, 10), OnValueChanged("CalculStatsForAll"), Range(0, 100)]
    public float movementPercentagePerLevel;

    [FoldoutGroup("Stats"), Indent, LabelText("current"), ReadOnly]
    public int movementAmount = 4;
    [FoldoutGroup("Stats"), Indent, LabelText("Max"), ReadOnly]
    public int maxLevelMovement;

    [FoldoutGroup("Stats"), Indent, PropertySpace(10, 0)]
    public float movementSpeed = 20;
    [FoldoutGroup("Stats"), Indent]
    public bool StopWhenTurn = true;



    [Title("Tackle"), FoldoutGroup("Stats"), Indent, LabelText("Base"), PropertySpace(20, 0), OnValueChanged("CalculStatsForAll"), Range(0, 100)]
    public int baseTackle = 0;
    [FoldoutGroup("Stats"), Indent, PropertySpace(0, 10), Range(0, 100), OnValueChanged("CalculStatsForAll"), LabelText("% Per Level")]
    public float tacklePercentagePerLevel;

    [FoldoutGroup("Stats"), Indent, LabelText("current"), ReadOnly]
    public int tackle;
    [FoldoutGroup("Stats"), Indent, LabelText("Max"), ReadOnly]
    public int maxLevelTackle;



    [Title("Leak"), FoldoutGroup("Stats"), Indent, LabelText("Base"), PropertySpace(20, 0), OnValueChanged("CalculStatsForAll"), Range(0, 100)]
    public int baseLeak = 0;
    [FoldoutGroup("Stats"), Indent, LabelText("% Per Level"), PropertySpace(0, 10), OnValueChanged("CalculStatsForAll"), Range(0, 100)]
    public float leakPercentagePerLevel;

    [FoldoutGroup("Stats"), Indent, LabelText("current"), ReadOnly]
    public int leak;
    [FoldoutGroup("Stats"), Indent, LabelText("Max"), ReadOnly]
    public int maxLevelLeak;



    [Title("Critical Hit Chance"), FoldoutGroup("Stats"), Indent, LabelText("Base"), PropertySpace(20, 0), OnValueChanged("CalculStatsForAll"), Range(0, 100)]
    public int baseCriticalChance = 0;
    [FoldoutGroup("Stats"), Indent, LabelText("% Per Level"), PropertySpace(0, 10), OnValueChanged("CalculStatsForAll"), Range(0, 100)]
    public float CriticalPercentagePerLevel;

    [FoldoutGroup("Stats"), Indent, LabelText("current"), ReadOnly]
    public int criticalHitChance;
    [FoldoutGroup("Stats"), Indent, LabelText("Max"), ReadOnly]
    public int maxLevelCritical;



    [Title("Physical Resistance"), FoldoutGroup("Stats"), Indent, LabelText("Base"),PropertySpace(20, 0), OnValueChanged("CalculStatsForAll"), Range(-100, 100)]
    public int basePhysicalRes = 0;
    [FoldoutGroup("Stats"), Indent, LabelText("% Per Level"), PropertySpace(0, 10), OnValueChanged("CalculStatsForAll"), Range(0, 100)]
    public float physicalResPercentagePerLevel;

    [FoldoutGroup("Stats"), Indent, LabelText("current"), ReadOnly]
    public int physicalResist;
    [FoldoutGroup("Stats"), Indent, LabelText("Max"), PropertySpace(0, 10), ReadOnly]
    public int maxLevelPhysicalRes;



    [Title("Magical Resistance"), FoldoutGroup("Stats"), Indent, LabelText("Base"), PropertySpace(20, 0),OnValueChanged("CalculStatsForAll"), Range(-100, 100)]
    public int baseMagicRes = 0;
    [FoldoutGroup("Stats"), Indent, LabelText("% Per Level"), PropertySpace(0, 10), OnValueChanged("CalculStatsForAll"), Range(0, 100)]
    public float magicResPercentagePerLevel;

    [FoldoutGroup("Stats"), Indent, LabelText("current"), ReadOnly]
    public int magicalResist;
    [FoldoutGroup("Stats"), Indent, LabelText("Max"), PropertySpace(0, 30), ReadOnly]
    public int maxLevelMagicRes;


    [FoldoutGroup("First Attack")]
    [HideLabel, Indent, InlineEditor(objectFieldMode:InlineEditorObjectFieldModes.Hidden)]
    public MinionAttack firstAttack;
    [FoldoutGroup("Second Attack")]
    [PropertySpace(10, 10), HideLabel, Indent, InlineEditor(objectFieldMode: InlineEditorObjectFieldModes.Hidden)]
    public MinionAttack secondAttack;
    [FoldoutGroup("Special Attack")]
    [PropertySpace(10, 10), HideLabel, Indent, InlineEditor(objectFieldMode: InlineEditorObjectFieldModes.Hidden)]
    public MinionAttack specialAttack;


    [FoldoutGroup("Infos"), ReadOnly]
    public bool myMinion;
    [FoldoutGroup("Infos"), ReadOnly]
    public bool isDead;
    [FoldoutGroup("Infos"), ReadOnly]
    public bool haveMoved;
    [FoldoutGroup("Infos"), ReadOnly]
    public Tile targetTile;

    [FoldoutGroup("Infos"), ReadOnly]
    public bool onAttack, onAttackAnim, onAttackConfirmation, onAttackInitiation, onPathSelection, onMove, waitForAnotherAction, ready;

    [FoldoutGroup("Infos"), ReadOnly]
    public int stundTurnLeft, silenceTurnLeft, superForceTurnLeft, freezeTurnLeft,
    overchargeTurnLeft, burnTurnLeft, burnDamageAmount, flowerSkinTurnLeft, weakenedTurnLeft, invulnerableTurnLeft;

    [FoldoutGroup("Infos"), ReadOnly]
    public Glyph.GlyphType currentGlyph;

    [FoldoutGroup("Infos"), ReadOnly]
    public int GoodEffectTurnAdd, badEffectTurnReduction, GiveEffectTurnAdd;


    [HideInInspector]
    public List<Tile> tilesPath = new List<Tile>();
    [HideInInspector]
    public Vector2[] coordsPath = new Vector2[20];

 
  




    [FoldoutGroup("Reference"), Indent, PropertySpace(10, 0)]
    public HealthBar healthBar;
    [FoldoutGroup("Reference"), Indent]
    public GameObject playerMark;
    [FoldoutGroup("Reference"), Indent]
    public GameObject shadow;
    [FoldoutGroup("Reference"), Indent]
    public Material playerMarkAllyMat;
    [FoldoutGroup("Reference"), Indent]
    public Transform spawnProjectilTransform;

    [FoldoutGroup("Reference"), Indent, PropertySpace(10, 0)]
    public ParticleSystem DeathVfx;
    [FoldoutGroup("Reference"), Indent]
    public ParticleSystem damageHit;

    [FoldoutGroup("Reference"), Indent, PropertySpace(10, 0)]
    public ParticleSystem stundVfx;
    [FoldoutGroup("Reference"), Indent]
    public ParticleSystem silenceVfx;
    [FoldoutGroup("Reference"), Indent]
    public ParticleSystem superForceVfx;
    [FoldoutGroup("Reference"), Indent]
    public ParticleSystem freezeVfx;
    [FoldoutGroup("Reference"), Indent]
    public ParticleSystem overChargeVfx;
    [FoldoutGroup("Reference"), Indent]
    public ParticleSystem burnVfx;
    [FoldoutGroup("Reference"), Indent]
    public ParticleSystem floweSkinVfx;
    [FoldoutGroup("Reference"), Indent]
    public ParticleSystem weakenedVfx;
    [FoldoutGroup("Reference"), Indent, PropertySpace(0, 10)]
    public ParticleSystem invulnerableVfx;



    [HideInInspector] public Tile currentTile;
    [HideInInspector] public Tile destinationTile;
    [HideInInspector] public MinionAttack currentAttack;
    [HideInInspector] public GameObject minionVisual;
    [HideInInspector] public PhotonView view;
    [HideInInspector] public Material MinionMaterial;

    MinionPathFinding pathFinder;
    Animator anim;
    Vector3 originLocalTrans;
    Coroutine movementCoroutine;
    SkinnedMeshRenderer MinionMeshRenderer;
    MeshRenderer[] allMeshRenderer;

    List<Tile> PossibleTiles = new List<Tile>();
    List<Tile> PossibleTilesAfterTackle = new List<Tile>();


    private void Awake()
    {
        view = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        healthBar = GetComponentInChildren<HealthBar>();
        MinionMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        allMeshRenderer = GetComponentsInChildren<MeshRenderer>();

        minionVisual = transform.GetChild(0).gameObject;
        originLocalTrans = minionVisual.transform.localPosition;

        Material newMat = new Material(MinionMeshRenderer.sharedMaterial);
        MinionMaterial = newMat;
        MinionMaterial.SetFloat("Hit", 0);
        MinionMeshRenderer.sharedMaterial = newMat;

        foreach (var renderer in allMeshRenderer)
        {
            if (renderer.gameObject != playerMark && renderer.gameObject != shadow)
                renderer.sharedMaterial = newMat;
        }

        for (int i = 0; i < GameManager.Instance.allMinions.Length; i++)
        {
            if (GameManager.Instance.allMinions[i] == null)
            {
                GameManager.Instance.allMinions[i] = this;
                break;
            }
        }
          
    }
    void Update()
    {
        HealthBarFaceCam();
        //   minionVisual.transform.localPosition = originLocalTrans;

        if (onAttackAnim)
            MoveWithAnim();
    }


    public void NewTurn()
    {
        ResetAttacks();
        ResetPathFinder();

        if (burnTurnLeft > 0)
            GetDamage(burnDamageAmount, DamageType.Magic);

        haveMoved = false;

        if (freezeTurnLeft > 0)
            movementAmountLeft = Mathf.CeilToInt(movementAmount / 2);
        else
            movementAmountLeft = movementAmount;

        if (firstAttack.cooldownLeft > 0)
            firstAttack.cooldownLeft--;

        if (secondAttack.cooldownLeft > 0)
            secondAttack.cooldownLeft--;

        if (specialAttack.cooldownLeft > 0)
            specialAttack.cooldownLeft--;

        if (currentTile.haveGlyph)
        {
            currentTile.glyph.ApplyEffect();
        }
    }

    public void EndTurn()
    {
        view.RPC("EndTurnForAll", RpcTarget.All);
    }
    [PunRPC]
    void EndTurnForAll()
    {
        // onMove = false;
        // onAttack = false;

        if (silenceTurnLeft > 0)
            silenceTurnLeft--;

        if (stundTurnLeft > 0)
            stundTurnLeft--;

        if (superForceTurnLeft > 0)
            superForceTurnLeft--;

        if (burnTurnLeft > 0)
            burnTurnLeft--;

        if (freezeTurnLeft > 0)
            freezeTurnLeft--;

        if (overchargeTurnLeft > 0)
            overchargeTurnLeft--;

        UpdateEffectVfx();
    }



    #region movement and rotation
    //Bouge le minion dune tile a lautre
    public void MovementAction(Vector2[] pathCoords)
    {
        view.RPC("MoveForAll", RpcTarget.All, pathCoords);
    }
    [PunRPC]
    public void MoveForAll(Vector2[] pathCoords)
    {
        if (movementCoroutine == null)
            movementCoroutine = StartCoroutine(pathTravel());

        IEnumerator pathTravel()
        {
            Tween t = null;
            bool reached = false;
            onMove = true;

            Tile nextTile;
            Tile lastTile = null;
            GameManager.Instance.tilesDictionary.TryGetValue(pathCoords[0], out nextTile);

            transform.DOKill();

            RotateWithLerp(Quaternion.LookRotation((nextTile.worldPos) - (currentTile.worldPos)));
            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < pathCoords.Length; i++)
            {
                reached = false;


                if (pathCoords[i] == Vector2.zero)
                    break;

                PlayAnimation("Walk");

                t = transform.DOMove(nextTile.worldPos, movementSpeed)
                    .SetEase(Ease.Linear)
                    .SetSpeedBased(true);

                t.OnComplete(() =>
                {
                    reached = true;

                    if (StopWhenTurn)
                        PlayAnimation("Idle");

                    transform.position = nextTile.worldPos;
                    GameManager.Instance.tilesDictionary.TryGetValue(pathCoords[i + 1], out nextTile);
                    GameManager.Instance.tilesDictionary.TryGetValue(pathCoords[i], out lastTile);

                    RotateWithLerp(Quaternion.LookRotation((nextTile.worldPos) - (lastTile.worldPos)));
                });


                yield return new WaitWhile(() => reached == false);


                if (StopWhenTurn)
                    yield return new WaitForSeconds(0.2f);
            }



            PlayAnimation("Idle");
            UIManager.Instance.UpdateActionPanel(this);
            if (view.IsMine)
                ResetPathFinder();

            if (currentTile.haveTrap)
            {
                yield return new WaitForSeconds(0.2f);

                currentTile.currentTrap.ApplyEffect();
            }

            if (currentTile.haveGlyph)
            {
                yield return new WaitForSeconds(0.2f);

                if (currentTile.haveGlyph)
                {
                    currentTile.glyph.ApplyEffect();
                }
            }
            else
            {
                CloseAllGlyphEffect();
                UpdateEffectVfx();
            }

            foreach (var tile in pathFinder.PathTile)
                tile.ActivePathDestinationTarget(false);


            onMove = false;
            movementCoroutine = null;
        }
    }

    public void MoveMinionByAttack(Vector2 pathCoord, int pushDamage, float speed)
    {
        Tile destinationTile = null;
        GameManager.Instance.tilesDictionary.TryGetValue(pathCoord, out destinationTile);
        ChangeCurrentTile(destinationTile);
        view.RPC("MoveMinionByAttackForAll", RpcTarget.All, pathCoord, pushDamage, speed);
    }
    [PunRPC]
    public void MoveMinionByAttackForAll(Vector2 pathCoord, int pushDamage, float speed)
    {
        onMove = true;
        Tile destinationTile;
        GameManager.Instance.tilesDictionary.TryGetValue(pathCoord, out destinationTile);

        Tween t = transform.DOMove(destinationTile.worldPos, speed)
                .SetEase(Ease.Linear)
                .SetSpeedBased(true);

        t.OnComplete(() =>
        {
            onMove = false;

            if (pushDamage > 0)
                GetDamage(pushDamage, DamageType.Push);
        });
    }

    public void TeleportMinion(Tile destinationTile)
    {
        ChangeCurrentTile(destinationTile);
        view.RPC("TeleportMinionForAll", RpcTarget.All, destinationTile.coords);
    }
    [PunRPC]
    public void TeleportMinionForAll(Vector2 destinationTileCoords)
    {
        GameManager.Instance.tilesDictionary.TryGetValue(destinationTileCoords, out destinationTile);
        transform.position = destinationTile.worldPos;
    }


    //rotate le minion avec un lerp
    public void RotateWithLerp(Quaternion rotation)
    {
        view.RPC("RotateWithLerpForAll", RpcTarget.All, rotation);
    }
    [PunRPC]
    public void RotateWithLerpForAll(Quaternion rotation)
    {
        minionVisual.transform.DORotateQuaternion(rotation, 0.2f)
                             .SetEase(Ease.InOutSine);
    }


    //Rotationne instantanément le minion dans la direction voulue
    public void RotateMinion(Vector3 lookDirection)
    {
        view.RPC("RotateMinionForAll", RpcTarget.All, lookDirection);
    }
    [PunRPC]
    void RotateMinionForAll(Vector3 lookDirection)
    {
        minionVisual.transform.rotation = Quaternion.LookRotation(lookDirection);
    }
    #endregion


    #region animation
    //fait bouger le minion avec les animation
    void MoveWithAnim()
    {
        minionVisual.transform.localPosition = originLocalTrans;
        transform.position += anim.deltaPosition;
    }

    //Joue une animation
    public void PlayAnimation(string animName)
    {
        view.RPC("PlayAnimationForAll", RpcTarget.All, animName);
    }
    public void PlayAttackAnimation(string animName)
    {
        onAttackAnim = true;
        view.RPC("PlayAnimationForAll", RpcTarget.All, animName);
    }
    [PunRPC]
    void PlayAnimationForAll(string animName)
    {
        anim.CrossFade(animName, 0.1f);
    }

    //active ou desactive le rootmotion de l'animator
    public void SetRootMotion(bool value)
    {
        view.RPC("SetRootMotionForAll", RpcTarget.All, value);
    }
    [PunRPC]
    void SetRootMotionForAll(bool value)
    {
        anim.applyRootMotion = value;
    }




    //Re centre le minion sur ca current tile apres une animation
    public void RecenterMinion()
    {
        view.RPC("RecenterMinionForAll", RpcTarget.All);
    }
    [PunRPC]
    public void RecenterMinionForAll()
    {
        transform.DOMove(currentTile.worldPos, 2f)
            .SetEase(Ease.InOutSine);
    }
    #endregion


    #region damage and effect
    //applique le damage  et les effet sur le minions
    public void GetDamage(int damageAmount, DamageType damageType, bool critical = false)
    {
        view.RPC("GetDamageForAll", RpcTarget.All, damageAmount, (int)damageType, critical);
    }
    [PunRPC]
    public void GetDamageForAll(int damageAmount, int type, bool critical)
    {
        if (invulnerableTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Invulnerable)
            return;

        float finalDamage = damageAmount;

        DamageType damageType = (DamageType)type;

        if (flowerSkinTurnLeft <= 0)
        {
            switch (damageType)
            {
                case DamageType.Physical:

                    finalDamage -= ((physicalResist * finalDamage) / 100);

                    break;
                case DamageType.Magic:


                    finalDamage -= ((magicalResist * finalDamage) / 100);

                    break;
                default:
                    break;
            }
        }

        currentHealth -= Mathf.FloorToInt(finalDamage);

        float floatMaxHealth = maxHealth;
        float floatDamage = finalDamage;


        healthBar.ShowDamageAmount(Mathf.FloorToInt(floatDamage));
        healthBar.ChangeHealthBarValue(floatMaxHealth, Damage: floatDamage, critical: critical);

        if (damageHit != null)
            damageHit.Play();

        if (currentHealth <= 0)
            Dead();

        MinionMaterial.SetFloat("_Hit", 1);
        Tween t = minionVisual.transform.DOScale(1.3f, 0.2f)
            .SetRelative(true)
            .SetLoops(2, LoopType.Yoyo);

        t.OnComplete(() =>
        {
            MinionMaterial.SetFloat("_Hit", 0);
        });
    }

    public void GetHeal(int healAmount)
    {
        view.RPC("GetHealForAll", RpcTarget.All, healAmount);
    }
    [PunRPC]
    public void GetHealForAll(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        float floatMaxHealth = maxHealth;
        float floatCurrentHealth = currentHealth;
        float floatHeal = healAmount;


        healthBar.ChangeHealthBarValue(floatMaxHealth, heal: floatHeal);
    }

    public void GetEffect(EffectType type, int turnAmount, int burnDamage)
    {
        view.RPC("GetHealForAll", RpcTarget.All, (int)type, turnAmount, type == EffectType.Burn ? burnDamage : 0);
    }
    [PunRPC]
    void GetEffectForAll(int type, int turnAmount, int burnDamage)
    {
        EffectType effect = (EffectType)type;

        switch (effect)
        {
            case EffectType.Stun:
                freezeTurnLeft = 0;
                overchargeTurnLeft = 0;
                stundTurnLeft = turnAmount;
                break;
            case EffectType.Silence:
                superForceTurnLeft = 0;
                overchargeTurnLeft = 0;
                silenceTurnLeft = turnAmount;
                break;
            case EffectType.SuperForce:
                silenceTurnLeft = 0;
                weakenedTurnLeft = 0;
                superForceTurnLeft = turnAmount;
                break;
            case EffectType.Freeze:
                stundTurnLeft = 0;
                burnTurnLeft = 0;
                freezeTurnLeft = turnAmount;
                break;
            case EffectType.Overcharge:
                stundTurnLeft = 0;
                silenceTurnLeft = 0;
                overchargeTurnLeft = turnAmount;
                break;
            case EffectType.Burn:
                freezeTurnLeft = 0;
                flowerSkinTurnLeft = 0;
                burnTurnLeft = turnAmount;
                burnDamageAmount = burnDamage;
                break;
            case EffectType.FlowerSkin:
                weakenedTurnLeft = 0;
                invulnerableTurnLeft = 0;
                flowerSkinTurnLeft = turnAmount;
                break;
            case EffectType.Weakened:
                superForceTurnLeft = 0;
                invulnerableTurnLeft = 0;
                weakenedTurnLeft = turnAmount;
                break;
            case EffectType.Invulnerable:
                burnTurnLeft = 0;
                flowerSkinTurnLeft = 0;
                invulnerableTurnLeft = turnAmount;
                break;
            default:
                break;
        }
    }


    public void GetEffectByGlyph(Glyph.GlyphType type, bool forAll)
    {
        if (forAll)
            view.RPC("GetEffectByGlyphForAll", RpcTarget.All, (int)type);
        else
        {
            currentGlyph = type;
           
            UpdateEffectVfx();
        }
    }
    [PunRPC]
    public void GetEffectByGlyphForAll(int type)
    {
        Glyph.GlyphType glyphType = (Glyph.GlyphType)type;

        CloseAllGlyphEffect();

        currentGlyph = glyphType;

        UpdateEffectVfx();
    }
    public void CloseAllGlyphEffect()
    {
        currentGlyph = Glyph.GlyphType.None;
    }


    public void UpdateEffectVfx()
    {
        if (stundTurnLeft > 0)
        {
            if (stundVfx != null)
                if (!stundVfx.isPlaying)
                    stundVfx.Play();
        }
        else
        {
            if (stundVfx != null)
                stundVfx.Stop();
        }

        if (silenceTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Silence)
        {
            if (silenceVfx != null)
                if (!silenceVfx.isPlaying)
                    silenceVfx.Play();
        }
        else
        {
            if (silenceVfx != null)
                silenceVfx.Stop();
        }

        if (superForceTurnLeft > 0 || currentGlyph == Glyph.GlyphType.SuperForce)
        {
            if (superForceVfx != null)
                if (!superForceVfx.isPlaying)
                    superForceVfx.Play();
        }
        else
        {
            if (superForceVfx != null)
                superForceVfx.Stop();
        }

        if (freezeTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Freeze)
        {
            if (freezeVfx != null)
                if (!freezeVfx.isPlaying)
                    freezeVfx.Play();
        }
        else
        {
            if (freezeVfx != null)
                freezeVfx.Stop();
        }

        if (overchargeTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Overcharge)
        {
            if (overChargeVfx != null)
                if (!overChargeVfx.isPlaying)
                    overChargeVfx.Play();
        }
        else
        {
            if (overChargeVfx != null)
                overChargeVfx.Stop();
        }

        if (burnTurnLeft > 0)
        {
            if (burnVfx != null)
                if (!burnVfx.isPlaying)
                    burnVfx.Play();
        }
        else
        {
            if (burnVfx != null)
                burnVfx.Stop();
        }

        if (flowerSkinTurnLeft > 0 || currentGlyph == Glyph.GlyphType.SkinFlower)
        {
            if (floweSkinVfx != null)
                if (!floweSkinVfx.isPlaying)
                    floweSkinVfx.Play();
        }
        else
        {
            if (floweSkinVfx != null)
                floweSkinVfx.Stop();
        }

        if (weakenedTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Weakened)
        {
            if (weakenedVfx != null)
                if (!weakenedVfx.isPlaying)
                    weakenedVfx.Play();
        }
        else
        {
            if (weakenedVfx != null)
                weakenedVfx.Stop();
        }

        if (invulnerableTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Invulnerable)
        {
            if (invulnerableVfx != null)
                if (!invulnerableVfx.isPlaying)
                    invulnerableVfx.Play();
        }
        else
        {
            if (invulnerableVfx != null)
                invulnerableVfx.Stop();
        }
    }

    #endregion


    #region attack


    //selectionne normal attack ou si en mode waitforconfirm copnfirm lattack
    public void FirstAttack()
    {
        if (silenceTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Silence)
        {
            return;
        }

        if (currentAttack == null)
        {
            if (firstAttack.cooldownLeft == 0)
                firstAttack.InitiateAttack();
        }
        else
        {
            if (currentAttack == firstAttack)
            {
                if (onAttackConfirmation)
                {
                    if (GameManager.Instance.yourTurn)
                    {
                        if (myMinion)
                            firstAttack.ConfirmAttack(targetTile);
                    }
                    else
                    {
                        if (myMinion)
                            UIManager.Instance.ShowNotYourTurnPanel();
                        else if (NetworkManager.Instance.testSolo)
                            firstAttack.ConfirmAttack(targetTile);
                    }
                }
            }
            else
            {
                currentAttack = firstAttack;
                firstAttack.InitiateAttack();
            }
        }
    }

    //selectionne special attack ou si en mode waitforconfirm copnfirm lattack
    public void SecondAttack()
    {
        if (silenceTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Silence)
        {
            return;
        }

        if (currentAttack == null)
        {
            if (secondAttack.cooldownLeft == 0)
                secondAttack.InitiateAttack();

        }
        else
        {
            if (currentAttack == secondAttack)
            {
                if (onAttackConfirmation)
                {
                    if (GameManager.Instance.yourTurn)
                    {
                        if (myMinion)
                            secondAttack.ConfirmAttack(targetTile);
                    }
                    else
                    {
                        if (myMinion)
                            UIManager.Instance.ShowNotYourTurnPanel();
                        else if (NetworkManager.Instance.testSolo)
                            secondAttack.ConfirmAttack(targetTile);
                    }
                }
            }
            else
            {
                secondAttack.InitiateAttack();
                currentAttack = secondAttack;
            }
        }
    }

    //selectionne special attack ou si en mode waitforconfirm copnfirm lattack
    public void SpecialAttack()
    {
        if (silenceTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Silence)
        {
            return;
        }
        if (currentAttack == null)
        {
            if (specialAttack.cooldownLeft == 0)
                specialAttack.InitiateAttack();

        }
        else
        {
            if (currentAttack == specialAttack)
            {
                if (onAttackConfirmation)
                {
                    if (GameManager.Instance.yourTurn)
                    {
                        if (myMinion)
                            specialAttack.ConfirmAttack(targetTile);
                    }
                    else
                    {
                        if (myMinion)
                            UIManager.Instance.ShowNotYourTurnPanel();
                        else if (NetworkManager.Instance.testSolo)
                            specialAttack.ConfirmAttack(targetTile);
                    }
                }

            }
            else
            {
                specialAttack.InitiateAttack();
                currentAttack = specialAttack;
            }
        }
    }

    public void ResetAttacks()
    {
        currentAttack = null;

        firstAttack.ResetAttack();
        secondAttack.ResetAttack();
        specialAttack.ResetAttack();
    }
    public void ResetAttacksTarget()
    {
        firstAttack.ResetAttackTargetTile();
        secondAttack.ResetAttackTargetTile();
        specialAttack.ResetAttackTargetTile();
    }

    public void SetOnAttack(bool value)
    {
        onAttack = value;
    }
    #endregion


    #region Dead
    void Dead()
    {
        view.RPC("DeadForAll", RpcTarget.All);
    }
    [PunRPC]
    void DeadForAll()
    {
        Debug.Log(gameObject.name + " : is Dead");

        StartCoroutine(deathProcess());
        IEnumerator deathProcess()
        {
            isDead = true;
            healthBar.gameObject.SetActive(false);

            anim.CrossFade("Death", 0.1f);

            yield return new WaitForSeconds(3.5f);

            GameObject vfx;
            gameObject.SetActive(false);
            if (DeathVfx != null)
            {
                vfx = Instantiate(DeathVfx, transform.position, Quaternion.identity).gameObject;

                if (PhotonNetwork.IsMasterClient == true)
                    vfx.transform.eulerAngles = new Vector3(-90, 180, 0);
                else
                    vfx.transform.eulerAngles = new Vector3(-90, 0, 0);
            }

            currentTile.filled = false;
            currentTile.currentMinion = null;
        }
    }

    public void PlayOnMinionVFX(int whichAttack)
    {
        view.RPC("PlayOnMinionVFXForAll", RpcTarget.All, whichAttack);
    }
    [PunRPC]
    void PlayOnMinionVFXForAll(int wichAttack)
    {
        switch (wichAttack)
        {
            case 1:
                firstAttack.vfxOnMinionPrefab.Play();
                break;
            case 2:
                secondAttack.vfxOnMinionPrefab.Play();
                break;
            case 3:
                specialAttack.vfxOnMinionPrefab.Play();
                break;
            default:
                break;
        }
    }
    #endregion


    #region PathFinder
    //Regarde ou le minion peu bouger avec sont mouvement range
    public void FindPossiblePath()
    {
        if (stundTurnLeft > 0)
            return;

        pathFinder = new MinionPathFinding();
        onPathSelection = true;
        //ResetAttacks();
        PossibleTiles.Clear();
        PossibleTilesAfterTackle.Clear();
        GameManager.Instance.DeactivateAllTargetOnTile();


        PossibleTiles = pathFinder.GetPossibleDestinationTile(this);
        foreach (var tile in pathFinder.GetPossibleDestinationAfterTackle(this))
        {
            PossibleTilesAfterTackle.Add(tile);
            tile.ActivePossiblePathTarget(true);
        }


        foreach (var tile in PossibleTiles)
        {
            if (!PossibleTilesAfterTackle.Contains(tile))
                tile.ActiveImpossiblePathTarget(true);
            else
                tile.ActivePossiblePathTarget(true);
        }
    }

    Coroutine findPathCoroutine;

    //choisie le chemin le plus court
    public void FindShortestPath(Tile startTile, Tile destination)
    {
        if (GameManager.Instance.actionPointLeft < 1 * (overchargeTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1) && haveMoved == false)
        {
            UIManager.Instance.ShowNotEnoughtActionPointPanel();
            return;
        }


        onPathSelection = false;

        GameManager.Instance.DeactivateAllTargetOnTile();

        if (findPathCoroutine == null)
            findPathCoroutine = StartCoroutine(FindBestPath());
        else
        {
            StopCoroutine(findPathCoroutine);
            findPathCoroutine = StartCoroutine(FindBestPath());
        }
        IEnumerator FindBestPath()
        {
            SoundManager.Instance.PlayUISound("PathDestination");

            yield return new WaitWhile(() => onAttack);
            yield return new WaitWhile(() => onMove);

            List<Tile> tilePath = new List<Tile>();
            List<Tile> BestPathTiles = new List<Tile>();
            List<Tile> tilesSearched = new List<Tile>();
            List<Tile> searchingTiles = new List<Tile>();
            List<Tile> nextTilesSearch = new List<Tile>();
            bool pathFound = false;
            int pathCost = 0;

            searchingTiles.Add(currentTile);

            onMove = true;

            //regarde toute les tile une apres lautre pour trouver le path le plus court
            while (pathFound == false)
            {

                pathCost++;
                foreach (var tile in searchingTiles)
                    tilesSearched.Add(tile);

                foreach (var tile in pathFinder.GetPossibleTileDirection(this, searchingTiles, tilesSearched, destination))
                {
                    nextTilesSearch.Add(tile);

                    if (tile == destination)
                    {
                        pathFound = true;
                        break;
                    }
                }

                foreach (var tile in searchingTiles)
                    tilesSearched.Add(tile);

                searchingTiles.Clear();
                foreach (Tile tile in nextTilesSearch)
                    searchingTiles.Add(tile);

                yield return null;
            }


            foreach (var tile in pathFinder.ReconstitutePath(destination, pathCost))
                tilePath.Add(tile);

            foreach (var tile in pathFinder.PathAfterTrapCheck(this, tilePath))
                BestPathTiles.Add(tile);

            for (int i = 0; i < 20; i++)
                coordsPath[i] = Vector2.zero;
            for (int i = 0; i < pathFinder.GetPathCorner(this, BestPathTiles).Count; i++)
                coordsPath[i] = pathFinder.GetPathCorner(this, BestPathTiles)[i];

            foreach (var tile in tilePath)
                tile.ActivePathDestinationTarget(true);

            if (!haveMoved)
                GameManager.Instance.ConsumeActionPoint(1 * (overchargeTurnLeft > 0 || currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1));

            destination.ActivePathDestinationTarget(true);
            MovementAction(coordsPath);
            ChangeCurrentTile(BestPathTiles[BestPathTiles.Count - 1]);
            movementAmountLeft -= BestPathTiles.Count;

            haveMoved = true;

            findPathCoroutine = null;
        }
    }
    public bool PossiblePathTile(Tile tile)
    {
        if (!PossibleTilesAfterTackle.Contains(tile) || tile.haveObstacle || tile.filled)
            return false;
        else return true;
    }
    public void ResetPathFinder()
    {
        onPathSelection = false;
        PossibleTilesAfterTackle.Clear();
    }
    #endregion


    public void InitiateTrap(int whichAttack, Tile tile)
    {
        view.RPC("InitiateTrapForAll", RpcTarget.All, whichAttack, tile.coords);
    }
    [PunRPC]
    public void InitiateTrapForAll(int whichAttack, Vector2 tileCoord)
    {
        MinionAttack attack = null;
        Tile tile = GameManager.Instance.tilesDictionary[tileCoord];

        switch (whichAttack)
        {
            case 1:
                attack = firstAttack;
                break;
            case 2:
                attack = secondAttack;
                break;
            case 3:
                attack = specialAttack;
                break;
            default:
                break;
        }

        GameObject newTrap = Instantiate(attack.trapModel, tile.worldPos, Quaternion.identity);
        Trap trapComponent = newTrap.GetComponent<Trap>();


        trapComponent.UpdateInfo(myMinion, tileCoord,
            attack.trapActiveTime, attack.damageRange,
            attack.damageType, attack.silence,
            attack.stun, attack.superForce,
            attack.freeze, attack.overcharge,
            attack.burn, attack.burnDamageAmount,
            attack.flowerSkin, attack.weakened,
            attack.invulnerable, attack.turnAmountEffect, criticalHitChance);
    }

    public void InitiateGlyph(int whichAttack, Tile tile)
    {
        view.RPC("InitiateGlyphForAll", RpcTarget.All, whichAttack, tile.coords);
    }
    [PunRPC]
    public void InitiateGlyphForAll(int whichAttack, Vector2 tileCoord)
    {
        MinionAttack attack = null;
        Tile tile = GameManager.Instance.tilesDictionary[tileCoord];

        switch (whichAttack)
        {
            case 1:
                attack = firstAttack;
                break;
            case 2:
                attack = secondAttack;
                break;
            case 3:
                attack = specialAttack;
                break;
            default:
                break;
        }

        tile.glyph.InitiateGlyph(myMinion, tile.coords, attack.glyphTurnAmount, (int)attack.glyphTarget, (attack.glyphType == Glyph.GlyphType.Damage ? attack.glyphDamageRange : Vector2.zero), (int)attack.glyphType);
    }


    void HealthBarFaceCam()
    {
        if (healthBar != null)
            healthBar.transform.LookAt(new Vector3(healthBar.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z));
    }

    public void ChangeCurrentTile(Tile tile)
    {
        view.RPC("ChangeTile", RpcTarget.All, tile.coords);
    }
    [PunRPC]
    void ChangeTile(Vector2 tileCoords)
    {
        if (currentTile != null)
        {
            currentTile.currentMinion = null;
            currentTile.filled = false;
        }
        GameManager.Instance.tilesDictionary.TryGetValue(tileCoords, out currentTile);

        currentTile.filled = true;
        currentTile.currentMinion = this;
    }


    public void CalculStats()
    {
        view.RPC("CalculStatsForAll", RpcTarget.All, minionLevel);
    }

    [PunRPC]
    public void CalculStatsForAll(int level)
    {     
        float processHealth = baseMaxHealth;
        for (int i = 1; i < 10; i++)
        {
            if(i == level)
                maxHealth = Mathf.RoundToInt(processHealth);

            processHealth += ((processHealth * healthPercentagePerLevel) / 100);
        }
        maxLevelHealth = Mathf.RoundToInt(processHealth);

        float processMovement = baseMovement;
        for (int i = 1; i < 10; i++)
        {
            if (i == level)
                movementAmount = Mathf.RoundToInt(processMovement);

            processMovement += ((processMovement * movementPercentagePerLevel) / 100);
        }     
        maxLevelMovement = Mathf.RoundToInt(processMovement);

        float processTackle = baseTackle;
        for (int i = 1; i < 10; i++)
        {
            if (i == level)
                tackle = Mathf.RoundToInt(processTackle);

            processTackle += ((processTackle * tacklePercentagePerLevel) / 100);
        }
        maxLevelTackle = Mathf.RoundToInt(processTackle);

        float processLeak = baseLeak;
        for (int i = 1; i < 10; i++)
        {
            if (i == level)
                leak = Mathf.RoundToInt(processLeak);

            processLeak += ((processLeak * leakPercentagePerLevel) / 100);
        }
        maxLevelLeak = Mathf.RoundToInt(processLeak);

        float processCritical = baseCriticalChance;
        for (int i = 1; i < 10; i++)
        {
            if (i == level)
                criticalHitChance = Mathf.RoundToInt(processCritical);

            processCritical += ((processCritical * CriticalPercentagePerLevel) / 100);
        }       
        maxLevelCritical = Mathf.RoundToInt(processCritical);

        if(basePhysicalRes > 0)
        {
            float processPhysicalRes = basePhysicalRes;
            for (int i = 1; i < 10; i++)
            {
                if (i == level)
                    physicalResist = Mathf.RoundToInt(processPhysicalRes);

                processPhysicalRes += ((processPhysicalRes * physicalResPercentagePerLevel) / 100);
            }
            maxLevelPhysicalRes = Mathf.RoundToInt(processPhysicalRes);
        }
        else
        {
            maxLevelPhysicalRes = basePhysicalRes;
            physicalResist = basePhysicalRes;
        }

        if(baseMagicRes > 0)
        {
            float processMagicRes = baseMagicRes;
            for (int i = 1; i < 10; i++)
            {
                if (i == level)
                    magicalResist = Mathf.RoundToInt(processMagicRes);

                processMagicRes += ((processMagicRes * magicResPercentagePerLevel) / 100);
            }
            maxLevelMagicRes = Mathf.RoundToInt(processMagicRes);
        }
        else
        {
            maxLevelMagicRes = baseMagicRes;
            magicalResist = baseMagicRes;
        }

        if(level == 10)
        {
            maxHealth = maxLevelHealth;
            movementAmount = maxLevelMovement;
            tackle = maxLevelTackle;
            leak = maxLevelLeak;
            criticalHitChance = maxLevelCritical;
            physicalResist = maxLevelPhysicalRes;
            physicalResist = maxLevelMagicRes;
        }

        firstAttack.ClalculeAttackStat();
        secondAttack.ClalculeAttackStat();
        specialAttack.ClalculeAttackStat();      
    }

}
