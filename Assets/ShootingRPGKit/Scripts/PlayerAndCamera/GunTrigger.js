#pragma strict
private var mainModel : GameObject;
var useMecanim : boolean = true;
var attackPoint : Transform;

var mainCamPrefab : GameObject;
private var nextFire : float = 0.0;
private var reloading : boolean = false;
private var meleefwd : boolean = false;
var aimIcon : Texture2D;
private var zoomIcon : Texture2D;
private var stat : Status;
@HideInInspector
var weaponEquip : int = 0;

@HideInInspector
var attacking : boolean = false;
@HideInInspector
var c : int = 0;

@HideInInspector
var mainCam : GameObject;

private var str : int = 0;
private var matk : int = 0;

var weaponPosition : Transform; //Position of Your Weapon

var primaryWeaponModel : GameObject; //Assign Model of your Primary Weapon in your Main Model.
var secondaryWeaponModel : GameObject; //Assign Model of your Secondary Weapon in your Main Model.
var meleeWeaponModel : GameObject; //Assign Model of your Primary Weapon in your Main Model.

class WeaponSet{
	var bulletPrefab : GameObject;
	var weaponAtkPoint : Transform;
	var shootAnimation : AnimationClip;
	var shootAnimationSpeed : float = 1.0f;
	var animateWhileMoving : boolean = false; //Mark on If you want to play Shooting Animation while Moving.
	var reloadAnimation : AnimationClip;
	var gunFireEffect : GameObject;
	var gunSound : AudioClip;
	var soundRadius : float = 0; // Can attract the enemy to gun fire position.
	var reloadSound : AudioClip;
	var ammo : int = 30;
	var maxAmmo : int = 30;
	var useAmmo : AmmoType = AmmoType.HandgunAmmo;
	var attackDelay : float = 0.15;
	var equipAnimation : AnimationClip;
	var cameraShakeValue : float = 0;
	var automatic : boolean = true;
	var aimIcon : Texture2D;
	var zoomIcon : Texture2D;
	var zoomLevel : float = 30.0;
}
var primaryWeapon : WeaponSet;
var secondaryWeapon : WeaponSet;

class MeleeSet{
	var canMelee : boolean = false;
	var meleePrefab : GameObject;
	var meleeAnimation : AnimationClip[] = new AnimationClip[3];
	var meleeAnimationSpeed : float = 1.0f;
	var meleeCast : float = 0.15;
	var meleeDelay : float = 0.15;
	var meleeSound : AudioClip;
}
var meleeAttack : MeleeSet;

class AllAmmo {
	var handgunAmmo : int = 0;
	var machinegunAmmo : int = 0;
	var shotgunAmmo : int = 0;
	var magnumAmmo : int = 0;
	var smgAmmo : int = 0;
	var sniperRifleAmmo : int = 0;
	var grenadeRounds : int = 0;
}
var allAmmo : AllAmmo;

class SkilAtk {
	var icon : Texture2D;
	var skillPrefab : Transform;
	var skillAnimation : AnimationClip;
	var skillAnimationSpeed : float = 1.0f;
	var castTime : float = 0.3;
	var skillDelay : float = 0.3;
	var manaCost : int = 10;
}
var skill : SkilAtk[] = new SkilAtk[4];

class AtkSound{
	var attackComboVoice : AudioClip[] = new AudioClip[3];
	var magicCastVoice : AudioClip;
	var hurtVoice : AudioClip;
}
var sound : AtkSound;
var statusFont : GUIStyle;
var ammoFont : GUIStyle;

var hpBar : Texture2D;
var mpBar : Texture2D;
var shieldBar : Texture2D;
var expBar : Texture2D;
var backGroundBar : Texture2D;

private var zoomLevel : float = 30.0;
private var onAiming : boolean = false;
private var automatic : boolean = true;
private var freeze : boolean = false;
private var spareAmmo : int = 30;

//Icon for Buffs
var braveIcon : Texture2D;
var barrierIcon : Texture2D;
var faithIcon : Texture2D;
var magicBarrierIcon : Texture2D;
var sharpIcon : Texture2D;

var useLegacyUi : boolean = false;

function Start(){
	gameObject.tag = "Player";
	if(!attackPoint){
		var n : GameObject = new GameObject();
		n.transform.parent = this.transform;
		attackPoint = n.transform;
	}
	stat = GetComponent(Status);
	
	if(!mainModel){
		mainModel = stat.mainModel;
	}
	
	stat.useMecanim = useMecanim;
	stat.CalculateStatus();
	
	var oldcam : GameObject[] = GameObject.FindGameObjectsWithTag("MainCamera");
	for(var o : GameObject in oldcam){
		Destroy(o);
	}
	var newCam : GameObject = Instantiate(mainCamPrefab, transform.position , transform.rotation);
    mainCam = newCam;
    
    if(sound.hurtVoice){
		stat.hurtVoice = sound.hurtVoice;
	}
	SettingWeapon();

}

function Update(){
	//Release Zoom
    if(onAiming && Input.GetButtonUp("Fire2")){
       	GetComponent.<Camera>().main.fieldOfView = 60;
       	onAiming = false;
    }
	if(stat.freeze || Time.timeScale == 0.0 || freeze){
		return;
	}
	var controller : CharacterController = GetComponent(CharacterController);
	if (stat.flinch){
		var knock : Vector3 = transform.TransformDirection(Vector3.back);
		controller.Move(knock * 8 *Time.deltaTime);
		return;
	}
	if (meleefwd){
		var lui : Vector3 = transform.TransformDirection(Vector3.forward);
		controller.Move(lui * 5 * Time.deltaTime);
	}
	
	if(automatic){
		if (Input.GetButton("Fire1") && Time.time > nextFire && !reloading && !attacking) {
			if(weaponEquip == 0){
				PrimaryAttack();
			}else{
				SecondaryAttack();
			}
		}
	}
	if(!automatic){
		if (Input.GetButtonDown("Fire1") && Time.time > nextFire && !reloading && !attacking) {
			if(weaponEquip == 0){
				PrimaryAttack();
			}else{
				SecondaryAttack();
			}
		}
	}
	if(Input.GetKeyDown("r")){
		//Reload();
		StartCoroutine("Reload"); 
	}
	Aiming();
	
	//Melee
	if (Input.GetKey("f") && Time.time > nextFire && !attacking && meleeAttack.canMelee) {
		if (Time.time > nextFire) {
			if(Time.time > (nextFire + 0.5)){
				c = 0;
			}
			//Attack Combo
			if(meleeAttack.meleeAnimation.Length >= 1){
				MeleeCombo();
			}
		}
	}
	
	//Zoom
	if(Input.GetButton("Fire2")){
       	GetComponent.<Camera>().main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomLevel, Time.deltaTime * 8);
       	onAiming = true;
    }
		
	//Magic
	if(Input.GetKeyDown("1") && !attacking && skill[0].skillPrefab){
		MagicSkill(0);
	}
	if(Input.GetKeyDown("2") && !attacking && skill[1].skillPrefab){
		MagicSkill(1);
	}
	if(Input.GetKeyDown("3") && !attacking && skill[2].skillPrefab){
		MagicSkill(2);
	}
	if(Input.GetKeyDown("4") && !attacking && skill[3].skillPrefab){
		MagicSkill(3);
	}
	
	//Switch Weapon
	if (Input.GetKeyDown("q") && Time.time > nextFire && !reloading) {
		SwapWeapon();
	}
    
    //Primary Weapon Ammo
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.HandgunAmmo){
    	spareAmmo = allAmmo.handgunAmmo;
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.MachinegunAmmo){
    	spareAmmo = allAmmo.machinegunAmmo;
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.ShotgunAmmo){
    	spareAmmo = allAmmo.shotgunAmmo;
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.MagnumAmmo){
    	spareAmmo = allAmmo.magnumAmmo;
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.SMGAmmo){
    	spareAmmo = allAmmo.smgAmmo;
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.SniperRifleAmmo){
    	spareAmmo = allAmmo.sniperRifleAmmo;
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.GrenadeRounds){
    	spareAmmo = allAmmo.grenadeRounds;
    }
    //Secondary Weapon Ammo
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.HandgunAmmo){
    	spareAmmo = allAmmo.handgunAmmo;
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.MachinegunAmmo){
    	spareAmmo = allAmmo.machinegunAmmo;
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.ShotgunAmmo){
    	spareAmmo = allAmmo.shotgunAmmo;
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.MagnumAmmo){
    	spareAmmo = allAmmo.magnumAmmo;
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.SMGAmmo){
    	spareAmmo = allAmmo.smgAmmo;
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.SniperRifleAmmo){
    	spareAmmo = allAmmo.sniperRifleAmmo;
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.GrenadeRounds){
    	spareAmmo = allAmmo.grenadeRounds;
    }
}

function PrimaryAttack(){
	if(!primaryWeapon.bulletPrefab){
		return;
	}
	if(primaryWeapon.ammo <= 0){
		//Reload();
		StartCoroutine("Reload");
		return;
	}
	str = stat.addAtk;
	matk = stat.addMatk;
	nextFire = Time.time + primaryWeapon.attackDelay;
	c = 0;
	//Shooting Animation
	if(primaryWeapon.shootAnimation && primaryWeapon.animateWhileMoving || primaryWeapon.shootAnimation && !primaryWeapon.animateWhileMoving && !Input.GetButton("Horizontal") && !Input.GetButton("Vertical")){
		if(!useMecanim){
			//For Legacy Animation
			mainModel.GetComponent.<Animation>()[primaryWeapon.shootAnimation.name].layer = 15;
			mainModel.GetComponent.<Animation>().PlayQueued(primaryWeapon.shootAnimation.name, QueueMode.PlayNow).speed = primaryWeapon.shootAnimationSpeed;
		}else{
			//For Mecanim Animation
			GetComponent(PlayerMecanimAnimation).AttackAnimation(primaryWeapon.shootAnimation.name);
		}
	}
	if(primaryWeapon.cameraShakeValue > 0){
		GetComponent.<Camera>().main.GetComponent(ThirdPersonCamera).Shake(primaryWeapon.cameraShakeValue , 0.05);
	}
	if(primaryWeapon.soundRadius > 0){
		GunSoundRadius(primaryWeapon.soundRadius);
	}
	
	if(primaryWeapon.gunFireEffect){
		var eff : GameObject = Instantiate(primaryWeapon.gunFireEffect, primaryWeapon.weaponAtkPoint.transform.position , primaryWeapon.weaponAtkPoint.transform.rotation);
		eff.transform.parent = primaryWeapon.weaponAtkPoint.transform;
	}
	if(primaryWeapon.gunSound){
		GetComponent.<AudioSource>().PlayOneShot(primaryWeapon.gunSound);
	}
	var bulletShootout : GameObject = Instantiate(primaryWeapon.bulletPrefab, primaryWeapon.weaponAtkPoint.transform.position , primaryWeapon.weaponAtkPoint.transform.rotation);
	bulletShootout.GetComponent(BulletStatus).Setting(str , matk , "Player" , this.gameObject);
	primaryWeapon.ammo--;
}

function SecondaryAttack(){
	if(!secondaryWeapon.bulletPrefab){
		return;
	}
	if(secondaryWeapon.ammo <= 0){
		//Reload();
		StartCoroutine("Reload");
		return;
	}
	str = stat.addAtk;
	matk = stat.addMatk;
	nextFire = Time.time + secondaryWeapon.attackDelay;
	c = 0;
	//Shooting Animation
	if(secondaryWeapon.shootAnimation  && secondaryWeapon.animateWhileMoving || secondaryWeapon.shootAnimation && !secondaryWeapon.animateWhileMoving && !Input.GetButton("Horizontal") && !Input.GetButton("Vertical")){
		if(!useMecanim){
			//For Legacy Animation
			mainModel.GetComponent.<Animation>()[secondaryWeapon.shootAnimation.name].layer = 15;
			mainModel.GetComponent.<Animation>().PlayQueued(secondaryWeapon.shootAnimation.name, QueueMode.PlayNow).speed = secondaryWeapon.shootAnimationSpeed;
		}else{
			//For Mecanim Animation
			GetComponent(PlayerMecanimAnimation).AttackAnimation(secondaryWeapon.shootAnimation.name);
		}
	}
	if(secondaryWeapon.cameraShakeValue > 0){
		GetComponent.<Camera>().main.GetComponent(ThirdPersonCamera).Shake(secondaryWeapon.cameraShakeValue , 0.1);
	}
	if(secondaryWeapon.soundRadius > 0){
		GunSoundRadius(secondaryWeapon.soundRadius);
	}
	
	if(secondaryWeapon.gunFireEffect){
		var eff : GameObject = Instantiate(secondaryWeapon.gunFireEffect, secondaryWeapon.weaponAtkPoint.transform.position , secondaryWeapon.weaponAtkPoint.transform.rotation);
		eff.transform.parent = secondaryWeapon.weaponAtkPoint.transform;
	}
	if(secondaryWeapon.gunSound){
		GetComponent.<AudioSource>().PlayOneShot(secondaryWeapon.gunSound);
	}
	var bulletShootout : GameObject = Instantiate(secondaryWeapon.bulletPrefab, secondaryWeapon.weaponAtkPoint.transform.position , secondaryWeapon.weaponAtkPoint.transform.rotation);
	bulletShootout.GetComponent(BulletStatus).Setting(str , matk , "Player" , this.gameObject);
	secondaryWeapon.ammo--;
}

function GunSoundRadius(radius : float){
	var hitColliders = Physics.OverlapSphere(transform.position, radius);
 		 
	for (var i = 0; i < hitColliders.Length; i++) {
		if(hitColliders[i].tag == "Enemy"){	  
	    	hitColliders[i].SendMessage("SetDestination" , transform.position);
	    }
	}
}

function Reload(){
	if(reloading || primaryWeapon.ammo >= primaryWeapon.maxAmmo && weaponEquip == 0 || secondaryWeapon.ammo >= secondaryWeapon.maxAmmo && weaponEquip == 1 || spareAmmo <= 0){
		return;
	}
	reloading = true;
	var mecanim : PlayerMecanimAnimation = GetComponent(PlayerMecanimAnimation);
	if(weaponEquip == 0){ //Primary Weapon
		if(primaryWeapon.reloadSound){
			GetComponent.<AudioSource>().PlayOneShot(primaryWeapon.reloadSound);
		}
		if(useMecanim){
			//Mecanim
			mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 1);
			mecanim.AttackAnimation(primaryWeapon.reloadAnimation.name);
			var clip = mecanim.animator.GetCurrentAnimatorClipInfo(0);
			var wait : float = clip.Length;
		}else{
			//Legacy
			mainModel.GetComponent.<Animation>()[primaryWeapon.reloadAnimation.name].layer = 6;
			mainModel.GetComponent.<Animation>().Play(primaryWeapon.reloadAnimation.name);
			wait = mainModel.GetComponent.<Animation>()[primaryWeapon.reloadAnimation.name].length;
		}
		yield WaitForSeconds(wait);
	}else{
		if(secondaryWeapon.reloadSound){
			GetComponent.<AudioSource>().PlayOneShot(secondaryWeapon.reloadSound);
		}
		if(useMecanim){
			//Mecanim
			mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 1);
			mecanim.AttackAnimation(secondaryWeapon.reloadAnimation.name);
			clip = mecanim.animator.GetCurrentAnimatorClipInfo(0);
			wait = clip.Length;
		}else{
			//Legacy
			mainModel.GetComponent.<Animation>()[secondaryWeapon.reloadAnimation.name].layer = 6;
			mainModel.GetComponent.<Animation>().Play(secondaryWeapon.reloadAnimation.name);
			wait = mainModel.GetComponent.<Animation>()[secondaryWeapon.reloadAnimation.name].length;
		}
		yield WaitForSeconds(wait);
	}
	if(useMecanim){
		mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 0);
	}
	ResetAmmo();
	reloading = false;
}

function ResetAmmo(){
	//Primary Weapon Ammo
	if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.HandgunAmmo){
		allAmmo.handgunAmmo += primaryWeapon.ammo;
		if(allAmmo.handgunAmmo >= primaryWeapon.maxAmmo){
			primaryWeapon.ammo = primaryWeapon.maxAmmo;
			allAmmo.handgunAmmo -= primaryWeapon.maxAmmo;
		}else if(allAmmo.handgunAmmo < primaryWeapon.maxAmmo){
			primaryWeapon.ammo = allAmmo.handgunAmmo;
			allAmmo.handgunAmmo = 0;
		}
	}
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.MachinegunAmmo){
    	allAmmo.machinegunAmmo += primaryWeapon.ammo;
		if(allAmmo.machinegunAmmo >= primaryWeapon.maxAmmo){
			primaryWeapon.ammo = primaryWeapon.maxAmmo;
			allAmmo.machinegunAmmo -= primaryWeapon.maxAmmo;
		}else if(allAmmo.machinegunAmmo < primaryWeapon.maxAmmo){
			primaryWeapon.ammo = allAmmo.machinegunAmmo;
			allAmmo.machinegunAmmo = 0;
		}
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.ShotgunAmmo){
    	allAmmo.shotgunAmmo += primaryWeapon.ammo;
		if(allAmmo.shotgunAmmo >= primaryWeapon.maxAmmo){
			primaryWeapon.ammo = primaryWeapon.maxAmmo;
			allAmmo.shotgunAmmo -= primaryWeapon.maxAmmo;
		}else if(allAmmo.shotgunAmmo < primaryWeapon.maxAmmo){
			primaryWeapon.ammo = allAmmo.shotgunAmmo;
			allAmmo.shotgunAmmo = 0;
		}
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.MagnumAmmo){
    	allAmmo.magnumAmmo += primaryWeapon.ammo;
    	if(allAmmo.magnumAmmo >= primaryWeapon.maxAmmo){
			primaryWeapon.ammo = primaryWeapon.maxAmmo;
			allAmmo.magnumAmmo -= primaryWeapon.maxAmmo;
		}else if(allAmmo.magnumAmmo < primaryWeapon.maxAmmo){
			primaryWeapon.ammo = allAmmo.magnumAmmo;
			allAmmo.magnumAmmo = 0;
		}
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.SMGAmmo){
    	allAmmo.smgAmmo += primaryWeapon.ammo;
    	if(allAmmo.smgAmmo >= primaryWeapon.maxAmmo){
			primaryWeapon.ammo = primaryWeapon.maxAmmo;
			allAmmo.smgAmmo -= primaryWeapon.maxAmmo;
		}else if(allAmmo.smgAmmo < primaryWeapon.maxAmmo){
			primaryWeapon.ammo = allAmmo.smgAmmo;
			allAmmo.smgAmmo = 0;
		}
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.SniperRifleAmmo){
    	allAmmo.sniperRifleAmmo += primaryWeapon.ammo;
    	if(allAmmo.sniperRifleAmmo >= primaryWeapon.maxAmmo){
			primaryWeapon.ammo = primaryWeapon.maxAmmo;
			allAmmo.sniperRifleAmmo -= primaryWeapon.maxAmmo;
		}else if(allAmmo.sniperRifleAmmo < primaryWeapon.maxAmmo){
			primaryWeapon.ammo = allAmmo.sniperRifleAmmo;
			allAmmo.sniperRifleAmmo = 0;
		}
    }
    if(weaponEquip == 0 && primaryWeapon.useAmmo == AmmoType.GrenadeRounds){
    	allAmmo.grenadeRounds += primaryWeapon.ammo;
    	if(allAmmo.grenadeRounds >= primaryWeapon.maxAmmo){
			primaryWeapon.ammo = primaryWeapon.maxAmmo;
			allAmmo.grenadeRounds -= primaryWeapon.maxAmmo;
		}else if(allAmmo.grenadeRounds < primaryWeapon.maxAmmo){
			primaryWeapon.ammo = allAmmo.grenadeRounds;
			allAmmo.grenadeRounds = 0;
		}
    }
    //Secondary Weapon Ammo
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.HandgunAmmo){
    	allAmmo.handgunAmmo += secondaryWeapon.ammo;
    	if(allAmmo.handgunAmmo >= secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = secondaryWeapon.maxAmmo;
			allAmmo.handgunAmmo -= secondaryWeapon.maxAmmo;
		}else if(allAmmo.handgunAmmo < secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = allAmmo.handgunAmmo;
			allAmmo.handgunAmmo = 0;
		}
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.MachinegunAmmo){
    	allAmmo.machinegunAmmo += secondaryWeapon.ammo;
    	if(allAmmo.machinegunAmmo >= secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = secondaryWeapon.maxAmmo;
			allAmmo.machinegunAmmo -= secondaryWeapon.maxAmmo;
		}else if(allAmmo.machinegunAmmo < secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = allAmmo.machinegunAmmo;
			allAmmo.machinegunAmmo = 0;
		}
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.ShotgunAmmo){
    	allAmmo.shotgunAmmo += secondaryWeapon.ammo;
    	if(allAmmo.shotgunAmmo >= secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = secondaryWeapon.maxAmmo;
			allAmmo.shotgunAmmo -= secondaryWeapon.maxAmmo;
		}else if(allAmmo.shotgunAmmo < secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = allAmmo.shotgunAmmo;
			allAmmo.shotgunAmmo = 0;
		}
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.MagnumAmmo){
    	allAmmo.magnumAmmo += secondaryWeapon.ammo;
    	if(allAmmo.magnumAmmo >= secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = secondaryWeapon.maxAmmo;
			allAmmo.magnumAmmo -= secondaryWeapon.maxAmmo;
		}else if(allAmmo.magnumAmmo < secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = allAmmo.magnumAmmo;
			allAmmo.magnumAmmo = 0;
		}
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.SMGAmmo){
    	allAmmo.smgAmmo += secondaryWeapon.ammo;
    	if(allAmmo.smgAmmo >= secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = secondaryWeapon.maxAmmo;
			allAmmo.smgAmmo -= secondaryWeapon.maxAmmo;
		}else if(allAmmo.smgAmmo < secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = allAmmo.smgAmmo;
			allAmmo.smgAmmo = 0;
		}
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.SniperRifleAmmo){
    	allAmmo.sniperRifleAmmo += secondaryWeapon.ammo;
    	if(allAmmo.sniperRifleAmmo >= secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = secondaryWeapon.maxAmmo;
			allAmmo.sniperRifleAmmo -= secondaryWeapon.maxAmmo;
		}else if(allAmmo.sniperRifleAmmo < secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = allAmmo.sniperRifleAmmo;
			allAmmo.sniperRifleAmmo = 0;
		}
    }
    if(weaponEquip == 1 && secondaryWeapon.useAmmo == AmmoType.GrenadeRounds){
    	allAmmo.grenadeRounds += secondaryWeapon.ammo;
    	if(allAmmo.grenadeRounds >= secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = secondaryWeapon.maxAmmo;
			allAmmo.grenadeRounds -= secondaryWeapon.maxAmmo;
		}else if(allAmmo.grenadeRounds < secondaryWeapon.maxAmmo){
			secondaryWeapon.ammo = allAmmo.grenadeRounds;
			allAmmo.grenadeRounds = 0;
		}
    }
}

function OnGUI(){
	if(aimIcon){
		GUI.DrawTexture(Rect(Screen.width /2 -20,Screen.height /2 -20,40,40), aimIcon);
	}
	if(zoomIcon && onAiming){
		GUI.DrawTexture(Rect(0 ,0 ,Screen.width,Screen.height), zoomIcon);
	}
	if(useLegacyUi){
		var hp : int = stat.health * 100 / stat.maxHealth * 1.5;
		GUI.Label (Rect (50, Screen.height - 100, 200, 50), "HP : " + stat.health.ToString() , statusFont);
		GUI.DrawTexture(Rect(50 ,Screen.height - 80 ,150,10), backGroundBar);
		GUI.DrawTexture(Rect(50 ,Screen.height - 80 ,hp,10), hpBar);
		
		var mp : int = stat.mana * 100 / stat.maxMana * 1.5;
		GUI.Label (Rect (50, Screen.height - 60, 200, 50), "MP : " + stat.mana.ToString() , statusFont);
		GUI.DrawTexture(Rect(50 ,Screen.height - 40 ,150,10), backGroundBar);
		GUI.DrawTexture(Rect(50 ,Screen.height - 40 ,mp,10), mpBar);
		
		if(stat.maxShieldPlus > 0){
			var sh : int = stat.shield * 100 / stat.maxShieldPlus * 1.5;
			GUI.Label (Rect (50, Screen.height - 140, 200, 50), "Shield : " + stat.shield.ToString() , statusFont);
			GUI.DrawTexture(Rect(50 ,Screen.height - 120 ,150,10), backGroundBar);
			GUI.DrawTexture(Rect(50 ,Screen.height - 120 ,sh,10), shieldBar );
		}
		
		var xp : int = stat.exp * 100 / stat.maxExp * 1.5;
		GUI.Label (Rect (50, 40, 200, 50), "Level : " + stat.level.ToString() , statusFont);
		GUI.DrawTexture(Rect(50 ,60 ,150,10), backGroundBar);
		GUI.DrawTexture(Rect(50 ,60 ,xp,10), expBar);
	}
	
	if(weaponEquip == 0){ //Primary Weapon
		GUI.Label (Rect (Screen.width - 280, Screen.height - 80, 200, 50), "Ammo : " + primaryWeapon.ammo.ToString() + " / " + spareAmmo.ToString(), ammoFont);
	}
	if(weaponEquip == 1){ //Secondary Weapon
		GUI.Label (Rect (Screen.width - 280, Screen.height - 80, 200, 50), "Ammo : " + secondaryWeapon.ammo.ToString() + " / " + spareAmmo.ToString() , ammoFont);
	}
	
	if(skill[0].icon){
		GUI.DrawTexture(Rect(Screen.width -280 ,Screen.height - 125 ,40,40), skill[0].icon);
	}
	if(skill[1].icon){
		GUI.DrawTexture(Rect(Screen.width -240 ,Screen.height - 125 ,40,40), skill[1].icon);
	}
	if(skill[2].icon){
		GUI.DrawTexture(Rect(Screen.width -200 ,Screen.height - 125 ,40,40), skill[2].icon);
	}
	if(skill[3].icon){
		GUI.DrawTexture(Rect(Screen.width -160 ,Screen.height - 125 ,40,40), skill[3].icon);
	}
	//Show Buffs Icon
	if(stat.brave){
		GUI.DrawTexture( new Rect(30,200,60,60), braveIcon);
	}
	if(stat.barrier){
		GUI.DrawTexture( new Rect(30,260,60,60), barrierIcon);
	}
	if(stat.faith){
		GUI.DrawTexture( new Rect(30,320,60,60), faithIcon);
	}
	if(stat.mbarrier){
		GUI.DrawTexture( new Rect(30,380,60,60), magicBarrierIcon);
	}
	if(stat.sharp){
		GUI.DrawTexture( new Rect(30,440,60,60), sharpIcon);
	}
}

function Aiming(){
		var ray : Ray = Camera.main.ViewportPointToRay (Vector3(0.5,0.5,0));
		// Do a raycast
		var hit : RaycastHit;
		if (Physics.Raycast (ray, hit)){
			if(Vector3.Distance(hit.point, Camera.main.transform.position) < Vector3.Distance(Camera.main.transform.position, transform.position)){
				attackPoint.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(0.52f, 0.54f, 100.0f)));
				if(secondaryWeapon.weaponAtkPoint)
					secondaryWeapon.weaponAtkPoint.transform.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(0.52f, 0.54f, 100.0f)));
				if(primaryWeapon.weaponAtkPoint)
					primaryWeapon.weaponAtkPoint.transform.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(0.52f, 0.54f, 100.0f)));
			}else{
				attackPoint.transform.LookAt(hit.point);
				if(secondaryWeapon.weaponAtkPoint)
					secondaryWeapon.weaponAtkPoint.transform.LookAt(hit.point);
				if(primaryWeapon.weaponAtkPoint)
					primaryWeapon.weaponAtkPoint.transform.LookAt(hit.point);
			}
		}else{
			attackPoint.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(0.52f, 0.54f, 100.0f)));
			if(secondaryWeapon.weaponAtkPoint)
				secondaryWeapon.weaponAtkPoint.transform.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(0.52f, 0.54f, 100.0f)));
			if(primaryWeapon.weaponAtkPoint)
				primaryWeapon.weaponAtkPoint.transform.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(0.52f, 0.54f, 100.0f)));
		}
}

function MagicSkill(skillID : int){
	if(!skill[skillID].skillAnimation){
		print("Please assign skill animation in Skill Animation");
		return;
	}
	str = stat.addAtk;
	matk = stat.addMatk;
	
	if(stat.mana < skill[skillID].manaCost || stat.silence){
		return;
	}
	if(sound.magicCastVoice){
		GetComponent.<AudioSource>().PlayOneShot(sound.magicCastVoice);
	}
	attacking = true;
	GetComponent(CharacterMotor).canControl = false;
	
	if(!useMecanim){
		//For Legacy Animation
		mainModel.GetComponent.<Animation>()[skill[skillID].skillAnimation.name].layer = 16;
		mainModel.GetComponent.<Animation>()[skill[skillID].skillAnimation.name].speed = skill[skillID].skillAnimationSpeed;
		mainModel.GetComponent.<Animation>().Play(skill[skillID].skillAnimation.name);
	}else{
		//For Mecanim Animation
		GetComponent(PlayerMecanimAnimation).AttackAnimation(skill[skillID].skillAnimation.name);
	}
		
	nextFire = Time.time + skill[skillID].skillDelay;
	
	yield WaitForSeconds(skill[skillID].castTime);
	var bulletShootout : Transform = Instantiate(skill[skillID].skillPrefab, attackPoint.transform.position , attackPoint.transform.rotation);
	bulletShootout.GetComponent(BulletStatus).Setting(str , matk , "Player" , this.gameObject);
	yield WaitForSeconds(skill[skillID].skillDelay);
	attacking = false;
	GetComponent(CharacterMotor).canControl = true;
	stat.mana -= skill[skillID].manaCost;
}

function MeleeCombo(){
	if(!meleeAttack.meleePrefab){
		return;
	}
	if(!meleeAttack.meleeAnimation[c]){
		print("Please assign attack animation in Attack Combo");
		return;
	}
	str = stat.addMelee;
	matk = stat.addMatk;
	attacking = true;
	GetComponent(CharacterMotor).canControl = false;
	MeleeDash();
	//---------Hide Gun-----------
	if(primaryWeaponModel){
		primaryWeaponModel.SetActive(false);
	}
	if(secondaryWeaponModel){
		secondaryWeaponModel.SetActive(false);
	}
	//----------------------------
	if(meleeWeaponModel){
		meleeWeaponModel.SetActive(true);
	}
	//---------------------------
	if(meleeAttack.meleeSound){
		GetComponent.<AudioSource>().PlayOneShot(meleeAttack.meleeSound);
	}
	if(sound.attackComboVoice.Length > c && sound.attackComboVoice[c]){
		GetComponent.<AudioSource>().PlayOneShot(sound.attackComboVoice[c]);
	}
	if(!useMecanim){
		//For Legacy Animation
		mainModel.GetComponent.<Animation>()[meleeAttack.meleeAnimation[c].name].layer = 15;
		mainModel.GetComponent.<Animation>().PlayQueued(meleeAttack.meleeAnimation[c].name, QueueMode.PlayNow).speed = meleeAttack.meleeAnimationSpeed;
		var wait : float = mainModel.GetComponent.<Animation>()[meleeAttack.meleeAnimation[c].name].length;
	}else{
		//For Mecanim Animation
		GetComponent(PlayerMecanimAnimation).AttackAnimation(meleeAttack.meleeAnimation[c].name);
		var clip = GetComponent(PlayerMecanimAnimation).animator.GetCurrentAnimatorClipInfo(0);
		wait = clip.Length -0.5;
	}
	
	yield WaitForSeconds(meleeAttack.meleeCast);
	c++;
	
	nextFire = Time.time + meleeAttack.meleeDelay;
	var bulletShootout : GameObject = Instantiate(meleeAttack.meleePrefab, attackPoint.transform.position , attackPoint.transform.rotation);
	bulletShootout.GetComponent(BulletStatus).Setting(str , matk , "Player" , this.gameObject);
			
	if(c >= meleeAttack.meleeAnimation.Length){
		c = 0;
		yield WaitForSeconds(wait);
	}else{
		yield WaitForSeconds(meleeAttack.meleeDelay);
	}
	
	attacking = false;
	GetComponent(CharacterMotor).canControl = true;
	
	if(primaryWeaponModel && weaponEquip == 0){
		primaryWeaponModel.SetActive(true);
	}
	if(secondaryWeaponModel && weaponEquip == 1){
		secondaryWeaponModel.SetActive(true);
	}
	//----------------------------
	if(meleeWeaponModel){
		meleeWeaponModel.SetActive(false);
	}
	//---------------------------
}

function MeleeDash(){
	meleefwd = true;
	yield WaitForSeconds(0.2);
	meleefwd = false;
}

function SettingWeapon(){
	stat = GetComponent(Status);
	if(onAiming){
       	GetComponent.<Camera>().main.fieldOfView = 60;
       	onAiming = false;
    }
    
	if(weaponEquip == 0){
		//Primary Weapon
		zoomLevel = primaryWeapon.zoomLevel;
		if(primaryWeapon.aimIcon){
			aimIcon = primaryWeapon.aimIcon;
		}
		zoomIcon = primaryWeapon.zoomIcon;
		automatic = primaryWeapon.automatic;
		/*if(primaryWeapon.weaponAtkPoint){
			attackPoint = primaryWeapon.weaponAtkPoint;
		}*/
		stat.currentWeaponAtk = stat.weaponAtk;
		stat.CalculateStatus();
		
		if(primaryWeaponModel){
			primaryWeaponModel.SetActive(true);
		}
		if(secondaryWeaponModel){
			secondaryWeaponModel.SetActive(false);
		}
	}else{
		//Secondary Weapon
		zoomLevel = secondaryWeapon.zoomLevel;
		if(secondaryWeapon.aimIcon){
			aimIcon = secondaryWeapon.aimIcon;
		}
		zoomIcon = secondaryWeapon.zoomIcon;
		automatic = secondaryWeapon.automatic;
		/*if(secondaryWeapon.weaponAtkPoint){
			attackPoint = primaryWeapon.weaponAtkPoint;
		}*/
		stat.currentWeaponAtk = stat.weaponAtk2;
		stat.CalculateStatus();
		
		if(primaryWeaponModel){
			primaryWeaponModel.SetActive(false);
		}
		if(secondaryWeaponModel){
			secondaryWeaponModel.SetActive(true);
		}
	}

}


function SwapWeapon(){
	/*if(reloading){
		return;
	}*/
	StopCoroutine("Reload");
	
	if(onAiming){
       	GetComponent.<Camera>().main.fieldOfView = 60;
       	onAiming = false;
    }
    var mecanim : PlayerMecanimAnimation = GetComponent(PlayerMecanimAnimation);
	if(weaponEquip == 0){
		//Swap from Primary to Secondary
		if(!secondaryWeapon.bulletPrefab){
			return;	//Do Nothing If you don't have Secondary Weapon's Bullet
		}
		if(primaryWeaponModel){
			primaryWeaponModel.SetActive(false);
		}
		if(secondaryWeaponModel){
			secondaryWeaponModel.SetActive(true);
		}
		weaponEquip = 1;
		if(!useMecanim && secondaryWeapon.equipAnimation){
			//For Legacy Animation
			mainModel.GetComponent.<Animation>()[secondaryWeapon.equipAnimation.name].layer = 10;
			mainModel.GetComponent.<Animation>().Play(secondaryWeapon.equipAnimation.name);
			freeze = true;
			var wait : float = mainModel.GetComponent.<Animation>()[secondaryWeapon.equipAnimation.name].length;
			yield WaitForSeconds(wait);
			freeze = false;
		}else{
			//For Mecanim Animation
			mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 1);
			GetComponent(PlayerMecanimAnimation).PlayAnim(secondaryWeapon.equipAnimation.name);
			var clip = GetComponent(PlayerMecanimAnimation).animator.GetCurrentAnimatorClipInfo(0);
			freeze = true;
			wait = clip.Length -0.4;
			yield WaitForSeconds(wait);
			mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 0);
			freeze = false;
		}
		
	}else{
		//Swap from Secondary to Primary
		if(!primaryWeapon.bulletPrefab){
			return; //Do Nothing If you don't have Primary Weapon's Bullet
		}
		if(primaryWeaponModel){
			primaryWeaponModel.SetActive(true);
		}
		if(secondaryWeaponModel){
			secondaryWeaponModel.SetActive(false);
		}
		weaponEquip = 0;
		if(!useMecanim && primaryWeapon.equipAnimation){
			//For Legacy Animation
			mainModel.GetComponent.<Animation>()[primaryWeapon.equipAnimation.name].layer = 10;
			mainModel.GetComponent.<Animation>().Play(primaryWeapon.equipAnimation.name);
			freeze = true;
			wait = mainModel.GetComponent.<Animation>()[primaryWeapon.equipAnimation.name].length;
			yield WaitForSeconds(wait);
			freeze = false;
		}else{
			//For Mecanim Animation
			mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 1);
			GetComponent(PlayerMecanimAnimation).PlayAnim(primaryWeapon.equipAnimation.name);
			clip = GetComponent(PlayerMecanimAnimation).animator.GetCurrentAnimatorClipInfo(0);
			freeze = true;
			wait = clip.Length -0.4;
			yield WaitForSeconds(wait);
			mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 0);
			freeze = false;
		}
	}
	if(!useMecanim){
		GetComponent(PlayerLegacyAnimation).SetAnimation();
	}else{
		GetComponent(PlayerMecanimAnimation).SetAnimation();
		GetComponent(PlayerMecanimAnimation).SetIdle();
	}
	SettingWeapon();
}

function ForceSwap(){
	//Use when call by other script.
	SwapWeapon();
}

function ShowMeleeWeapon(b : boolean){
	//---------Hide Gun-----------
	if(b){
		if(primaryWeaponModel && weaponEquip == 0){
			primaryWeaponModel.SetActive(false);
		}
		if(secondaryWeaponModel && weaponEquip == 1){
			secondaryWeaponModel.SetActive(false);
		}
	}else{
		if(primaryWeaponModel && weaponEquip == 0){
			primaryWeaponModel.SetActive(true);
		}
		if(secondaryWeaponModel && weaponEquip == 1){
			secondaryWeaponModel.SetActive(true);
		}
	}
	//----------------------------
	if(meleeWeaponModel){
		meleeWeaponModel.SetActive(b);
	}

}

@script RequireComponent(Status)
@script RequireComponent(Inventory)
@script RequireComponent(UiMaster)
@script RequireComponent(ShowEnemyHealth)
@script RequireComponent(SkillWindow)
@script RequireComponent(SaveLoad)
@script RequireComponent(DontDestroyOnload)
