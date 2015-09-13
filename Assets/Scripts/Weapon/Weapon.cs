using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public abstract class Weapon : MonoBehaviour {

	public enum ActionTypes
	{
		Manual,
		Automatic
	};
	public ActionTypes Action;
	public int ClipSize;			//# of projectiles that can be shot before reload
	public float MaxSpread;			//The Largest Possible spread of the projectiles
	public float MinSpread;			//The Smallest Possible spread of the projectiles
	public float Recoil; 			//Rate at which the spread increases or decreases for every shot fired in one burst
	public float SteadyRate; 		//Rate at which the spread decreases per update frame not firing
	public int BulletsPerBurst; 	//For burst, how many projectiles are shot per burst
	public float FireRate;			//how fast the projectiles are shot
	public GameObject Projectile;	//What the gun shoots
	public float ShotSpeed;
	public float MaxDamage;			//MaxDamage of the projectile
	public float ReloadTime;		//Time it takes to reload
	public bool HasSecondaryReloadTime;
	public float SecondaryReloadTime;
	public float MaxRange;			//Max Distance the projectile can travel
	public float MaxFullDamageRange;//Max distance the projectile can travel while still at max damage
	public float DamageLossRate;	//The rate at which damage is lost per distance traveled
	public PlayerControl myPlayerControl;
	protected float _spread;
	protected int _projectilesInClip;
	protected int _burstShotNumber = 0;

	public abstract bool Fire(Transform playerTransform, Vector3 targetPos);
	public abstract int Reload(int TotalAmmo);
	public void SetPlayerControl(PlayerControl playerControl)
	{
		myPlayerControl = playerControl;
	}
	public float GetReloadTime()
	{
		if (HasSecondaryReloadTime && _projectilesInClip > 0)
			return SecondaryReloadTime;
		else
			return ReloadTime;
	}
	public int GetProjectilesInClip
	{
		get {return _projectilesInClip;}
	}
	public int GetBurstShotNumber
	{
		get {return _burstShotNumber;}
	}
}
