﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float moveSpeed = 0.5f;
    
    private Coroutine playerMovement;

    // Update is called once per frame
    void Update () {
        if (playerMovement == null)
        {
            if (Input.GetKey(KeyCode.A)) // -x
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
                playerMovement = StartCoroutine(Move(Vector2.left));
            }
            if (Input.GetKey(KeyCode.D)) // +x
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                playerMovement = StartCoroutine(Move(Vector2.right));
            }
            if (Input.GetKey(KeyCode.W)) // +z
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                playerMovement = StartCoroutine(Move(Vector2.up));
            }
            if (Input.GetKey(KeyCode.S)) // -z
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                playerMovement = StartCoroutine(Move(Vector2.down));
            }
        }
    }

    private IEnumerator Move(Vector2 dir)
    {
        Vector3 start = transform.position;
        Vector3 dest = transform.position + new Vector3(dir.x, 0, dir.y);
        float t = 0.0f;

        if (GameManager.IsLegalPos(dest))
        {
            while (t < 1.0f)
            {
                transform.position = Vector3.Lerp(start, dest, t);
                t += Time.deltaTime / moveSpeed;
                yield return new WaitForFixedUpdate();
            }

            transform.position = dest;
        }
        
        playerMovement = null;
    }
}
