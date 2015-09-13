using UnityEngine;
using System.Collections;

public class EdgePan : MonoBehaviour {
	public float Margin;
	public float PanSpeed;				//should be from 10 to 100
	public float smoothTime = 0.3F;		//should be from 0 to 5
	public KeyCode LockCameraOnPlayerKey;

	private float _panSpeed;	
	private float _yVelocity = 0.0F;
	private float _xVelocity = 0;
	private bool _canEdgePan = true;
	private GameObject _player;
	// Do camera movement by mouse position if (mPosX < scrollArea) {myTransform.Translate(Vector3.right -scrollSpeed Time.deltaTime);} if (mPosX >= Screen.width-scrollArea) {myTransform.Translate(Vector3.right scrollSpeed Time.deltaTime);} if (mPosY < scrollArea) {myTransform.Translate(Vector3.up -scrollSpeed Time.deltaTime);} if (mPosY >= Screen.height-scrollArea) {myTransform.Translate(Vector3.up scrollSpeed Time.deltaTime);}
	
	// Do camera movement by keyboard myTransform.Translate(Vector3(Input.GetAxis("EditorHorizontal") scrollSpeed Time.deltaTime, Input.GetAxis("EditorVertical") scrollSpeed Time.deltaTime, 0) );
	
	// Do camera movement by holding down option or middle mouse button and then moving mouse if ( (Input.GetKey("left alt") || Input.GetKey("right alt")) || Input.GetMouseButton(2) ) { myTransform.Translate(-Vector3(Input.GetAxis("Mouse X")*dragSpeed, Input.GetAxis("Mouse Y")*dragSpeed, 0) ); } 
	void Start(){
		_player = GetComponent<CameraNetworkSetup>().PlayerPrefab;
	}

	void Update () {
		if (Input.GetKeyDown(LockCameraOnPlayerKey)){
			_canEdgePan = !_canEdgePan;
		}
		if (_canEdgePan){
			if (smoothTime > 0)
				_panSpeed = PanSpeed * smoothTime * 40;
			else
				_panSpeed = PanSpeed;
			float mousePosX = Input.mousePosition.x; 
			float mousePosY = Input.mousePosition.y;


			if (mousePosX < Margin) 
			{ 

				float newPosition = Mathf.SmoothDamp(transform.position.x, transform.position.x - (_panSpeed * Time.deltaTime), ref _xVelocity, smoothTime);
				transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
				//transform.Translate(Vector3.right * -PanSpeed * Time.deltaTime); 
			} 
			
			if (mousePosX >= Screen.width - Margin) 
			{ 

				float newPosition = Mathf.SmoothDamp(transform.position.x, transform.position.x + (_panSpeed * Time.deltaTime), ref _xVelocity, smoothTime);
				transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
				//transform.Translate(Vector3.right * PanSpeed * Time.deltaTime); 
			}
			if (!(mousePosX < Margin) && !(mousePosX >= Screen.width - Margin))
			{
				_xVelocity = 0;
			}

			if (mousePosY < Margin) 
			{ 
				float newPosition = Mathf.SmoothDamp(transform.position.y, transform.position.y - (_panSpeed * Time.deltaTime), ref _yVelocity, smoothTime);
				transform.position = new Vector3(transform.position.x, newPosition, transform.position.z);
				//transform.Translate(transform.up * -PanSpeed * Time.deltaTime); 
			} 
			
			if (mousePosY >= Screen.height - Margin) 
			{ 
				float newPosition = Mathf.SmoothDamp(transform.position.y, transform.position.y + (_panSpeed * Time.deltaTime), ref _yVelocity, smoothTime);
				transform.position = new Vector3(transform.position.x, newPosition, transform.position.z);
				//transform.Translate(transform.up * PanSpeed * Time.deltaTime); 
			}
			if (!(mousePosY < Margin) && !(mousePosY >= Screen.height - Margin))
			{
				_yVelocity = 0;
			}
		}
		else{
			transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, transform.position.z);
		}
	}
}
