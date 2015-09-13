using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {

	public float MoveSpeed;
	public int MoveButtonInt;
	//values for internal use
	private PlayerControl _playerControl;
	private bool _facingTarget = false;
	private Quaternion _lookRotation;
	private Vector3 _direction = Vector3.zero;

	// Use this for initialization
	void Start () {
		if (MoveSpeed > 1)
			MoveSpeed = 1;

		_playerControl = GetComponent<PlayerControl>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(MoveButtonInt))
		{
			//GameObject tempNode = (GameObject)Instantiate(Node,Camera.main.ScreenToWorldPoint(Input.mousePosition),new Quaternion(0,0,0,0));
			_facingTarget = false;

			if (Input.GetKey(KeyCode.LeftShift)){
				AddNode();
			}
			else if (gameObject.GetComponent<PlayerShooting>().Shooting()){
				if (_playerControl.NodeList.Count >= 2)
					_playerControl.NodeList.RemoveAt(1);
				_playerControl.NodeList.Insert(1,new Node(Node.NodeTypes.Move, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
			}
			else
			{
				if (_playerControl.NodeList.Count > 0){
					_playerControl.NodeList.Clear();
					_direction = Vector3.zero;
					//GetComponent<Rigidbody2D>().velocity = Vector3.zero;
				}

				AddNode();
			}

		}
		if (_playerControl.NodeList.Count > 0 && _playerControl.NodeList[0].NodeType == Node.NodeTypes.Move)
		{
			Move(_playerControl.NodeList[0].Target);
		}
	}
	void AddNode()
	{
		_playerControl.NodeList.Add(new Node(Node.NodeTypes.Move, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
	}
	void Move(Vector3 nodePos)
		//i love michael a lot!!
	{
		//if (direction == Vector3.zero)
		//{
		//	GetComponent<Rigidbody2D>().velocity = (nodePos - transform.position).normalized*speed;
		//}
		//find the vector pointing from our position to the target
		_direction = (nodePos - transform.position).normalized;
		bool CloseEnoughToMove = false;
		if (!_facingTarget)
		{
			Quaternion lastRot = transform.rotation;
			//create the rotation we need to be in to look at the target
			_lookRotation = Quaternion.LookRotation(_direction,Vector3.forward);
			_lookRotation.x = 0;
			_lookRotation.y = 0;
			//Mathf.Atan2((screenPos.y - transform.position.y), (screenPos.x - transform.position.x))*Mathf.Rad2Deg;
			//rotate us over time according to speed until we are in the required rotation
			///////?????////
			_lookRotation.eulerAngles = _lookRotation.eulerAngles + (180f * Vector3.forward);
	
			transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation,_playerControl.RotationSpeed);

			if (Mathf.Abs(transform.rotation.eulerAngles.z - lastRot.eulerAngles.z) < 1f)
				_facingTarget = true;

			if (Mathf.Abs(transform.rotation.eulerAngles.z - lastRot.eulerAngles.z) < 10f)
				CloseEnoughToMove = true;
		}

		if (_facingTarget || CloseEnoughToMove)
		{

			transform.position = Vector2.MoveTowards(transform.position,nodePos,MoveSpeed);

			if ((int)(transform.position.x * 10) == (int)(nodePos.x * 10) && (int)(transform.position.y * 10) == (int)(nodePos.y * 10)){
				_playerControl.NodeList.RemoveAt(0);
				_direction = Vector3.zero;
				_facingTarget = false;
				//GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			}
		}
	}
}
