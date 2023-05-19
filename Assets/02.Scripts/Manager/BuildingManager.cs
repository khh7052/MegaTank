using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MouseType
{
    NONE,
    SPAWN,
    INFOMATION,
    PLACEMENT,
    REMOVE,
}




public class BuildingManager : Singleton<BuildingManager>
{
    private Unit selectUnit;
    private GameObject selectUnitObject;
    public float rotateSpeed = 5f;

    public Material spawnMaterial;
    public Material removeMaterial;
    private Material originalMaterial;
    
    private Renderer[] selectUnitObjectRenderers;
    
    public Material UnitMaterial
    {
        get { return Instantiate(selectUnitObjectRenderers[0].material); }
        set {
            foreach (Renderer renderer in selectUnitObjectRenderers)
            {
                renderer.material = value;
            }
        }
    }
    
    public MouseType mouseType;
    public RaycastHit unitHit;
    public RaycastHit terrainHit;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip spawnSound;
    public AudioClip removeSound;

    private void Awake()
    {
        GameManager.Instance.OnRest.AddListener(Init);
        GameManager.Instance.OnOpening.AddListener(Init);
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // UI 위에 마우스 있을 때 중단

        MouseHitUpdate();

        switch (mouseType)
        {
            case MouseType.NONE:
                break;
            case MouseType.SPAWN:
                if (Input.GetKey(KeyCode.Q)) SelectUnitObjectRotate(-1);
                if (Input.GetKey(KeyCode.E)) SelectUnitObjectRotate(1);

                if (selectUnitObject)
                {
                    SelectUnitObjectMove();
                    if (Input.GetMouseButtonDown(0)) Spawn();
                    else if (Input.GetMouseButtonDown(1)) Init();
                }
                break;
            case MouseType.INFOMATION:
                InfomationUnitUpdate();
                if (Input.GetMouseButtonDown(0)) OpenUnitInfomation();
                else if (Input.GetMouseButtonDown(1)) Init();
                break;
            case MouseType.REMOVE:
                RemoveUnitUpdate();
                if (Input.GetMouseButtonDown(0)) SelectUnitRemove();
                else if (Input.GetMouseButtonDown(1)) Init();
                break;
        }
    }

    void MouseHitUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out unitHit, Mathf.Infinity, GameManager.Instance.unitLayerMask);
        Physics.Raycast(ray, out terrainHit, Mathf.Infinity, GameManager.Instance.terrainLayerMask);
    }

    void Init()
    {
        if (mouseType == MouseType.SPAWN) SelectUnitReset();
        if (mouseType == MouseType.REMOVE) SelectUnitMaterialReset();
        mouseType = MouseType.NONE;
    }

    void SelectUnitReset()
    {
        if (selectUnit)
        {
            UnitMaterial = originalMaterial;
            selectUnitObject.SetActive(false);
            selectUnit = null;
            selectUnitObject = null;
        }
        
    }

    void SelectUnitMaterialReset()
    {
        if (selectUnit)
        {
            UnitMaterial = originalMaterial;
            selectUnit = null;
            selectUnitObject = null;
        }
    }


    void SpawnInit(Unit unit)
    {
        selectUnitObject = PoolManager.Instance.Pop(unit.gameObject);
        selectUnitObjectRenderers = selectUnitObject.GetComponentsInChildren<Renderer>();
        selectUnit = unit;
        originalMaterial = selectUnitObject.GetComponentsInChildren<Renderer>()[0].material;
        UnitMaterial = spawnMaterial;
        mouseType = MouseType.SPAWN;
    }

    // SPAWN
    public void StartSpawn(Unit unit)
    {
        if (GameManager.Instance.CurrentMoney < unit.spawnMoney) return;
        if (!unit.makeCondition.ConditionCheck(unit))
        {
            print("조건을 달성하지 못했습니다.");
            return; // 조건 달성 안되면 금지
        }

        Init();
        SpawnInit(unit);
    }

    void Spawn()
    {
        if (selectUnit == null) return;
        if (GameManager.Instance.CurrentMoney < selectUnit.spawnMoney) return;

        GameManager.Instance.CurrentMoney -= selectUnit.spawnMoney;
        UnitMaterial = originalMaterial;
        SpawnInit(selectUnit);

        audioSource.clip = spawnSound;
        audioSource.Play();

        if (GameManager.Instance.CurrentMoney < selectUnit.spawnMoney) Init();
        if (!selectUnit.makeCondition.ConditionCheck(selectUnit))
        {
            print("조건을 달성하지 못했습니다.");
            Init();
        }
    }

    
    void SelectUnitObjectMove()
    {
        selectUnitObject.transform.position = terrainHit.point;
    }

    // 선택유닛 회전
    void SelectUnitObjectRotate(float dir)
    {
        selectUnitObject.transform.Rotate(0, dir * Time.deltaTime * rotateSpeed, 0);
    }

    // Infomation
    public void StartInfomation()
    {
        Init();
        mouseType = MouseType.INFOMATION;
    }

    void InfomationUnitUpdate()
    {
        SelectUnitMaterialReset();

        if (unitHit.collider)
        {
            selectUnit = unitHit.collider.GetComponent<Unit>();
            if (selectUnit == null) return;
            
            selectUnitObject = unitHit.collider.gameObject;
            selectUnitObjectRenderers = selectUnitObject.GetComponentsInChildren<Renderer>();
            originalMaterial = selectUnitObject.GetComponentsInChildren<Renderer>()[0].material;
            UnitMaterial = spawnMaterial;
        }
    }

    void OpenUnitInfomation()
    {
        if (selectUnit == null) return;
        UIManager.Instance.InfomationUIUpdate(selectUnit);
    }



    // Remove
    public void StartRemove()
    {
        Init();
        mouseType = MouseType.REMOVE;
    }

    void RemoveUnitUpdate()
    {
        SelectUnitMaterialReset();

        if (unitHit.collider)
        {
            selectUnit = unitHit.collider.GetComponent<Unit>();
            if (selectUnit == null) return;
            if (selectUnit == GameManager.Instance.baseUnit || selectUnit == GameManager.Instance.playerUnit) return;

            selectUnitObject = unitHit.collider.gameObject;
            selectUnitObjectRenderers = selectUnitObject.GetComponentsInChildren<Renderer>();
            originalMaterial = selectUnitObject.GetComponentsInChildren<Renderer>()[0].material;
            UnitMaterial = removeMaterial;
        }
    }

    void SelectUnitRemove()
    {
        if (selectUnitObject == null) return;

        GameManager.Instance.CurrentMoney += selectUnit.spawnMoney;
        SelectUnitReset();

        audioSource.clip = removeSound;
        audioSource.Play();
    }

    

}
