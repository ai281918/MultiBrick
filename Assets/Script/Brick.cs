using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {
	PhotonManager photonManager;
	public int idx, idy;

	void Awake()
	{
		photonManager = GameObject.Find("Manager").GetComponent<PhotonManager>();
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.CompareTag("Ball")){
			photonManager.DeleteBrick(idx, idy);
		}
	}

}
