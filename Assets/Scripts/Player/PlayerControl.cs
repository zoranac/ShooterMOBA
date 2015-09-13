using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : NetworkBehaviour {
	public List<Node> NodeList = new List<Node>();
	public List<Node> AttackNodeList = new List<Node>();
	public float RotationSpeed;
	public GameObject BulletProjectile;
	//Public Objects that should not be in the inspector
	//[HideInInspector]
	public GameObject WeaponRack;
	public Weapon MyWeapon;


	public int Ammo;
	//Class
	//Equipment
	//Etc

	void Awake () {
		if (WeaponRack == null)
			WeaponRack = GameObject.Find("WeaponRack");

		GameObject tempWeapon = (GameObject)Instantiate( WeaponRack.GetComponent<WeaponRack>().AssaultRifle);
		tempWeapon.transform.parent = gameObject.transform;
		MyWeapon = tempWeapon.GetComponent<Weapon>();
		MyWeapon.SetPlayerControl(this);
	}
	void Update()
	{
		//print(NodeList.Count);
	}
	[Command]
	public void CmdSpawnBullet(Vector3 newTarget,float ShotSpeed,float MaxDamage,float MaxRange,float MaxFullDamageRange,float DamageLossRate){

		GameObject tempProjectile = (GameObject)Instantiate(BulletProjectile,(transform.position+transform.up),transform.rotation);

		tempProjectile.GetComponent<Bullet>().SetVariables(gameObject, MaxDamage,MaxRange,MaxFullDamageRange,DamageLossRate);
//			try{
//				
//			}
//			catch(Exception e) {
//				Debug.LogError("Projectile not set to correct projectile. Full Error: " + e.ToString());
//			}
		//sets projectile velocity towards the new target
		tempProjectile.GetComponent<Rigidbody2D>().velocity = (newTarget - transform.position).normalized * ShotSpeed;
		NetworkServer.Spawn(tempProjectile);
	}
}
