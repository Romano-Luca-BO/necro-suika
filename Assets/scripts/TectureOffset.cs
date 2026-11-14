using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class TectureOffset : MonoBehaviour
{

    private RawImage rawImage;
    private Rect uvRect;
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0f, 0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rawImage = GetComponent<RawImage>();
        uvRect = rawImage.uvRect;
    }

    // Update is called once per frame
    void Update()
    {
        float x = (uvRect.x + scrollSpeed.x * Time.deltaTime) % 1f;
        float y = (uvRect.y + scrollSpeed.y * Time.deltaTime) % 1f;
        uvRect.x = x;
        uvRect.y = y;
        rawImage.uvRect = uvRect;
    }
}
