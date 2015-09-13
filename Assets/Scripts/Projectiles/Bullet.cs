using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {
	//Passed Varriables
	private float MaxDamage;
	private float MaxRange;
	private float MaxFullDamageRange;
	private float DamageLossRate;
	private Transform myTransform;
	public bool piercing;							//NeedToSetUp
	[SyncVar]private float _damage;
	[SyncVar]public GameObject ShotFromObj;
	private Vector3 _startPoint;
	private float _distance;

	public void SetVariables(GameObject shotFromObj, float maxDamage, float maxRange, float maxFullDamageRange, float damageLossRate){
			MaxDamage = maxDamage;
			MaxRange = maxRange;
			MaxFullDamageRange = maxFullDamageRange;
			DamageLossRate = damageLossRate;
			ShotFromObj = shotFromObj;
	}
	public float GetDamage(){
		return _damage;
	}
	public void DestroySelf(){
		Destroy(gameObject);
	}
	// Use this for initialization
	void Start () {
		_startPoint = transform.position;
		_damage = MaxDamage;
		myTransform = transform;
	}

	// Update is called once per frame
	void FixedUpdate () {
		Debug.DrawLine((Vector2)transform.position, (Vector2)transform.position+(Vector2)GetComponent<Rigidbody2D>().velocity, Color.red);
	if (!isServer){
		return;
	}
		_distance = Vector3.Distance(_startPoint,transform.position);

		if (_distance > MaxFullDamageRange){
			_damage = MaxDamage - (_distance - MaxFullDamageRange) * DamageLossRate;
		}
		if (_damage <= 0){
			DestroySelf();
		}
		if (_distance > MaxRange){
			DestroySelf();
		}
		//TransmitVar(_damage,myTransform);

	}
	[Server]
	void TransmitVar(float damage,Transform trans,GameObject shotFromObj){
		_damage = damage;
		myTransform = trans;
		ShotFromObj = shotFromObj;
	}
}
