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
        Move();
    }

    void Move()
    {
        playerCharacter.MovePosition(transform.position + movementInput * speed);
    }
}
