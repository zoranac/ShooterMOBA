using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour {

	public Weapon MyWeapon;					// The Player's Weapon					
	public Image ReloadUI;					// Reload UI image
	public Text AmmoText;
	public int ShootButtonInt;
	public KeyCode ReloadKey;
	public bool SeparateShootingAndMovement = true;
	private PlayerControl _playerControl;
	private bool _facingTarget = false;			
	private Quaternion _lookRotation;
	private Vector3 _direction = Vector3.zero;
	private bool _reloading = false;
	private float _reloadTime = 0;
	private bool _canShoot = true;				//If manual test if the shoot button has been lifted and other factors that allow the manual weapon to shoot
	private bool _turning = false;
	private bool _shooting = false;
	private float _duration = 0;				//If automatic and shift quene tracks duration
	private float _startDuration = 0;
	private bool _countDuration = false;
	private float _shootingStart = 0;
	private bool _canAddNode = false;			//only adds node if shoot button has been lifted
	private float _automaticShiftQueneDistanceLimit = .5f;
	private Vector3 _automaticShiftQueneStartPos;
	public bool Shooting()
	{
		return (_shooting && !_reloading);
	}
	// Use this for initialization
	void Start () {
		_playerControl = GetComponent<PlayerControl>();

		//Temp
		MyWeapon = _playerControl.MyWeapon;
		ReloadUI = GameObject.Find("ReloadUI").GetComponent<Image>();
		AmmoText = GameObject.Find("AmmoText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {

		//if you have an automatic weapon and are shift quening it, start tracking the duration
		if (Input.GetMouseButtonDown(ShootButtonInt)){
			if (MyWeapon.Action == Weapon.ActionTypes.Automatic){
				if (Input.GetKey(KeyCode.LeftShift)){
					_countDuration = true;
					_startDuration = Time.time;
					_duration = 0;
					_automaticShiftQueneStartPos = Input.mousePosition;
				}
			}
		}
		if (_countDuration){
			if (Vector3.Distance(Input.mousePosition,_automaticShiftQueneStartPos) > _automaticShiftQueneDistanceLimit){
				AddNode(_duration);
				_startDuration = Time.time;
				_duration = 0;
				_automaticShiftQueneStartPos = Input.mousePosition;
			}
		}
		if (!Input.GetMouseButton(ShootButtonInt)){
			_canShoot = true;
			_canAddNode = true;
		}
		//Update the ammo UI
		AmmoText.text = MyWeapon.GetProjectilesInClip.ToString() + " / " + MyWeapon.ClipSize.ToString();

		//If reloading reload, else see if shooting
		if (_reloading){
			Reload();
			if (Input.GetMouseButton(ShootButtonInt)){
				_canShoot = false;
			}
			//Set up for testing SeparateShootingAndMovement
			if(SeparateShootingAndMovement){
				if (!Input.GetMouseButton(ShootButtonInt) && _playerControl.AttackNodeList.Count == 1){
					if (MyWeapon.Action == Weapon.ActionTypes.Automatic && _playerControl.AttackNodeList[0].NodeType == Node.NodeTypes.Attack){ //if automatic
						_playerControl.AttackNodeList.Clear();
					}
				}
			}else{
				if (!Input.GetMouseButton(ShootButtonInt) && _playerControl.NodeList.Count == 1){
					if (MyWeapon.Action == Weapon.ActionTypes.Automatic && _playerControl.NodeList[0].NodeType == Node.NodeTypes.Attack){ //if automatic
						_playerControl.NodeList.Clear();
					}
				}
			}
		}
		//Set up for testing SeparateShootingAndMovement
		//if the node list is not empty and the current node is an attack node
		if (SeparateShootingAndMovement){
			if (_playerControl.AttackNodeList.Count > 0){
				if (_playerControl.AttackNodeList[0].NodeType == Node.NodeTypes.Attack){
					//if not set to turn and shoot, start turning
					if (!_turning && !_shooting)
					{
						_turning = true;
						_facingTarget = false;
					}
					//Turn
					if (_turning)
						TurnToShoot(_playerControl.AttackNodeList[0].Target);
					//Shoot
					if (_shooting && !_reloading)
						Shoot(_playerControl.AttackNodeList[0].Target);

					if (_shooting && _playerControl.AttackNodeList[0].Duration != 0)
					{
					    if (Time.time >= _shootingStart + _playerControl.AttackNodeList[0].Duration)
					    {
							_shootingStart = 0;
							_playerControl.AttackNodeList.RemoveAt(0);
							if (MyWeapon.Action == Weapon.ActionTypes.Manual)
								_canShoot = false;
							_shooting = false;
						}
					}
				}
			}
			else //if the node list is empty
			{
				_turning = false;
				_shooting = false;
			}
		}else{
			if (_playerControl.NodeList.Count > 0){
				if (_playerControl.NodeList[0].NodeType == Node.NodeTypes.Attack){
					//if not set to turn and shoot, start turning
					if (!_turning && !_shooting)
					{
						_turning = true;
						_facingTarget = false;
					}
					//Turn
					if (_turning)
						TurnToShoot(_playerControl.NodeList[0].Target);
					//Shoot
					if (_shooting && !_reloading)
						Shoot(_playerControl.NodeList[0].Target);
					
					if (_shooting && _playerControl.NodeList[0].Duration != 0)
					{
						if (Time.time >= _shootingStart + _playerControl.NodeList[0].Duration)
						{
							_shootingStart = 0;
							_playerControl.NodeList.RemoveAt(0);
							if (MyWeapon.Action == Weapon.ActionTypes.Manual)
								_canShoot = false;
							_shooting = false;
						}
					}
				}
			}
			else //if the node list is empty
			{
				_turning = false;
				_shooting = false;
			}
		}
		//if you are pressing the shoot button
		if (Input.GetMouseButton(ShootButtonInt)){
			if (MyWeapon.Action == Weapon.ActionTypes.Automatic){ //if automatic
				if (_countDuration){
					_duration = Time.time - _startDuration;
				}else{
					ClearAndAddNode();
				}
			}else if (MyWeapon.Action == Weapon.ActionTypes.Manual){ //if manual
				if (_canShoot){
					if (Input.GetKey(KeyCode.LeftShift)){
						if (_canAddNode)
						{
							AddNode();
							_canAddNode = false;
						}
					}else{
						ClearAndAddNode();
					}
				}
			}	
		}
		//if you release the shoot button
		if (Input.GetMouseButtonUp(ShootButtonInt)){
			_canShoot = true;
			if (_countDuration){
				_countDuration = false;
				AddNode(_duration);
			}
		}
		//if you press the reload button or your gun is out of ammo, reload
		if ((Input.GetKeyDown(ReloadKey) || MyWeapon.GetProjectilesInClip <= 0) && !_reloading){
			_reloading = true;
			_reloadTime = Time.time;
		}
	}
	//clear all nodes and add a new attack node
	void ClearAndAddNode(){
		if (SeparateShootingAndMovement){
			_turning = true;
			_facingTarget = false;
			_playerControl.AttackNodeList.Clear();
			_playerControl.AttackNodeList.Add(new Node(Node.NodeTypes.Attack, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
		}else{
			_turning = true;
			_facingTarget = false;
			_playerControl.NodeList.Clear();
			_playerControl.NodeList.Add(new Node(Node.NodeTypes.Attack, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
		}
	}
	//add an attack node to the back of the list
	void AddNode(){
		if (SeparateShootingAndMovement){
			_turning = true;
			_facingTarget = false;
			_playerControl.AttackNodeList.Add(new Node(Node.NodeTypes.Attack, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
		}else{
			_turning = true;
			_facingTarget = false;
			_playerControl.NodeList.Add(new Node(Node.NodeTypes.Attack, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
		}
	}
	//add an attack node to the back of the list and note its duration
	void AddNode(float duration){
		if (SeparateShootingAndMovement){
			_turning = true;
			_facingTarget = false;
			_playerControl.AttackNodeList.Add(new Node(Node.NodeTypes.Attack, Camera.main.ScreenToWorldPoint(Input.mousePosition),duration));
		}else{
			_turning = true;
			_facingTarget = false;
			_playerControl.NodeList.Add(new Node(Node.NodeTypes.Attack, Camera.main.ScreenToWorldPoint(Input.mousePosition),duration));
		}
	}
	//Reload
	void Reload()
	{
		int ammo = MyWeapon.Reload(_playerControl.Ammo);
		if (ammo == -1){
			ReloadUI.fillAmount = (Time.time - _reloadTime) / MyWeapon.GetReloadTime();
		}
		else{
			_playerControl.Ammo = ammo;
			_reloading = false;
		}
	}
	void TurnToShoot(Vector3 shootPos){

		//find the vector pointing from our position to the target
		_direction = (shootPos - transform.position).normalized;
		
		if (!_facingTarget)
		{
			Quaternion lastRot = transform.rotation;
			//create the rotation we need to be in to look at the target
			_lookRotation = Quaternion.LookRotation(_direction,Vector3.forward);
			_lookRotation.x = 0;
			_lookRotation.y = 0;
			//Mathf.Atan2((screenPos.y - transform.position.y), (screenPos.x - transform.position.x))*Mathf.Rad2Deg;
			//rotate us over time according to speed until we are in the required rotation
			_lookRotation.eulerAngles = _lookRotation.eulerAngles + (180f * Vector3.forward);
			
			transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, _playerControl.RotationSpeed);
			if (MyWeapon.Action == Weapon.ActionTypes.Manual)
			{
				if (Mathf.Abs(transform.rotation.eulerAngles.z - lastRot.eulerAngles.z) < 5f)
					_facingTarget = true;
			}
			else if (MyWeapon.Action == Weapon.ActionTypes.Automatic)
			{
				if (Mathf.Abs(transform.rotation.eulerAngles.z - lastRot.eulerAngles.z) < 5f)
					_facingTarget = true;
			}
		}

		if (_facingTarget)
		{
			_turning = false;
			_shooting = true;
		}
	}
	void Shoot(Vector3 shootPos){
		if (SeparateShootingAndMovement){
			if (_playerControl.AttackNodeList[0].Duration != 0){
				if (_shootingStart == 0)
					_shootingStart = Time.time;
			}
			if (_facingTarget || MyWeapon.GetBurstShotNumber != 0){
				if (MyWeapon.Fire(transform,shootPos))	{
					if (_playerControl.AttackNodeList[0].Duration == 0){
						_playerControl.AttackNodeList.RemoveAt(0);
						if (MyWeapon.Action == Weapon.ActionTypes.Manual)
							_canShoot = false;
						_shooting = false;
						_facingTarget = false;
						return;
					}
				}else{
					if (MyWeapon.Action == Weapon.ActionTypes.Automatic && _playerControl.AttackNodeList.Count > 0){
						if (_playerControl.AttackNodeList[0].Duration == 0){
							_playerControl.AttackNodeList.RemoveAt(0);
							_shooting = false;
							_facingTarget = false;
							return;
						}
					}
				}
			}
			if ( _playerControl.AttackNodeList.Count > 0)
			{
				if (Time.time >= _shootingStart + _playerControl.NodeList[0].Duration && _playerControl.NodeList[0].Duration != 0)
				{
					_shootingStart = 0;
					_playerControl.NodeList.RemoveAt(0);
					if (MyWeapon.Action == Weapon.ActionTypes.Manual)
						_canShoot = false;
					_shooting = false;
				}
			}
		}else{
			if (_playerControl.NodeList[0].Duration != 0){
				if (_shootingStart == 0)
					_shootingStart = Time.time;
			}
			if (_facingTarget || MyWeapon.GetBurstShotNumber != 0){
				if (MyWeapon.Fire(transform,shootPos))	{
					if (_playerControl.NodeList[0].Duration == 0){
						_playerControl.NodeList.RemoveAt(0);
						if (MyWeapon.Action == Weapon.ActionTypes.Manual)
							_canShoot = false;
						_shooting = false;
						_facingTarget = false;
						return;
					}
				}else{
					if (MyWeapon.Action == Weapon.ActionTypes.Automatic && _playerControl.NodeList.Count > 0){
						if (_playerControl.NodeList[0].Duration == 0){
							_playerControl.NodeList.RemoveAt(0);
							_shooting = false;
							_facingTarget = false;
							return;
						}
					}
				}
			}
			if ( _playerControl.NodeList.Count > 0)
			{
				if (Time.time >= _shootingStart + _playerControl.NodeList[0].Duration && _playerControl.NodeList[0].Duration != 0)
				{
					_shootingStart = 0;
					_playerControl.NodeList.RemoveAt(0);
					if (MyWeapon.Action == Weapon.ActionTypes.Manual)
						_canShoot = false;
					_shooting = false;
				}
			}
		}
	}
}
