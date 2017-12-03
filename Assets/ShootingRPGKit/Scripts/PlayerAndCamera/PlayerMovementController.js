#pragma strict
private var mainModel : GameObject;
var walkSpeed : float = 6.0;
var sprintSpeed : float = 12.0;
var canSprint : boolean = true;
private var sprint : boolean = false;
private var recover : boolean = false;
private var staminaRecover : float = 1.4;
private var useStamina : float = 0.04;
@HideInInspector
var dodging : boolean = false;

var staminaGauge : Texture2D;
var staminaBorder : Texture2D;

var maxStamina : float = 100.0;
var stamina : float = 100.0;

private var lastTime : float = 0.0;
private var recoverStamina : float = 0.0;
private var dir : Vector3 = Vector3.forward;

private var useMecanim : boolean = true;

class DodgeSetting{
	var canDodgeRoll : boolean = false;
	var staminaUse : int = 25;
	
	var dodgeForward : AnimationClip;
	var dodgeLeft : AnimationClip;
	var dodgeRight : AnimationClip;
	var dodgeBack : AnimationClip;
}
var dodgeRollSetting : DodgeSetting;

class CrouchSetting{
	var canCrouch : boolean = false;
	var crouchDeltaHeight : float = 1.0;
	var positionPlus : float = 0.1;
	var crouchingMoveSpeed : float = 4.0;
}
var crouchSetting : CrouchSetting;
@HideInInspector
var crouching : boolean = false;

private var motor : CharacterMotor;
private var controller : CharacterController;
// Use this for initialization
function Start () {
	motor = GetComponent(CharacterMotor);
	controller = GetComponent(CharacterController);
	stamina = maxStamina;
	if(!mainModel){
		mainModel = GetComponent(Status).mainModel;
	}
	useMecanim = GetComponent(GunTrigger).useMecanim;
}

// Update is called once per frame
function Update () {
	var stat : Status = GetComponent(Status);
	if(Input.GetKeyUp(KeyCode.LeftControl) && crouching && crouchSetting.canCrouch || Input.GetKeyUp("z") && crouching && crouchSetting.canCrouch){
		StopCrouching();
	}
	
	if(stat.freeze || stat.flinch){
		motor.inputMoveDirection = Vector3(0,0,0);
		return;
	}
	 if(Time.timeScale == 0.0){
     	return;
     }
     if (dodging){
		var fwd : Vector3 = transform.TransformDirection(dir);
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
			if(Input.GetButtonDown ("Vertical") && (Time.time - lastTime) < 0.4 && Input.GetButtonDown ("Vertical") && (Time.time - lastTime) > 0.1 && Input.GetAxis("Vertical") > 0.03){
				lastTime = Time.time;
				dir = Vector3.forward;
				//DodgeRoll(dodgeRollSetting.dodgeForward);
			}else
				lastTime = Time.time;
		}
		//Dodge Backward
	     if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") < 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && Input.GetAxis("Horizontal") == 0){
			if(Input.GetButtonDown ("Vertical") && (Time.time - lastTime) < 0.4 && Input.GetButtonDown ("Vertical") && (Time.time - lastTime) > 0.1 && Input.GetAxis("Vertical") < -0.03){
				lastTime = Time.time;
				dir = Vector3.back;
				//DodgeRoll(dodgeRollSetting.dodgeBack);
			}else
				lastTime = Time.time;
		}
		//Dodge Left
		 //if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") < 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && Input.GetAxis("Vertical") == 0){
	     if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") < 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && !Input.GetButton("Vertical")){
			if(Input.GetButtonDown ("Horizontal") && (Time.time - lastTime) < 0.3 && Input.GetButtonDown ("Horizontal") && (Time.time - lastTime) > 0.15 && Input.GetAxis("Horizontal") < -0.03){
				lastTime = Time.time;
				dir = Vector3.left;
				//DodgeRoll(dodgeRollSetting.dodgeLeft);
			}else
				lastTime = Time.time;
		}
		//Dodge Right
		 //if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") > 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && Input.GetAxis("Vertical") == 0){
	     if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") > 0 && (controller.collisionFlags & CollisionFlags.Below) != 0 && !Input.GetButton("Vertical")){
			if(Input.GetButtonDown ("Horizontal") && (Time.time - lastTime) < 0.3 && Input.GetButtonDown ("Horizontal") && (Time.time - lastTime) > 0.15 && Input.GetAxis("Horizontal") > 0.03){
				lastTime = Time.time;
				dir = Vector3.right;
				//DodgeRoll(dodgeRollSetting.dodgeRight);
			}else
				lastTime = Time.time;
		}
	}
	
	//Cancel Sprint
    if (sprint && Input.GetAxis("Vertical") < 0.02 || sprint && stamina <= 0 || sprint && Input.GetButtonDown("Fire1") || sprint && Input.GetKeyUp(KeyCode.LeftShift) || stat.flinch && sprint){
		sprint = false;
		recover = true;
		motor.movement.maxForwardSpeed = walkSpeed;
		motor.movement.maxSidewaysSpeed = walkSpeed;
		recoverStamina = 0.0f;
	}
	
	if(Input.GetKeyDown(KeyCode.LeftControl) && !crouching && crouchSetting.canCrouch || Input.GetKeyDown("z") && !crouching && crouchSetting.canCrouch ){
		Crouch();
	}
	
	var directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	
	if (directionVector != Vector3.zero) {
		var directionLength : float = directionVector.magnitude;
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
	}
}

function Crouch() {
	motor.movement.maxForwardSpeed = crouchSetting.crouchingMoveSpeed;
	motor.movement.maxSidewaysSpeed = crouchSetting.crouchingMoveSpeed;
	motor.movement.maxBackwardsSpeed = crouchSetting.crouchingMoveSpeed;
	transform.position.y += crouchSetting.positionPlus;
	GetComponent(CharacterController).height -= crouchSetting.crouchDeltaHeight;
	GetComponent(CharacterController).center -= Vector3(0,crouchSetting.crouchDeltaHeight/2, 0);
	crouching = true;
}

function StopCrouching(){
	crouching = false;
	motor.movement.maxForwardSpeed = walkSpeed;
	motor.movement.maxSidewaysSpeed = walkSpeed;
	motor.movement.maxBackwardsSpeed = walkSpeed;
	transform.position.y += crouchSetting.positionPlus;
	GetComponent(CharacterController).height += crouchSetting.crouchDeltaHeight;
	GetComponent(CharacterController).center += Vector3(0,crouchSetting.crouchDeltaHeight/2, 0);
}

function OnGUI () {
	if (sprint || recover || dodging){
		var staminaPercent : float = stamina * 100 / maxStamina *3;
		//GUI.DrawTexture (Rect ((Screen.width /2) -150,Screen.height - 120,stamina *3,10), staminaGauge);
		//GUI.DrawTexture (Rect ((Screen.width /2) -150,Screen.height - 120, staminaPercent ,10), staminaGauge);
		//GUI.DrawTexture (Rect ((Screen.width /2) -153,Screen.height - 123, 306 ,16), staminaBorder);
	}

}

function Dasher(){
	 while (sprint){
		yield WaitForSeconds(useStamina);
		if(stamina > 0){
			stamina -= 1;
		}else{
			stamina = 0;
		}
	 }
}

function StaminaRecovery(){
		stamina += 1;
		if(stamina >= maxStamina){
			stamina = maxStamina;
			recoverStamina = 0.0f;
			recover = false;
		}else{
			recoverStamina = staminaRecover - 0.02f;
		}
}

function DodgeRoll(anim : AnimationClip){
	if(stamina < 25 || dodging){
		return;
	}
	if(!useMecanim){
		//For Legacy Animation
		mainModel.GetComponent.<Animation>()[anim.name].layer = 18;
		mainModel.GetComponent.<Animation>().PlayQueued(anim.name, QueueMode.PlayNow);
	}else{
		//For Mecanim Animation
		GetComponent(PlayerMecanimAnimation).AttackAnimation(anim.name);
	}
	
	dodging = true;
	stamina -= dodgeRollSetting.staminaUse;
	GetComponent(Status).dodge = true;
	motor.canControl = false;
	yield WaitForSeconds(0.5);
	GetComponent(Status).dodge = false;
	recover = true;
	motor.canControl = true;
	dodging = false;
	recoverStamina = 0.0f;

}

// Require a character controller to be attached to the same game object
@script RequireComponent (CharacterMotor)
