using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class HealthBar : MonoBehaviour
{
    Vector3 healthBarOriginalPos;
    Vector3 damageInfoBgOriginalPos;

    public Minion currentMinion;


    [FoldoutGroup("Health Bar")]
    public GameObject HealthBarBG;

    [FoldoutGroup("Health Bar")]
    public Slider healthBar;
    [FoldoutGroup("Health Bar")]
    public Slider DamageBar;
    [FoldoutGroup("Health Bar")]
    public TextMeshProUGUI textHealthInfo;
    [FoldoutGroup("Health Bar")]
    public Color allyHealthBarColor;
    [FoldoutGroup("Health Bar")]
    public Color enemyHealthBarColor;

    [FoldoutGroup("Damage Text")]
    public TextMeshProUGUI damageTextInfo;
    [FoldoutGroup("Damage Text")]
    public GameObject criticalText;

    [FoldoutGroup("Damage Infos")]
    public GameObject damageInfoBg;
    [FoldoutGroup("Damage Infos")]
    public Image minionIcon;
    [FoldoutGroup("Damage Infos")]
    public TextMeshProUGUI textMinionLevel;
    [FoldoutGroup("Damage Infos")]
    public TextMeshProUGUI HealthTextInfo;
    [FoldoutGroup("Damage Infos")]
    public Slider damageHealthSlider;
    [FoldoutGroup("Damage Infos")]
    public TextMeshProUGUI damageAmountText;
    [FoldoutGroup("Damage Infos")]
    public Image ColorBanner;
    [FoldoutGroup("Damage Infos")]
    public Color allyColor;
    [FoldoutGroup("Damage Infos")]
    public Color enemyColor;

    private void Awake()
    {
        damageInfoBgOriginalPos = damageInfoBg.transform.localPosition;
        healthBarOriginalPos = transform.localPosition;
        HealthBarBG.SetActive(false);
        damageInfoBg.SetActive(false);
        damageAmountText.gameObject.SetActive(false);

       
    }

    Coroutine healthBarDamageCoroutine;
    public void ChangeHealthBarValue(float maxHealth, float Damage = 0, float heal = 0, bool critical = false)
    {
        if (Damage <= 0 && heal <= 0)
            return;

        if (currentMinion.isDead == true)
            return;

        if (Damage > 0)
        {
            if (healthBar.value - (Damage / maxHealth) >= 0)
                healthBar.value -= Damage / maxHealth;
        }
        
        if (heal > 0)
        {
            if (healthBar.value + (heal / maxHealth) <= 1)
                healthBar.value += heal / maxHealth;
        }
     

        float DamagePercentil = Damage / maxHealth;

        textHealthInfo.text = currentMinion.currentHealth.ToString() + " / " + currentMinion.maxHealth.ToString();

        transform.localPosition = healthBarOriginalPos;
        transform.localScale = Vector3.one;
        transform.DOKill();
        transform.DOShakePosition(0.5f, Remap(DamagePercentil * 2, 0, 1, 0.3f, 1f), 20);
        transform.DOScale(Vector3.one * 1.5f, 0.05f)
            .SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo);

        if (healthBarDamageCoroutine != null)
            StopCoroutine(healthBarProcess());
        healthBarDamageCoroutine = StartCoroutine(healthBarProcess());
        IEnumerator healthBarProcess()
        {
            HealthBarBG.SetActive(true);

            if (critical)
            {
            
                criticalText.transform.DOKill();
                criticalText.transform.localScale = Vector3.zero;


                criticalText.transform.DOScale(1, 0.2f)
                    .SetEase(Ease.OutBack);

                criticalText.transform.DOScale(0, 0.2f)
                    .SetDelay(0.65f)
                    .SetEase(Ease.InBack);
            }
        

            yield return new WaitForSeconds(1);

            if (Damage > 0)
            {
                while (healthBar.value < DamageBar.value)
                {
                    DamageBar.value -= Time.deltaTime * 0.65f;

                    yield return new WaitForEndOfFrame();
                }
            }

            if (heal > 0)
            {
                while (healthBar.value < DamageBar.value)
                {
                    DamageBar.value += Time.deltaTime * 0.65f;

                    yield return new WaitForEndOfFrame();
                }
            }

            DamageBar.value = healthBar.value;

            yield return new WaitForSeconds(1);

            transform.localPosition = healthBarOriginalPos;
            transform.localScale = Vector3.one;
            HealthBarBG.SetActive(false);
        }

    }

    public void ShowHealthBar(bool value)
    {
        if (currentMinion.isDead == true)
            return;


        if (value)
        {
            HealthBarBG.SetActive(true);
            HealthBarBG.transform.DOScale(1, 0.3f)
                .SetEase(Ease.OutBack);

            textHealthInfo.text = currentMinion.currentHealth.ToString() + " / " + currentMinion.maxHealth.ToString();

            if (NetworkManager.Instance.testSolo)
            {
                if (GameManager.Instance.yourTurn)
                {
                    if (currentMinion.myMinion)
                        healthBar.fillRect.gameObject.GetComponent<Image>().color = allyHealthBarColor;
                    else
                        healthBar.fillRect.gameObject.GetComponent<Image>().color = enemyHealthBarColor;
                }
                else
                {
                    if (currentMinion.myMinion)
                        healthBar.fillRect.gameObject.GetComponent<Image>().color = enemyHealthBarColor;
                    else
                        healthBar.fillRect.gameObject.GetComponent<Image>().color = allyHealthBarColor;
                }
            }
            else
            {
                if (currentMinion.view.IsMine)
                    healthBar.fillRect.gameObject.GetComponent<Image>().color = allyHealthBarColor;
                else
                    healthBar.fillRect.gameObject.GetComponent<Image>().color = enemyHealthBarColor;
            }
        }
        else
        {
            Tween t = HealthBarBG.transform.DOScale(0, 0.3f)
             .SetEase(Ease.InBack);

            t.OnComplete(() =>
            {
                HealthBarBG.SetActive(false);
            });
        }   
    }

    public void ShowDamageAmount(int damageAmount)
    {
        
        if(damageAmount <= 0)
            return;

        if (currentMinion.isDead == true)
            return;


        StartCoroutine(showDamageAmount());
        IEnumerator showDamageAmount()
        {
            damageAmountText.gameObject.SetActive(true);
            damageAmountText.transform.localPosition = Vector3.zero;
            damageAmountText.text = "- " + damageAmount.ToString();

            damageAmountText.transform.DOKill();
            damageAmountText.transform.localScale = Vector3.zero;

            damageAmountText.transform.DOScale(1.5f, 0.2f)
                .SetEase(Ease.OutBack);

            damageAmountText.transform.DOLocalMoveY(3, 2)
                .SetRelative(true)
                .SetEase(Ease.OutSine);

            damageAmountText.transform.DOScale(0, 0.4f)
                .SetDelay(1.5f)
                .SetEase(Ease.InBack);

            yield return new WaitForSeconds(2.5f);

            damageAmountText.gameObject.SetActive(false);
        }
 
    }

    public void ShowDamageInfo(Minion minion, Vector2 damageRange, DamageType damageType, Vector3 position)
    {
        if (currentMinion.isDead == true)
            return;

        damageInfoBg.SetActive(true); 
        HealthBarBG.SetActive(false);

        minionIcon.sprite = currentMinion.minionIconSprite;

        damageInfoBg.transform.localScale = Vector3.zero;     
        damageInfoBg.transform.localPosition = damageInfoBgOriginalPos;
        damageInfoBg.transform.DOScale(1, 0.3f)
         .SetEase(Ease.OutBack);
        damageInfoBg.transform.DOMove(position, 0.7f)
            .SetEase(Ease.OutBack);
                   
       
        float floatMinDamage = damageRange.x;
        float floatMaxDamage = damageRange.y;

        switch (damageType)
        {
            case DamageType.Physical:

                floatMinDamage -= ((currentMinion.physicalResist * floatMinDamage) / 100);
                floatMaxDamage -= ((currentMinion.physicalResist * floatMaxDamage) / 100);

                break;
            case DamageType.Magic:

                floatMinDamage -= ((currentMinion.magicalResist * floatMinDamage) / 100);
                floatMaxDamage -= ((currentMinion.magicalResist * floatMaxDamage) / 100);

                break;
            default:
                break;
        }
        
        if (currentMinion.minionIconSprite)
            minionIcon.sprite = currentMinion.minionIconSprite;

        if(minion.superForceTurnLeft > 0 || minion.currentGlyph == Glyph.GlyphType.SuperForce)
        {
            floatMinDamage *= 2;
            floatMaxDamage *= 2;
        }

        float currentHeatFloat = currentMinion.currentHealth;
        float maxHeatFloat = currentMinion.maxHealth;
        damageHealthSlider.value = currentHeatFloat / maxHeatFloat;
        HealthTextInfo.text = currentMinion.currentHealth.ToString() + " / " + currentMinion.maxHealth.ToString();
        textMinionLevel.text = currentMinion.minionLevel.ToString();
        damageTextInfo.text = Mathf.RoundToInt(floatMinDamage).ToString() + " - " + Mathf.RoundToInt(floatMaxDamage).ToString()
            + "  (" + Mathf.RoundToInt((floatMinDamage * 1.5f)).ToString() + " - " + Mathf.RoundToInt((floatMaxDamage * 1.5f)).ToString() + ")";

        if (NetworkManager.Instance.testSolo)
        {
            if (GameManager.Instance.yourTurn)
            {
                if (currentMinion.myMinion)
                {
                    ColorBanner.color = allyColor;
                    damageHealthSlider.fillRect.gameObject.GetComponent<Image>().color = allyHealthBarColor;
                }
                else
                {
                    ColorBanner.color = enemyColor;
                    damageHealthSlider.fillRect.gameObject.GetComponent<Image>().color = enemyHealthBarColor;
                }
                 
            }
            else
            {
                if (currentMinion.myMinion)
                {
                    ColorBanner.color = enemyColor;
                    damageHealthSlider.fillRect.gameObject.GetComponent<Image>().color = enemyHealthBarColor;
                }
                else
                {
                    ColorBanner.color = allyColor;
                    damageHealthSlider.fillRect.gameObject.GetComponent<Image>().color = allyHealthBarColor;
                }
            }
        }
        else
        {
            if(currentMinion.view.IsMine)
            {
                ColorBanner.color = allyColor;
                damageHealthSlider.fillRect.gameObject.GetComponent<Image>().color = allyHealthBarColor;
            }
            else
            {
                ColorBanner.color = enemyColor;
                damageHealthSlider.fillRect.gameObject.GetComponent<Image>().color = enemyHealthBarColor;
            }


        }
    }

    public void HideDamageInfo()
    {
        damageInfoBg.transform.localPosition = damageInfoBgOriginalPos;
        damageInfoBg.SetActive(false);
    }

    
    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


 
}
