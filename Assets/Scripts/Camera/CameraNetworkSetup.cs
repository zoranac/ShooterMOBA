using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class CameraNetworkSetup : NetworkBehaviour {
	public GameObject PlayerPrefab;
	// Use this for initialization
	void Start () {
		//if (isLocalPlayer){
			GetComponent<Camera>().enabled = true;
			GetComponent<EdgePan>().enabled = true;
			GetComponent<MoveToPlayer>().enabled = true;
			GetComponent<AudioListener>().enabled = true;
		//}
	}
}
