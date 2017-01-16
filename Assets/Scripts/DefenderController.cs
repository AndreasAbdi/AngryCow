using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderController : MonoBehaviour {
    public float lifetime;

    float currentTimeRemaining;

	// Use this for initialization
	void Start () {
        currentTimeRemaining = lifetime;	
	}
	
    void FixedUpdate()
    {
        currentTimeRemaining -= Time.deltaTime;
        if(currentTimeRemaining <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
