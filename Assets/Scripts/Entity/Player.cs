using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float moveSpeed = 5.0f;

    private Vector3 pos;

	// Use this for initialization
	void Start () {
        pos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetKey(KeyCode.A) && transform.position == pos) // -x
        {
            pos.x -= 1;
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        if (Input.GetKey(KeyCode.D) && transform.position == pos) // +x
        {
            pos.x += 1;
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        if (Input.GetKey(KeyCode.W) && transform.position == pos) // +z
        {
            pos.z += 1;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.S) && transform.position == pos) // -z
        {
            pos.z -= 1;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        
        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * moveSpeed);
    }
}
