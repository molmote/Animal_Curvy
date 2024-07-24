using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGemEntity : GameEntity 
{
	public 	TileEntity 		tile;
	public  float 			radius;
	public  Transform 		myTransform; 	

	public  bool 			isPassed;
	public  bool 			lockedOn;

	void Update()
	{
		if ( false == this.isAlive )
			return;

		if ( false == GameSystemInfo.instance.IsMagneticForce() )
			return;

		float magnetic = _SETTING._.GetMagneticDistance();
		
		Transform player = GamePlayerEntity.instance.GetTransform();
		float dist = Vector3.Distance ( player.position, this.myTransform.position);
		
		// MyLogger.Red( "On Gem Update", string.Format("dist{0}, magnetic{1}", dist, magnetic ) );
		if ( dist < magnetic || this.lockedOn )
		{
			this.lockedOn = true;
			if ( dist < _SETTING._.magneticMinDistance )
			{
				MyLogger.Log("GameGemEntity-Magnetic", "magneticMinDistance : "+dist);
				GamePlayerEntity.instance.OnEatingColliderEnter(this.gameObject);
			}
			else
			{
				float max   = _SETTING._.magneticSpeed.max;
				if ( (GamePlayerEntity.instance.verticalSpeed * 1.5f)> max )
					max = GamePlayerEntity.instance.GetVelocity();

				float speed = Mathf.Lerp(	max, _SETTING._.magneticSpeed.min,
											dist / magnetic );

				if ( this.myTransform.position.y > player.position.y )
				{
					speed = Mathf.Lerp( _SETTING._.magneticSpeed.max, max, dist / magnetic );
				}
				this.MoveToward( GamePlayerEntity.instance.GetTransform().position, 
								 speed * Time.deltaTime );
			}
		}
	}

	private void MoveToward(Vector3 target, float dist)
	{
		this.transform.position = Vector3.MoveTowards(this.transform.position, target, dist);
	}
}
