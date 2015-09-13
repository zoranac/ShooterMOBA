using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = .033f)]
public class ProjectileSyncTransform : NetworkBehaviour {
	[SyncVar (hook = "SyncPosValues")]
	private Vector3 syncPos;

	[SyncVar (hook = "SyncVelocityValues")]
	private Vector3 syncVelocity;

	[SerializeField]Rigidbody2D myRigidbody2D;
	[SerializeField]Transform myTransform;

	private float lerpRateVelocity = 10;
	private Vector3 lastVelocity;
	private float velocityThreshold = .5f;

	private float lerpRatePos;
	private float normalPosLerpRate = 10f;
	private float lerpRateMultiplierPos = 1f;
	private Vector3 lastPos;
	private float posThreshold = .5f;
	private List<Vector3> syncPosList = new List<Vector3>();

	[SerializeField]private bool useHistoricalInterpolation = false;
	private float CloseEnoughPos = .5f;
	// Use this for initialization
	void Start () {
		lerpRatePos = normalPosLerpRate;
	}
	
	// Update is called once per frame
	void Update(){
		LerpPos();
	}
	void FixedUpdate () {
		TransmitPosition();
		TransmitVelocity();
	}
	void LerpVelocity(){
		if (!isLocalPlayer){
			OrdinaryLerpingVelocity();
		}
	}
	void OrdinaryLerpingVelocity(){
		myRigidbody2D.velocity = Vector3.Lerp(myRigidbody2D.velocity,syncVelocity,Time.deltaTime * lerpRateVelocity);
	}
	void LerpPos(){
		if (!isLocalPlayer){
			if (useHistoricalInterpolation){
				HistoricalLerpingPos();
			}else{
				OrdinaryLerpingPos();
			}
		}
	}
	void OrdinaryLerpingPos(){
		myTransform.position = Vector3.Lerp(myTransform.position,syncPos,Time.deltaTime * lerpRatePos);
	}
	void HistoricalLerpingPos(){
		if (syncPosList.Count > 0){
			lerpRatePos = normalPosLerpRate + (syncPosList.Count * lerpRateMultiplierPos);
			
			myTransform.position = Vector3.Lerp(myTransform.position,syncPosList[0],Time.deltaTime * lerpRatePos);
			
			if (Vector3.Distance(myTransform.position,syncPosList[0]) < CloseEnoughPos){
				//print (Vector3.Distance(myTransform.position,syncPosList[0]));
				syncPosList.RemoveAt(0);
			}
		}
	}
	[Command]
	void CmdProvidePositionToServer(Vector3 pos){
		syncPos = pos;
	}
	[Command]
	void CmdProvideVelocityToServer(Vector3 velocity){
		syncVelocity = velocity;
	}
	[ClientCallback]
	void TransmitPosition(){
		if (isLocalPlayer && Vector3.Distance(myTransform.position,lastPos) > posThreshold){
			lastPos = myTransform.position;
			CmdProvidePositionToServer(lastPos);
			//print("POS");
		}
	}
	[ClientCallback]
	void	TransmitVelocity(){
		if (isLocalPlayer && Vector3.Distance(myRigidbody2D.velocity,lastVelocity) > velocityThreshold){
			lastVelocity = myRigidbody2D.velocity;
			CmdProvideVelocityToServer(lastVelocity);
		}
	}
	[Client]
	void SyncPosValues(Vector3 latestPos){
		syncPos = latestPos;
		if (useHistoricalInterpolation  && !isLocalPlayer)
			syncPosList.Add(syncPos);
	}
	[Client]
	void SyncVelocityValues(Vector3 velocity){
		syncVelocity = velocity;
	}
}
