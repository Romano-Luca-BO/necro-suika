
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SpawnerScript : MonoBehaviour
{
    InputAction clickAction;
    InputAction pointAction;
    [SerializeField] List<GameObject> SpawnableObjects;
    [SerializeField] GameObject LeftLimit, RightLimit;
    Vector3 mousePosition;
    int currentID = 0;
    [SerializeField] float SpawnCD = 1;
    float TimerSpawnCD;
    GameObject lastSpawned;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
        TimerSpawnCD = SpawnCD;

        if (clickAction != null)
        {
            clickAction.Enable();
            clickAction.performed += OnClick;
        }
        if (pointAction != null)
        {
            pointAction.Enable();
        }
        spawnNew(SpawnableObjects[0], mousePosition, false);

    }

    // Update is called once per frame
    void Update()
    {
        TimerSpawnCD -= Time.deltaTime;
    }


    void OnDisable()
    {
        if (clickAction != null)
        {
            clickAction.performed -= OnClick;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (/*TimerSpawnCD <= 0 &&*/ lastSpawned.GetComponent<unit>().StartDrop())
        {
            GameObject ToSpawn = SpawnableObjects[0];
            Vector2 screenPos = pointAction.ReadValue<Vector2>();
            mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));

            //                                                                                                       this is temporary
            Vector2 pointertemp = new Vector2(mousePosition.x, LeftLimit.transform.position.y);
            lastSpawned.transform.position = pointertemp;
            //                                                                                                       this is temporary

            spawnNew(ToSpawn, mousePosition, false);
            TimerSpawnCD = SpawnCD;
        }
    }

    public void spawnNew(GameObject unitPrefab, Vector3 position, bool fromFusion) //cosa, dove, fusione/click
    {
        currentID++;
        GameObject newSpawn = Instantiate(unitPrefab, position, Quaternion.identity);
        unit newunit = newSpawn.GetComponent<unit>();
        newunit.setID(currentID);
        newunit.setKinematic();
        newunit.setSpawner(this);
        
        if (!fromFusion)
        {
            if (LeftLimit != null && RightLimit != null)
            {
                newunit.SetLimit(RightLimit, LeftLimit);
            }
            newunit.setFixed();
            lastSpawned = newSpawn;
        }
    }
}