using UnityEngine;
using System.Collections;

public class PlayerFogOfWar : MonoBehaviour {
	
	private Transform FogOfWarPlane;
	public int Number = 1;
	private Camera camera;
	// Use this for initialization
	void Start () {
		camera = GameObject.Find("PlayerCamera(Clone)").GetComponent<Camera>();
		FogOfWarPlane = GameObject.Find("FogOfWar").GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 screenPos = camera.WorldToScreenPoint(transform.position);
		Ray rayToPlayerPos = camera.ScreenPointToRay(screenPos);
		int layermask = (int)(1<<8);
		RaycastHit hit = new RaycastHit();
		if (Physics2D.Raycast(transform.position,screenPos,1000,layermask)){
			FogOfWarPlane.GetComponent<Renderer>().material.SetVector("_Player" + Number.ToString() +"_Pos", hit.point);
		}
//		if(Physics.Raycast(rayToPlayerPos, out hit, 1000, layermask)) {
//
//		}
	}
}