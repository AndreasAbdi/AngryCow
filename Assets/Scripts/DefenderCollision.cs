using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
}