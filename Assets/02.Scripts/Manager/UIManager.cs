using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
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



    private void Awake()
    {
        Init();
        UIUpdate();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.STARTBATTLE) return;

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
                DayTextUpdate();
                break;
            case GameState.SETTING:
                RestUIUpdate();
                infomationUI.SetActive(false);
                openingUI.SetActive(false);
                restUI.SetActive(true);
                break;
            case GameState.STARTBATTLE:
                openingUI.SetActive(false);
                startBattleUI.SetActive(true);
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

    public void OnReportCheckBtn()
    {
        GameManager.Instance.State = GameState.REST;
    }

    public void EndBattleUIUpdate()
    {
        saveMoneyText_Report.text = $"흭득한 돈 : {GameManager.Instance.saveMoney}";
        currentMoneyText_Report.text = $"현재 돈 : {GameManager.Instance.CurrentMoney + GameManager.Instance.saveMoney}";
        enemyDeathText_Report.text = $"사망 : 0";
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
            return;
        }

        if(infomationUI.activeInHierarchy == false) infomationUI.SetActive(true);

        unitNameText.text = unit.unitName;
        unitSpawnMoneyText.text = $"비용 : {unit.spawnMoney}";
        unitHpText.text = $"체력 : {unit.maxHP}";
        unitArmorText.text = $"방어력 : {unit.armor}";
        unitAttackDamageText.text = $"공격력 : {unit.attackDamage}";
        unitAttackDistanceText.text = $"사거리 : {unit.attackDistance}";
        unitAttackRateText.text = $"공격속도 : {unit.attackRate}";
        unitMoveSpeedText.text = $"이동속도 : {unit.moveSpeed}";
        unitDescriptionText.text = unit.unitDescription;
    }

    public void OnBuildBtn()
    {

    }

    public void OnDemolishBtn()
    {

    }

    public void OnRestEndBtn()
    {
        GameManager.Instance.State = GameState.OPENING;
    }

}
