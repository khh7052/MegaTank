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
    public Unit selectUnit;
    public GameObject selectUnitObject;
    public float rotateSpeed = 5f;

    public Material spawnMaterial;
    public Material removeMaterial;
    private Material[] originalMaterials;
    
    private Renderer[] selectUnitObjectRenderers;
    
    public Material UnitMaterial
    {
        set
        {
            foreach (Renderer renderer in selectUnitObjectRenderers) renderer.material = value;
        }
    }

    public Material[] UnitMaterials
    {
        get { 
            Material[] result = new Material[originalMaterials.Length];

            for(int i = 0; i < result.Length; i++)
            {
                result[i] = Instantiate(selectUnitObjectRenderers[i].material);
            }

            return result; 
        }
        set {

            for (int i = 0; i < selectUnitObjectRenderers.Length; i++)
            {
                selectUnitObjectRenderers[i].material = value[i];
            }
            /*
            foreach (Renderer renderer in selectUnitObjectRenderers)
            {
                renderer.material = value;
            }
            */
        }
    }
    
    public MouseType mouseType;
    public RaycastHit unitHit;
    public RaycastHit terrainHit;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip spawnSound;
    public AudioClip removeSound;
    public AudioClip selectSound;

    private void Awake()
    {
        GameManager.Instance.OnRest.AddListener(Init);
        GameManager.Instance.OnOpening.AddListener(Init);
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // UI ���� ���콺 ���� �� �ߴ�

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

    public void Init()
    {
        if (mouseType == MouseType.SPAWN) SelectUnitReset();
        if (mouseType == MouseType.REMOVE) SelectUnitMaterialReset();
        mouseType = MouseType.NONE;
    }

    void SelectUnitReset()
    {
        if (selectUnit)
        {
            UnitMaterials = originalMaterials;
            selectUnitObject.SetActive(false);
            selectUnit = null;
            selectUnitObject = null;
        }
    }

    void SelectUnitMaterialReset()
    {
        if (selectUnit)
        {
            UnitMaterials = originalMaterials;
            selectUnit = null;
            selectUnitObject = null;
        }
    }

    // true = ���� �����
    bool CheckSpawnMoney(Unit unit)
    {
        if (GameManager.Instance.CurrentMoney < unit.spawnMoney)
        {
            UIManager.Instance.CenterExplainTextFade($"���� {unit.spawnMoney - GameManager.Instance.CurrentMoney}��ŭ �����մϴ�!");
            return false;
        }
        else return true;
    }

    void SelectUnitMaterialSave()
    {
        Renderer[] renderers = selectUnitObject.GetComponentsInChildren<Renderer>();
        Material[] materials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
        }

        originalMaterials = materials;
    }

    // SPAWN
    public void StartSpawn(Unit unit)
    {
        if (!CheckSpawnMoney(unit)) return;
        if (!unit.makeCondition.ConditionCheck(unit)) return;

        Init();
        SpawnInit(unit);
    }

    void SpawnInit(Unit unit)
    {
        selectUnitObject = PoolManager.Instance.Pop(unit.gameObject);
        selectUnitObjectRenderers = selectUnitObject.GetComponentsInChildren<Renderer>();
        selectUnit = unit;

        SelectUnitMaterialSave();
        UnitMaterial = spawnMaterial;
        mouseType = MouseType.SPAWN;
    }

    void Spawn()
    {
        if (selectUnit == null) return;
        if (GameManager.Instance.CurrentMoney < selectUnit.spawnMoney) return;

        GameManager.Instance.CurrentMoney -= selectUnit.spawnMoney;
        UnitMaterials = originalMaterials;
        
        audioSource.clip = spawnSound;
        audioSource.Play();

        // �� �Ⱥ����ϰ�, ���� ���ǿ� ���� �� ����
        if (selectUnit.makeCondition.ConditionCheck(selectUnit) && CheckSpawnMoney(selectUnit)) SpawnInit(selectUnit);
        else SelectUnitMaterialReset();
    }

    
    void SelectUnitObjectMove()
    {
        selectUnitObject.transform.position = terrainHit.point;
    }

    // �������� ȸ��
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
            SelectUnitMaterialSave();
            UnitMaterial = spawnMaterial;
        }
    }

    void OpenUnitInfomation()
    {
        if (selectUnit == null) return;
        SoundManager.Instance.PlaySFX(selectSound);
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
            SelectUnitMaterialSave();
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
