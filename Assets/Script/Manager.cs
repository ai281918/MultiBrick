using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
	Vector3 spawnPos;
	public GameObject brickPrefab;
	public Brick[,] bricks = new Brick[20, 24];

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CreateBrick()
	{
		spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if(spawnPos.x < -2.5f || spawnPos.x > 2.5f || spawnPos.y < -1.3f || spawnPos.y > 4.7f) return;

		int x = (int)((spawnPos.x + 2.5f) / 0.25f);
		int y = (int)((spawnPos.y + 1.3f) / 0.25f);

		if(bricks[x, y] == null){
			bricks[x, y] = Instantiate(brickPrefab, new Vector3(-2.375f + 0.25f * x, -1.175f + 0.25f * y, 0f), Quaternion.identity).GetComponent<Brick>();
		}
	}
}
