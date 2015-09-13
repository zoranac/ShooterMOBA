using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 0, sendInterval = .033f)]
public class PlayerSyncTransform : NetworkBehaviour {

	[SyncVar (hook = "SyncPosValues")]
	private Vector3 syncPos;
	[SyncVar (hook = "SyncRotValues")]
	private float syncRot;

	[SerializeField]Transform myTransform;
	private float lerpRatePos;
	private float normalPosLerpRate = 10f;
	private float lerpRateMultiplierPos = 1f;
	private float lerpRateRot;
	private float normalRotLerpRate = 60f;
	private float lerpRateMultiplierRot = 2f;

	private Vector3 lastPos;
	private float lastRot;
	private float posThreshold = .5f;
	private float rotThreshold = 1f;

	private NetworkClient nClient;
	private int latency;
	private Text latencyText;

	private List<Vector3> syncPosList = new List<Vector3>();
	private List<float> syncRotList = new List<float>();

	[SerializeField]private bool useHistoricalInterpolation = false;
	private float CloseEnoughPos = .05f;
	private float CloseEnoughRot = .04f;

	ConnectionConfig config = new ConnectionConfig();


	void Start(){
		config.IsAcksLong = true;
		config.MaxSentMessageQueueSize = 256;
		nClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
		latencyText = GameObject.Find("Latency Text").GetComponent<Text>();
		lerpRatePos = normalPosLerpRate;
		lerpRateRot = normalRotLerpRate;
	}
	// Update is called once per frame
	void Update(){
		LerpPos();
		LerpRot();
		ShowLatency();
//		print("POS: " + syncPosList.Count.ToString());
		//print("ROT: " + syncRotList.Count.ToString());
	}

	void FixedUpdate () {
		TransmitPosition();
		TransmitRotation();
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
	void LerpRot(){
		if (!isLocalPlayer){
			if (useHistoricalInterpolation){
				HistoricalLerpingRot();
			}else{
				OrdinaryLerpingRot();
			}

		}
	}
	void OrdinaryLerpingRot(){
		Vector3 newRot = new Vector3(0,0,syncRot);
		myTransform.rotation = Quaternion.Lerp(myTransform.rotation,Quaternion.Euler(newRot),Time.deltaTime * lerpRateRot);
	}
	void HistoricalLerpingRot(){
		if (syncRotList.Count > 0){
			lerpRateRot = normalRotLerpRate + (syncPosList.Count * lerpRateMultiplierRot);

			Vector3 newRot = new Vector3(0,0,syncRotList[0]);
			myTransform.rotation = Quaternion.Lerp(myTransform.rotation,Quaternion.Euler(newRot),Time.deltaTime * lerpRateRot);

			if (Mathf.Abs(myTransform.localEulerAngles.z - syncRotList[0]) < CloseEnoughRot){
				syncRotList.RemoveAt(0);
			}
		}
	}
	[Command]
	void CmdProvidePositionToServer(Vector3 pos){
		syncPos = pos;
	}
	[Command]
	void CmdProvideRotationToServer(float rot){
		syncRot = rot;
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
	void TransmitRotation(){
		if (isLocalPlayer && CheckIfBeyondRotThreshold(myTransform.localEulerAngles.z,syncRot)){
			lastRot = myTransform.localEulerAngles.z;
			CmdProvideRotationToServer(lastRot);
			//print("ROT");
		}
	}
	bool CheckIfBeyondRotThreshold(float rot1, float rot2){
		if (Mathf.Abs(rot1-rot2) > rotThreshold){
			return true;
		}else{
			return false;
		}
	}
	[Client]
	void SyncPosValues(Vector3 latestPos){
		syncPos = latestPos;
		if (useHistoricalInterpolation  && !isLocalPlayer)
			syncPosList.Add(syncPos);
	}
	[Client]
	void SyncRotValues(float latestRot){
		syncRot = latestRot;
		if (useHistoricalInterpolation && !isLocalPlayer)
			syncRotList.Add(syncRot);
	}
	void ShowLatency(){
		if (isLocalPlayer){
			latency = nClient.GetRTT();
			latencyText.text = latency.ToString();
		}
	}
}
