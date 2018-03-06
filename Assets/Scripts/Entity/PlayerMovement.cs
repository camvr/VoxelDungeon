﻿using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 0.5f;
    public float animFade  = 0.1f;

    private Animator anim;
    private Coroutine playerMovement;
    private BoxCollider boxCollider;
    private float inverseMoveSpeed;

    void Start ()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        inverseMoveSpeed = 1f / moveSpeed;
        anim.SetFloat("walkSpeedModifier", 1.8f / moveSpeed);
    }

    public bool AttemptMove(Vector2 dir)
    {
        if (playerMovement == null)
        {
            Vector3 dest = transform.position + new Vector3(dir.x, 0, dir.y);
            RotateToDir(dir);

            RaycastHit ray;
            if (!Physics.Raycast(transform.position, new Vector3(dir.x, 0, dir.y), out ray, 1f) && BoardManager.instance.IsLegalPos((int)dest.x, (int)dest.z))
            {
                boxCollider.center += new Vector3(dir.x, 0, dir.y);
                playerMovement = StartCoroutine(SmoothMove(dest));
                return true;
            }
        }

        return false;
    }

    private IEnumerator SmoothMove(Vector3 dest)
    {
        Vector3 start = transform.position;
        Vector3 cStart = boxCollider.center;
        float t = 0.0f;
        
        // start walk animation
        anim.CrossFade("WalkState", animFade);
        anim.SetBool("isWalking", true);
        anim.SetBool("isIdle", false);

        while (t < 1.0f)
        {
            transform.position = Vector3.Lerp(start, dest, t);
            boxCollider.center = Vector3.Lerp(cStart, new Vector3(0, cStart.y, 0), t);

            t += Time.deltaTime * inverseMoveSpeed;
            yield return null;
        }

        transform.position = dest;
        boxCollider.center = new Vector3(0, cStart.y, 0);

        // return to idle animation
        anim.CrossFade("IdleState", animFade);
        anim.SetBool("isWalking", false);
        anim.SetBool("isIdle", true);

        playerMovement = null;
    }

    public void RotateToDir(Vector2 dir)
    {
        if (dir == Vector2.left) // -x
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (dir == Vector2.right) // +x
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (dir == Vector2.up) // +z
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dir == Vector2.down) // -z
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
