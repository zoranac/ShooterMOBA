using UnityEngine;
using System.Collections;

public class MoveToPlayer : MonoBehaviour {
	public KeyCode MoveToPlayerKey;

	GameObject player;
	// Use this for initialization
	void Start () {
		player = GetComponent<CameraNetworkSetup>().PlayerPrefab;
	}
	
	// Update is called once per frame
	void Update () {

		//This will need to be edited
		if (Input.GetKey(MoveToPlayerKey)){
			Vector3 pos = transform.position;
			pos.x = player.transform.position.x;
			pos.y = player.transform.position.y;
			transform.position = pos;
		}
	}
}
