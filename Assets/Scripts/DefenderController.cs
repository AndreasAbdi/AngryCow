using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderController : MonoBehaviour {
    public float lifetime;
    float currentTimeRemaining;
    public MapGenerator mapGenerator;

    // Use this for initialization
    void Start () {
        currentTimeRemaining = lifetime;
        mapGenerator = GameObject.FindObjectOfType<MapGenerator>();

	}
	
    void FixedUpdate()
    {
        currentTimeRemaining -= Time.deltaTime;
        if(currentTimeRemaining <= 0)
        {
            mapGenerator.ClearPosition(transform.position);
            Destroy(this.gameObject);
        }
    }
}
