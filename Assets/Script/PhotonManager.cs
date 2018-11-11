using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PhotonManager : Photon.PunBehaviour {
    public static PhotonManager instance;
	public GameObject startCanvas, spawnCanvas;
	public GameObject barPrefab;
	public Text status;
	Vector3 spawnPos;
	public InputField roomName;
	public GameObject brickPrefab;
	Ball ball;
	Bar bar;
	public Brick[,] bricks = new Brick[20, 24];
	public static int playerID;
 
    void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

	void Start () {
		Debug.Log("Connecting server...");
        PhotonNetwork.ConnectUsingSettings("v1.0");
		// Use PhotonVoice
		// var tmp = PhotonVoiceNetwork.Client;
    }

	void Update()
	{
		if(PhotonNetwork.playerList.Length == 2){
			status.text = "Ready";
		}
	}

	public virtual void OnfailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.LogError(cause);
	}

	public override void OnConnectedToMaster()
	{
		status.text = "Please enter room name.";
		// JoinGameRoom();
	}

	public override void OnJoinedLobby()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	public void JoinGameRoom()
	{
		if(!PhotonNetwork.connectedAndReady){
			return;
		}

		status.text = "Join game room...";
		Debug.Log("Join game room...");
		if(!PhotonNetwork.inRoom && !PhotonNetwork.insideLobby){
			RoomOptions options = new RoomOptions();
			options.MaxPlayers = 2;
			if(roomName.text.Length == 0){
				PhotonNetwork.JoinOrCreateRoom("Master room", options, null);
			}
			else{
				PhotonNetwork.JoinOrCreateRoom(roomName.text, options, null);
			}
		}
		else{
			Debug.Log("Already in room.");
		}
	}

	public override void OnJoinedRoom()
	{
		status.text = "Wait for another player...";
		Debug.Log("Success to join game room.");
		// 如果是Master Client, 即可建立/初始化,與載入遊戲場景
		if (PhotonNetwork.isMasterClient)
		{
			Debug.Log("I am master client.");
			playerID = 0;
			// PhotonNetwork.LoadLevel("GameRoomScene");
		}
		else{
			playerID = 1;
		}
		// Use PhotonVoice
		//PhotonNetwork.Instantiate(voiceRecoder.name, Vector3.zero, Quaternion.identity, 0);
	}

	public void CallPunRPC()
	{
		if(!PhotonNetwork.inRoom) return;
		photonView.RPC("PunRPCTest", PhotonTargets.All);
	}

	[PunRPC]
	void PunRPCTest()
	{

	}

	public void GameStart()
	{
		if(!PhotonNetwork.inRoom) return;
		if(PhotonNetwork.playerList.Length != 2){
			return;
		}
		photonView.RPC("GameStartRPC", PhotonTargets.All);
	}

	[PunRPC]
	void GameStartRPC()
	{
		Debug.Log(playerID);
		bar = Instantiate(barPrefab, new Vector3(0.01f, -3.86f, 0f), Quaternion.identity).GetComponent<Bar>();
		ball = bar.transform.GetChild(0).GetComponent<Ball>();
		if(playerID != 0){
			bar.GetComponent<BoxCollider2D>().enabled = false;
			ball.GetComponent<CircleCollider2D>().enabled = false;
		}
		startCanvas.SetActive(false);
		spawnCanvas.SetActive(true);
	}

	public void BallPosition(float x, float y, float z)
	{
		if(!PhotonNetwork.inRoom || playerID != 0) return;
		photonView.RPC("BallPositionRPC", PhotonTargets.Others, x, y ,z);
	}

	[PunRPC]
	void BallPositionRPC(float x, float y, float z)
	{
		ball.targetPosition.Set(x, y, z);
	}

	public void BarPosition(float x, float y, float z)
	{
		if(!PhotonNetwork.inRoom || playerID != 0) return;
		photonView.RPC("BarPositionRPC", PhotonTargets.Others, x, y, z);
	}

	[PunRPC]
	void BarPositionRPC(float x, float y, float z)
	{
		bar.targetPosition.Set(x, y, z);
	}

	public void CreateBrick()
	{
		if(!PhotonNetwork.inRoom) return;
		if(playerID != 1) return;

		spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if(spawnPos.x < -2.5f || spawnPos.x > 2.5f || spawnPos.y < -1.3f || spawnPos.y > 4.7f) return;

		int x = (int)((spawnPos.x + 2.5f) / 0.25f);
		int y = (int)((spawnPos.y + 1.3f) / 0.25f);
		
		photonView.RPC("UpdateBrickRPC", PhotonTargets.All, x, y, true);
	}

	public void DeleteBrick(int x, int y)
	{
		if(!PhotonNetwork.inRoom) return;
		photonView.RPC("UpdateBrickRPC", PhotonTargets.All, x, y, false);
	}

	[PunRPC]
	void UpdateBrickRPC(int x, int y, bool fill)
	{
		if(playerID == 0){
			if(fill){
				if(bricks[x, y] == null){
					bricks[x, y] = Instantiate(brickPrefab, new Vector3(-2.375f + 0.25f * x, -1.175f + 0.25f * y, 0f), Quaternion.identity).GetComponent<Brick>();
					bricks[x, y].idx = x;
					bricks[x, y].idy = y;
				}
			}
			else{
				Destroy(bricks[x, y].gameObject);
				bricks[x, y] = null;
			}
		}
		else{
			if(fill){
				if(bricks[x, y] != null){
					Destroy(bricks[x, y].gameObject);
				}
				bricks[x, y] = Instantiate(brickPrefab, new Vector3(-2.375f + 0.25f * x, -1.175f + 0.25f * y, 0f), Quaternion.identity).GetComponent<Brick>();
				bricks[x, y].gameObject.GetComponent<BoxCollider2D>().enabled = false;
			}
			else{
				Destroy(bricks[x, y].gameObject);
				bricks[x, y] = null;
			}
		}
		
	}

	public void Launch()
	{
		if(!PhotonNetwork.inRoom) return;
		photonView.RPC("LaunchRPC", PhotonTargets.All);
	}

	[PunRPC]
	void LaunchRPC()
	{
		if(playerID == 0){
			ball.Launch();
		}
		else{
			ball.transform.parent = null;
		}
		ball.isLaunch = true;
	}

	public void CreateBrick_old()
	{
		// if(playerID != 1) return;

		spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if(spawnPos.x < -2.5f || spawnPos.x > 2.5f || spawnPos.y < -1.3f || spawnPos.y > 4.7f) return;

		int x = (int)((spawnPos.x + 2.5f) / 0.25f);
		int y = (int)((spawnPos.y + 1.3f) / 0.25f);

		if(bricks[x, y] == null){
			bricks[x, y] = PhotonNetwork.Instantiate(brickPrefab.name, new Vector3(-2.375f + 0.25f * x, -1.175f + 0.25f * y, 0f), Quaternion.identity, 0).GetComponent<Brick>();
		}
	}
}