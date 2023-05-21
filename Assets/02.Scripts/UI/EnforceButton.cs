using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public enum EnforceType
{
    MAXHP, // 최대체력
    ARMOR, // 방어력
    MOVESPEED, // 이동속도
    ROTATESPEED, // 회전속도
    ATTACKDAMAGE, // 공격력
    ATTACKRATE, // 공격속도
    REPAIR, // 수리
}

public struct EnforceData
{
    public int maxHP;
    public int armor;
    public int moveSpeed;
    public int rotateSpeed;
    public int attackDamage;
    public int attackRate;
    public int repair;
}


public class EnforceButton : MonoBehaviour
{
    public UnityEvent OnEnforceEvent;
    public static Unit SelectUnit;
    public TMP_Text priceText;
    public Condition condition;
    public EnforceType type;
    public float amount = 1;
    public int initPrice = 100;
    public int plusPrice = 100;
    private int currentPrice = 100;
    
    private Button button;
    public AudioClip enforceSound;
    

    private void Awake()
    {
        button = GetComponent<Button>();
        foreach (var enforceButton in FindObjectsOfType<EnforceButton>())
        {
            if (enforceButton == this) continue;
            enforceButton.OnEnforceEvent.AddListener(EnableUpdate);
        }

    }

    private void OnEnable()
    {
        EnableUpdate();
    }

    public void EnableUpdate()
    {
        button.interactable = condition.ConditionCheck(SelectUnit);
        PriceUpdate();

        switch (type)
        {
            case EnforceType.MOVESPEED:
                button.interactable = (SelectUnit.unitType == UnitType.UNIT);
                break;
            case EnforceType.ROTATESPEED:
                button.interactable = (SelectUnit.unitType == UnitType.UNIT);
                break;
            case EnforceType.ATTACKDAMAGE:
                button.interactable = (SelectUnit.unitType == UnitType.UNIT);
                break;
            case EnforceType.ATTACKRATE:
                button.interactable = (SelectUnit.unitType == UnitType.UNIT);
                break;
            case EnforceType.REPAIR:
                button.interactable = (SelectUnit.CurrentHP < SelectUnit.maxHP);
                break;
        }

        if (button.interactable) button.interactable = GameManager.Instance.CurrentMoney >= currentPrice;
    }

    int UnitEnforceCount
    {
        get
        {
            switch (type)
            {
                case EnforceType.MAXHP: return SelectUnit.enforceData.maxHP;
                case EnforceType.ARMOR: return SelectUnit.enforceData.armor;
                case EnforceType.MOVESPEED: return SelectUnit.enforceData.moveSpeed;
                case EnforceType.ROTATESPEED: return SelectUnit.enforceData.rotateSpeed;
                case EnforceType.ATTACKDAMAGE: return SelectUnit.enforceData.attackDamage;
                case EnforceType.ATTACKRATE: return SelectUnit.enforceData.attackRate;
                case EnforceType.REPAIR: return SelectUnit.enforceData.moveSpeed;
            }
            return 0;
        }
        set
        {
            switch (type)
            {
                case EnforceType.MAXHP: SelectUnit.enforceData.maxHP = value;
                    break;
                case EnforceType.ARMOR: SelectUnit.enforceData.armor = value;
                    break;
                case EnforceType.MOVESPEED: SelectUnit.enforceData.moveSpeed = value;
                    break;
                case EnforceType.ROTATESPEED: SelectUnit.enforceData.rotateSpeed = value;
                    break;
                case EnforceType.ATTACKDAMAGE: SelectUnit.enforceData.attackDamage = value;
                    break;
                case EnforceType.ATTACKRATE: SelectUnit.enforceData.attackRate = value;
                    break;
                case EnforceType.REPAIR: SelectUnit.enforceData.moveSpeed = value;
                    break;
            }
        }
    }

    void PriceUpdate()
    {
        currentPrice = (UnitEnforceCount * plusPrice) + initPrice;
        priceText.text = currentPrice.ToString();
    }

    public void OnEnforce()
    {
        if (SelectUnit == null) return;
        if (GameManager.Instance.CurrentMoney < currentPrice) return;

        switch (type)
        {
            case EnforceType.MAXHP:
                SelectUnit.maxHP += (int)amount;
                SelectUnit.currentHP += (int)amount;
                break;
            case EnforceType.ARMOR:
                SelectUnit.armor += (int)amount;
                break;
            case EnforceType.MOVESPEED:
                SelectUnit.MoveSpeed += amount;
                break;
            case EnforceType.ROTATESPEED:
                SelectUnit.RotateSpeed += amount;
                break;
            case EnforceType.ATTACKDAMAGE:
                SelectUnit.attackDamage += (int)amount;
                break;
            case EnforceType.ATTACKRATE:
                SelectUnit.AttackRate += amount;
                break;
            case EnforceType.REPAIR:
                SelectUnit.CurrentHP += (int)amount;
                break;
        }

        UnitEnforceCount++;
        GameManager.Instance.CurrentMoney -= currentPrice;
        EnableUpdate();
        OnEnforceEvent.Invoke();
        SoundManager.Instance.PlaySFX(enforceSound);
        UIManager.Instance.InfomationUIUpdate(SelectUnit);
    }



}
