using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour {
	[SyncVar] private float health;
	public float MaxHealth;
	private Text healthText;
	private float lastHealth;
	// Use this for initialization
	void Start () {
		healthText = GameObject.Find("HealthText").GetComponent<Text>();
		health = MaxHealth;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		TransmitPosition();
		SetHealthText();
	}
	void SetHealthText(){
		healthText.text = health.ToString();
	}
	[Command]
	void CmdSendHealthToServer(float Health){
		health = Health;
	}
	[Command]
	void CmdTellServerWhoWasDamaged(string uniqueID, float dmg){
		GameObject go = GameObject.Find(uniqueID);
		go.GetComponent<PlayerHealth>().CalculateDamage(dmg);
	}
	[ClientCallback]
	void TransmitPosition(){
		if (isLocalPlayer && lastHealth != health){
			if (health < 0)
				health = 0;
			else if (health > MaxHealth)
				health = MaxHealth;
			lastHealth = health;
			CmdSendHealthToServer(lastHealth);
			//print("POS");
		}
	}
	void CalculateDamage(Bullet bullet){

	}
	void CalculateDamage(float damage){
		health -= damage;
		if (health < 0)
			health = 0;
		else if (health > MaxHealth)
			health = MaxHealth;
	}

	void OnTriggerEnter2D(Collider2D otherObj){
		if (otherObj.tag == "Bullet"){
			if (otherObj.GetComponent<Bullet>().ShotFromObj != gameObject){
				CmdTellServerWhoWasDamaged(transform.name,otherObj.GetComponent<Bullet>().GetDamage());
				otherObj.GetComponent<Bullet>().DestroySelf();
			}
		}
	}
}
