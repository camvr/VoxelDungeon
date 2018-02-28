using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {
    public float moveSpeed = 0.5f;
    public float animFade = 0.1f;

    private Animator anim;
    //private Coroutine enemyMovement;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public bool AttemptMove(Vector2 dir)
    {
        Vector3 dest = transform.position + new Vector3(dir.x, 0, dir.y);
        if (Map.IsLegalPos(dest))
        {
            RotateToDir(dir);
            
            StartCoroutine(Move(dest));
            
            return true;
        }

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
