using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GunTrigger))]
[RequireComponent (typeof (PlayerMovementController))]
[AddComponentMenu("Shooting-RPG Kit(CS)/Create Player(Mecanim)")]
public class PlayerMecanimAnimation : MonoBehaviour {
	private GameObject player;
	private GameObject mainModel;
	public Animator animator;
	private CharacterController controller;

	public string moveHorizontalState = "horizontal";
	public string moveVerticalState = "vertical";
	public string jumpState = "jump";
	public string dodgeState = "dodge";
	public string hurtState = "hurt";
	public string crouchState = "crouch";
	public int upperBodyLayer = 1;
	private bool jumping = false;
	private bool dodging = false;
	private bool attacking = false;
	private bool flinch = false;
	private bool crouching = false;
	
	public int primaryWeaponType = 1;
	public int secondaryWeaponType = 2;
	
	void Start(){
		if(!player){
			player = this.gameObject;
		}
		mainModel = GetComponent<Status>().mainModel;
		if(!mainModel && GetComponent<Status>().mainModel){
			mainModel = GetComponent<Status>().mainModel;
		}else if(!mainModel){
			mainModel = this.gameObject;
		}
		if(!animator){
			animator = mainModel.GetComponent<Animator>();
		}
		controller = player.GetComponent<CharacterController>();
		
	}
	
	void Update(){
		//Set attacking variable = isCasting in GunTrigger
		attacking = GetComponent<GunTrigger>().attacking;
		flinch = GetComponent<Status>().flinch;
		dodging = GetComponent<PlayerMovementController>().dodging;
		crouching = GetComponent<PlayerMovementController>().crouching;
		//Set Hurt Animation
		animator.SetBool(hurtState , flinch);
		animator.SetBool(dodgeState , dodging);
		animator.SetBool(crouchState , crouching);
		
		if(attacking || flinch || GetComponent<Status>().freeze){
			return;
		}
		
		if ((controller.collisionFlags & CollisionFlags.Below) != 0){
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");
			
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
	
	public void AttackAnimation (string anim){
		animator.SetBool(jumpState , false);
		animator.Play(anim);
	}
	
	public void PlayAnim(string anim){
		animator.Play(anim);
	}
	
	public void SetAnimation(){
		int weaponEquip = GetComponent<GunTrigger>().weaponEquip;
		if(weaponEquip == 0){
			animator.SetInteger("weaponType" , primaryWeaponType);
		}else{
			animator.SetInteger("weaponType" , secondaryWeaponType);
		}
	}
	
	public IEnumerator SetIdle (){
		animator.SetBool("idle" , true);
		yield return new WaitForSeconds(0.02f);
		animator.SetBool("idle" , false);
	}
			
}
