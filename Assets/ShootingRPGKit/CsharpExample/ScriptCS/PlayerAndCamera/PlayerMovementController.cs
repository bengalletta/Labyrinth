using UnityEngine;
using System.Collections;

public class PlayerMovementController : MonoBehaviour {
	private GameObject mainModel;
	public float walkSpeed = 6.0f;
	public float sprintSpeed = 12.0f;
	public bool canSprint = true;
	private bool sprint = false;
	private bool recover = false;
	private float staminaRecover = 1.4f;
	private float useStamina = 0.04f;
	[HideInInspector]
		public bool dodging = false;
	
	public Texture2D staminaGauge;
	public Texture2D staminaBorder;
	
	public float maxStamina = 100.0f;
	public float stamina = 100.0f;
	
	private float lastTime = 0.0f;
	private float recoverStamina = 0.0f;
	private Vector3 dir = Vector3.forward;
	
	private bool useMecanim = true;
	public DodgeSetting dodgeRollSetting;
	
	public CrouchSetting crouchSetting;
	[HideInInspector]
	public bool crouching = false;
	
	private CharacterMotorC motor;
	private CharacterController controller;
	// Use this for initialization
	void  Start (){
		motor = GetComponent<CharacterMotorC>();
		controller = GetComponent<CharacterController>();
		stamina = maxStamina;
		if(!mainModel){
			mainModel = GetComponent<Status>().mainModel;
		}
		useMecanim = GetComponent<GunTrigger>().useMecanim;
	}
	
	// Update is called once per frame
	void  Update (){
		Status stat = GetComponent<Status>();
		if(Input.GetKeyUp(KeyCode.LeftControl) && crouching && crouchSetting.canCrouch || Input.GetKeyUp("z") && crouching && crouchSetting.canCrouch){
			StopCrouching();
		}
		
		if(stat.freeze || stat.flinch){
			motor.inputMoveDirection = Vector3.zero;
			return;
		}
		if(Time.timeScale == 0.0f){
			return;
		}
		if (dodging){
			Vector3 fwd = transform.TransformDirection(dir);
			controller.Move(fwd * 8 * Time.deltaTime);
			return;
		}
		if(recover && !sprint && !dodging){
			if(recoverStamina >= staminaRecover){
				StaminaRecovery();
			}else{
				recoverStamina += Time.deltaTime;
			}
		}
		
		if(dodgeRollSetting.canDodgeRoll){
			//Dodge Forward
			if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && Input.GetAxis("Horizontal") == 0){
				if(Input.GetButtonDown ("Vertical") && (Time.time - lastTime) < 0.4f && Input.GetButtonDown ("Vertical") && (Time.time - lastTime) > 0.1f && Input.GetAxis("Vertical") > 0.03f){
					lastTime = Time.time;
					dir = Vector3.forward;
					//DodgeRoll(dodgeRollSetting.dodgeForward);
					StartCoroutine(DodgeRoll(dodgeRollSetting.dodgeForward));
				}else
					lastTime = Time.time;
			}
			//Dodge Backward
			if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") < 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && Input.GetAxis("Horizontal") == 0){
				if(Input.GetButtonDown ("Vertical") && (Time.time - lastTime) < 0.4f && Input.GetButtonDown ("Vertical") && (Time.time - lastTime) > 0.1f && Input.GetAxis("Vertical") < -0.03f){
					lastTime = Time.time;
					dir = Vector3.back;
					//DodgeRoll(dodgeRollSetting.dodgeBack);
					StartCoroutine(DodgeRoll(dodgeRollSetting.dodgeBack));
				}else
					lastTime = Time.time;
			}
			//Dodge Left
			//if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") < 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && Input.GetAxis("Vertical") == 0){
			if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") < 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && !Input.GetButton("Vertical")){
				if(Input.GetButtonDown ("Horizontal") && (Time.time - lastTime) < 0.3f && Input.GetButtonDown ("Horizontal") && (Time.time - lastTime) > 0.15f && Input.GetAxis("Horizontal") < -0.03f){
					lastTime = Time.time;
					dir = Vector3.left;
					//DodgeRoll(dodgeRollSetting.dodgeLeft);
					StartCoroutine(DodgeRoll(dodgeRollSetting.dodgeLeft));
				}else
					lastTime = Time.time;
			}
			//Dodge Right
			//if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") > 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && Input.GetAxis("Vertical") == 0){
			if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") > 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && !Input.GetButton("Vertical")){
				if(Input.GetButtonDown ("Horizontal") && (Time.time - lastTime) < 0.3f && Input.GetButtonDown ("Horizontal") && (Time.time - lastTime) > 0.15f && Input.GetAxis("Horizontal") > 0.03f){
					lastTime = Time.time;
					dir = Vector3.right;
					//DodgeRoll(dodgeRollSetting.dodgeRight);
					StartCoroutine(DodgeRoll(dodgeRollSetting.dodgeRight));
				}else
					lastTime = Time.time;
			}
		}
		
		//Cancel Sprint
		if (sprint && Input.GetAxis("Vertical") < 0.02f || sprint && stamina <= 0 || sprint && Input.GetButtonDown("Fire1") || sprint && Input.GetKeyUp(KeyCode.LeftShift) || stat.flinch && sprint){
			sprint = false;
			recover = true;
			motor.movement.maxForwardSpeed = walkSpeed;
			motor.movement.maxSidewaysSpeed = walkSpeed;
			recoverStamina = 0.0f;
		}
		
		if(Input.GetKeyDown(KeyCode.LeftControl) && !crouching && crouchSetting.canCrouch || Input.GetKeyDown("z") && !crouching && crouchSetting.canCrouch ){
			Crouch();
		}
		
		Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		
		if (directionVector != Vector3.zero) {
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			directionLength = Mathf.Min(1, directionLength);
			
			directionLength = directionLength * directionLength;
			directionVector = directionVector * directionLength;
		}
		
		// Apply the direction to the CharacterMotor
		motor.inputMoveDirection = transform.rotation * directionVector;
		motor.inputJump = Input.GetButton("Jump");
		
		if (sprint){
			if(crouching){
				StopCrouching();
			}
			motor.movement.maxForwardSpeed = sprintSpeed;
			motor.movement.maxSidewaysSpeed = sprintSpeed;
			return;
		}
		//Activate Sprint
		if(Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && canSprint && stamina > 0){
			sprint = true;
			//Dasher();
			StartCoroutine(Dasher());
		}
	}
	
	void  Crouch (){
		motor.movement.maxForwardSpeed = crouchSetting.crouchingMoveSpeed;
		motor.movement.maxSidewaysSpeed = crouchSetting.crouchingMoveSpeed;
		motor.movement.maxBackwardsSpeed = crouchSetting.crouchingMoveSpeed;
		//transform.position.y += crouchSetting.positionPlus;
		transform.position = new Vector3(transform.position.x , transform.position.y + crouchSetting.positionPlus , transform.position.z);
		GetComponent<CharacterController>().height -= crouchSetting.crouchDeltaHeight;
		GetComponent<CharacterController>().center -= new Vector3(0,crouchSetting.crouchDeltaHeight/2, 0);
		crouching = true;
	}
	
	void  StopCrouching (){
		crouching = false;
		motor.movement.maxForwardSpeed = walkSpeed;
		motor.movement.maxSidewaysSpeed = walkSpeed;
		motor.movement.maxBackwardsSpeed = walkSpeed;
		//transform.position.y += crouchSetting.positionPlus;
		transform.position = new Vector3(transform.position.x , transform.position.y + crouchSetting.positionPlus , transform.position.z);
		GetComponent<CharacterController>().height += crouchSetting.crouchDeltaHeight;
		GetComponent<CharacterController>().center += new Vector3(0,crouchSetting.crouchDeltaHeight/2, 0);
	}
	
	void  OnGUI (){
		if (sprint || recover || dodging){
			float staminaPercent = stamina * 100 / maxStamina *3;
			//GUI.DrawTexture ( new Rect((Screen.width /2) -150,Screen.height - 120,stamina *3,10), staminaGauge);
			GUI.DrawTexture ( new Rect((Screen.width /2) -150,Screen.height - 120, staminaPercent ,10), staminaGauge);
			GUI.DrawTexture ( new Rect((Screen.width /2) -153,Screen.height - 123, 306 ,16), staminaBorder);
		}
		
	}
	
	public IEnumerator Dasher(){
		while (sprint){
			yield return new WaitForSeconds(useStamina);
			if(stamina > 0){
				stamina -= 1;
			}else{
				stamina = 0;
			}
		}
	}
	
	void StaminaRecovery (){
		stamina += 1;
		if(stamina >= maxStamina){
			stamina = maxStamina;
			recoverStamina = 0.0f;
			recover = false;
		}else{
			recoverStamina = staminaRecover - 0.02f;
		}
	}
	
	public IEnumerator DodgeRoll (AnimationClip anim){
		if(stamina < 25 || dodging){
			yield break;
		}
		if(!useMecanim){
			//For Legacy Animation
			mainModel.GetComponent<Animation>()[anim.name].layer = 18;
			mainModel.GetComponent<Animation>().PlayQueued(anim.name, QueueMode.PlayNow);
		}else{
			//For Mecanim Animation
			GetComponent<PlayerMecanimAnimation>().AttackAnimation(anim.name);
		}
		
		dodging = true;
		stamina -= dodgeRollSetting.staminaUse;
		GetComponent<Status>().dodge = true;
		motor.canControl = false;
		yield return new WaitForSeconds(0.5f);
		GetComponent<Status>().dodge = false;
		recover = true;
		motor.canControl = true;
		dodging = false;
		recoverStamina = 0.0f;
		
	}
	
	// Require a character controller to be attached to the same game object
	//@script RequireComponent (CharacterMotor)
		
}
[System.Serializable]
public class DodgeSetting{
	public bool  canDodgeRoll = false;
	public int staminaUse = 25;
	public AnimationClip dodgeForward;
	public AnimationClip dodgeLeft;
	public AnimationClip dodgeRight;
	public AnimationClip dodgeBack;
}
[System.Serializable]
public class CrouchSetting{
	public bool  canCrouch = false;
	public float crouchDeltaHeight = 1.0f;
	public float positionPlus = 0.1f;
	public float crouchingMoveSpeed = 4.0f;
}