using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    LOBBY, // �κ�
    OPENING, // ������
    SETTING, // �� ó���� ����
    STARTBATTLE, // ���� ����
    ENDBATTLE, // ���� ��
    REST, // ����
    WIN, // �¸� ����
    DEFEAT, // �й� ����
}

public class GameManager : Singleton<GameManager>
{
    public GameState startState;
    public UnityEvent OnOpening;
    public UnityEvent OnStartBattle;
    public UnityEvent OnEndBattle;
    public UnityEvent OnRest;
    public UnityEvent OnEnding;
    public UnityEvent OnStateChange;

    public Unit playerUnit; // �÷��̾� ����
    public Unit baseUnit; // �÷��̾� ����

    public Light sunLight;

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
                    sunLight.intensity = 1.5f;
                    RenderSettings.skybox = morningSkybox;
                    RenderSettings.fog = (Random.value > 0.5f);
                    Setting();
                    break;
                case GameState.STARTBATTLE:
                    playerUnit.agent.enabled = true;
                    sunLight.intensity = 1.5f;
                    RenderSettings.skybox = morningSkybox;
                    Cursor.lockState = CursorLockMode.Locked;
                    StartBattle();
                    break;
                case GameState.ENDBATTLE:
                    Cursor.lockState = CursorLockMode.Confined;
                    EndBattle();
                    break;
                case GameState.REST:
                    sunLight.intensity = 0.4f;
                    playerUnit.agent.enabled = false;
                    playerUnit.gameObject.SetActive(false);
                    playerUnit = null;
                    RenderSettings.skybox = nightSkybox;
                    RenderSettings.fog = false;
                    Cursor.lockState = CursorLockMode.Confined;
                    Rest();
                    break;
                case GameState.WIN:
                    Cursor.lockState = CursorLockMode.Confined;
                    Win();
                    break;
                case GameState.DEFEAT:
                    Cursor.lockState = CursorLockMode.Confined;
                    Defeat();
                    break;
                default:
                    break;
            }

            SoundManager.Instance.SoundUpdate();
            
            if(state != GameState.LOBBY && state != GameState.WIN && state != GameState.DEFEAT)
            {
                UIManager.Instance.UIUpdate();
                FollowCam.Instance.CamTargetUpdate();
            }

            OnStateChange.Invoke();
        }
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        State = startState;
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
        OnOpening.Invoke();
        StartCoroutine(OpeningCoroutine());
    }

    IEnumerator OpeningCoroutine()
    {
        yield return new WaitForSeconds(UIFade.Instance.fadeTime + 2f);
        // ���ȼҸ� ȿ���� �߰��ؾߵ�
        if(Day == 0)
        {
            State = GameState.SETTING;
        }
        else if (Day == endingDay)
        {
            State = GameState.WIN;
        }
        else
        {
            State = GameState.STARTBATTLE;
        }
    }

    void Setting()
    {
        Rest();
    }

    // ���� ����
    void StartBattle()
    {
        SpawnManager.Instance.Spawn();
        OnStartBattle.Invoke();
    }

    // ���� ����
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

    void Win()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2) return; // ���� ���ϰ� �����ϸ� ����
        SceneManager.LoadScene(2);
    }

    void Defeat()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3) return; // ���� ���ϰ� �����ϸ� ����
        SceneManager.LoadScene(3);
    }
    
}
