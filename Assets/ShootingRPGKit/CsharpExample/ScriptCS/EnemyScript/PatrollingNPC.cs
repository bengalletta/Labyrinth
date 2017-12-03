using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AIenemy))]
[AddComponentMenu("Shooting-RPG Kit(CS)/Create Patrolling Enemy")]
public class PatrollingNPC : MonoBehaviour {
	public PatrolType movement = PatrolType.RandomPatrol;
	
	public Transform[] waypoints = new Transform[0];
	public float speed = 4.0f;
	private int state = 0; //0 = Idle , 1 = Moving , 2 = MoveToWaypoint.
	private AnimationClip movingAnimation;
	private AnimationClip idleAnimation;
	private GameObject mainModel;
	private AIenemy ai;
	private Status stat;
	
	public Vector2 idleDuration = new Vector2(1.5f , 2.5f);
	public Vector2 moveDuration = new Vector2(1.0f , 2.0f);
	public float moveToPointDuration = 7.5f; //Use when enemy hearing gun sound. It will done it's move when it cannot reach that point in time.
	
	private float waitDuration = 3.0f;
	private float wait = 0;
	public bool freeze = false;
	private Transform headToPoint;
	private float distance = 0.0f;
	private int step = 0;
	private bool useMecanim = false;
	private Animator animator; //For Mecanim
	
	private float moveEnough = 0.0f;
	
	void  Start (){
		if(waypoints.Length > 1){
			foreach(Transform go in waypoints) {
				go.parent = null;
			}
		}
		stat = GetComponent<Status>();
		ai = GetComponent<AIenemy>();
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
				animator = mainModel.GetComponent<Animator>();
			}
		}
	}
	
	void  Update (){
		if(freeze || stat.freeze){
			return;
		}
		if(ai.followState == AIState.Idle){
			if(state >= 1){//Moving
				CharacterController controller = GetComponent<CharacterController>();
				Vector3 forward = transform.TransformDirection(Vector3.forward);
				controller.Move(forward * speed * Time.deltaTime);
				if(movingAnimation && !useMecanim){
					//For Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
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
					mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);
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
				Vector3 destination = headToPoint.position;
				destination.y = transform.position.y;
				transform.LookAt(destination);
				
				distance = (transform.position - GetDestination()).magnitude;
				if (distance <= 0.2f) {
					//Set to Idle Mode.
					if(idleAnimation && !useMecanim){
						//For Legacy Animation
						mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);
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
						mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);
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
	
	void RandomTurning (){
		float dir = Random.Range(0 , 360);
		//transform.eulerAngles.y = dir;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x , dir , transform.eulerAngles.z);
		
		wait = 0; // Reset wait time.
		waitDuration = Random.Range(moveDuration.x , moveDuration.y);
		state = 1; // Change State to Move.
		moveEnough = Time.time + moveToPointDuration;
		
	}
	
	void RandomWaypoint (){
		headToPoint = waypoints[Random.Range(0, waypoints.Length)];
		
		wait = 0; // Reset wait time.
		state = 2; // Change State to Move.
		moveEnough = Time.time + moveToPointDuration;
	}
	
	void WaypointStep (){
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
	
	Vector3 GetDestination (){
		Vector3 destination = headToPoint.position;
		destination.y = transform.position.y;
		return destination;
	}
			
}

public enum PatrolType{
	RandomPatrol = 0,
	WaypointRandom = 1,
	WaypointByStep = 2
}