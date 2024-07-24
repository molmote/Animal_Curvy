using UnityEngine;
using System.Collections;

public class BaseLifeSpanObject : GameEntity {

	//[SerializeField]
	public float  						lifeSpan; // skill duration??
	protected float 					elapsedSinceLaunched;
	[SerializeField] 		  protected ParticleSystem 			particle;


	// Use this for initialization
	void Start () 
	{
		this.elapsedSinceLaunched = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{		
		if( GameInfo.instance.IsGamePaused() )
		{
			return;
		}

		this.elapsedSinceLaunched += Time.deltaTime;	

		if(this.elapsedSinceLaunched >= this.lifeSpan || isAlive == false)
		{
			this.DestroySelf();
		}
	}

	public override void Pause()
	{
		if ( this.particle )
			this.particle.Pause();
		//this.particle.enableEmission = false;
	}

	public override void Resume()
	{
		if ( this.particle )
		{
			this.particle.Play();
			this.particle.enableEmission = true;
		}
	}

	public virtual void Initialize()//bool isParticle = true)
	{
		this.gameObject.SetActive(true);
		this.elapsedSinceLaunched 	= 0;
		this.isAlive 				= true;

		//Transform t = this.transform.Find("particle");
		//if ( t != null )
		//	this.particle = t.GetComponent<ParticleSystem>();					
	}
	
	public float GetLifeSpan()
	{
		return this.lifeSpan;
	}
}
