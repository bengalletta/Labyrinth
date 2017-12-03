using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GunTrigger))]
[RequireComponent (typeof (PlayerMovementController))]
[AddComponentMenu("Shooting-RPG Kit(CS)/Create Player(Legacy)")]
public class PlayerLegacyAnimation : MonoBehaviour {
	private GameObject mainModel;
	public float runMaxAnimationSpeed = 1.0f;
	public float backMaxAnimationSpeed = 1.0f;
	
	public AnimationClip idle;
	public AnimationClip run;
	public AnimationClip right;
	public AnimationClip left;
	public AnimationClip back;
	public AnimationClip jump;
	public AnimationClip hurt;
	public AnimationClip crouchIdle;
	public AnimationClip crouchForward;
	public AnimationClip crouchRight;
	public AnimationClip crouchLeft;
	public AnimationClip crouchBack;
	
	[HideInInspector]
		public LegacyAnimSet[] weaponAnimSet = new LegacyAnimSet[2];
	[HideInInspector]
		public int weaponEquip = 0; //0 = Primary , 1 = Secondary
	
	void Awake (){
		if(!mainModel && GetComponent<Status>().mainModel){
			mainModel = GetComponent<Status>().mainModel;
		}else if(!mainModel){
			mainModel = this.gameObject;
		}
		
		mainModel.GetComponent<Animation>()[run.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[right.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[left.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[back.name].speed = backMaxAnimationSpeed;
		
		mainModel.GetComponent<Animation>()[jump.name].wrapMode  = WrapMode.ClampForever;
		
		if(hurt){
			GetComponent<Status>().hurt = hurt;
		}
		GetComponent<GunTrigger>().useMecanim = false;
		
	}
	
	void Update (){
		CharacterController controller = GetComponent<CharacterController>();
		
		if(GetComponent<PlayerMovementController>().crouching){
			//---------------------------------
			if (Input.GetAxis("Horizontal") > 0.1f)
				mainModel.GetComponent<Animation>().CrossFade(crouchRight.name);
			else if (Input.GetAxis("Horizontal") < -0.1f)
				mainModel.GetComponent<Animation>().CrossFade(crouchLeft.name);
			else if (Input.GetAxis("Vertical") > 0.1f)
				mainModel.GetComponent<Animation>().CrossFade(crouchForward.name);
			else if (Input.GetAxis("Vertical") < -0.1f)
				mainModel.GetComponent<Animation>().CrossFade(crouchBack.name);
			else
				mainModel.GetComponent<Animation>().CrossFade(crouchIdle.name);
			//---------------------------------
		}else{
			//---------------------------------
			if ((controller.collisionFlags & CollisionFlags.Below) != 0){
				if (Input.GetAxis("Horizontal") > 0.1f)
					mainModel.GetComponent<Animation>().CrossFade(right.name);
				else if (Input.GetAxis("Horizontal") < -0.1f)
					mainModel.GetComponent<Animation>().CrossFade(left.name);
				else if (Input.GetAxis("Vertical") > 0.1f)
					mainModel.GetComponent<Animation>().CrossFade(run.name);
				else if (Input.GetAxis("Vertical") < -0.1f)
					mainModel.GetComponent<Animation>().CrossFade(back.name);
				else
					mainModel.GetComponent<Animation>().CrossFade(idle.name);
			}else if(jump){
				mainModel.GetComponent<Animation>().CrossFade(jump.name);
			}
			//---------------------------------
		}
	}
	
	public void SetAnimation(){
		if(!mainModel && GetComponent<Status>().mainModel){
			mainModel = GetComponent<Status>().mainModel;
		}else if(!mainModel){
			mainModel = this.gameObject;
		}
		weaponEquip = GetComponent<GunTrigger>().weaponEquip;
		if(weaponAnimSet[weaponEquip].idle)
			idle = weaponAnimSet[weaponEquip].idle;
		if(weaponAnimSet[weaponEquip].run)
			run = weaponAnimSet[weaponEquip].run;
		if(weaponAnimSet[weaponEquip].left)
			left = weaponAnimSet[weaponEquip].left;
		if(weaponAnimSet[weaponEquip].right)
			right = weaponAnimSet[weaponEquip].right;
		if(weaponAnimSet[weaponEquip].back)
			back = weaponAnimSet[weaponEquip].back;
		if(weaponAnimSet[weaponEquip].jump)
			jump = weaponAnimSet[weaponEquip].jump;
		
		if(weaponAnimSet[weaponEquip].crouchIdle)
			crouchIdle = weaponAnimSet[weaponEquip].crouchIdle;
		if(weaponAnimSet[weaponEquip].crouchForward)
			crouchForward = weaponAnimSet[weaponEquip].crouchForward;
		if(weaponAnimSet[weaponEquip].crouchRight)
			crouchRight = weaponAnimSet[weaponEquip].crouchRight;
		if(weaponAnimSet[weaponEquip].crouchLeft)
			crouchLeft = weaponAnimSet[weaponEquip].crouchLeft;
		if(weaponAnimSet[weaponEquip].crouchBack)
			crouchBack = weaponAnimSet[weaponEquip].crouchBack;
		
		mainModel.GetComponent<Animation>()[weaponAnimSet[weaponEquip].run.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[weaponAnimSet[weaponEquip].right.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[weaponAnimSet[weaponEquip].left.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[weaponAnimSet[weaponEquip].back.name].speed = backMaxAnimationSpeed;
		
		mainModel.GetComponent<Animation>()[weaponAnimSet[weaponEquip].jump.name].wrapMode  = WrapMode.ClampForever;
		
	}
			
}
