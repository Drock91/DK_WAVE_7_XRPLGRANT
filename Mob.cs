using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
namespace dragon.mirror{
public class Mob : MovingObject
{
	public int CurrentPatrolIndex { get; set; }
    public bool IsMovingForward { get; set; }
	public List<Vector3> PatrolPath { get; set; }
    [SerializeField] GameObject castBarPrefab;
	[SyncVar]
	[SerializeField] public string NAME;
	[SyncVar]
	public Match assignedMatch;
    [SyncVar]
	[SerializeField] public MovingObject activeTarget;
    //private Pathfinding pathfinding;
	//Integrating our new code from enemytestmovements here
    LayerMask movingObjects;
    LayerMask blockingObjects;
	//[SerializeField] public bool targetting = true;
	[SerializeField] public float Vision = 5f;
	[SerializeField] public float Hearing = 5f;
	[SerializeField] public float Smell = 5f;
	[SerializeField] public bool Resetting = false;
	[SerializeField] public bool Searching = true;
	[SerializeField] public bool Ranged = false;

 	//public bool OriginTaken = false;

	public Dictionary<MovingObject, float> threatList = new Dictionary<MovingObject, float>();
	public float MaxDistanceFromOrigin = 15f;
	[SerializeField] public string groupNumber;
	[SerializeField] public bool Aggro = false;
	//[SerializeField] public bool ChillOut = false;
	[SerializeField] public bool SwitchFlip = true;
    [SerializeField] public static UnityEvent<Mob>  MobFogPosition = new UnityEvent<Mob>();
    [SerializeField] public static UnityEvent<MovingObject, Match>  MobDiedRemovePossibleTarget = new UnityEvent<MovingObject, Match>();


	public IMobState currentState;
    
	// State Interface and Classes

    public interface IMobState
    {
        void Execute(Mob mob, TurnManager turnManager);
    }
	public class DeathState : IMobState
    {
        public void Execute(Mob mob, TurnManager turnManager)
        {
		    // implement Death behavior here

			turnManager.RemoveMob(mob.assignedMatch, mob);
        }
    }
	public class ResetState : IMobState
    {
        public void Execute(Mob mob, TurnManager turnManager)
        {
			for (int i = 0; i < mob.curatorTM.MobGroups[mob.groupNumber].Count; i++)
			{
			    Mob currentMob = mob.curatorTM.MobGroups[mob.groupNumber][i];

			    currentMob.Resetting = true;
			    currentMob.cur_hp = currentMob.max_hp;
			    currentMob.cur_mp = currentMob.max_mp;
			    currentMob.stamina = -100f;
				currentMob.Target = null;
			    currentMob.threatList.Clear();
				currentMob.moving = true;
				currentMob.agent.isStopped = false;
			    currentMob.agent.SetDestination(currentMob.Origin);
			}

        }

    }
   
    public class PatrolState : IMobState
    {
        public void Execute(Mob mob, TurnManager turnManager)
        {
			//print($"Running Execute on PatrolState");
		    // implement Patrol behavior here
        }
    }
    public class EnemyActionState : IMobState
    {
        public void Execute(Mob mob, TurnManager turnManager)
        {
			if(mob.GetATTACKING() != null){
				mob.StopATTACKINGMob();
			}
			// Initialize variables to store the highest threat and corresponding player.
			//float distanceCheck = Vector2.Distance(mob.Origin, mob.transform.position);
			//if(distanceCheck > 10f){
            //    mob.TransitionToState(new ResetState(), "ResetState", mob.curatorTM); 
			//	return;
			//} else {
			//	print($"Mob was in range {distanceCheck} was our distance");
			//}

        	// Here, highestThreatPlayer will be the player with the highest threat, or null if no players exist in the threat list.
        	// You can now use highestThreatPlayer as the target.

        	// For example:
        	
				if(mob.Target != null && !mob.Target.Stealth){
					mob.SetATTACKING(mob.Target);
				} else {
                	mob.TransitionToState(new OnGuardState(), "OnGuardState", mob.curatorTM); 
				}
        	    // implement Enemy Action behavior here
        	    //turnManager.EnemyAction(mob.assignedMatch, mob);
        	
				
        	    // Handle the case where there is no target
        }
    }
	public class OnGuardState : IMobState
    {
		//Idle waiting
        public void Execute(Mob mob, TurnManager turnManager)
        {
        	
        	MovingObject highestThreatPlayer = mob.GetHighestThreat();

			if(highestThreatPlayer != null){
				mob.Target = highestThreatPlayer;
				mob.StartCoroutine(mob.HandleAttackTarget());

			} else {
				mob.StartCoroutine(mob.HandlePlayersInRadius());
			}
            // implement Enemy Action behavior here
			//print($"Running Execute on OnGuardState");

        }
    }
	private bool isTriggerStay = true;
	protected override void Awake ()
    {
		if(isServer){
		}
		base.Awake();
    }
	
    protected override void Start()
    {
		if(isServer){
			MovingObject.UnStealthedChar.AddListener(ReAddToThreat);
		}
        base.Start();
    }
	protected override void Update()
    {
		if(isServer && !Dying){
		// Increment the timer by the time since the last frame.
    		// If the timer is greater than 1 second,
    		// reset it back to 0.
    		
			//if(Resetting){
			//	float distanceCheck = Vector2.Distance(Origin, transform.position);
			//	if(distanceCheck < 2f){
    		//        Resetting = false; 
			//	} 
			//}
		}
		base.Update();
    }
	void ReAddToThreat(MovingObject player, Match match){
		if(match == assignedMatch){
			StartCoroutine(HandlePlayersInRadius());
		}
	}
	public MovingObject GetHighestThreat(){
		
		float highestThreat = float.MinValue;
        	MovingObject highestThreatPlayer = null;

        	// Get the list of players from the threat dictionary.
        	var players = new List<MovingObject>(threatList.Keys);
			for (int i = 0; i < players.Count; i++)
        	{
				if(players[i] == null){
					continue;
				}
        	    // Get the threat for this player.
        	    float newThreat = threatList[players[i]];

        	    // Check if this threat is higher than the current highest threat.
        	    if (newThreat > highestThreat)
        	    {
        	        // If so, update the highest threat and corresponding player.
        	        highestThreat = newThreat;
        	        highestThreatPlayer = players[i];
        	    }
        	}
			
			return highestThreatPlayer;
	}
	IEnumerator HandleAttackTarget()
{
	yield return new WaitForSeconds(0.5f);
	if(Mesmerized){
		yield break;
	}
	TransitionToState(new EnemyActionState(), "EnemyActionState", curatorTM);
}
	IEnumerator HandlePlayersInRadius()
{
	yield return new WaitForSeconds(0.5f);
	if(Mesmerized){
		yield break;
	}
    // Get all colliders in the specified radius
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Vision);

    // Iterate through the colliders
    foreach (Collider2D collider in colliders)
    {
        // Try to get a PlayerCharacter component from the collider's GameObject
        PlayerCharacter player = collider.GetComponent<PlayerCharacter>();

        // If the GameObject has a PlayerCharacter component, handle the player
        if (player != null && HasLineOfSight(player.transform.position, transform.position))
        {
            HandleCharacter(player);
			yield break;
        }
    }

}
	void HandleCharacter(PlayerCharacter character)
{
    // Your existing logic here
	if (character != null && !character.Stealth)
        {
			if(!HasLineOfSight(this.transform.position, character.transform.position)){
				return;
			}
			if (!threatList.ContainsKey(character)){
                threatList.Add(character, 1f);
				foreach(var groupMember in curatorTM.MobGroups[groupNumber]){
					if (!groupMember.threatList.ContainsKey(character)){
                		groupMember.threatList.Add(character, 1f);
					}
					if(!groupMember.Target){
						groupMember.Target = character;
						groupMember.DamageTaken();
					}
				}
			} else {
				foreach(var groupMember in curatorTM.MobGroups[groupNumber]){
					if (!groupMember.threatList.ContainsKey(character)){
                		groupMember.threatList.Add(character, 1f);
					}
					if(!groupMember.Target){
						groupMember.Target = character;
						if(!groupMember.Mesmerized){
							groupMember.DamageTaken();
						}
					}
					return;// this way we call ours
				}
			}
            // Set the player as the target
        	MovingObject highestThreatPlayer = GetHighestThreat();
			if(highestThreatPlayer != null){
				if(Target == null){
					Target = highestThreatPlayer;
					if(!Mesmerized){
						StartCoroutine(HandleAttackTarget());
					}
				} else {
					if(highestThreatPlayer != Target){
						Target = highestThreatPlayer;
						if(!Mesmerized){
							StartCoroutine(HandleAttackTarget());
						}
					}
				}
			}
		} 
}
private IEnumerator PulseCheck()
{
	float pulseInterval = 1f;
    while (!Dying)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, Vision);
        foreach (Collider2D other in hits)
        {
            // Check if the entering collider is a PlayerCharacter
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();
            if (player != null)
            {
                if (!HasLineOfSight(this.transform.position, player.transform.position))
                {
                    print($"Does NOT have line of sight of {player.CharacterName}");
                    continue;
                }
                else
                {
                    print($"Does have line of sight of {player.CharacterName}");
        	// Check if the entering collider is a PlayerCharacter
        	
				if (!threatList.ContainsKey(player)){
        	        threatList.Add(player, 1f);
					foreach(var groupMember in curatorTM.MobGroups[groupNumber]){
						if (!groupMember.threatList.ContainsKey(player)){
        	        		groupMember.threatList.Add(player, 1f);
						}
						if(!groupMember.Target){
							groupMember.DamageTaken();
						}
					}
				} else {
					foreach(var groupMember in curatorTM.MobGroups[groupNumber]){
						if (!groupMember.threatList.ContainsKey(player)){
        	        		groupMember.threatList.Add(player, 1f);
						}
						if(!groupMember.Target){
							groupMember.DamageTaken();
						}
					}
					continue;
				}
					MovingObject highestThreatPlayer = GetHighestThreat();
					if(highestThreatPlayer != null){
						if(Target == null){
							Target = highestThreatPlayer;
							StartCoroutine(HandleAttackTarget());
						} else {
							if(highestThreatPlayer != Target){
								Target = highestThreatPlayer;
								StartCoroutine(HandleAttackTarget());
							}
						}
					}
				
        	    // Set the player as the target
        		

                // your threat list logic here
            }
        }
		}

        // Wait for the defined pulse interval (in seconds) before pulsing again
        yield return new WaitForSeconds(pulseInterval);
    }
}






	void OnTriggerEnter2D(Collider2D other){
		if(isServer && !Dying && !Resetting){
        	// Check if the entering collider is a PlayerCharacter
        	PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        	if (player != null){
				if(!HasLineOfSight(this.transform.position, player.transform.position)){
					print($"Does NOT have line of sight of {player.CharacterName}");
					return;
				} else {
					print($"Does have line of sight of {player.CharacterName}");
				}

				if (!threatList.ContainsKey(player)){
        	        threatList.Add(player, 1f);
					foreach(var groupMember in curatorTM.MobGroups[groupNumber]){
						if (!groupMember.threatList.ContainsKey(player)){
        	        		groupMember.threatList.Add(player, 1f);
						}
						if(!groupMember.Target){
							groupMember.DamageTaken();
						}
					}
				} else {
					foreach(var groupMember in curatorTM.MobGroups[groupNumber]){
						if (!groupMember.threatList.ContainsKey(player)){
        	        		groupMember.threatList.Add(player, 1f);
						}
						if(!groupMember.Target){
							groupMember.DamageTaken();
						}
					}
					return;
				}
        	    // Set the player as the target
        		MovingObject highestThreatPlayer = GetHighestThreat();
				if(highestThreatPlayer != null){
					if(Target == null){
						Target = highestThreatPlayer;
						StartCoroutine(HandleAttackTarget());
					} else {
						if(highestThreatPlayer != Target){
							Target = highestThreatPlayer;
							StartCoroutine(HandleAttackTarget());
						}
					}
				}
			}
        }
    }
	public void ExecuteCurrentState(TurnManager turnManager) {
    if(currentState != null) {
        currentState.Execute(this, turnManager);
    }
}
	public void TransitionToState(IMobState state, string mobState, TurnManager turnManager) {
        currentState = state;
		//print($"{gameObject.name} transitioned to {mobState}");
		ExecuteCurrentState(turnManager);
    }
	
	public void DamageTaken(){
		if(Mesmerized){
			return;
		}
		MovingObject newTarget = GetHighestThreat();
		if(newTarget){
			Target = newTarget;
		}
		StartCoroutine(HandleAttackTarget());
	}
	public void ResettingMob(){
        TransitionToState(new ResetState(), "ResetState", curatorTM); // 
	}
	public void EnergySpark(MovingObject obj){
		if(obj == this){
			//currentState = new OnGuardState();
			StartCoroutine(EnergyUpdaterMob());
			StartCoroutine(PulseCheck());
		}
	}
	[Server]
        public IEnumerator EnergyUpdaterMob() {
			float rechargeTime = .5f;
			float haste = 0f;
			Energized = true;
        	stamina = 0f;
			int dodgeC = 0;
			dodgeC = dodge;
			SetStatsServer();
        	//StartCoroutine(UpdateAgentRoutineMob());

            while (Energized && !Dying) {
                if (!Casting && !Mesmerized && !Stunned && !Feared && !Resetting) {
					if(GetAgility() > 101){
						haste = Mathf.Floor((GetAgility() - 100) / 2);
					} else {
						haste = 0f;
					}
					rechargeTime = .5f / (1f + (haste / 100f));
					stamina -= 5f;
        			stamina = Mathf.Clamp(stamina, -100f, 250f);
                    //if (stamina == -100f && SwitchFlip) {
                    //    if (PatrolPath == null) {
                    //        if (threatList.Count > 0 && !Dying){
                    //            SwitchFlip = false;
                    //            TransitionToState(new EnemyActionState(), "EnemyActionState", curatorTM); // New EnemyActionState
                    //        }
                    //    } else {
                    //        TransitionToState(new PatrolState(), "PatrolState", curatorTM); // New PatrolState
                    //    }
                    //}
                    yield return new WaitForSeconds(rechargeTime);
                } else {
                    yield return null;
                }
            }
        }
		//[Server]
		//public void MobAggroed(MovingObject obj){
		//	if (threatList.Count == 0 && stamina == -100f && !Resetting && Searching) {
        //        TransitionToState(new AggroState(obj), "AggroState", curatorTM); // New AggroState
        //    }
		//}
		IEnumerator UpdateAgentRoutineMob() {
        while (true) {
            if(isServer && Energized)
            {
                //if(transform.position.z != 0){
                //    transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                //}
                if(!agent.enabled){
                    yield return new WaitForSeconds(.25f);
                    continue;
                }
                if (!agent.pathPending && agent.enabled)
                {
                    if(moving && Casting){
                        ServerStopCasting();
                    }
                    if (agent.remainingDistance <= GetAttacKRange() && agent.enabled)
                    {
                        RadiusLock = false;
                        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f && agent.enabled)
                        {
                            if (moving) // If the agent was previously moving
                            {
                                moving = false;
                                // If agent has moved at least 1 unit since last update
                                if (accumulatedDistance >= 1f)
                                {
                                    accumulatedDistance = 0; // Reset the accumulated distance
                                    Vector3 updateLocation = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f, 0);
                                    NewFogUpdate(updateLocation); // Run NewFogUpdate
                                }

                            }
                        }
                    }
                    else
                    {
                        accumulatedDistance += Vector3.Distance(agent.transform.position, lastPosition); // Update the accumulated distance
                        if (accumulatedDistance >= 1f)
                        {
                            accumulatedDistance = 0; // Reset the accumulated distance
                            Vector3 updateLocation = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f, 0);
                            NewFogUpdate(updateLocation); // Run NewFogUpdate
                        }
                        lastPosition = agent.transform.position; // Update the last position
                    }
                }

            }
            yield return new WaitForSeconds(.25f);
        }
    }
	public string GetName(){
		return NAME;
	}
	[Server]
	public IEnumerator MobCastingSpell(string spellName, float duration, int cost, MovingObject target){
		Casting = true;
		float timer = 0f;
		while(Casting && duration > timer){
			timer += Time.deltaTime;
			yield return null;
		}
		Casting = false;
		if(timer >= duration){
			// cast spell
			curatorTM.EnemyCastingSpell(assignedMatch, this, spellName, cost, target);
		} else {
			RpcMobInterrupted();
		}
	}
	[ClientRpc]
	void RpcMobInterrupted(){
		CancelCast.Invoke(this);
	}
	[Server]
	public void SetGroupNumber(string gNum){
		groupNumber = gNum;
	}
	
	public IEnumerator AddingThreatNewmod(MovingObject pc, float amount) {
		while (!SwitchFlip) {
			yield return null;
		}
			if (threatList.ContainsKey(pc)) {
        	    threatList[pc] += amount;
				if(activeTarget != null){
					if(pc != activeTarget){
						if(threatList[pc] > threatList[activeTarget]){
							activeTarget = null;
						}
					}
				}
        	} else if(!threatList.ContainsKey(pc)){
				threatList.Add(pc, amount);
				if(activeTarget != null){
					if(pc != activeTarget){
						if(threatList[pc] > threatList[activeTarget]){
							activeTarget = null;
						}
					}
				}
			}
	}
	public IEnumerator ResetPosition(List<PathNode> path)
	{	
		Movement(path[0].position);
		RpcPlayWalkingSound();
		path.Remove(path[0]);
		while(LerpInProgress)
			yield return new WaitForSeconds(.01f);
		if(path.Count > 0){
			StartCoroutine(ResetPosition(path));
		} else {
			threatList.Clear();
			activeTarget = null;
			moving = false;
			Aggro = false;
			Searching = true;
			//targetting = true;
			SwitchFlip = true;
			//ChillOut = false;
			stamina = -100f;
			//print($"* Reset mob ** {gameObject.name}!!");
			//foreach(Mob enemy in curatorTM.GetENEMYList()){
            //	if(enemy.ChillOut){
            //	    enemy.ChillOut = false;
            //	}
        	//}
		}
	}
  
	[ClientRpc]
	public void ClientMobFog(){
		MobFogPosition.Invoke(this);

	}
    bool AttackRange(Vector2 enemy, MovingObject player){
        float xDifference = Mathf.Abs(enemy.x - player.transform.position.x);
        float yDifference = Mathf.Abs(enemy.y - player.transform.position.y);
        if(xDifference <= 1.59f && yDifference <= 1.59f){
            return true;
        }
        return false;
    }
	public void AddStaminaMob(float delay){
		if(stamina + delay > 250f){
			stamina = 250f;
		} else {
        	stamina += delay;
		}
    }
	public void SetMATCH(Match match){
		assignedMatch = match;
	}
	public override void OnStartServer()
	{	
		#if UNITY_SERVER
		PlayFabServer.ENDMATCHFULLY.AddListener(SelfDestruction);
		#endif
		//SetCharactersForTargetting();
	}
	void SelfDestruction(Match match){

		if(match == assignedMatch){
			Destroy(this.gameObject);
		}
	}
    [Server]
    public void Die()
    {
		//RpcSpawnDeath();
		Dying = true;
		StopATTACKINGMob();
		Target = null;
		CircleCollider2D[] circleColliders = GetComponents<CircleCollider2D>();
    	foreach (CircleCollider2D circleCollider in circleColliders)
    	{
    	    circleCollider.enabled = false;
    	}
		Energized = false;
		moving = false;
		agent.isStopped = true;
		agent.enabled = false;
		// If the agent has CircleColliders, disable them
    	
		RpcRemoveMobCast();
		RpcSpawnDeath();
		//process all of death here
		//DeathMob.Invoke(assignedMatch, this);
        TransitionToState(new DeathState(), "DeathState", curatorTM); // New ResetState
    }
	[ClientRpc]
	void RpcRemoveMobCast(){
		CancelCast.Invoke(this);
	}
	[ClientRpc]
	public void RpcMobCasting(float castTime, string spell){
		GameObject castBarObject = Instantiate(castBarPrefab, new Vector3(transform.position.x, transform.position.y - .8f, 0f) , Quaternion.identity);
        CastbarEnemy castbar = castBarObject.GetComponent<CastbarEnemy>();
        castbar.SetMob(this, castTime, spell);
	}
	
	[Server]
	public IEnumerator ProcessDeath(){
		//while(thinking){
		RpcMobEXPCPDisplay(CLASSPOINTS, (int)EXPERIENCE, TIER);
		//}
		//Destroy(this.gameObject);
		
		MobDiedRemovePossibleTarget.Invoke(this, assignedMatch);
		yield return new WaitForSeconds(.1f);
		
		StartCoroutine(WaitTwoMinutes());
		
	}
	IEnumerator WaitTwoMinutes(){
		yield return new WaitForSeconds(120);
		Destroy(this);
	}
	 public void OnMouseEnter(){
		if (isServer)
    	{
    	    return;
    	}
		if(Dying){
			return;
		}
		this.transform.GetComponent<SpriteRenderer>().color = new Color32 (208,70,72, 255);
    	MouseOverCombat mouseOverBox = GameObject.Find("MouseOverCombat").GetComponent<MouseOverCombat>();
		if(!mouseOverBox){
			return;
		}
		SpriteRenderer sRend = GetComponent<SpriteRenderer>();
        if(sRend){
            if(!sRend.enabled){
				return;
            }
        }
    	Canvas mouseOverBoxCanvas = mouseOverBox.GetComponent<Canvas>();
    	mouseOverBoxCanvas.enabled = true;
    	mouseOverBox.InjectName(NAME);
    	mouseOverBox.transform.position = Input.mousePosition + new Vector3(100, 100, 0);
        HoverNoise.Invoke();
    }
    public void OnMouseExit(){
		if(isServer){
			return;
		}
		if(Dying){
			return;
		}
		this.transform.GetComponent<SpriteRenderer>().color = new Color32(255,255,255,255);
		MouseOverCombat mouseOverBox = GameObject.Find("MouseOverCombat").GetComponent<MouseOverCombat>();
		if(!mouseOverBox){
			return;
		}
    	Canvas mouseOverBoxCanvas = mouseOverBox.GetComponent<Canvas>();
		mouseOverBoxCanvas.enabled = false;
    }
}
}