using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    public float speed;
    public Rigidbody rigidBody;
    public NavMeshAgent navMeshAgent;
    public GameObject player;

    void Start()
    {
    }

    void FixedUpdate()
    {
        navMeshAgent.destination = player.transform.position;

        //Vector3 movement = Vector3.left * speed * Time.deltaTime;
        //rigidBody.MovePosition(transform.position + movement);
    }
}
