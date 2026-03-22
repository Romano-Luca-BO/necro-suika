
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SpawnerScript : MonoBehaviour
{
    InputAction clickAction;
    InputAction pointAction;
    bool gameoverState = false;
    [SerializeField] List<GameObject> SpawnableObjects;
    [SerializeField] GameObject LeftLimit, RightLimit;
    Vector3 mousePosition;
    int currentID = 0, score = -1;
    [SerializeField] float unitScaleSpeed = 1;
    GameObject lastSpawned;
    unit lastSpawnedUnit;
    [SerializeField] TMP_Text ScoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");

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
        if (lastSpawned != null)
        {
            Vector2 screenPos = pointAction.ReadValue<Vector2>();
            mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
            lastSpawnedUnit.setMousePosition(mousePosition);
        }
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
        if (lastSpawnedUnit.StartDrop() && !gameoverState)
        {
            GameObject ToSpawn = SpawnableObjects[0];
            spawnNew(ToSpawn, mousePosition, false);
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
        newunit.setScaleSpeed(unitScaleSpeed);
        
        if (!fromFusion)
        {
            if (LeftLimit != null && RightLimit != null)
            {
                newunit.SetLimit(RightLimit, LeftLimit);
            }
            newunit.setFixed();
            lastSpawned = newSpawn;
            lastSpawnedUnit = newunit;
        }
    }
    public void GameOver()
    {
        gameoverState = true;
    }
    public void UpdateScore(int p)
    {
        score += p;
        ScoreText.text = $"SCORE {score}";
    }
}