using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
	public float speed = 10f;
	public bool isLaunch = false;
	Rigidbody2D rigidbody2D;
	public Vector3 targetPosition;
	PhotonManager photonManager;
	int cnt = 0;
	// Use this for initialization

	void OnCollisionEnter2D(Collision2D other)
	{
		if(rigidbody2D == null) return;

		rigidbody2D.velocity = rigidbody2D.velocity.normalized * speed;

		if(other.gameObject.CompareTag("Bar")){
			rigidbody2D.velocity = (rigidbody2D.velocity.normalized + other.gameObject.GetComponent<Rigidbody2D>().velocity * 0.3f).normalized * speed;
		}
	}

	void Awake()
	{
		photonManager = GameObject.Find("Manager").GetComponent<PhotonManager>();
	}

	void Start()
	{
		targetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isLaunch && PhotonManager.playerID == 0 && Input.GetKeyDown(KeyCode.Space)){
			photonManager.Launch();
		}
		if(isLaunch){
			if(PhotonManager.playerID == 0){
				if(Mathf.Abs(rigidbody2D.velocity.y) < 0.05f){
					cnt++;
					if(cnt > 300){
						rigidbody2D.velocity = new Vector2(1, 1) * speed;
						cnt = 0;
					}
				}
				else{
					cnt = 0;
				}
				photonManager.BallPosition(transform.position.x, transform.position.y, transform.position.z);
			}
			else{
				transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);
			}
		}
	}

	public void Launch()
	{
		Vector2 tv = transform.parent.GetComponent<Rigidbody2D>().velocity;
		transform.parent = null;
		rigidbody2D = (Rigidbody2D)gameObject.AddComponent(typeof(Rigidbody2D));
		rigidbody2D.mass = 0.001f;
		// rigidbody2D = GetComponent<Rigidbody2D>();
		rigidbody2D.gravityScale = 0;
		rigidbody2D.velocity = (tv + new Vector2(0, 1)*speed).normalized * speed;
	}
}
