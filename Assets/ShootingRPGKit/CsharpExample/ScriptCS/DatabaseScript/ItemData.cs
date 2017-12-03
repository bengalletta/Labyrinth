using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemData : MonoBehaviour {
	public Usable[] usableItem = new Usable[3];
	public Equip[] equipment = new Equip[3];
	
}
[System.Serializable]
public class Usable {
	public string itemName = "";
	public Texture2D icon;
	public Sprite iconSprite;
	public GameObject model;
	public string description = "";
	public int price = 10;
	public int hpRecover = 0;
	public int mpRecover = 0;
	public int shieldRecover = 0;
}

public enum EqType {
	PrimaryWeapon = 0,
	SecondaryWeapon = 1,
	MeleeWeapon = 2,
	Armor = 3
}

public enum AmmoType {
	HandgunAmmo = 0,
	MachinegunAmmo = 1,
	ShotgunAmmo = 2,
	MagnumAmmo = 3,
	SMGAmmo = 4,
	SniperRifleAmmo = 5,
	GrenadeRounds = 6,
	NoAmmo = 7
}
[System.Serializable]
public class LegacyAnimSet{
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
}
[System.Serializable]
public class Equip {
	public string itemName = "";
	public Texture2D icon;
	public Sprite iconSprite;
	public GameObject model;
	public string description = "";
	public int price = 10;
	public int weaponAnimationType = 0; //Use for Mecanim
	public int attack = 5;
	public int defense = 0;
	public int magicAttack = 0;
	public int magicDefense = 0;
	public int meleeDamage = 0;
	public int shieldPlus = 0;
	
	public EqType equipmentType = EqType.PrimaryWeapon; 
	
	//Ignore if the equipment type is not weapons
	public GameObject attackPrefab;
	public AnimationClip[] attackAnimation = new AnimationClip[1];
	public float attackAnimationSpeed = 1.0f;
	public AnimationClip reloadAnimation;
	public AnimationClip equipAnimation;
	public int maxAmmo = 30;
	public AmmoType useAmmo = AmmoType.HandgunAmmo;
	public LegacyAnimSet legacyAnimationSet; //Use for LegacyAnimation
	public GameObject gunFireEffect;
	public AudioClip gunSound;
	public float soundRadius = 0; // Can attract the enemy to gun fire position.
	public AudioClip reloadSound;
	public float attackCast = 0.18f; //For Melee
	public float attackDelay = 0.12f;
	public float cameraShakeValue = 0;
	public bool automatic = true;
	public bool animateWhileMoving = false; //Mark on If you want to play Shooting Animation while Moving.
	public Texture2D aimIcon;
	public Texture2D zoomIcon;
	public float zoomLevel = 30.0f;
}