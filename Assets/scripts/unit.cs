

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
    bool StartingStatic = false, dropReady = false, isGrowing = true, isShrinking = false;
    float isFusingTimer;
    Vector3 mousePosition, fusionPoint, fusionStartingPoint;
    [SerializeField] bool isFusing = false;



    void Awake()
    {
        RB = this.gameObject.GetComponent<Rigidbody2D>();
        this.gameObject.transform.localScale = new Vector3(minSize*normalScale, minSize*normalScale, minSize * normalScale);
        InPlay = LayerMask.NameToLayer("InPlay");
        Fusion1 = LayerMask.NameToLayer("Fusion1");
        Fusion2 = LayerMask.NameToLayer("Fusion2");
        RB.useFullKinematicContacts = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetTimer();
        spawner.UpdateScore(size);


    }
    private void FixedUpdate()
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
    // Update is called once per frame	
    void Update()
    {

    }

    void LateUpdate()
    {
        if (StartingStatic)
        {

            Vector3 newpos = new Vector3(Mathf.Clamp(mousePosition.x, LeftLimit.transform.position.x, RightLimit.transform.position.x), RightLimit.transform.position.y, transform.position.z);
            transform.position = newpos;
        }

    }

    private void wallevade(Collision2D collision)
    {
        print("wallevade");
        Collider2D thisCollider = collision.otherCollider, otherCollider = collision.collider;
        ColliderDistance2D d = thisCollider.Distance(otherCollider);
        Vector2 correction = d.normal * d.distance;
        correction = new Vector2(correction.x, correction.y);
        //Vector3 vector3 = new Vector3(correction.x, correction.y, 0);
        RB.position += correction;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" && RB.bodyType == RigidbodyType2D.Kinematic && isGrowing)
        {
            wallevade(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" && RB.bodyType == RigidbodyType2D.Kinematic)
        {
            //wallevade(collision);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        int otherID = -1;
        if (collision.gameObject.tag == "Wall" && RB.bodyType == RigidbodyType2D.Kinematic && isGrowing)
        {
            wallevade(collision);
        }
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
        setKinematic();
        setShrinking();
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
            StartingStatic = false;
            mousePosition = spawner.getMousePosition();
            Vector3 newpos = new Vector3(Mathf.Clamp(mousePosition.x, LeftLimit.transform.position.x, RightLimit.transform.position.x), RightLimit.transform.position.y, transform.position.z); 
            transform.position = newpos;
            setDynamic();

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
        this.gameObject.layer = Fusion1;
        otherUnitGameObject.layer = Fusion2;
        //Vector3 midpoint = (transform.position + otherUnitGameObject.transform.position) / 2f;
        fusionPoint = (transform.position + otherUnitGameObject.transform.position) / 2f;
        fusionStartingPoint = transform.position;
        if (nextTier != null)
        {
            spawner.spawnNew(nextTier, fusionPoint, true);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "DeathTrigger")
        {
            spawner.GameOver();
            Destroy(gameObject);
        }
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
            newScale += Time.deltaTime*scaleSpeed*normalScale*2;
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