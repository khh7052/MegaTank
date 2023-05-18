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
    STARTBATTLE, // 전투 시작
    ENDBATTLE, // 전투 끝
    REST, // 정비
    ENDING, // 엔딩
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

    public Unit playerUnit; // 플레이어 유닛
    public Unit baseUnit; // 플레이어 기지

    public int initSpawnMoney = 500; // 스폰 최초 돈
    public int currentSpawnMoney = 500;
    public int initMoney = 500;
    private int currentMoney = 500;
    public int saveMoney = 0;
    public float plusSpawnMoney = 1.2f; // 스폰 돈 증가량

    public Material morningSkybox;
    public Material nightSkybox;
    
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
                    RenderSettings.skybox = morningSkybox;
                    RenderSettings.fog = (Random.value > 0.5f);
                    Cursor.lockState = CursorLockMode.Confined;
                    Opening();
                    break;
                case GameState.SETTING:
                    Cursor.lockState = CursorLockMode.Confined;
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

    // 새로운 날 시작 연출부분 (DAY 0 -> DAY 1)
    void Opening()
    {
        currentSpawnMoney += (int)(currentSpawnMoney * plusSpawnMoney); // Spawn 돈 증가
        Day++; // Day 증가
        fade.FadeOut();
        OnOpening.Invoke();
        StartCoroutine(OpeningCoroutine());
    }

    IEnumerator OpeningCoroutine()
    {
        yield return new WaitForSeconds(3f);
        // 나팔소리 효과음 추가해야됨
        if(Day == 0)
        {
            State = GameState.SETTING;
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

    // 전투 시작
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
        // 흭득한 돈 추가
        currentMoney += saveMoney;
        saveMoney = 0;
        OnRest.Invoke();
    }
    
}
