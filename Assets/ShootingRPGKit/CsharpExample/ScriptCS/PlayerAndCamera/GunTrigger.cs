using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Status))]
[RequireComponent (typeof(Inventory))]
[RequireComponent (typeof(ShowEnemyHealth))]
[RequireComponent (typeof(SkillWindow))]
[RequireComponent (typeof(DontDestroyOnload))]
[RequireComponent (typeof(UiMasterC))]
[RequireComponent (typeof(SaveLoad))]
public class GunTrigger : MonoBehaviour {
	private GameObject mainModel;
	public bool useMecanim = true;
	public Transform attackPoint;
	
	public GameObject mainCamPrefab;
	private float nextFire = 0.0f;
	private bool reloading = false;
	private bool meleefwd = false;
	public Texture2D aimIcon;
	private Texture2D zoomIcon;
	private Status stat;
	[HideInInspector]
		public int weaponEquip = 0;
	
	[HideInInspector]
		public bool attacking = false;
	[HideInInspector]
		public int c = 0;
	
	[HideInInspector]
		public GameObject mainCam;
	
	private int str = 0;
	private int matk = 0;
	
	public Transform weaponPosition; //Position of Your Weapon
	
	public GameObject primaryWeaponModel; //Assign Model of your Primary Weapon in your Main Model.
	public GameObject secondaryWeaponModel; //Assign Model of your Secondary Weapon in your Main Model.
	public GameObject meleeWeaponModel; //Assign Model of your Primary Weapon in your Main Model.
	
	public WeaponSet primaryWeapon;
	public WeaponSet secondaryWeapon;
	
	public MeleeSet meleeAttack;
	
	public AllAmmo allAmmo;
	
	public SkilAtk[] skill = new SkilAtk[4];

	public AtkSound sound;
	public GUIStyle statusFont;
	public GUIStyle ammoFont;
	
	public Texture2D hpBar;
	public Texture2D mpBar;
	public Texture2D shieldBar;
	public Texture2D expBar;
	public Texture2D backGroundBar;
	
	private float zoomLevel = 30.0f;
	private bool onAiming = false;
	private bool automatic = true;
	private bool freeze = false;
	private int spareAmmo = 30;

	public Texture2D braveIcon;
	public Texture2D barrierIcon;
	public Texture2D faithIcon;
	public Texture2D magicBarrierIcon;
	public Texture2D sharpIcon;

	public bool useLegacyUi = false;
	
	void Start (){
		gameObject.tag = "Player";
		if(!attackPoint){
			GameObject n = new GameObject();
			n.transform.parent = this.transform;
			attackPoint = n.transform;
		}
		stat = GetComponent<Status>();
		
		if(!mainModel){
			mainModel = stat.mainModel;
		}
		
		stat.useMecanim = useMecanim;
		stat.CalculateStatus();
		
		GameObject[] oldcam = GameObject.FindGameObjectsWithTag("MainCamera");
		foreach(GameObject o in oldcam){
			Destroy(o);
		}
		GameObject newCam = Instantiate(mainCamPrefab, transform.position , transform.rotation) as GameObject;
		mainCam = newCam;
		
		if(sound.hurtVoice){
			stat.hurtVoice = sound.hurtVoice;
		}
		SettingWeapon();
		
	}
	
	void  Update (){
		//Release Zoom
		if(onAiming && Input.GetButtonUp("Fire2")){
			Camera.main.fieldOfView = 60;
			onAiming = false;
		}
		if(stat.freeze || Time.timeScale == 0.0f || freeze){
			return;
		}
		CharacterController controller = GetComponent<CharacterController>();
		if (stat.flinch){
			Vector3 knock = transform.TransformDirection(Vector3.back);
			controller.Move(knock * 8 *Time.deltaTime);
			return;
		}
		if (meleefwd){
			Vector3 lui = transform.TransformDirection(Vector3.forward);
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
				if(Time.time > (nextFire + 0.5f)){
					c = 0;
				}
				//Attack Combo
				if(meleeAttack.meleeAnimation.Length >= 1){
					//MeleeCombo();
					StartCoroutine(MeleeCombo());
				}
			}
		}
		
		//Zoom
		if(Input.GetButton("Fire2")){
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomLevel, Time.deltaTime * 8);
			onAiming = true;
		}
		
		//Magic
		if(Input.GetKeyDown("1") && !attacking && skill[0].skillPrefab){
			//MagicSkill(0);
			StartCoroutine(MagicSkill(0));
		}
		if(Input.GetKeyDown("2") && !attacking && skill[1].skillPrefab){
			//MagicSkill(1);
			StartCoroutine(MagicSkill(1));
		}
		if(Input.GetKeyDown("3") && !attacking && skill[2].skillPrefab){
			//MagicSkill(2);
			StartCoroutine(MagicSkill(2));
		}
		if(Input.GetKeyDown("4") && !attacking && skill[3].skillPrefab){
			//MagicSkill(3);
			StartCoroutine(MagicSkill(3));
		}
		
		//Switch Weapon
		if (Input.GetKeyDown("q") && Time.time > nextFire && !reloading) {
			StartCoroutine(SwapWeapon());
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
	
	void PrimaryAttack(){
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
				mainModel.GetComponent<Animation>()[primaryWeapon.shootAnimation.name].layer = 15;
				mainModel.GetComponent<Animation>().PlayQueued(primaryWeapon.shootAnimation.name, QueueMode.PlayNow).speed = primaryWeapon.shootAnimationSpeed;
			}else{
				//For Mecanim Animation
				GetComponent<PlayerMecanimAnimation>().AttackAnimation(primaryWeapon.shootAnimation.name);
			}
		}
		if(primaryWeapon.cameraShakeValue > 0){
			Camera.main.GetComponent<ThirdPersonCamera>().Shake(primaryWeapon.cameraShakeValue , 0.05f);
		}
		if(primaryWeapon.soundRadius > 0){
			GunSoundRadius(primaryWeapon.soundRadius);
		}
		
		if(primaryWeapon.gunFireEffect){
			GameObject eff = Instantiate(primaryWeapon.gunFireEffect, primaryWeapon.weaponAtkPoint.transform.position , primaryWeapon.weaponAtkPoint.transform.rotation) as GameObject;
			eff.transform.parent = primaryWeapon.weaponAtkPoint.transform;
		}
		if(primaryWeapon.gunSound){
			GetComponent<AudioSource>().PlayOneShot(primaryWeapon.gunSound);
		}
		GameObject bulletShootout = Instantiate(primaryWeapon.bulletPrefab, primaryWeapon.weaponAtkPoint.transform.position , primaryWeapon.weaponAtkPoint.transform.rotation) as GameObject;
		bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
		primaryWeapon.ammo--;
	}
	
	void SecondaryAttack(){
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
				mainModel.GetComponent<Animation>()[secondaryWeapon.shootAnimation.name].layer = 15;
				mainModel.GetComponent<Animation>().PlayQueued(secondaryWeapon.shootAnimation.name, QueueMode.PlayNow).speed = secondaryWeapon.shootAnimationSpeed;
			}else{
				//For Mecanim Animation
				GetComponent<PlayerMecanimAnimation>().AttackAnimation(secondaryWeapon.shootAnimation.name);
			}
		}
		if(secondaryWeapon.cameraShakeValue > 0){
			Camera.main.GetComponent<ThirdPersonCamera>().Shake(secondaryWeapon.cameraShakeValue , 0.1f);
		}
		if(secondaryWeapon.soundRadius > 0){
			GunSoundRadius(secondaryWeapon.soundRadius);
		}
		
		if(secondaryWeapon.gunFireEffect){
			GameObject eff = Instantiate(secondaryWeapon.gunFireEffect, secondaryWeapon.weaponAtkPoint.transform.position , secondaryWeapon.weaponAtkPoint.transform.rotation) as GameObject;
			eff.transform.parent = secondaryWeapon.weaponAtkPoint.transform;
		}
		if(secondaryWeapon.gunSound){
			GetComponent<AudioSource>().PlayOneShot(secondaryWeapon.gunSound);
		}
		GameObject bulletShootout = Instantiate(secondaryWeapon.bulletPrefab, secondaryWeapon.weaponAtkPoint.transform.position , secondaryWeapon.weaponAtkPoint.transform.rotation) as GameObject;
		bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
		secondaryWeapon.ammo--;
	}
	
	void GunSoundRadius (float radius){
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
		
		for(int i= 0; i < hitColliders.Length; i++) {
			if(hitColliders[i].tag == "Enemy"){	  
				hitColliders[i].SendMessage("SetDestination" , transform.position , SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	IEnumerator Reload(){
		if(reloading || primaryWeapon.ammo >= primaryWeapon.maxAmmo && weaponEquip == 0 || secondaryWeapon.ammo >= secondaryWeapon.maxAmmo && weaponEquip == 1 || spareAmmo <= 0){
			yield break;
		}
		reloading = true;
		float wait = 0;
		PlayerMecanimAnimation mecanim = GetComponent<PlayerMecanimAnimation>();
		if(weaponEquip == 0){ //Primary Weapon
			if(primaryWeapon.reloadSound){
				GetComponent<AudioSource>().PlayOneShot(primaryWeapon.reloadSound);
			}
			if(useMecanim){
				//Mecanim
				mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 1);
				mecanim.AttackAnimation(primaryWeapon.reloadAnimation.name);
				wait = mecanim.animator.GetCurrentAnimatorClipInfo(0).Length;
			}else{
				//Legacy
				mainModel.GetComponent<Animation>()[primaryWeapon.reloadAnimation.name].layer = 6;
				mainModel.GetComponent<Animation>().Play(primaryWeapon.reloadAnimation.name);
				wait = mainModel.GetComponent<Animation>()[primaryWeapon.reloadAnimation.name].length;
			}
			yield return new WaitForSeconds(wait);
		}else{
			if(secondaryWeapon.reloadSound){
				GetComponent<AudioSource>().PlayOneShot(secondaryWeapon.reloadSound);
			}
			if(useMecanim){
				//Mecanim
				mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 1);
				mecanim.AttackAnimation(secondaryWeapon.reloadAnimation.name);
				wait = mecanim.animator.GetCurrentAnimatorClipInfo(0).Length;
			}else{
				//Legacy
				mainModel.GetComponent<Animation>()[secondaryWeapon.reloadAnimation.name].layer = 6;
				mainModel.GetComponent<Animation>().Play(secondaryWeapon.reloadAnimation.name);
				wait = mainModel.GetComponent<Animation>()[secondaryWeapon.reloadAnimation.name].length;
			}
			yield return new WaitForSeconds(wait);
		}
		if(useMecanim){
			mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 0);
		}
		ResetAmmo();
		reloading = false;
	}
	
	public void ResetAmmo (){
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
	
	void OnGUI (){
		if(aimIcon){
			GUI.DrawTexture( new Rect(Screen.width /2 -20,Screen.height /2 -20,40,40), aimIcon);
		}
		if(zoomIcon && onAiming){
			GUI.DrawTexture( new Rect(0 ,0 ,Screen.width,Screen.height), zoomIcon);
		}
		if(useLegacyUi){
			int hp = stat.health * 100 / stat.maxHealth * 150 / 100;
			GUI.Label(new Rect(50, Screen.height - 100, 200, 50), "HP : " + stat.health.ToString() , statusFont);
			GUI.DrawTexture(new Rect(50 ,Screen.height - 80 ,150,10), backGroundBar);
			GUI.DrawTexture(new Rect(50 ,Screen.height - 80 ,hp,10), hpBar);
			
			int mp = stat.mana * 100 / stat.maxMana * 150 / 100;
			GUI.Label(new Rect(50, Screen.height - 60, 200, 50), "MP : " + stat.mana.ToString() , statusFont);
			GUI.DrawTexture(new Rect(50 ,Screen.height - 40 ,150,10), backGroundBar);
			GUI.DrawTexture(new Rect(50 ,Screen.height - 40 ,mp,10), mpBar);
			
			if(stat.maxShieldPlus > 0){
				int sh = stat.shield * 100 / stat.maxShieldPlus * 150 / 100;
				GUI.Label(new Rect(50, Screen.height - 140, 200, 50), "Shield : " + stat.shield.ToString() , statusFont);
				GUI.DrawTexture(new Rect(50 ,Screen.height - 120 ,150,10), backGroundBar);
				GUI.DrawTexture(new Rect(50 ,Screen.height - 120 ,sh,10), shieldBar );
			}
			
			int xp = stat.exp * 100 / stat.maxExp * 150 / 100;
			GUI.Label(new Rect(50, 40, 200, 50), "Level : " + stat.level.ToString() , statusFont);
			GUI.DrawTexture(new Rect(50 ,60 ,150,10), backGroundBar);
			GUI.DrawTexture(new Rect(50 ,60 ,xp,10), expBar);
		}

		if(weaponEquip == 0){ //Primary Weapon
			GUI.Label ( new Rect(Screen.width - 280, Screen.height - 80, 200, 50), "Ammo : " + primaryWeapon.ammo.ToString() + " / " + spareAmmo.ToString(), ammoFont);
		}
		if(weaponEquip == 1){ //Secondary Weapon
			GUI.Label ( new Rect(Screen.width - 280, Screen.height - 80, 200, 50), "Ammo : " + secondaryWeapon.ammo.ToString() + " / " + spareAmmo.ToString() , ammoFont);
		}
		
		if(skill[0].icon){
			GUI.DrawTexture( new Rect(Screen.width -280 ,Screen.height - 125 ,40,40), skill[0].icon);
		}
		if(skill[1].icon){
			GUI.DrawTexture( new Rect(Screen.width -240 ,Screen.height - 125 ,40,40), skill[1].icon);
		}
		if(skill[2].icon){
			GUI.DrawTexture( new Rect(Screen.width -200 ,Screen.height - 125 ,40,40), skill[2].icon);
		}
		if(skill[3].icon){
			GUI.DrawTexture( new Rect(Screen.width -160 ,Screen.height - 125 ,40,40), skill[3].icon);
		}

		//Show Buffs Icon
		if(stat.brave){
			GUI.DrawTexture(new Rect(30,200,60,60), braveIcon);
		}
		if(stat.barrier){
			GUI.DrawTexture(new Rect(30,260,60,60), barrierIcon);
		}
		if(stat.faith){
			GUI.DrawTexture(new Rect(30,320,60,60), faithIcon);
		}
		if(stat.mbarrier){
			GUI.DrawTexture(new Rect(30,380,60,60), magicBarrierIcon);
		}
		if(stat.sharp){
			GUI.DrawTexture(new Rect(30,440,60,60), sharpIcon);
		}
	}
	
	void  Aiming (){
		Ray ray = Camera.main.ViewportPointToRay (new Vector3(0.5f,0.5f,0.0f));
		// Do a raycast
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)){
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
	
	IEnumerator MagicSkill(int skillID){
		if(!skill[skillID].skillAnimation){
			print("Please assign skill animation in Skill Animation");
			yield break;
		}
		str = stat.addAtk;
		matk = stat.addMatk;
		
		if(stat.mana < skill[skillID].manaCost || stat.silence){
			yield break;
		}
		if(sound.magicCastVoice){
			GetComponent<AudioSource>().PlayOneShot(sound.magicCastVoice);
		}
		attacking = true;
		GetComponent<CharacterMotorC>().canControl = false;
		
		if(!useMecanim){
			//For Legacy Animation
			mainModel.GetComponent<Animation>()[skill[skillID].skillAnimation.name].layer = 16;
			mainModel.GetComponent<Animation>()[skill[skillID].skillAnimation.name].speed = skill[skillID].skillAnimationSpeed;
			mainModel.GetComponent<Animation>().Play(skill[skillID].skillAnimation.name);
		}else{
			//For Mecanim Animation
			GetComponent<PlayerMecanimAnimation>().AttackAnimation(skill[skillID].skillAnimation.name);
		}
		
		nextFire = Time.time + skill[skillID].skillDelay;
		
		yield return new WaitForSeconds(skill[skillID].castTime);
		Transform bulletShootout = Instantiate(skill[skillID].skillPrefab, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
		bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
		yield return new WaitForSeconds(skill[skillID].skillDelay);
		attacking = false;
		GetComponent<CharacterMotorC>().canControl = true;
		stat.mana -= skill[skillID].manaCost;
	}
	
	IEnumerator MeleeCombo(){
		if(!meleeAttack.meleePrefab){
			yield break;
		}
		if(!meleeAttack.meleeAnimation[c]){
			print("Please assign attack animation in Attack Combo");
			yield break;
		}
		str = stat.addMelee;
		matk = stat.addMatk;
		attacking = true;
		GetComponent<CharacterMotorC>().canControl = false;
		//MeleeDash();
		StartCoroutine(MeleeDash());
		float wait = 0;
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
			GetComponent<AudioSource>().PlayOneShot(meleeAttack.meleeSound);
		}
		if(sound.attackComboVoice.Length > c && sound.attackComboVoice[c]){
			GetComponent<AudioSource>().PlayOneShot(sound.attackComboVoice[c]);
		}
		if(!useMecanim){
			//For Legacy Animation
			mainModel.GetComponent<Animation>()[meleeAttack.meleeAnimation[c].name].layer = 15;
			mainModel.GetComponent<Animation>().PlayQueued(meleeAttack.meleeAnimation[c].name, QueueMode.PlayNow).speed = meleeAttack.meleeAnimationSpeed;
			wait = mainModel.GetComponent<Animation>()[meleeAttack.meleeAnimation[c].name].length;
		}else{
			//For Mecanim Animation
			GetComponent<PlayerMecanimAnimation>().AttackAnimation(meleeAttack.meleeAnimation[c].name);
			wait = GetComponent<PlayerMecanimAnimation>().animator.GetCurrentAnimatorClipInfo(0).Length -0.5f;
		}
		
		yield return new WaitForSeconds(meleeAttack.meleeCast);
		c++;
		
		nextFire = Time.time + meleeAttack.meleeDelay;
		GameObject bulletShootout = Instantiate(meleeAttack.meleePrefab, attackPoint.transform.position , attackPoint.transform.rotation) as GameObject;
		bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
		
		if(c >= meleeAttack.meleeAnimation.Length){
			c = 0;
			yield return new WaitForSeconds(wait);
		}else{
			yield return new WaitForSeconds(meleeAttack.meleeDelay);
		}
		
		attacking = false;
		GetComponent<CharacterMotorC>().canControl = true;
		
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
	
	IEnumerator MeleeDash(){
		meleefwd = true;
		yield return new WaitForSeconds(0.2f);
		meleefwd = false;
	}
	
	public void SettingWeapon(){
		stat = GetComponent<Status>();
		if(onAiming){
			Camera.main.fieldOfView = 60;
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
	
	
	IEnumerator SwapWeapon(){
		/*if(reloading){
		return;
	}*/
		StopCoroutine("Reload");
		
		if(onAiming){
			Camera.main.fieldOfView = 60;
			onAiming = false;
		}
		PlayerMecanimAnimation mecanim = GetComponent<PlayerMecanimAnimation>();
		if(weaponEquip == 0){
			//Swap from Primary to Secondary
			if(!secondaryWeapon.bulletPrefab){
				yield break;	//Do Nothing If you don't have Secondary Weapon's Bullet
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
				mainModel.GetComponent<Animation>()[secondaryWeapon.equipAnimation.name].layer = 10;
				mainModel.GetComponent<Animation>().Play(secondaryWeapon.equipAnimation.name);
				freeze = true;
				float wait = mainModel.GetComponent<Animation>()[secondaryWeapon.equipAnimation.name].length;
				yield return new WaitForSeconds(wait);
				freeze = false;
			}else{
				//For Mecanim Animation
				mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 1);
				GetComponent<PlayerMecanimAnimation>().PlayAnim(secondaryWeapon.equipAnimation.name);
				freeze = true;
				float wait = GetComponent<PlayerMecanimAnimation>().animator.GetCurrentAnimatorClipInfo(0).Length -0.4f;
				yield return new WaitForSeconds(wait);
				mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 0);
				freeze = false;
			}
			
		}else{
			//Swap from Secondary to Primary
			if(!primaryWeapon.bulletPrefab){
				yield break; //Do Nothing If you don't have Primary Weapon's Bullet
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
				mainModel.GetComponent<Animation>()[primaryWeapon.equipAnimation.name].layer = 10;
				mainModel.GetComponent<Animation>().Play(primaryWeapon.equipAnimation.name);
				freeze = true;
				float wait = mainModel.GetComponent<Animation>()[primaryWeapon.equipAnimation.name].length;
				yield return new WaitForSeconds(wait);
				freeze = false;
			}else{
				//For Mecanim Animation
				mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 1);
				GetComponent<PlayerMecanimAnimation>().PlayAnim(primaryWeapon.equipAnimation.name);
				freeze = true;
				float wait = GetComponent<PlayerMecanimAnimation>().animator.GetCurrentAnimatorClipInfo(0).Length -0.4f;
				yield return new WaitForSeconds(wait);
				mecanim.animator.SetLayerWeight(mecanim.upperBodyLayer, 0);
				freeze = false;
			}
		}
		if(!useMecanim){
			GetComponent<PlayerLegacyAnimation>().SetAnimation();
		}else{
			GetComponent<PlayerMecanimAnimation>().SetAnimation();
			GetComponent<PlayerMecanimAnimation>().SetIdle();
		}
		SettingWeapon();
	}
	
	public void ForceSwap(){
		//Use when call by other script.
		StartCoroutine(SwapWeapon());
	}
	
	public void ShowMeleeWeapon (bool b){
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
			
}
[System.Serializable]
public class WeaponSet{
	public GameObject bulletPrefab;
	public Transform weaponAtkPoint;
	public AnimationClip shootAnimation;
	public float shootAnimationSpeed = 1.0f;
	public bool  animateWhileMoving = false; //Mark on If you want to play Shooting Animation while Moving.
	public AnimationClip reloadAnimation;
	public GameObject gunFireEffect;
	public AudioClip gunSound;
	public float soundRadius = 0; // Can attract the enemy to gun fire position.
	public AudioClip reloadSound;
	public int ammo = 30;
	public int maxAmmo = 30;
	public AmmoType useAmmo = AmmoType.HandgunAmmo;
	public float attackDelay = 0.15f;
	public AnimationClip equipAnimation;
	public float cameraShakeValue = 0;
	public bool  automatic = true;
	public Texture2D aimIcon;
	public Texture2D zoomIcon;
	public float zoomLevel = 30.0f;
}
[System.Serializable]
public class MeleeSet{
	public bool  canMelee = false;
	public GameObject meleePrefab;
	public AnimationClip[] meleeAnimation = new AnimationClip[3];
	public float meleeAnimationSpeed = 1.0f;
	public float meleeCast = 0.15f;
	public float meleeDelay = 0.15f;
	public AudioClip meleeSound;
}
[System.Serializable]
public class AllAmmo {
	public int handgunAmmo = 0;
	public int machinegunAmmo = 0;
	public int shotgunAmmo = 0;
	public int magnumAmmo = 0;
	public int smgAmmo = 0;
	public int sniperRifleAmmo = 0;
	public int grenadeRounds = 0;
}
[System.Serializable]
public class SkilAtk {
	public Texture2D icon;
	public Transform skillPrefab;
	public AnimationClip skillAnimation;
	public float skillAnimationSpeed = 1.0f;
	public float castTime = 0.3f;
	public float skillDelay = 0.3f;
	public int manaCost = 10;
}
[System.Serializable]
public class AtkSound{
	public AudioClip[] attackComboVoice = new AudioClip[3];
	public AudioClip magicCastVoice;
	public AudioClip hurtVoice;
}