#pragma strict
enum PatrolType{
	RandomPatrol = 0,
	WaypointRandom = 1,
	WaypointByStep = 2
}
var movement : PatrolType = PatrolType.RandomPatrol;

var waypoints : Transform[] = new Transform[0];
var speed : float = 4.0;
private var state : int = 0; //0 = Idle , 1 = Moving , 2 = MoveToWaypoint.
private var movingAnimation : AnimationClip;
private var idleAnimation : AnimationClip;
private var mainModel : GameObject;
private var ai : AIenemy;
private var stat : Status;

var idleDuration : Vector2 = new Vector2(1.5 , 2.5);
var moveDuration : Vector2 = new Vector2(1.0 , 2.0);
var moveToPointDuration : float = 7.5; //Use when enemy hearing gun sound. It will done it's move when it cannot reach that point in time.

private var waitDuration : float = 3.0;
private var wait : float = 0;
var freeze : boolean = false;
private var headToPoint : Transform;
private var distance : float = 0.0;
private var step : int = 0;
private var useMecanim : boolean = false;
private var animator : Animator; //For Mecanim

private var moveEnough : float = 0.0;

function Start () {
	if(waypoints.Length > 1){
		for (var go : Transform in waypoints) {
			go.parent = null;
		}
	}
	stat = GetComponent(Status);
	ai = GetComponent(AIenemy);
	mainModel = stat.mainModel;
	useMecanim = ai.useMecanim;
	if(!mainModel){
		mainModel = this.gameObject;
	}
	movingAnimation = ai.movingAnimation;
	idleAnimation = ai.idleAnimation;
	
	if(!mainModel){
		mainModel = this.gameObject;
	}
	if(waypoints.Length <= 0 && movement != PatrolType.RandomPatrol){
		movement = PatrolType.RandomPatrol;
	}
	//-------Check for Mecanim Animator-----------
	if(useMecanim){
		animator = ai.animator;
		if(!animator){
			animator = mainModel.GetComponent(Animator);
		}
	}
}

function Update () {
	if(freeze || stat.freeze){
		return;
	}
	if(ai.followState == AIState.Idle){
		if(state >= 1){//Moving
			var controller : CharacterController = GetComponent(CharacterController);
			var forward : Vector3 = transform.TransformDirection(Vector3.forward);
	     	controller.Move(forward * speed * Time.deltaTime);
	     	if(movingAnimation && !useMecanim){
				//For Legacy Animation
				mainModel.GetComponent.<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else if(useMecanim){
				//For Mecanim Animation
				animator.SetBool("run" , true);
			}
	    }
	    	//----------------------------
			if(wait >= waitDuration && state == 0){
				if(movement != PatrolType.RandomPatrol){
					//Set to Moving Mode.
					if(movement == PatrolType.WaypointRandom){
						RandomWaypoint();
					}else{
						WaypointStep();
					}
				}else{
					//Set to Moving Mode.
					RandomTurning();
				}
			}
			//-------------------------------------
			if(wait >= waitDuration && state == 1){
				//Set to Idle Mode.
			     if(idleAnimation && !useMecanim){
			     	//For Legacy Animation
			     	mainModel.GetComponent.<Animation>().CrossFade(idleAnimation.name, 0.2f);
			     }else if(useMecanim){
			     	//For Mecanim Animation
					animator.SetBool("run" , false);
				}
				wait = 0;
				waitDuration = Random.Range(idleDuration.x , idleDuration.y);
				state = 0;
			}
			//----------------------------------------
			if(state == 2){
				var destination : Vector3 = headToPoint.position;
			    destination.y = transform.position.y;
			  	transform.LookAt(destination);
				
				distance = (transform.position - GetDestination()).magnitude;
				if (distance <= 0.2) {
					//Set to Idle Mode.
					if(idleAnimation && !useMecanim){
				    	//For Legacy Animation
				    	mainModel.GetComponent.<Animation>().CrossFade(idleAnimation.name, 0.2f);
				    }else if(useMecanim){
				    	//For Mecanim Animation
						animator.SetBool("run" , false);
					}
					wait = 0;
					waitDuration = Random.Range(idleDuration.x , idleDuration.y);
					state = 0;
				}else if(Time.time > moveEnough){
					//If this enemy cannot reach the waypoint in time.
					//Set to Idle Mode.
					if(idleAnimation && !useMecanim){
				    	//For Legacy Animation
				    	mainModel.GetComponent.<Animation>().CrossFade(idleAnimation.name, 0.2f);
				    }else if(useMecanim){
				    	//For Mecanim Animation
						animator.SetBool("run" , false);
					}
					wait = 0;
					waitDuration = Random.Range(idleDuration.x , idleDuration.y);
					state = 0;
					//Reset the Movement type to Random
					movement = PatrolType.RandomPatrol;
				}
			
			}
			wait += Time.deltaTime;
			//-----------------------------
	}

}

function RandomTurning(){
		var dir : float = Random.Range(0 , 360);
		transform.eulerAngles.y = dir;
		
		wait = 0; // Reset wait time.
		waitDuration = Random.Range(moveDuration.x , moveDuration.y);
		state = 1; // Change State to Move.
		moveEnough = Time.time + moveToPointDuration;

}

function RandomWaypoint(){
	headToPoint = waypoints[Random.Range(0, waypoints.Length)];
  				   
	wait = 0; // Reset wait time.
	state = 2; // Change State to Move.
	moveEnough = Time.time + moveToPointDuration;
}

function WaypointStep(){
	headToPoint = waypoints[step];
  				   
	wait = 0; // Reset wait time.
	state = 2; // Change State to Move.
	
	if(step >= waypoints.Length -1){
		step = 0;
	}else{
		step++;
	}
	moveEnough = Time.time + moveToPointDuration;
}

function GetDestination() : Vector3{
        var destination : Vector3 = headToPoint.position;
        destination.y = transform.position.y;
        return destination;
}

@script RequireComponent (AIenemy)
@script AddComponentMenu ("Shooting RPG Kit/Create Patrolling Enemy")
