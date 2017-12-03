#pragma strict
private var player : GameObject;
private var mainModel : GameObject;
var animator : Animator;
private var controller : CharacterController;

var moveHorizontalState : String = "horizontal";
var moveVerticalState : String = "vertical";
var jumpState : String = "jump";
var dodgeState : String = "dodge";
var hurtState : String = "hurt";
var crouchState : String = "crouch";
var upperBodyLayer : int = 1;
private var jumping : boolean = false;
private var dodging : boolean = false;
private var attacking : boolean = false;
private var flinch : boolean = false;
private var crouching : boolean = false;

var primaryWeaponType : int = 1;
var secondaryWeaponType : int = 2;

function Start () {
	if(!player){
		player = this.gameObject;
	}
	mainModel = GetComponent(Status).mainModel;
	if(!mainModel && GetComponent(Status).mainModel){
		mainModel = GetComponent(Status).mainModel;
	}else if(!mainModel){
		mainModel = this.gameObject;
	}
	if(!animator){
		animator = mainModel.GetComponent(Animator);
	}
	controller = player.GetComponent(CharacterController);

}

function Update () {
	//Set attacking variable = isCasting in GunTrigger
	attacking = GetComponent(GunTrigger).attacking;
	flinch = GetComponent(Status).flinch;
	dodging = GetComponent(PlayerMovementController).dodging;
	crouching = GetComponent(PlayerMovementController).crouching;
	//Set Hurt Animation
	animator.SetBool(hurtState , flinch);
	animator.SetBool(dodgeState , dodging);
	animator.SetBool(crouchState , crouching);

	if(attacking || flinch || GetComponent(Status).freeze){
		return;
	}
	
	if ((controller.collisionFlags & CollisionFlags.Below) != 0){
		var h : float = Input.GetAxis("Horizontal");
		var v : float = Input.GetAxis("Vertical");

		animator.SetFloat(moveHorizontalState , h);
		animator.SetFloat(moveVerticalState , v);
		if(jumping){
			jumping = false;
			animator.SetBool(jumpState , jumping);
			//animator.StopPlayback(jumpState);
		}
        
	}else{
		jumping = true;
		animator.SetBool(jumpState , jumping);
		//animator.Play(jumpState);
	}

}

function AttackAnimation(anim : String){
	animator.SetBool(jumpState , false);
	animator.Play(anim);
}

function PlayAnim(anim : String){
	animator.Play(anim);
}

function SetAnimation(){
	var weaponEquip : int = GetComponent(GunTrigger).weaponEquip;
	if(weaponEquip == 0){
		animator.SetInteger("weaponType" , primaryWeaponType);
	}else{
		animator.SetInteger("weaponType" , secondaryWeaponType);
	}
}

function SetIdle(){
	animator.SetBool("idle" , true);
	yield WaitForSeconds(0.02);
	animator.SetBool("idle" , false);
}

@script RequireComponent(GunTrigger)
@script RequireComponent(PlayerMovementController)

@script AddComponentMenu ("Shooting RPG Kit/Create Player(Mecanim)")
