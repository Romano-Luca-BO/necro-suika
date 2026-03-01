using Unity.VisualScripting;
using UnityEngine;

public class unit : MonoBehaviour
{
    [SerializeField] int id = 0, size = 1;
    [SerializeField] float isFusingCD = 100;
    [SerializeField] GameObject nextTier;
    [SerializeField] SpawnerScript spawner;
    private GameObject LeftLimit, RightLimit;

    bool isFusing = false, StartingStatic = false;
    float isFusingTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFusing) 
        {
            isFusingTimer -= Time.deltaTime;
            if (isFusingTimer < 0)
            {
                isFusing = false;
                ResetTimer();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int otherID = -1;
        if (collision != null) 
        {
            var otherUnit = collision.gameObject.GetComponent<unit>();
            if (otherUnit != null)
            {
                otherID = otherUnit.CheckBallId();

                if (otherID < id && !CheckBallFusing() && !otherUnit.CheckBallFusing() && otherUnit.getSize() == size)
                {
                    StartFusion(collision.gameObject, otherUnit);
                }

            }
        }
    }
    void ResetTimer()
    {
        isFusingTimer = isFusingCD;
    }
    public int CheckBallId()
    {
        return id;
    }
    public bool CheckBallFusing()
    {
        return isFusing;
    }
    public void SetBallIsFusing()
    {
        isFusing = true;
    }
    private void OnDestroy()
    {
        print($"detroyed {id}");
        //TODO send updated score
    }
    public void setID(int newID)
    {
        id = newID;
    }
    public void setSpawner(SpawnerScript newSpawnerScript)
    {
        spawner = newSpawnerScript;
    }
    public int getSize()
    { 
        return size; 
    }
    public void setFixed()
    {

        StartingStatic = true;
        //TODO set in fixed position and kinestetic
    }

    

    private void StartFusion(GameObject otherUnitGameObject, unit otherUnitScript)
    {
        SetBallIsFusing();
        otherUnitScript.SetBallIsFusing();
        Vector3 midpoint = (transform.position + otherUnitGameObject.transform.position) / 2f;
        if (nextTier != null) 
        {
            spawner.spawnNew(nextTier, midpoint, true);
        }
        Destroy(otherUnitGameObject);

        Destroy(this.gameObject);
        
    }
    public void SetLimit (GameObject Right, GameObject Left)
    {
        LeftLimit = Left;
        RightLimit = Right;
    }
}

