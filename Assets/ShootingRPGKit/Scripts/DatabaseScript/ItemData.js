#pragma strict
import UnityEngine.UI;

class Usable {
	var itemName : String = "";
	var icon : Texture2D;
	var iconSprite : Sprite;
	var model : GameObject;
	var description : String = "";
	var price : int = 10;
	var hpRecover : int = 0;
	var mpRecover : int = 0;
	var shieldRecover : int = 0;
}

enum EqType {
	PrimaryWeapon = 0,
	SecondaryWeapon = 1,
	MeleeWeapon = 2,
	Armor = 3
}

enum AmmoType {
	HandgunAmmo = 0,
	MachinegunAmmo = 1,
	ShotgunAmmo = 2,
	MagnumAmmo = 3,
	SMGAmmo = 4,
	SniperRifleAmmo = 5,
	GrenadeRounds = 6,
	NoAmmo = 7
}

class LegacyAnimSet{
	var idle : AnimationClip;
	var run : AnimationClip;
	var right : AnimationClip;
	var left : AnimationClip;
	var back : AnimationClip;
	var jump : AnimationClip;
	var hurt : AnimationClip;
	var crouchIdle : AnimationClip;
	var crouchForward : AnimationClip;
	var crouchRight : AnimationClip;
	var crouchLeft : AnimationClip;
	var crouchBack : AnimationClip;
}

class Equip {
		var itemName : String = "";
		var icon : Texture2D;
		var iconSprite : Sprite;
		var model : GameObject;
		var description : String = "";
		var price : int = 10;
		var weaponAnimationType : int = 0; //Use for Mecanim
		var attack : int = 5;
		var defense : int = 0;
		var magicAttack : int = 0;
		var magicDefense : int = 0;
		var meleeDamage : int = 0;
		var shieldPlus : int = 0;
		
		var equipmentType : EqType = EqType.PrimaryWeapon; 
		
		//Ignore if the equipment type is not weapons
		var attackPrefab : GameObject;
		var attackAnimation : AnimationClip[] = new AnimationClip[1];
		var attackAnimationSpeed : float = 1.0f;
		var reloadAnimation : AnimationClip;
		var equipAnimation : AnimationClip;
		var maxAmmo : int = 30;
		var useAmmo : AmmoType = AmmoType.HandgunAmmo;
		var legacyAnimationSet : LegacyAnimSet; //Use for LegacyAnimation
		var gunFireEffect : GameObject;
		var gunSound : AudioClip;
		var soundRadius : float = 0; // Can attract the enemy to gun fire position.
		var reloadSound : AudioClip;
		var attackCast : float = 0.18; //For Melee
		var attackDelay : float = 0.12;
		var cameraShakeValue : float = 0;
		var automatic : boolean = true;
		var animateWhileMoving : boolean = false; //Mark on If you want to play Shooting Animation while Moving.
		var aimIcon : Texture2D;
		var zoomIcon : Texture2D;
		var zoomLevel : float = 30.0;
} 


var usableItem : Usable[] = new Usable[3];
var equipment : Equip[] = new Equip[3];
