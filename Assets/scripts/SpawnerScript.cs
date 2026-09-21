
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpawnerScript : MonoBehaviour
{
    InputAction clickAction;
    InputAction pointAction;
    bool gameoverState = false;
    [SerializeField] List<GameObject> SpawnableObjects;
    [SerializeField] GameObject LeftLimit, RightLimit, endgameCanvas;
    Vector3 mousePosition;
    int currentID = 0, score = -1, maxscore = 0, toSpawn = 0;
    [SerializeField] float unitScaleSpeed = 1;
    GameObject lastSpawned;
    unit lastSpawnedUnit;
    [SerializeField] TMP_Text ScoreText, HighScore, EndScore, ScoreBeaten;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");

        if (clickAction != null)
        {
            clickAction.Enable();
            if (!Application.isMobilePlatform)
            {
                clickAction.performed += OnClick;
            }
            else
            {
                clickAction.canceled += OnClick;
            }
        }
        if (pointAction != null)
        {
            pointAction.Enable();
        }


            spawnNew(SpawnableObjects[toSpawn], mousePosition, false);
        maxscore = PlayerPrefs.GetInt("maxScore", 0);

    }
    public bool isOver()
    {
        return gameoverState;
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

    public Vector3 getMousePosition()
    {
        Vector2 screenPos = pointAction.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
        return mousePosition;
    }
    void OnDisable()
    {
        if (clickAction != null)
        {
            if (!Application.isMobilePlatform)
            { 
                clickAction.performed -= OnClick;
            }
            else
            {
                clickAction.canceled -= OnClick;
            }
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (isOver())
        {
            return;
        }
        if (lastSpawnedUnit.StartDrop())
        {
            if (currentID % 7 == 0)
            {
                toSpawn = 3;
            }
            else if (currentID % 5 == 0)
            {
                toSpawn = 2;
            }
            else if (currentID % 3 == 0)
            {
                toSpawn = 1;
            }
            else
            {
                toSpawn = 0;
            }
            //GameObject ToSpawn = SpawnableObjects[0];
            //spawnNew(ToSpawn, mousePosition, false);
            spawnNew(SpawnableObjects[toSpawn], mousePosition, false);
        }
    }

    public void spawnNew(GameObject unitPrefab, Vector3 position, bool fromFusion) //cosa, dove, fusione/click
    {
        currentID++;
        GameObject newSpawn = Instantiate(unitPrefab, position, Quaternion.identity);
        unit newunit = newSpawn.GetComponent<unit>();
        newunit.setID(currentID);
        //newunit.setKinematic();
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
        endgameCanvas.SetActive(true);
        PlayerPrefs.SetInt("maxScore", maxscore);
        PlayerPrefs.Save();
        if (score == maxscore)
        {
            ScoreBeaten.gameObject.SetActive(true);
        }
        HighScore.text = maxscore.ToString();
        EndScore.text = score.ToString();

    }
    public void UpdateScore(int p)
    {
        score += p;
        if (score >= maxscore)
        {
            maxscore = score;
        }
        ScoreText.text = $"SCORE {score}";
    }
}