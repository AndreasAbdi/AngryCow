using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public Rigidbody playerCharacter;
    public float speed;

    Vector3 movementInput;
    
	void OnEnable()
    {
        movementInput = new Vector3();
    }

	// Update is called once per frame
	void Update () {
        movementInput = new Vector3(Input.GetAxis("Horizontal"), 0f , Input.GetAxis("Vertical"));
	}

    void FixedUpdate()
    {
        Vector3 targetPosition = GetTargetPosition();
        Move(targetPosition);
        Rotate(targetPosition);
    }

    Vector3 GetTargetPosition()
    {
        return transform.position + movementInput * speed * Time.deltaTime;
    }

    void Move(Vector3 targetPosition)
    {
        playerCharacter.MovePosition(targetPosition);
    }

    void Rotate(Vector3 targetPosition)
    {
        transform.LookAt(targetPosition);
    }
}
