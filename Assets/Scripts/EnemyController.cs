using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public float speed;
    public Rigidbody rigidBody;

    void FixedUpdate()
    {
        Vector3 movement = Vector3.left * speed * Time.deltaTime;
        rigidBody.MovePosition(transform.position + movement);    
    }
}
