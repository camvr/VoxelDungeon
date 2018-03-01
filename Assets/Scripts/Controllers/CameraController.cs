using UnityEngine;

public class CameraController : MonoBehaviour {
    public Vector3 offset;
    public float scrollSpeed = 2.0f;
    public float maxZoom = 4f;

    private Transform target;
    private Vector3 zoomOffset;

    private void Start()
    {
        zoomOffset = new Vector3(0, 0, 0);
        target = PlayerController.instance.transform;
    }

    void FixedUpdate()
    {
        if (GameManager.gameState != GameState.gameOver)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

            zoomOffset -= new Vector3(-scroll, scroll * 1.5f, -scroll);
            if (zoomOffset.y > maxZoom * 1.5f)
                zoomOffset = new Vector3(-maxZoom, maxZoom * 1.5f, -maxZoom);
            else if (zoomOffset.y < 0)
                zoomOffset = new Vector3(0, 0, 0);
        }

        transform.position = target.position + offset + zoomOffset;
        transform.LookAt(target);
    }
}
