using UnityEngine;

public class CameraLift : MonoBehaviour
{
    int n = 0;
    [SerializeField] int AscendAmount, DescendAmount;
    [SerializeField] float SlideSpeedMult;
    float StartingHeight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartingHeight = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (n>=AscendAmount)
        {
            transform.position += Vector3.up * SlideSpeedMult * Time.deltaTime;
        }
        else if (n <= DescendAmount)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(transform.position.x, StartingHeight, transform.position.z),
                SlideSpeedMult * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        n++;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        n--;
    }
}
