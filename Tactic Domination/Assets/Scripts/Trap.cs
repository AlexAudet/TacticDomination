using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Photon.Pun;

public class Trap : MonoBehaviour
{
    public PhotonView view;
    public int turnLeft;
    public Tile currentTile;
    public bool myTrap;
    public MeshRenderer targetMark;
    public Material allyMaterial;
    public ParticleSystem spawnVfx;
    public ParticleSystem activationVfx;

    public DamageType damageType;
    public Vector2 damageRange;
    public bool silence;
    public bool stun;
    public bool superForce;
    public bool freeze;
    public bool overcharge;
    public bool burn;
    public int burnDamage;
    public bool skinFlower;
    public bool weakened;
    public bool invulnerable;
    public int turnEffectTime;
    public int criticalHitChance;


    public void UpdateInfo(bool myMinion, Vector2 tileCoords, int activeTime, Vector2 dr, DamageType dt, bool sn, bool st, bool sfo, bool fz, bool oc, bool bu, int bd, bool sf, bool wk, bool iv, int turnEffect, int chc)
    {
        if (spawnVfx != null)
            spawnVfx.Play();

        GameManager.Instance.tilesDictionary.TryGetValue(tileCoords, out currentTile);

        if (currentTile.haveTrap)
            currentTile.DestroyTrap(false);

        myTrap = myMinion;
        turnLeft = activeTime;
        damageRange = dr;
        silence = sn;
        stun = st;
        superForce = sfo;
        freeze = fz;
        overcharge = oc;
        burn = bu;
        burnDamage = bd;
        skinFlower = sf;
        weakened = wk;
        invulnerable = iv;
        damageType = dt;
        turnEffectTime = turnEffect;
        criticalHitChance = chc;


        currentTile.haveTrap = true;
        currentTile.currentTrap = this;

        if (myTrap)
            targetMark.sharedMaterial = allyMaterial;
    }


    public void NewTurn()
    {
        turnLeft--;

        if(turnLeft == 0)
            currentTile.DestroyTrap(true);
    }

    public void ApplyEffect()
    {
        if (activationVfx != null)
            activationVfx.Play();

        if (currentTile.currentMinion != null)
        {
            if (silence)
                currentTile.currentMinion.GetEffect(EffectType.Silence, turnEffectTime, 0);
            if (stun)
                currentTile.currentMinion.GetEffect(EffectType.Stun, turnEffectTime, 0);
            if (freeze)
                currentTile.currentMinion.GetEffect(EffectType.Freeze, turnEffectTime, 0);
            if (overcharge)
                currentTile.currentMinion.GetEffect(EffectType.Overcharge, turnEffectTime, 0);
            if (burn)
                currentTile.currentMinion.GetEffect(EffectType.Burn, turnEffectTime, burnDamage);
            if (skinFlower)
                currentTile.currentMinion.GetEffect(EffectType.FlowerSkin, turnEffectTime, 0);
            if (weakened)
                currentTile.currentMinion.GetEffect(EffectType.Weakened, turnEffectTime, 0);
            if (invulnerable)
                currentTile.currentMinion.GetEffect(EffectType.Invulnerable, turnEffectTime, 0);

            bool critical = Random.Range(0, 100) >= criticalHitChance;
            int damage = Mathf.FloorToInt(Random.Range(damageRange.x, damageRange.y));

            if (critical)
                damage = Mathf.FloorToInt(damage * 1.5f);

            currentTile.currentMinion.GetDamage(damage, damageType, critical);
        }

        currentTile.DestroyTrap(false);
    }

    public void Destoy()
    {
        view.RPC("DestroyTrap", RpcTarget.All);
    }
    [PunRPC]
    void DestroyTrap() 
    {
        currentTile.haveTrap = false;
        currentTile.currentTrap = null;
        currentTile.UpdateInfo();

        Destroy(gameObject, 2);
    }
}