using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MouseType
{
    NONE,
    BUILD,
    DEMOLISH,
}

public class BuildingManager : Singleton<BuildingManager>
{
    public List<GameObject> buildings;
    private Unit currentUnit;
    public bool isBuilding = false;
    RaycastHit hit;
    public float rotateSpeed = 5f;

    public Material selectMaterial;
    public Material focusDemolishMaterial;
    private Material originalMaterial;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip buildSound;
    public AudioClip demolishSound;

    public GameObject SelectObject
    {
        get { return currentUnit.gameObject; }
    }

    public Material UnitMaterial
    {
        get { return currentUnit.GetComponent<Renderer>().material; }
        set
        {
            currentUnit.GetComponent<Renderer>().material = value;
        }
    }

    public MouseType mouseType;

    private void Awake()
    {
        StopBuild();
        GameManager.Instance.OnRest.AddListener(()=> mouseType = MouseType.NONE);
    }

    private void Update()
    {
        switch (mouseType)
        {
            case MouseType.NONE:
                break;
            case MouseType.BUILD:
                if (Input.GetKey(KeyCode.Q)) RotateBuilding(-1);
                if (Input.GetKey(KeyCode.E)) RotateBuilding(1);

                if (currentUnit)
                {
                    MoveBuilding();
                    if (Input.GetMouseButtonDown(0)) Build();
                    else if (Input.GetMouseButtonDown(1)) StopBuild();
                }
                break;
            case MouseType.DEMOLISH:
                LastFocusUnitUpdate();
                if (Input.GetMouseButtonDown(0))
                {
                    Demolish();
                }
                break;

        }
    }

    void LastFocusUnitUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameManager.Instance.unitLayerMask))
        {
            currentUnit = hit.collider.GetComponent<Unit>();
            originalMaterial = UnitMaterial;
        }
        else
        {
            if (currentUnit)
            {
                UnitMaterial = originalMaterial;
                currentUnit = null;
                originalMaterial = null;
            }
        }
    }

    void Build()
    {
        if (currentUnit == null) return;
        if (GameManager.Instance.CurrentMoney < currentUnit.spawnMoney) return;
        GameManager.Instance.CurrentMoney -= currentUnit.spawnMoney;
        UnitMaterial = originalMaterial;
        CurrentUnitInit(currentUnit);

        audioSource.clip = buildSound;
        audioSource.Play();

        if (GameManager.Instance.CurrentMoney < currentUnit.spawnMoney) StopBuild();
    }

    void CurrentUnitInit(Unit unit = null)
    {
        if(unit == null)
        {
            if (currentUnit)
            {
                SelectObject.SetActive(false);
                UnitMaterial = originalMaterial;
            }

            currentUnit = null;
            originalMaterial = null;
        }
        else
        {
            PoolManager.Instance.Pop(unit.gameObject);
            currentUnit = unit;
            originalMaterial = UnitMaterial;
            if(mouseType == MouseType.BUILD) UnitMaterial = selectMaterial;
            else if (mouseType == MouseType.DEMOLISH) UnitMaterial = focusDemolishMaterial;
        }
    }

    public void StartDemolish()
    {
        mouseType = MouseType.DEMOLISH;
    }

    public void StartBuild(Unit unit)
    {
        if (GameManager.Instance.CurrentMoney < unit.spawnMoney) return;
        mouseType = MouseType.BUILD;
        isBuilding = true;
        CurrentUnitInit(unit);
    }

    public void StopBuild()
    {
        isBuilding = false;
        CurrentUnitInit();
    }

    void MoveBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameManager.Instance.terrainLayerMask))
        {
            SelectObject.transform.position = hit.point;
        }
    }

    void RotateBuilding(float dir)
    {
        SelectObject.transform.Rotate(0, dir * Time.deltaTime * rotateSpeed, 0);
    }


    void Demolish()
    {
        if (currentUnit == null) return;

        currentUnit.gameObject.SetActive(false);
        GameManager.Instance.CurrentMoney += currentUnit.spawnMoney;

        audioSource.clip = demolishSound;
        audioSource.Play();
    }

    

}
