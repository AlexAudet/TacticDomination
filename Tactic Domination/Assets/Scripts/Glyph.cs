using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Photon.Pun;

public class Glyph : MonoBehaviour
{
    public enum GlyphType {None, Damage, Silence, SuperForce, Freeze, Overcharge, SkinFlower, Weakened, Invulnerable }
    public enum GlyphTarget { Enemy, Ally, Both}

    [FoldoutGroup("GlyphType Material")]
    public Material DamageMat;
    [FoldoutGroup("GlyphType Material")]
    public Material SilenceMat;
    [FoldoutGroup("GlyphType Material")]
    public Material SuperForceMat;
    [FoldoutGroup("GlyphType Material")]
    public Material FreezeMat;
    [FoldoutGroup("GlyphType Material")]
    public Material OverchargeMat;
    [FoldoutGroup("GlyphType Material")]
    public Material SkinFlowerMat;
    [FoldoutGroup("GlyphType Material")]
    public Material WeakenedMat;
    [FoldoutGroup("GlyphType Material")]
    public Material InvulnerableMat;

    [HideInInspector] public PhotonView view;
    [HideInInspector] public Tile currentTile;
    public int turnLeft;
    public bool myGlyph;
    public GameObject glyphObject;
    public Material allyMaterial;
    public Material enemyMaterial;
    public ParticleSystem activationVfx;

    public Vector2 damageRange;
    public GlyphType type;

    public void InitiateGlyph(bool myMinion, Vector2 tileCoords, int activeTime, int glyphTarget, Vector2 dr, int glyphType)
    {
        currentTile = GetComponent<Tile>();

        if (currentTile.haveGlyph)
            currentTile.CloseGlyph(false);

        myGlyph = myMinion;
        turnLeft = activeTime;
        damageRange = dr;
        type = (GlyphType)glyphType;
        currentTile.haveGlyph = true;

        glyphObject.SetActive(true);



        switch (type)
        {
            case GlyphType.Damage:
                glyphObject.GetComponent<MeshRenderer>().sharedMaterial = DamageMat;
                break;
            case GlyphType.Silence:
                glyphObject.GetComponent<MeshRenderer>().sharedMaterial = SilenceMat;
                break;
            case GlyphType.SuperForce:
                glyphObject.GetComponent<MeshRenderer>().sharedMaterial = SuperForceMat;
                break;
            case GlyphType.Freeze:
                glyphObject.GetComponent<MeshRenderer>().sharedMaterial = FreezeMat;
                break;
            case GlyphType.Overcharge:
                glyphObject.GetComponent<MeshRenderer>().sharedMaterial = OverchargeMat;
                break;
            case GlyphType.SkinFlower:
                glyphObject.GetComponent<MeshRenderer>().sharedMaterial = SkinFlowerMat;
                break;
            case GlyphType.Weakened:
                glyphObject.GetComponent<MeshRenderer>().sharedMaterial = WeakenedMat;
                break;
            case GlyphType.Invulnerable:
                glyphObject.GetComponent<MeshRenderer>().sharedMaterial = InvulnerableMat;
                break;
            default:
                break;
        }

        if (currentTile.currentMinion != null)
            ApplyEffect();
    }


    public void NewTurn()
    {
        turnLeft--;

        if (turnLeft == 0)
            currentTile.CloseGlyph(true);
    }

    public void ApplyEffect()
    {
        if (activationVfx != null)
            Instantiate(activationVfx, transform.position, activationVfx.transform.rotation);

        switch (type)
        {
            case GlyphType.Damage:
                int damage = Mathf.FloorToInt(Random.Range(damageRange.x, damageRange.y));
                currentTile.currentMinion.GetDamage(damage, DamageType.Magic);
                break;
            case GlyphType.Silence:
                currentTile.currentMinion.GetEffectByGlyph(GlyphType.Silence, true);
                break;
            case GlyphType.SuperForce:
                currentTile.currentMinion.GetEffectByGlyph(GlyphType.SuperForce, true);
                break;
            case GlyphType.Freeze:
                currentTile.currentMinion.GetEffectByGlyph(GlyphType.Freeze, true);
                break;
            case GlyphType.Overcharge:
                currentTile.currentMinion.GetEffectByGlyph(GlyphType.Overcharge, true);
                break;
            case GlyphType.SkinFlower:
                currentTile.currentMinion.GetEffectByGlyph(GlyphType.SkinFlower, true);
                break;
            case GlyphType.Weakened:
                currentTile.currentMinion.GetEffectByGlyph(GlyphType.Weakened, true);
                break;
            case GlyphType.Invulnerable:
                currentTile.currentMinion.GetEffectByGlyph(GlyphType.Invulnerable, true);
                break;
            default:
                break;
        }

    }

}
