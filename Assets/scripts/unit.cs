

using UnityEngine;

public class unit : MonoBehaviour
{
    [SerializeField] int id = 0, size = 1;
    [SerializeField] float isFusingCD = 100, minSize = 0.2f, normalScale = 1, scaleSpeed =1;
    [SerializeField] GameObject nextTier;
    [SerializeField] SpawnerScript spawner;
    private GameObject LeftLimit, RightLimit;
    private Rigidbody2D RB;
    private int InPlay, Hanging, Fusion1, Fusion2;
    bool isFusing = false, StartingStatic = false, dropReady = false, isGrowing = true, isShrinking = false;
    float isFusingTimer;
    Vector3 mousePosition;



    void Awake()
    {
        RB = this.gameObject.GetComponent<Rigidbody2D>();
        this.gameObject.transform.localScale = new Vector3(minSize, minSize, minSize);
        InPlay = LayerMask.NameToLayer("InPlay");
        Fusion1 = LayerMask.NameToLayer("Fusion1");
        Fusion2 = LayerMask.NameToLayer("Fusion2");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetTimer();
        spawner.UpdateScore(size);


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
        if (isGrowing || isShrinking)
        {
            setScale();

            }
    }

    void LateUpdate()
    {
        if (StartingStatic)
        {

            Vector3 newpos = new Vector3(Mathf.Clamp(mousePosition.x, LeftLimit.transform.position.x, RightLimit.transform.position.x), RightLimit.transform.position.y, transform.position.z);
            transform.position = newpos;
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
    public void setMousePosition(Vector3 newMousePos)
    {
        mousePosition = newMousePos;
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
        setKinematic();
        Hanging = LayerMask.NameToLayer("Hanging");
        this.gameObject.layer = Hanging;
    }
    public bool StartDrop()
    {
        if (dropReady)
        {
            setDynamic();
            StartingStatic = false;
            
        }
        return dropReady;
    }
    public void setKinematic()
    {
        RB.bodyType = RigidbodyType2D.Kinematic;
        RB.linearVelocity = Vector2.zero;
    }
    void setDynamic()
    {
        RB.bodyType = RigidbodyType2D.Dynamic;
        this.gameObject.layer = InPlay;
    }

    private void StartFusion(GameObject otherUnitGameObject, unit otherUnitScript)
    {
        SetBallIsFusing();
        otherUnitScript.SetBallIsFusing();
        setKinematic();
        otherUnitScript.setKinematic();
        this.gameObject.layer = Fusion1;
        otherUnitGameObject.layer = Fusion2;
        Vector3 midpoint = (transform.position + otherUnitGameObject.transform.position) / 2f;
        if (nextTier != null)
        {
            spawner.spawnNew(nextTier, midpoint, true);
        }
        setShrinking();
        otherUnitScript.setShrinking();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        spawner.GameOver();
        Destroy(gameObject);
    }
    public void setShrinking()
    {
        isGrowing = false;
        isShrinking = true;

        }
    private void setScale()
    {
        float newScale = this.gameObject.transform.localScale.x;

            if (isGrowing)
        {
            newScale += Time.deltaTime*scaleSpeed*normalScale;
            if (newScale >= normalScale)
            {
                isGrowing = false;
                newScale = normalScale;
                dropReady = true;
                if (!StartingStatic)
                {
                    setDynamic();
                    this.gameObject.layer = InPlay;

                }
            }
        }
        else if (isShrinking)
        {
            newScale -= Time.deltaTime * scaleSpeed * normalScale;
            if (newScale < minSize)
            {
                Destroy(this.gameObject);

                }
        }

        this.gameObject.transform.localScale = new Vector3(newScale, newScale, newScale);
    }
    public void setScaleSpeed(float sped)
    {
        scaleSpeed = sped;
    }

    public void SetLimit(GameObject Right, GameObject Left)
    {
        LeftLimit = Left;
        RightLimit = Right;
    }
}