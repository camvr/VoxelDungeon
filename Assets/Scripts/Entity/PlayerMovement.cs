using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 0.5f;
    public float animFade  = 0.1f;

    private Animator anim;
    private Coroutine playerMovement;

    void Start ()
    {
        anim = GetComponent<Animator>();

        anim.SetFloat("walkSpeedModifier", 1.8f / moveSpeed);
    }

    // Update is called once per frame
    void Update ()
    {
        if (playerMovement == null && GameManager.gameState == GameState.playerTurn)
        {
            if (Input.GetKey(KeyCode.A)) // -x
            {
                AttemptMove(Vector2.left);
            }
            if (Input.GetKey(KeyCode.D)) // +x
            {
                AttemptMove(Vector2.right);
            }
            if (Input.GetKey(KeyCode.W)) // +z
            {
                AttemptMove(Vector2.up);
            }
            if (Input.GetKey(KeyCode.S)) // -z
            {
                AttemptMove(Vector2.down);
            }
        }
    }

    public bool AttemptMove(Vector2 dir)
    {
        Vector3 dest = transform.position + new Vector3(dir.x, 0, dir.y);
        RotateToDir(dir);

        RaycastHit ray;
        if (!Physics.Raycast(transform.position, new Vector3(dir.x, 0, dir.y), out ray, 1f))
        {
            playerMovement = StartCoroutine(Move(dest));
            return true;
        }

        Debug.Log("Hit object: " + ray.transform.name);
        GameManager.gameState = GameState.worldTurn;

        return false;
    }

    private IEnumerator Move(Vector3 dest)
    {
        Vector3 start = transform.position;
        float t = 0.0f;
       
        // start walk animation
        anim.CrossFade("WalkState", animFade);
        anim.SetBool("isWalking", true);
        anim.SetBool("isIdle", false);

        while (t < 1.0f)
        {
            transform.position = Vector3.Lerp(start, dest, t);
            t += Time.deltaTime / moveSpeed;
            yield return new WaitForFixedUpdate();
        }

        transform.position = dest;

        // return to idle animation
        anim.CrossFade("IdleState", animFade);
        anim.SetBool("isWalking", false);
        anim.SetBool("isIdle", true);

        playerMovement = null;

        GameManager.gameState = GameState.worldTurn;
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
