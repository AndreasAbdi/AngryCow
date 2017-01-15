using UnityEngine;
using System.Collections;

public class PlayerSkill : MonoBehaviour {
    public Transform defender;
    public float defenderSpawnOffset;
    public MapGenerator mapGenerator;

    bool performSkill = false;
	
	void Update ()
    {
        performSkill = Input.GetButtonDown("Fire1");
	}

    void FixedUpdate()
    {
        if(performSkill)
        {
            SpawnOnMap();
        }
    }

    void SpawnOnMap()
    {
        mapGenerator.SpawnAt(GetTargetPosition(), defender, 1);
    }

    void SpawnOnCoordinate()
    {
        Vector3 position = GetTargetPosition();
        if (!Blocked(position))
        {
            SpawnDefender(position);
        }
    }

    Vector3 GetTargetPosition()
    {
        return transform.position + transform.forward * defenderSpawnOffset;
    }

    bool Blocked(Vector3 position)
    {
        return false;
    }

    void SpawnDefender(Vector3 targetPosition)
    {
        Instantiate(defender, targetPosition, Quaternion.identity);
    }
}
