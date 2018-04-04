using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
    public Vector3 offset;
    public float scrollSpeed = 2.0f;
    public float maxZoom = 4f;
    public LayerMask fadeMask;
    public float wallFadeMin = 0.1f;
    public float wallFadeTime = 0.5f;

    private Transform target;
    private Vector3 zoomOffset;
    private List<Transform> hiddenWalls;

    private void Start()
    {
        zoomOffset = new Vector3(0, 0, 0);
        target = PlayerController.instance.transform;
        hiddenWalls = new List<Transform>();
    }

    void Update()
    {
        if (!GameManager.instance.gameOver)
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

        // Raycast to player
        // change alpha of any walls in the way
        RaycastHit[] hits = Physics.RaycastAll(transform.position, target.position - transform.position, (target.position - transform.position).magnitude, fadeMask);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hiddenWalls.Contains(hits[i].transform))
            {
                hiddenWalls.Add(hits[i].transform);
                StartCoroutine(FadeWall(hits[i].transform, wallFadeMin, wallFadeTime));
            }
        }

        for (int i = 0; i < hiddenWalls.Count; i++)
        {
            bool isHit = false;
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].transform == hiddenWalls[i])
                {
                    isHit = true;
                    break;
                }
            }
            
            if (!isHit)
            {
                Transform currWall = hiddenWalls[i];
                StartCoroutine(FadeWall(currWall, 1f, wallFadeTime));
                hiddenWalls.RemoveAt(i);
                i--;
            }
        }
    }

    private IEnumerator FadeWall(Transform target, float alpha, float fadeTime)
    {
        float start = target.GetComponent<MeshRenderer>().material.color.a;
        float inverseFadeTime = 1f / fadeTime;

        float t = 0;
        while (t < 1.0f)
        {
            float currAlpha = Mathf.Lerp(start, alpha, t);
            target.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, currAlpha);

            t += Time.deltaTime * inverseFadeTime;
            yield return new WaitForFixedUpdate();
        }

        target.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, alpha);
    }
}
