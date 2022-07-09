using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    UIManager()
    {
        Instance = this;
    }



    [FoldoutGroup("Effect Icons")]
    public Sprite stundIcon;
    [FoldoutGroup("Effect Icons")]
    public Sprite snearIcon;
    [FoldoutGroup("Effect Icons")]
    public Sprite superForceIcon;
    [FoldoutGroup("Effect Icons")]
    public Sprite freezeIcon;
    [FoldoutGroup("Effect Icons")]
    public Sprite overchargeIcon;
    [FoldoutGroup("Effect Icons")]
    public Sprite burnIcon;
    [FoldoutGroup("Effect Icons")]
    public Sprite floweSkinIcon;
    [FoldoutGroup("Effect Icons")]
    public Sprite weakenedIcon;
    [FoldoutGroup("Effect Icons")]
    public Sprite InvulnerableIcon;


    [FoldoutGroup("Into")]
    [PropertySpace(0,10)]
    public Image introBg;
    [FoldoutGroup("Into")]
    public GameObject introMyNameBG;
    [FoldoutGroup("Into")]
    public GameObject introMyMinionLayout;
    [FoldoutGroup("Into")]
    public Image image_myFirstMinion;
    [FoldoutGroup("Into")]
    public Image image_mySecondMinion;
    [FoldoutGroup("Into")]
    public Image image_myThirdMinion;
    [FoldoutGroup("Into")]
    [PropertySpace(0, 10)]
    public TextMeshProUGUI textIntroMyName;

    [FoldoutGroup("Into")]
    public GameObject introEnemyNameBG;
    [FoldoutGroup("Into")]
    public GameObject introEnemyMinionLayout;
    [FoldoutGroup("Into")]
    public Image image_enemyFirstMinion;
    [FoldoutGroup("Into")]
    public Image image_enemySecondMinion;
    [FoldoutGroup("Into")]
    public Image image_enemyThirdMinion;
    [FoldoutGroup("Into")]
    public TextMeshProUGUI textIntroEnemyName;


    [FoldoutGroup("Into")]
    [PropertySpace(10, 00)]
    public GameObject yourTurnBanner;
    [FoldoutGroup("Into")]
    public GameObject rivalTurnBanner;
    [FoldoutGroup("Into")]
    public Image blackBg;

    [FoldoutGroup("BG")]
    public Image bottomBg;
    [FoldoutGroup("BG")]
    public Image secondBottomBg;

    [FoldoutGroup("Action Points")]
    public TextMeshProUGUI actionPointText;
    [FoldoutGroup("Action Points")]
    public GameObject actionPointBarBg;
    [FoldoutGroup("Action Points")]
    public Image actionPointImage;
    List<Image> actionPoints = new List<Image>();
    [FoldoutGroup("Action Points")]
    public Color activatedColor;
    [FoldoutGroup("Action Points")]
    public Color deactivatedColor;
    [FoldoutGroup("Action Points")]
    public GameObject notEnoughtAPPanel;


    [FoldoutGroup("Turn Infos")]
    public Slider timeSlider;
    [FoldoutGroup("Turn Infos")]
    public Button PassTurnButton;
    [FoldoutGroup("Turn Infos")]
    public GameObject rivalTurnText;


    [FoldoutGroup("Minion Infos")]
    public Button openMinionInfoButton;
    [FoldoutGroup("Minion Infos")]
    public GameObject ActionPanel;
    [FoldoutGroup("Minion Infos")]
    public GameObject EnemyStatPanel;
    [FoldoutGroup("Minion Infos")]
    public Color allyColor;
    [FoldoutGroup("Minion Infos")]
    public Color enemyColor;
    [FoldoutGroup("Minion Infos")]
    public Button ShowMinionHealthButton;
    [FoldoutGroup("Minion Infos")]
    public Image ImageMinionIcon;
    [FoldoutGroup("Minion Infos")]
    public TextMeshProUGUI textMinionName;
    [FoldoutGroup("Minion Infos")]
    public TextMeshProUGUI textMinionLevel;
    [FoldoutGroup("Minion Infos")]
    public Slider sliderHealthBar;
    [FoldoutGroup("Minion Infos")]
    public TextMeshProUGUI textHealthAmount;
    [FoldoutGroup("Minion Infos")]
    public List<GameObject> minonEffectSlot;


    #region Minion Info Template
    [FoldoutGroup("More Minion Info")]
    [PropertySpace(10, 0)]
    public GameObject minionInfoTemplate;
    [FoldoutGroup("More Minion Info")]
    public Image minionInfoBg;
    [FoldoutGroup("More Minion Info")]
    public Image minionInfoSecondBg;
    [FoldoutGroup("More Minion Info")]
    public Button minionInfoCloseBG;
    [FoldoutGroup("More Minion Info")]
    public Image image_StatInfo_Icon;
    [FoldoutGroup("More Minion Info")]
    public TextMeshProUGUI text_StatInfo_Level;
    [FoldoutGroup("More Minion Info")]
    public TextMeshProUGUI text_StatInfo_Name;
    [FoldoutGroup("More Minion Info")]
    public Slider slider_StatInfo_HealthBar;
    [FoldoutGroup("More Minion Info")]
    public TextMeshProUGUI text_StatInfo_HealthAmount;
    [FoldoutGroup("Minion Infos")]
    public List<GameObject> statInfo_minonEffectSlot;

    [PropertySpace(10, 0)]
    [FoldoutGroup("More Minion Info")]
    public TextMeshProUGUI text_StatInfo_MovementAmount;
    [FoldoutGroup("More Minion Info")]
    public TextMeshProUGUI text_StatInfo_criticalChance;
    [FoldoutGroup("More Minion Info")]
    public TextMeshProUGUI text_StatInfo_leak;
    [FoldoutGroup("More Minion Info")]
    public TextMeshProUGUI text_StatInfo_tackle;
    [FoldoutGroup("More Minion Info")]
    public TextMeshProUGUI text_StatInfo_physicalResist;
    [FoldoutGroup("More Minion Info")]
    public TextMeshProUGUI text_StatInfo_magicResist;

    [PropertySpace(10, 0)]
    [FoldoutGroup("More Minion Info")]
    public Image image_StatInfo_firstAttack;
    [FoldoutGroup("More Minion Info")]
    public Image image_StatInfo_secondAttack;
    [FoldoutGroup("More Minion Info")]
    public Image image_StatInfo_specialAttack;

    #endregion



    #region Fast Minion Action
    [FoldoutGroup("Minion Action")]
    [PropertySpace(10, 0)]
    public Image minionIcon;
    [FoldoutGroup("Minion Action")]
    public TextMeshProUGUI minionLevel;

    [FoldoutGroup("Minion Action")]
    [PropertySpace(10, 0)]
    public Button movementButton;
    [FoldoutGroup("Minion Action")]
    public Image movementHightLight;
    [FoldoutGroup("Minion Action")]
    public TextMeshProUGUI MovementAmountText;
    [FoldoutGroup("Minion Action")]
    public TextMeshProUGUI movementCost;

    [FoldoutGroup("Minion Action")]
    [PropertySpace(10, 0)]
    public Button cancelAttackButton;

    [FoldoutGroup("Minion Action")]
    [PropertySpace(10, 0)]
    public Button firstAttackButton;
    [FoldoutGroup("Minion Action")]
    public Image firstAttackHightLight;
    [FoldoutGroup("Minion Action")]
    public Image firstAttackIcon;
    [FoldoutGroup("Minion Action")]
    public GameObject firstAttackCooldownBg;
    [FoldoutGroup("Minion Action")]
    public TextMeshProUGUI firstAttackCooldDownText;
    [FoldoutGroup("Minion Action")]
    public TextMeshProUGUI firstAttackCost;

    [FoldoutGroup("Minion Action")]
    [PropertySpace(10, 0)]
    public Button secondAttackButton;
    [FoldoutGroup("Minion Action")]
    public Image secondAttackHightLight;
    [FoldoutGroup("Minion Action")]
    public Image secondAttackIcon;
    [FoldoutGroup("Minion Action")]
    public GameObject secondAttackCooldownBG;
    [FoldoutGroup("Minion Action")]
    public TextMeshProUGUI secondAttackCooldDownText;
    [FoldoutGroup("Minion Action")]
    public TextMeshProUGUI secondAttackCost;

    [FoldoutGroup("Minion Action")]
    [PropertySpace(10, 0)]
    public Button specialAttackButton;
    [FoldoutGroup("Minion Action")]
    public Image specialAttackHightLight;
    [FoldoutGroup("Minion Action")]
    public Image specialAttackIcon;
    [FoldoutGroup("Minion Action")]
    public GameObject specialAttackCooldownBG;
    [FoldoutGroup("Minion Action")]
    public TextMeshProUGUI specialAttackCooldDownText;
    [FoldoutGroup("Minion Action")]
    public TextMeshProUGUI specialAttackCost;
    #endregion

    #region Card
    [PropertySpace(10, 0)]
    [FoldoutGroup("Card")]
    public GameObject CardTemplate;
    [FoldoutGroup("Card")]
    public Button OpenCloseCardButton;

    [PropertySpace(10, 0)]
    [FoldoutGroup("Card")]
    public Button CancelCardButton;
    [FoldoutGroup("Card")]
    public Button ConfirmCardButton;
    [FoldoutGroup("Card")]
    public Button BgCardButton;
    [FoldoutGroup("Card")]
    public Image CardIconImage;
    [FoldoutGroup("Card")]
    public GameObject ActionPointForCardBg;
    [FoldoutGroup("Card")]
    public TextMeshProUGUI textActionPointForCard;

    [PropertySpace(10, 0)]
    [FoldoutGroup("Card")]
    public Button firstCardHolder;
    [FoldoutGroup("Card")]
    public Image firstCardIcon;
    [FoldoutGroup("Card")]
    public TextMeshProUGUI firstCardDescription, firstCardCost;

    [PropertySpace(10, 0)]
    [FoldoutGroup("Card")]
    public Button secondCardHolder;
    [FoldoutGroup("Card")]
    public Image secondCardIcon;
    [FoldoutGroup("Card")]
    public TextMeshProUGUI secondCardDescription, secondCardCost;

    [PropertySpace(10, 0)]
    [FoldoutGroup("Card")]
    public Button thirdCardHolder;
    [FoldoutGroup("Card")]
    public Image thirdCardIcon;
    [FoldoutGroup("Card")]
    public TextMeshProUGUI thirdCardDescription, thirdCardCost;
    #endregion

    public static bool OpenUI;
    public static bool minonActionOpen, minionInfoOpen, cardPanelOpen;

    Vector3 MinionDeckHidePos;

    private void Start()
    {
        InitiatePanels();

        movementButton.onClick.AddListener(CurrentMinionMovement);
        firstAttackButton.onClick.AddListener(CurrentMinionFirstAttack);
        secondAttackButton.onClick.AddListener(CurrentMinionSecondAttack);
        specialAttackButton.onClick.AddListener(CurrentMinionSpecialAttack);

        PassTurnButton.onClick.AddListener(NextTurn);
        minionInfoCloseBG.onClick.AddListener(CloseMinionsInfo);
        openMinionInfoButton.onClick.AddListener(OpenMinionInfo);

        cancelAttackButton.onClick.AddListener(CancelAttack);
        // ShowMinionHealthButton.onClick.AddListener(ShowMinionHealth);

    }


    public TextMeshProUGUI fpsText;
    float deltaTime;
    void Update()
    {
        // MoveMap();

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString();
    }


    public void InitiatePanels()
    {
        MinionDeckHidePos = minionInfoTemplate.transform.position;
    }

    public void PlayIntro()
    {
        introBg.DOFade(0.8f, 0.3f);

        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");

        if (!NetworkManager.Instance.testSolo)
        {
            image_myFirstMinion.sprite = allMinions[0].GetComponent<Minion>().minionIconSprite;
            image_mySecondMinion.sprite = allMinions[1].GetComponent<Minion>().minionIconSprite;
            image_myThirdMinion.sprite = allMinions[2].GetComponent<Minion>().minionIconSprite;
            image_enemyFirstMinion.sprite = allMinions[3].GetComponent<Minion>().minionIconSprite;
            image_enemySecondMinion.sprite = allMinions[4].GetComponent<Minion>().minionIconSprite;
            image_enemyThirdMinion.sprite = allMinions[5].GetComponent<Minion>().minionIconSprite;
        }
        else
        {
            image_myFirstMinion.sprite = allMinions[0].GetComponent<Minion>().minionIconSprite;
            image_enemyFirstMinion.sprite = allMinions[1].GetComponent<Minion>().minionIconSprite;
            image_mySecondMinion.sprite = allMinions[2].GetComponent<Minion>().minionIconSprite;
            image_enemySecondMinion.sprite = allMinions[3].GetComponent<Minion>().minionIconSprite;
            image_myThirdMinion.sprite = allMinions[4].GetComponent<Minion>().minionIconSprite;          
            image_enemyThirdMinion.sprite = allMinions[5].GetComponent<Minion>().minionIconSprite;
        }


        introMyNameBG.transform.DOLocalMoveX(0, 0.5f)
              .SetDelay(0.3f)
              .SetEase(Ease.OutBack);

        introEnemyNameBG.transform.DOLocalMoveX(0, 0.5f)
             .SetDelay(0.3f)
             .SetEase(Ease.OutBack);

        introMyMinionLayout.transform.DOScale(1, 0.2f)
             .SetDelay(1)
             .SetEase(Ease.OutBack);

        introEnemyMinionLayout.transform.DOScale(1, 0.2f)
             .SetDelay(1)
             .SetEase(Ease.OutBack);

        introMyNameBG.transform.DOLocalMoveY(-1300, 0.5f)
             .SetDelay(3f)
             .SetEase(Ease.InBack);

        introEnemyNameBG.transform.DOLocalMoveY(1300, 0.5f)
            .SetDelay(3f)
            .SetEase(Ease.InBack);

      // bottomBg.transform.DOMoveY(0, 0.3f)
      //     .SetDelay(4f)
      //     .SetEase(Ease.OutBack);

        Tween t = introBg.DOFade(0, 0.3f)
          .SetDelay(4f);

        t.OnComplete(() =>
        {
            introBg.gameObject.SetActive(false);

            GameManager.Instance.StartGame();
        });

    } 
    public void playYourTurn()
    {
        secondBottomBg.color = allyColor;
        actionPointBarBg.SetActive(true);
        rivalTurnText.SetActive(false);

        blackBg.gameObject.SetActive(true);
        blackBg.DOFade(0.6f, 0.2f);
        blackBg.DOFade(0, 0.3f)
              .SetDelay(1f);

        yourTurnBanner.SetActive(true);
        yourTurnBanner.transform.localScale = Vector3.zero;
        yourTurnBanner.transform.localPosition = Vector3.zero;

        yourTurnBanner.transform.DOScale(1, 0.3f)
            .SetEase(Ease.OutBack);

        Tween t = yourTurnBanner.transform.DOLocalMoveY(-1100, 0.4f)
            .SetDelay(1)
            .SetEase(Ease.InBack);

        t.OnComplete(() =>
        {
            yourTurnBanner.SetActive(false);
        });
    }
    public void playRivalTurn()
    {
        secondBottomBg.color = enemyColor;
        actionPointBarBg.SetActive(false);
        rivalTurnText.SetActive(true);

        blackBg.DOFade(0.6f, 0.2f);
        blackBg.DOFade(0, 0.3f)
                 .SetDelay(1f);

        rivalTurnBanner.SetActive(true);
        rivalTurnBanner.transform.localScale = Vector3.zero;
        rivalTurnBanner.transform.localPosition = Vector3.zero;

        rivalTurnBanner.transform.DOScale(1, 0.3f)
            .SetEase(Ease.OutBack);

        Tween t = rivalTurnBanner.transform.DOLocalMoveY(-1100, 0.4f)
            .SetDelay(1)
            .SetEase(Ease.InBack);

        t.OnComplete(() =>
        {
            rivalTurnBanner.SetActive(false);
            blackBg.gameObject.SetActive(false);
        });

    }


    public void CurrentMinionMovement()
    {
        Minion currentMinion = GameManager.Instance.GetCurrentMinion();

        if (minionInfoOpen)
            CloseMinionsInfo();

        if (currentMinion != null)
        {
            //Si le minion etait en train de regarder pour attaquer, annule
            if (GameManager.Instance.actionPointLeft >= 1 * (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1))
            {
               // currentMinion.ResetAttacks();

                cancelAttackButton.gameObject.SetActive(false);
                movementHightLight.gameObject.SetActive(true);
                firstAttackHightLight.gameObject.SetActive(false);
                secondAttackHightLight.gameObject.SetActive(false);
                specialAttackHightLight.gameObject.SetActive(false);
            }
            else
            {
                ShowNotEnoughtActionPointPanel();
                cancelAttackButton.gameObject.SetActive(false);
                TurnOffAllActionHightLight();
            }

            currentMinion.FindPossiblePath();
            //UIManager.Instance.CurrentMinionMovement();
        }

    }
    public void CurrentMinionFirstAttack()
    {
        if (minionInfoOpen)
            CloseMinionsInfo();

        Minion currentMinion = GameManager.Instance.GetCurrentMinion();

        if (currentMinion != null)
        {
            if (GameManager.Instance.actionPointLeft >= currentMinion.firstAttack.actionPointCost * (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1))
            {
                currentMinion.FirstAttack();
                cancelAttackButton.gameObject.SetActive(true);
                movementHightLight.gameObject.SetActive(false);
                firstAttackHightLight.gameObject.SetActive(true);
                secondAttackHightLight.gameObject.SetActive(false);
                specialAttackHightLight.gameObject.SetActive(false);
            }
            else
            {
                ShowNotEnoughtActionPointPanel();
                TurnOffAllActionHightLight();
            }
        }
    }
    public void CurrentMinionSecondAttack()
    {
        if (minionInfoOpen)
            CloseMinionsInfo();

        Minion currentMinion = GameManager.Instance.GetCurrentMinion();

        if (currentMinion != null)
        {
            if (GameManager.Instance.actionPointLeft >= currentMinion.secondAttack.actionPointCost * (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1))
            {
                currentMinion.SecondAttack();
                cancelAttackButton.gameObject.SetActive(true);
                movementHightLight.gameObject.SetActive(false);
                firstAttackHightLight.gameObject.SetActive(false);
                secondAttackHightLight.gameObject.SetActive(true);
                specialAttackHightLight.gameObject.SetActive(false);;
            }
            else
            {
                ShowNotEnoughtActionPointPanel();
                TurnOffAllActionHightLight();
            }
        }
    }
    public void CurrentMinionSpecialAttack()
    {
        if (minionInfoOpen)
            CloseMinionsInfo();

        Minion currentMinion = GameManager.Instance.GetCurrentMinion();

        if (currentMinion != null)
        {
            if (GameManager.Instance.actionPointLeft >= currentMinion.specialAttack.actionPointCost * (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1))
            {
                currentMinion.SpecialAttack();
                cancelAttackButton.gameObject.SetActive(true);
                movementHightLight.gameObject.SetActive(false);
                firstAttackHightLight.gameObject.SetActive(false);
                secondAttackHightLight.gameObject.SetActive(false);
                specialAttackHightLight.gameObject.SetActive(true);
            }
            else
            {
                ShowNotEnoughtActionPointPanel();
                TurnOffAllActionHightLight();
            }
        }
    }

    public void CancelAttack()
    {
        SoundManager.Instance.PlayUISound("CancelAttack");
        GameManager.Instance.GetCurrentMinion().ResetAttacks();
        TurnOffAllActionHightLight();
        CurrentMinionMovement();
        ActiveAllMinionHealth(false);
    }

    public void TurnOffAllActionHightLight()
    {
        cancelAttackButton.gameObject.SetActive(false);
        movementHightLight.gameObject.SetActive(false);
        firstAttackHightLight.gameObject.SetActive(false);
        secondAttackHightLight.gameObject.SetActive(false);
        specialAttackHightLight.gameObject.SetActive(false);
    }

    public void ActiveAllMinionHealth(bool value)
    {
        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
        for (int i = 0; i < allMinions.Length; i++)
        {
            Minion minion = allMinions[i].GetComponent<Minion>();

            minion.healthBar.ShowHealthBar(value);
        }
    }


    public void ShowNotEnoughtActionPointPanel()
    {

        StopCoroutine(tempShow());
        StartCoroutine(tempShow());
        IEnumerator tempShow()
        {
            SoundManager.Instance.PlayUISound("NotEnoughtAP");
            notEnoughtAPPanel.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            notEnoughtAPPanel.SetActive(false);
        }
    }

    public void ShowNotYourTurnPanel()
    {

       // StopCoroutine(tempShow());
       // StartCoroutine(tempShow());
       // IEnumerator tempShow()
       // {
       //     notEnoughtAPPanel.SetActive(true);
       //     yield return new WaitForSeconds(1.5f);
       //     notEnoughtAPPanel.SetActive(false);
       // }
    }





    void OpenMinionInfo()
    {
        if (minionInfoOpen)
        {
            CloseMinionsInfo();
            return;
        }
         
        OpenMinionsInfo(GameManager.Instance.GetCurrentMinion());
    }
    public void OpenMinionsInfo(Minion minion = null)
    {
        if (minionInfoOpen == true)
            return;

        minionInfoOpen = true;
        UpdateMinionsInfo(minion != null ? minion : GameManager.Instance.GetCurrentMinion());
        if (NetworkManager.Instance.testSolo)
        {
            if (GameManager.Instance.yourTurn)
            {
                if (minion.myMinion)
                {
                    minionInfoBg.color = allyColor;
                    minionInfoSecondBg.color = allyColor;
                }
                else
                {
                    minionInfoBg.color = enemyColor;
                    minionInfoSecondBg.color = enemyColor;
                }
            }
            else
            {
                if (minion.myMinion)
                {
                    minionInfoBg.color = enemyColor;
                    minionInfoSecondBg.color = enemyColor;
                }
                else
                {
                    minionInfoBg.color = allyColor;
                    minionInfoSecondBg.color = allyColor;
                }
            }
        }
        else
        {
            if (minion.myMinion)
            {
                minionInfoBg.color = allyColor;
                minionInfoSecondBg.color = allyColor;
            }
            else
            {
                minionInfoBg.color = enemyColor;
                minionInfoSecondBg.color = enemyColor;
            }
        }


        Tween t = minionInfoTemplate.transform.DOScale(1, 0.3f)
          .SetEase(Ease.OutBack);

        t.OnComplete(() =>
        {
            minionInfoCloseBG.gameObject.SetActive(true);
            minionInfoCloseBG.GetComponent<Image>().DOFade(0.95f, 0.3f);
        });
    }
    public void CloseMinionsInfo()
    {

        if (minionInfoOpen == false)
            return;

        minionInfoCloseBG.gameObject.SetActive(false);
        minionInfoCloseBG.GetComponent<Image>().DOFade(0, 0.3f);

        Tween t = minionInfoTemplate.transform.DOScale(0, 0.3f)
            .SetEase(Ease.InBack);
        t.OnComplete(() =>
        {
            minionInfoOpen = false;
        });
    }

    public void UpdateMinionsInfo(Minion minion)
    {

        image_StatInfo_Icon.sprite = minion.minionIconSprite;
        text_StatInfo_Level.text = minion.minionLevel.ToString();
        text_StatInfo_Name.text = minion.MinionName;

        float currentHealthFloat = minion.currentHealth;
        float maxHealthFloat = minion.maxHealth;

        slider_StatInfo_HealthBar.value = currentHealthFloat / maxHealthFloat;
        text_StatInfo_HealthAmount.text = minion.currentHealth.ToString() + " / " + minion.maxHealth.ToString();

        text_StatInfo_MovementAmount.text = minion.movementAmountLeft.ToString();
        text_StatInfo_criticalChance.text = (minion.criticalHitChance).ToString();
        text_StatInfo_leak.text = minion.leak.ToString();
        text_StatInfo_tackle.text = minion.tackle.ToString();

        text_StatInfo_physicalResist.text = minion.physicalResist.ToString();
        text_StatInfo_magicResist.text = minion.magicalResist.ToString();

        image_StatInfo_firstAttack.sprite = minion.firstAttack.attackIcon;
        image_StatInfo_secondAttack.sprite = minion.secondAttack.attackIcon;
        image_StatInfo_specialAttack.sprite = minion.specialAttack.attackIcon;
    }


    public void UpdateFastMinionInfo(Minion minion)
    {
        if(minion == null)
        {
           // openMinionInfoButton.gameObject.SetActive(false);
            return;
        }

        if (NetworkManager.Instance.testSolo)
        {
            if (GameManager.Instance.yourTurn)
            {
                if (minion.myMinion)
                {
                    bottomBg.color = allyColor;
                    openMinionInfoButton.GetComponent<Image>().color = allyColor;
                    sliderHealthBar.fillRect.GetComponent<Image>().color = minion.healthBar.allyHealthBarColor;
                    ActionPanel.SetActive(true);
                    EnemyStatPanel.SetActive(false);
                }
                else
                {
                    bottomBg.color = enemyColor;
                    openMinionInfoButton.GetComponent<Image>().color = enemyColor;
                    sliderHealthBar.fillRect.GetComponent<Image>().color = minion.healthBar.enemyHealthBarColor;
                    ActionPanel.SetActive(true);
                    ActionPanel.SetActive(false);
                    EnemyStatPanel.SetActive(true);
                }
            }
            else
            {
                if (minion.myMinion)
                {
                    bottomBg.color = enemyColor;
                    openMinionInfoButton.GetComponent<Image>().color = enemyColor;
                    sliderHealthBar.fillRect.GetComponent<Image>().color = minion.healthBar.enemyHealthBarColor;
                    ActionPanel.SetActive(true);
                    ActionPanel.SetActive(false);
                    EnemyStatPanel.SetActive(true);
                }
                else
                {
                    bottomBg.color = allyColor;
                    openMinionInfoButton.GetComponent<Image>().color = allyColor;
                    sliderHealthBar.fillRect.GetComponent<Image>().color = minion.healthBar.allyHealthBarColor;
                    ActionPanel.SetActive(true);
                    ActionPanel.SetActive(true);
                    EnemyStatPanel.SetActive(false);
                }
            }
        }
        else
        {
            if (minion.myMinion)
            {
                bottomBg.color = allyColor;
                openMinionInfoButton.GetComponent<Image>().color = allyColor;
                ActionPanel.SetActive(true);
                EnemyStatPanel.SetActive(false);
            }
            else
            {
                bottomBg.color = enemyColor;
                openMinionInfoButton.GetComponent<Image>().color = enemyColor;
                ActionPanel.SetActive(false);
                EnemyStatPanel.SetActive(true);
            }
        }

       
       // openMinionInfoButton.gameObject.SetActive(true);

        ImageMinionIcon.sprite = minion.minionIconSprite;
        textMinionLevel.text = minion.minionLevel.ToString();
        textMinionName.text = minion.MinionName;

        float currentHealthFloat = minion.currentHealth;
        float maxHealthFloat = minion.maxHealth;

        sliderHealthBar.value = currentHealthFloat / maxHealthFloat;
        textHealthAmount.text = minion.currentHealth.ToString() + " / " + minion.maxHealth.ToString();
        
        for (int i = 0; i < 5; i++)
        {
            minonEffectSlot[i].transform.GetChild(0).gameObject.SetActive(false);
            minonEffectSlot[i].transform.GetChild(1).gameObject.SetActive(false);
        }

        List<int> effects = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            if(minion.stundTurnLeft > 0 && !effects.Contains(1))
            {
                minonEffectSlot[i].transform.GetChild(0).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(0).GetComponent<Image>().sprite = stundIcon;
                minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = minion.stundTurnLeft.ToString();
                effects.Add(1);
                continue;
            }
            else if ((minion.silenceTurnLeft > 0 || minion.currentGlyph == Glyph.GlyphType.Silence) && !effects.Contains(2))
            {
                minonEffectSlot[i].transform.GetChild(0).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(0).GetComponent<Image>().sprite = superForceIcon;
                if(minion.currentGlyph != Glyph.GlyphType.Silence)
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = minion.silenceTurnLeft.ToString();
                else
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "?";
                effects.Add(2);
                continue;
            }
            else if ((minion.superForceTurnLeft > 0 || minion.currentGlyph == Glyph.GlyphType.SuperForce) && !effects.Contains(3))
            {
                minonEffectSlot[i].transform.GetChild(0).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(0).GetComponent<Image>().sprite = snearIcon;
                if (minion.currentGlyph != Glyph.GlyphType.SuperForce)
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = minion.superForceTurnLeft.ToString();
                else
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "?";
                effects.Add(3);
                continue;
            }
            else if((minion.freezeTurnLeft > 0 || minion.currentGlyph == Glyph.GlyphType.Freeze) && !effects.Contains(4))
            {
                minonEffectSlot[i].transform.GetChild(0).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(0).GetComponent<Image>().sprite = freezeIcon;
                if(minion.currentGlyph != Glyph.GlyphType.Freeze)
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = minion.freezeTurnLeft.ToString();
                else
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "?";
                effects.Add(4);
                continue;
            }
            else if ((minion.overchargeTurnLeft > 0 || minion.currentGlyph == Glyph.GlyphType.Overcharge) && !effects.Contains(5))
            {
                minonEffectSlot[i].transform.GetChild(0).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(0).GetComponent<Image>().sprite = overchargeIcon;
                if(minion.currentGlyph != Glyph.GlyphType.Overcharge)
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = minion.overchargeTurnLeft.ToString();
                else
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "?";
                effects.Add(5);
                continue;
            }
            else if (minion.burnTurnLeft > 0 && !effects.Contains(6))
            {
                minonEffectSlot[i].transform.GetChild(0).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(0).GetComponent<Image>().sprite = burnIcon;
                minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = minion.burnTurnLeft.ToString();
                effects.Add(6);
                continue;
            }
            else if ((minion.flowerSkinTurnLeft > 0 || minion.currentGlyph == Glyph.GlyphType.SkinFlower) && !effects.Contains(7))
            {
                minonEffectSlot[i].transform.GetChild(0).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(0).GetComponent<Image>().sprite = floweSkinIcon;
                if(minion.currentGlyph != Glyph.GlyphType.SkinFlower)
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = minion.flowerSkinTurnLeft.ToString();
                else
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "?";
                effects.Add(7);
                continue;
            }
            else if ((minion.weakenedTurnLeft > 0 || minion.currentGlyph == Glyph.GlyphType.Weakened) && !effects.Contains(8))
            {
                minonEffectSlot[i].transform.GetChild(0).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(0).GetComponent<Image>().sprite = weakenedIcon;
                if(minion.currentGlyph != Glyph.GlyphType.Weakened)
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = minion.weakenedTurnLeft.ToString();
                else
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "?";
                effects.Add(8);
                continue;
            }
            else if ((minion.invulnerableTurnLeft > 0 || minion.currentGlyph == Glyph.GlyphType.Invulnerable) && !effects.Contains(9))
            {
                minonEffectSlot[i].transform.GetChild(0).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                minonEffectSlot[i].transform.GetChild(0).GetComponent<Image>().sprite = InvulnerableIcon;
                if(minion.currentGlyph != Glyph.GlyphType.Invulnerable)
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = minion.invulnerableTurnLeft.ToString();
                else
                    minonEffectSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "?";
                effects.Add(9);
                continue;
            }
        }
    } 


    public void UpdateActionPanel(Minion minion)
    {
        Minion currentMinion = GameManager.Instance.GetCurrentMinion();

        MovementAmountText.text = minion.movementAmountLeft.ToString();

        if(minion.haveMoved == false)
            movementCost.text = (1 * (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1)).ToString();
        else
            movementCost.text = 0.ToString();

        firstAttackCost.text = (minion.firstAttack.actionPointCost * (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1)).ToString();
        secondAttackCost.text = (minion.secondAttack.actionPointCost * (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1)).ToString();
        specialAttackCost.text = (minion.specialAttack.actionPointCost * (currentMinion.overchargeTurnLeft > 0 || currentMinion.currentGlyph == Glyph.GlyphType.Overcharge ? 2 : 1)).ToString();

        if(currentMinion.firstAttack.attackIcon != null)
            firstAttackIcon.sprite = currentMinion.firstAttack.attackIcon;
        if (currentMinion.secondAttack.attackIcon != null)
            secondAttackIcon.sprite = currentMinion.secondAttack.attackIcon;
        if (currentMinion.specialAttack.attackIcon != null)
            specialAttackIcon.sprite = currentMinion.specialAttack.attackIcon;

        if (minion.firstAttack.cooldownLeft > 0)
        {
            firstAttackCooldownBg.SetActive(true);
            firstAttackCooldDownText.text = minion.firstAttack.cooldownLeft.ToString();
        }
        else if (minion.firstAttack.cooldownLeft != -1)
        {
            firstAttackCooldownBg.SetActive(false);
        }

        if (minion.secondAttack.cooldownLeft > 0)
        {
            secondAttackCooldownBG.SetActive(true);
            secondAttackCooldDownText.text = minion.secondAttack.cooldownLeft.ToString();
        }
        else if (minion.secondAttack.cooldownLeft != -1)
        {
            secondAttackCooldownBG.SetActive(false);
        }

        if (minion.specialAttack.cooldownLeft > 0)
        {
            specialAttackCooldownBG.SetActive(true);
            specialAttackCooldDownText.text = minion.specialAttack.cooldownLeft.ToString();
        }
        else if (minion.specialAttack.cooldownLeft != -1)
        {
            specialAttackCooldownBG.SetActive(false);
        }
    }








    public void NextTurn()
    {
        SoundManager.Instance.PlayUISound("PassTurn");
        GameManager.Instance.NextTurn();
    }




    public void SetTimer(float timer)
    {
        timeSlider.value = timer / GameManager.Instance.timeByTurn;
    }





    public void InitiateActionPointPanel()
    {
        actionPoints = new List<Image>();
        actionPoints.Add(actionPointImage);
        actionPointBarBg.SetActive(true);

        for (int i = 0; i < 10 - 1; i++)
        {
            Image newActionPointSlot = Instantiate(actionPointImage, actionPointImage.transform.parent);

            if (i < 3)
                newActionPointSlot.color = activatedColor;
            else
                newActionPointSlot.color = deactivatedColor;

            actionPoints.Add(newActionPointSlot);
        }

        actionPointText.text = 3.ToString() + "/" +
            10.ToString();
    }
    public void CloseActionPointPanel()
    {
        foreach (var point in actionPoints)
        {
            actionPointText.text = 0.ToString() + "/" + 10.ToString();
            point.color = deactivatedColor;
        }
    }
    public void SetActionPoint(int point)
    {
        actionPointText.text = point.ToString() + " / " + 
           10.ToString();
       
        for (int i = 0; i < 10; i++)
        {
            if(i < point)
                actionPoints[i].color = activatedColor;
            else
                actionPoints[i].color = deactivatedColor;
        }
    }
}
