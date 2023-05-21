using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("Ohter")]
    public Image fadeImage;

    // Option
    [Header("Option")]
    public GameObject optionUI;
    public float mouseZoomOutSensitivityX = 1f;
    public float mouseZoomOutSensitivityY = 1f;
    public float mouseZoomInSensitivityX = 1f;
    public float mouseZoomInSensitivityY = 1f;
    [Header("Opening")]
    // Opening
    public GameObject openingUI;
    public TMP_Text dayText; // - Day UI
    [Header("Battle")]
    // StartBattle
    public GameObject startBattleUI;
    public Image aimImage;
    public TMP_Text enemyCountText;

    // EndBattle
    public GameObject endBattleUI;
    public TMP_Text playerUnitDeathText_Report;
    public TMP_Text playerBuildingDestroyText_Report;
    public TMP_Text saveMoneyText_Report;
    public TMP_Text currentMoneyText_Report;
    public TMP_Text enemyDeathText_Report;

    [Header("Rest")]
    // Rest
    public GameObject restUI;
    public GameObject buildUI; // -Build
    public GameObject DemolishUI; // -Demolish
    public TMP_Text bottomExplainText;
    public TMP_Text TopExplainText;
    public TMP_Text leftExplainText;
    public TMP_Text centerExplainText;
    public TMP_Text currentMoneyText_Rest;

    [Header("Infomation")]
    // inofmation
    public GameObject infomationUI;
    public TMP_Text unitNameText;
    public TMP_Text unitSpawnMoneyText;
    public TMP_Text unitHpText;
    public TMP_Text unitArmorText;
    public TMP_Text unitAttackDamageText;
    public TMP_Text unitAttackDistanceText;
    public TMP_Text unitAttackRateText;
    public TMP_Text unitMoveSpeedText;
    public TMP_Text unitDescriptionText;

    [Multiline] public string buildExplain;
    [Multiline] public string infomationExplain;
    [Multiline] public string placementExplain;
    [Multiline] public string removeExplain;

    private void Awake()
    {
        Init();
        UIUpdate();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.STARTBATTLE && GameManager.Instance.State != GameState.REST && GameManager.Instance.State != GameState.SETTING) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnOption();
        }
    }

    void Init()
    {
        optionUI.SetActive(false);
        openingUI.SetActive(false);
        startBattleUI.SetActive(false);
        endBattleUI.SetActive(false);
        restUI.SetActive(false);

        leftExplainText.text = "";
        bottomExplainText.text = "";
        TopExplainText.text = "";
    }

    public void UIUpdate()
    {
        optionUI.SetActive(false);

        switch (GameManager.Instance.State)
        {
            case GameState.LOBBY:
                break;
            case GameState.OPENING:
                restUI.SetActive(false);
                openingUI.SetActive(true);
                UIFade.Instance.FadeUI(fadeImage, 0, 1);
                UIFade.Instance.FadeUI(dayText, 0, 1);
                DayTextUpdate();
                break;
            case GameState.SETTING:
                RestUIUpdate();
                UIFade.Instance.FadeUI(fadeImage, 1, 0);
                UIFade.Instance.FadeUI(dayText, 1, 0);
                infomationUI.SetActive(false);
                openingUI.SetActive(false);
                restUI.SetActive(true);
                break;
            case GameState.STARTBATTLE:
                UIFade.Instance.FadeUI(fadeImage, 1, 0);
                UIFade.Instance.FadeUI(dayText, 1, 0);
                openingUI.SetActive(false);
                startBattleUI.SetActive(true);
                EnemyCountUpdate();
                break;
            case GameState.ENDBATTLE:
                EndBattleUIUpdate();
                startBattleUI.SetActive(false);
                endBattleUI.SetActive(true);
                infomationUI.SetActive(false);
                break;
            case GameState.REST:
                RestUIUpdate();
                infomationUI.SetActive(false);
                endBattleUI.SetActive(false);
                restUI.SetActive(true);
                break;
        }
    }
    #region Option
    public void OnOption()
    {
        Cursor.lockState = CursorLockMode.Confined;
        optionUI.SetActive(true);
    }
    public void OnZoomOutSensitivityX(float value)
    {
        mouseZoomOutSensitivityX = value;
    }
    public void OnZoomOutSensitivityY(float value)
    {
        mouseZoomOutSensitivityY = value;
    }
    public void OnZoomInSensitivityX(float value)
    {
        mouseZoomInSensitivityX = value;
    }
    public void OnZoomInSensitivityY(float value)
    {
        mouseZoomInSensitivityY = value;
    }

    public void OnBGM(float value)
    {
        SoundManager.Instance.BGMVolume = value;
    }

    public void OnSFX(float value)
    {
        SoundManager.Instance.SFXVolume = value;
    }

    public void OnOptionApply()
    {
        Cursor.lockState = CursorLockMode.Locked;
        optionUI.SetActive(false);
    }
    #endregion

    public void DayTextUpdate()
    {
        dayText.text = $"DAY {GameManager.Instance.Day}";
    }

    public void EnemyCountUpdate()
    {
        enemyCountText.text = $"남은 적 : {GameManager.Instance.EnemyUnitRemainCount}";
    }

    public void OnReportCheckBtn()
    {
        GameManager.Instance.State = GameState.REST;
    }

    public void EndBattleUIUpdate()
    {
        saveMoneyText_Report.text = $"흭득한 돈 : {GameManager.Instance.saveMoney}";
        currentMoneyText_Report.text = $"현재 돈 : {GameManager.Instance.CurrentMoney + GameManager.Instance.saveMoney}";
        playerUnitDeathText_Report.text = $"사망한 유닛 : {GameManager.Instance.playerUnitDeathCount}";
        playerBuildingDestroyText_Report.text = $"파괴된 건물 : {GameManager.Instance.playerBuildingDeathCount}";
        enemyDeathText_Report.text = $"사망한 유닛 : {GameManager.Instance.enemyUnitDeathCount}";
    }

    public void RestUIUpdate()
    {
        currentMoneyText_Rest.text = $"현재 돈 : {GameManager.Instance.CurrentMoney}";
    }

    public void InfomationUIUpdate(Unit unit)
    {
        if(unit == null)
        {
            infomationUI.SetActive(false);
            leftExplainText.text = "";
            return;
        }

        if(infomationUI.activeInHierarchy == false) infomationUI.SetActive(true);

        unitNameText.text = unit.unitName;
        unitSpawnMoneyText.text = $"비용 : {unit.spawnMoney}";
        unitHpText.text = $"체력 :  {unit.currentHP} / {unit.maxHP}";
        unitArmorText.text = $"방어력 : {unit.armor}";
        unitAttackDamageText.text = $"공격력 : {unit.attackDamage}";
        unitAttackDistanceText.text = $"사거리 : {unit.attackDistance}";
        unitAttackRateText.text = $"공격속도 : {unit.attackRate}";
        unitMoveSpeedText.text = $"이동속도 : {unit.moveSpeed}";
        unitDescriptionText.text = unit.unitDescription;

        leftExplainText.text = buildExplain;

        EnforceButton.SelectUnit = unit;
    }
    public void OnBuildBtn()
    {
        leftExplainText.text = buildExplain;
        TopExplainText.text = "건설";
    }

    public void OnInfomationBtn()
    {
        leftExplainText.text = infomationExplain;
        TopExplainText.text = "정보";
    }

    public void OnPlacementBtn()
    {
        leftExplainText.text = placementExplain;
        TopExplainText.text = "배치";
    }

    public void OnRemoveBtn()
    {
        leftExplainText.text = removeExplain;
        TopExplainText.text = "제거";
    }

    public void OnRestEndBtn()
    {
        BuildingManager.Instance.Init();

        bool hasBaseUnit = GameManager.Instance.baseUnit != null;
        if (hasBaseUnit) hasBaseUnit = GameManager.Instance.baseUnit.gameObject.activeInHierarchy;
        bool hasPlayerUnit = GameManager.Instance.playerUnit != null;
        if (hasPlayerUnit) hasPlayerUnit = GameManager.Instance.playerUnit.gameObject.activeInHierarchy;

        if (hasBaseUnit && hasPlayerUnit)
        {
            GameManager.Instance.State = GameState.OPENING;
        }
        else
        {
            string s = "";
            if (!hasBaseUnit) s = "기지를 건설해야합니다!\n";
            if (!hasPlayerUnit) s += "플레이어 탱크를 배치해야합니다!";

            CenterExplainTextFade(s);
        }
        
    }

    public void CenterExplainTextFade(string s)
    {
        centerExplainText.text = s;
        UIFade.Instance.FadeUI(centerExplainText, 1, 0, 3f);
    }

}
