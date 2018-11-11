using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour {
	public float speed = 10f;
	Vector2 _speed;
	Rigidbody2D rigidbody2D;
	public Vector3 targetPosition;
	PhotonManager photonManager;

	void Awake()
	{
		_speed.Set(speed, 0f);
		rigidbody2D = GetComponent<Rigidbody2D>();
		photonManager = GameObject.Find("Manager").GetComponent<PhotonManager>();
	}

	// Use this for initialization
	void Start () {
		targetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(PhotonManager.playerID != 0){
			if(PhotonManager.playerID == 1){
				transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);
			}
		}
		else{
			Move();	
			photonManager.BarPosition(transform.position.x, transform.position.y, transform.position.z);
		}

	}

	void Move()
	{
		rigidbody2D.velocity = Input.GetAxis("Horizontal") * _speed;
	}
}
