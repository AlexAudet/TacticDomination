using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Photon.Pun;

public class Projectil : MonoBehaviour
{
    PhotonView view;
    MinionAttack currentAttack;
    Minion currentMinion;

    public ParticleSystem projectil;
    public ParticleSystem impact;
    Tile target;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    public void SetAttack(MinionAttack Attack, Minion minion, Tile targetTile)
    {
        target = targetTile;
        currentMinion = minion;
        currentAttack = Attack;
    }

    public void MoveProjectil(Vector3 startPos, Vector3 endPos)
    {
        float currentDistance = Vector2.Distance(endPos, startPos);
        float height = Remap(currentDistance, 1, currentAttack.attackRange.y, currentAttack.projectilHeight / 2, currentAttack.projectilHeight);

        view.RPC("MoveProjectilForAll", RpcTarget.All, startPos, endPos, currentAttack.projectilSpeed, height,
            currentAttack.projectilType == ProjectilType.Linear ? false : true);
    }
    [PunRPC] 
    void MoveProjectilForAll(Vector3 startPos, Vector3 endPos, float speed, float height, bool semiCircle)
    {
        projectil.Play();
        Tween p = null;
        if (!semiCircle)
        {
            p = transform.DOMove(endPos, speed)
                    .SetSpeedBased(true).SetEase(Ease.Linear);
        }
        else
        {
            p = transform.DOMove(endPos, speed)
                .SetSpeedBased(true).SetEase(Ease.Linear);

            float midTime = (Vector3.Distance(endPos, startPos) / 2) / speed;

            Tween ph = projectil.transform.DOMoveY(height, midTime)
              .SetRelative(true).SetEase(Ease.OutSine);

            ph.OnComplete(() =>
            {
                projectil.transform.DOMoveY(-height, midTime)
                    .SetRelative(true).SetEase(Ease.InSine);
            });
        }

        p.OnComplete(() =>
        {
            projectil.Stop();
            impact.Play();

            if (view.IsMine)
            {
                currentAttack.ExecuteAttack(target);
                currentAttack.ResetAttack();
            }
             
        });
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}