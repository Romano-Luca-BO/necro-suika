using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnerScript : MonoBehaviour
{
    InputAction clickAction;
    InputAction pointAction;
    [SerializeField] GameObject TESTSPAWN;
    Vector3 mousePosition;
    int currentID = 1;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnDisable()
    {
        clickAction.performed -= OnClick;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Vector2 screenPos = pointAction.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
        print(mousePosition);
        spawnNew(TESTSPAWN, mousePosition, false);
    }

    public void spawnNew(GameObject unitPrefab, Vector3 position, bool fromFusion) //cosa, dove, fusione/click
    {
        currentID++;
        GameObject newSpawn = Instantiate(unitPrefab, position, Quaternion.identity);
        unit newunit = newSpawn.GetComponent<unit>();
        newunit.setID (currentID);
        newunit.setSpawner(this);
    }
}
