#pragma strict
var speed : float = 20;
var relativeDirection = Vector3.forward;
var duration : float = 1.0;
private var hitEffect : GameObject;
private var bomb : boolean;

function Start () {
	GetComponent(Rigidbody).isKinematic = true;
	hitEffect = GetComponent(BulletStatus).hitEffect;
	bomb = GetComponent(BulletStatus).bombHitSetting.enable;
	GetComponent.<Collider>().isTrigger = true;
	Destroy();
}


function Update () {
    transform.Translate(relativeDirection * speed * Time.deltaTime);
}

function Destroy(){
	Destroy (gameObject, duration);

}


function OnTriggerEnter (other : Collider) {

  if (other.gameObject.tag == "Wall") {
		if(hitEffect){
			Instantiate(hitEffect, transform.position , transform.rotation);
		}
		if(bomb){
			GetComponent(BulletStatus).ExplosionDamage();
		}
    	Destroy (gameObject);
    	
	}
}

@script RequireComponent(BulletStatus)
@script RequireComponent(Rigidbody)

@script AddComponentMenu ("Shooting RPG Kit/Create Bullet")

