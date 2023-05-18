using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public enum GameState
{
    LOBBY,
    OPENING,
    SETTING,
    STARTBATTLE, // ���� ����
    ENDBATTLE, // ���� ��
    REST, // ����
    ENDING, // ����
}

public class GameManager : Singleton<GameManager>
{
    public UnityEvent OnOpening;
    public UnityEvent OnStartBattle;
    public UnityEvent OnEndBattle;
    public UnityEvent OnRest;
    public UnityEvent OnEnding;
    public UnityEvent OnStateChange;
    public CameraFade fade;

    public Unit playerUnit; // �÷��̾� ����
    private Unit baseUnit; // �÷��̾� ����

    public Unit BaseUnit
    {
        get { return baseUnit; }
        set { baseUnit = value; }
    }

    public int initSpawnMoney = 500; // ���� ���� ��
    public int currentSpawnMoney = 500;
    public int initMoney = 500;
    private int currentMoney = 500;
    public int saveMoney = 0;
    public float plusSpawnMoney = 1.2f; // ���� �� ������

    public Material morningSkybox;
    public Material nightSkybox;

    public int playerUnitDeathCount = 0;
    public int playerBuildingDeathCount = 0;
    private int enemyUnitRemainCount = 0;
    public int enemyUnitDeathCount = 0;

    public int endingDay = 30;

    public int EnemyUnitRemainCount
    {
        get { return enemyUnitRemainCount; }
        set
        {
            enemyUnitRemainCount = value;
            if (value == 0) State = GameState.ENDBATTLE;
        }
    }

    public int CurrentMoney
    {
        get { return currentMoney; }
        set
        {
            currentMoney = value;
            UIManager.Instance.RestUIUpdate();
        }
    }

    public LayerMask unitLayerMask;
    public LayerMask obstacleLayerMask;
    public LayerMask terrainLayerMask;

    private int day = -1;

    public int Day
    {
        get { return day; }
        set
        {
            day = value;
            UIManager.Instance.DayTextUpdate();
        }
    }

    private GameState state;

    public GameState State
    {
        get { return state; }
        set
        {
            state = value;

            switch (state)
            {
                case GameState.LOBBY:
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
                case GameState.OPENING:
                    Cursor.lockState = CursorLockMode.Confined;
                    Opening();
                    break;
                case GameState.SETTING:
                    Cursor.lockState = CursorLockMode.Confined;
                    RenderSettings.skybox = morningSkybox;
                    RenderSettings.fog = (Random.value > 0.5f);
                    Setting();
                    break;
                case GameState.STARTBATTLE:
                    Cursor.lockState = CursorLockMode.Locked;
                    StartBattle();
                    break;
                case GameState.ENDBATTLE:
                    Cursor.lockState = CursorLockMode.Confined;
                    EndBattle();
                    break;
                case GameState.REST:
                    RenderSettings.skybox = nightSkybox;
                    RenderSettings.fog = false;
                    Cursor.lockState = CursorLockMode.Confined;
                    Rest();
                    break;
                case GameState.ENDING:
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
                default:
                    break;
            }

            SoundManager.Instance.SoundUpdate();
            FollowCam.Instance.CamTargetUpdate();
            UIManager.Instance.UIUpdate();
            OnStateChange.Invoke();
        }
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        State = GameState.OPENING;
    }

    void Init()
    {
        currentSpawnMoney = initSpawnMoney;
        currentMoney = initMoney;
    }

    public void CountReset(int enemyCount)
    {
        EnemyUnitRemainCount = enemyCount;
        enemyUnitDeathCount = 0;
        playerUnitDeathCount = 0;
        playerBuildingDeathCount = 0;
    }

    // ���ο� �� ���� ����κ� (DAY 0 -> DAY 1)
    void Opening()
    {
        currentSpawnMoney += (int)(currentSpawnMoney * plusSpawnMoney); // Spawn �� ����
        Day++; // Day ����
        fade.FadeOut();
        OnOpening.Invoke();
        StartCoroutine(OpeningCoroutine());
    }

    IEnumerator OpeningCoroutine()
    {
        yield return new WaitForSeconds(fade.fadeTime + 2f);
        // ���ȼҸ� ȿ���� �߰��ؾߵ�
        if(Day == 0)
        {
            State = GameState.SETTING;
        }
        else if (Day == endingDay)
        {
            State = GameState.ENDING;
        }
        else
        {
            State = GameState.STARTBATTLE;
        }
    }

    void Setting()
    {
        fade.FadeIn();
        Rest();
    }

    // ���� ����
    void StartBattle()
    {
        fade.FadeIn();
        SpawnManager.Instance.Spawn();
        OnStartBattle.Invoke();
    }

    // 
    void EndBattle()
    {
        OnEndBattle.Invoke();
    }

    void Rest()
    {
        // ŉ���� �� �߰�
        currentMoney += saveMoney;
        saveMoney = 0;
        OnRest.Invoke();
    }
    
}
