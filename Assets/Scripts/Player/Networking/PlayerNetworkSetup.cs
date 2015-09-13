using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour {
	public GameObject PlayerCamera;
	// Use this for initialization
	void Start () {
		if (isLocalPlayer){
			GameObject tempPlayerCamera = (GameObject)Instantiate(PlayerCamera,new Vector3(transform.position.x,transform.position.y,-10),new Quaternion(0,0,0,0));
			tempPlayerCamera.GetComponent<CameraNetworkSetup>().PlayerPrefab = gameObject;

			GetComponent<PlayerControl>().enabled = true;
			GetComponent<PlayerMovement>().enabled = true;
			GetComponent<PlayerShooting>().enabled = true;
		}
	}

}
