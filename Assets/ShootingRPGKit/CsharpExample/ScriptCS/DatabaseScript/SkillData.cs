using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillData : MonoBehaviour {
	public Skil[] skill = new Skil[3];
}

[System.Serializable]
public class Skil{
	public string skillName = "";
	public Texture2D icon;
	public Sprite iconSprite;
	public string description = "";
	public Transform skillPrefab;
	public AnimationClip skillAnimation;
	public int manaCost = 10;
	public float castTime = 0.3f;
	public float skillDelay = 0.3f;
}