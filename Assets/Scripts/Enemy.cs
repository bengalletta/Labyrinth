//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public enum Behavior
//{
//	idle, search, confused, moveToPlayer, wander, attack, useAbility
//}
//
//public class Enemy : MonoBehaviour {
//
//		public Behavior currentState = Behavior.idle;
//		private Behavior nextState = Behavior.idle;
//		private Transform playerTarget;
//
//		public int playerLayer = 1<<8;          //Just saying the player layer is on 8
//		public float viewDistance = 5;          //Distance they can see in front of them.
//		public float viewAngle = 45;            //Angle they can see a player infront of them
//		public float awareDistance = 1;         //Aware of player if they are in this distnace
//
//		public float attackDistance = 2;        //Distance at which the player can attack
//
//		private bool isStateFinished = true;
//
//		//Maybe the player stunned the enemy..
//		private bool interuptState = false;
//		public bool InteruptState
//		{
//			get{return interuptState;}
//			set{interuptState = value;}
//		}
//
//		public void Update()
//		{
//			if(currentState != nextState  (isStateFinished || interuptState))
//			{
//				currentState = nextState;
//				isStateFinished = false;
//				switch(currentState)
//				{
//				case Behavior.idle:
//					Idle();
//					break;
//				case Behavior.search:
//					Search();
//					break;
//				case Behavior.confused:
//					Confused();
//					break;
//				case Behavior.MoveToPlayer:
//					MoveToPlayer();
//					break;
//				case Behavior.attack:
//					Attack();
//					break;
//				case Behavior.useAbility:
//					UseAbility();
//					break;
//				}
//			}
//
//		}
//
//		#region Behavior methods
//		private void Idle()
//		{
//			interuptState = true;
//			List<GameObject> viewedPlayers = CheckIfPlayerInView();
//			if(viewedPlayers.Count > 0)
//			{
//				playerTarget = viewedPlayers[0];    //Grab the first.. Just for an example.. Maybe it would check distance and get the closest.
//				currentState = Behavior.Move();
//			}
//			isStateFinished = false;
//		}
//
//		private void Search()
//		{
//			//Randomly move around and search?
//		}
//
//		private void Confused()
//		{
//			//Could be enabled by player attack/ability
//		}
//
//		private void MoveToPlayer()
//		{
//			//Move to the player's location
//			//Check if the player is in range
//		}
//
//		private void Wander()
//		{
//			//Move to a random legal location
//		}
//
//		private void Attack()
//		{
//			// animate attack..
//			// apply damage to the player
//		}
//
//		private void UseAbility()
//		{
//			//Enable some particle effect and apply dmg to the player.
//		}
//		#endregion
//
//		#region Behavior Checking methods
//		private List<GameObject> CheckIfPlayerInView()
//		{
//			List<GameObject> resultList = new List<GameObject>();
//			hit = Physics.OverlapSphere(this.transform.position, viewDistance, playerLayer);
//
//			foreach (Collider c in hit)
//			{
//				if (Vector3.Angle(this.transform.forward, c.transform.position - from.position) <= viewAngle)
//				{  
//					//Check if alive.. or whatever else before adding.
//					resultList.Add(c.gameObject);
//				}
//
//			}
//			return resultList;
//		}
//		#endregion
//
//
//}
