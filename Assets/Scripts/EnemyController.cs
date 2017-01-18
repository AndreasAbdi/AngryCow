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
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        navMeshAgent.destination = player.transform.position;
    }
}
