using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class BurstWeapon : Weapon {

	public float DurationBetweenShotsInBurst;

	private float _lastFireTime = 0;
	private bool _firing = false;
	private int _firingTestFrame = 0;
	private bool _reloading = false;
	private bool _reloadSet = false;
	private float _reloadTime = 0;
	private int _totalAmmo = 0;
	private float _lastFireTimeInBurst = 0;
	private Transform _playerTransform;
	private Vector3 _target;
	//Variables called here for testing;
	float distance;
	float angle;
	float trueSpread;
	Vector3 newTarget;
	
	void Start () {
		_spread = MinSpread;
		_projectilesInClip = ClipSize;
	}
	void FixedUpdate(){
		if (_firingTestFrame >= 10){
			_firing = false;
		}
		if (!_firing){
			if (_spread > MinSpread)
				_spread -= SteadyRate;
			if (_spread < MinSpread)
				_spread = MinSpread;
		}
		_firingTestFrame++;
	}
	public override int Reload(int Ammo){
		if (!_reloadSet){
			_reloading = true;
			_totalAmmo = Ammo;
			_reloadTime = Time.time;
			_reloadSet = true;
		}else{
			if (HasSecondaryReloadTime && _projectilesInClip > 0){
				if (Time.time >= _reloadTime + SecondaryReloadTime){
					_reloading = false;
					_reloadSet = false;
					if (Ammo < ClipSize)
						_projectilesInClip = Ammo;
					else
						_projectilesInClip = ClipSize;
					
					int returnInt = Ammo - ClipSize;
					
					if (returnInt < 0 )
						returnInt = 0;
					
					_totalAmmo = returnInt;
					
					return (returnInt);
				}
			}else if (Time.time >= _reloadTime + ReloadTime){
				_reloading = false;
				_reloadSet = false;
				if (Ammo < ClipSize)
					_projectilesInClip = Ammo;
				else
					_projectilesInClip = ClipSize;
				
				int returnInt = Ammo - ClipSize;
				
				if (returnInt < 0 )
					returnInt = 0;
				
				_totalAmmo = returnInt;
				
				return (returnInt);
			}
		}
		return -1;
	}
	public override bool Fire(Transform playerTransform, Vector3 targetPos) {
		//For Testing if the gun has stopped shooting after X frames
		_firing = true;
		_firingTestFrame = 0;
		
		_target = targetPos;
		_playerTransform = playerTransform;
		
		if (Time.time >= _lastFireTime + FireRate && _projectilesInClip > 0 && !_reloading){


			if (Time.time >= _lastFireTimeInBurst + DurationBetweenShotsInBurst){
				if (FireNextInBurst())
					return true;
			}
		}
		return false;
	}
	private bool FireNextInBurst(){
		_lastFireTimeInBurst = Time.time;

		//Finds distance from player to target
		distance = ((_target - _playerTransform.position).magnitude);
		
		//The Angle that is between the player and the target
		angle =  (Mathf.Atan2(_playerTransform.position.y - _target.y, _playerTransform.position.x - _target.x) * Mathf.Rad2Deg) + 180;
		
		//sets the angle away from the target based on the spread
		trueSpread = angle + UnityEngine.Random.Range(-_spread,_spread);
		//sets a new target based on that
		newTarget = _target + new Vector3(distance * Mathf.Cos(trueSpread * Mathf.Deg2Rad),distance * Mathf.Sin(trueSpread * Mathf.Deg2Rad));

		//Creates Projectile
		myPlayerControl.CmdSpawnBullet(newTarget,ShotSpeed,MaxDamage,MaxRange,MaxFullDamageRange,DamageLossRate);

		Debug.DrawLine(_playerTransform.position, newTarget, Color.red);
		Debug.DrawLine(_playerTransform.position, _target, Color.blue);
		
		_burstShotNumber++;
		
		//changes spread based on recoil
		if (_spread < MaxSpread)
			_spread += Recoil;
		if (_spread > MaxSpread)
			_spread = MaxSpread;
		
		//lowers the number of projectiles in the clip
		_projectilesInClip--;
		
		if (_burstShotNumber >= BulletsPerBurst){
			_burstShotNumber = 0;
			//Sets when the gun was shot last
			_lastFireTime = Time.time;
			return true;
		}
		else
			return false;
	}
}

