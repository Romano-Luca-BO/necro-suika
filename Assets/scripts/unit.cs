using UnityEngine;

public class unit : MonoBehaviour
{
    [SerializeField]int id = 0, isFusingCD = 100, size = 1;


    bool isFusing = false;
    int isFusingTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isFusingTimer = isFusingCD;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFusing) 
        {
            //insert code to remove is fusing status after cd, in case of fusion failure
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
            }

            //gestisci collisione, chiedendo all'altro oggetto il suo id
        }
    }

    public int CheckBallId()
    {
        return id;
    }
}

