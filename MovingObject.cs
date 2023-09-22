using UnityEngine; 
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace dragon.mirror{
//The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
public class StatModifier
{
    public enum Stat { Agility, Strength, Fortitude, Arcana, MagicResistance, PoisonResistance, DiseaseResistance, ColdResistance, FireResistance, Armor }
    public Stat TargetStat { get; private set; }
    public int Value { get; private set; }
    public float Duration { get; private set; }
    public string Spell { get; private set; }
    public bool Buff { get; private set; }
    public StatModifier(Stat targetStat, int value, float duration, string spellName, bool buff)
    {
        TargetStat = targetStat;
        Value = value;
        Duration = duration;
		Spell = spellName;
		Buff = buff;
    }
}
public class StatsHandler
{
    public int Agility { get; private set; }
    public int Strength { get; private set; }
    public int Fortitude { get; private set; }
    public int Arcana { get; private set; }
    public int Armor { get; private set; }
    public int MagicResist { get; private set; }
    public int FireResist { get; private set; }
    public int ColdResist { get; private set; }
    public int DiseaseResist { get; private set; }
    public int PoisonResist { get; private set; }
    public int Dodge { get; private set; }

    private List<StatModifier> activeModifiers = new List<StatModifier>();
	MovingObject ourObject;
	public void SetInitialStats(MovingObject obj, int agility, int strength, int fortitude, int arcana, int armor, int mr, int fr, int cr, int dr, int pr, int dodge)
    {
		ourObject = obj;
        Agility = agility;
        Strength = strength;
        Fortitude = fortitude;
        Arcana = arcana;
		Armor = armor;
		MagicResist = mr;
		FireResist = fr;
		ColdResist = cr;
		DiseaseResist = dr;
		PoisonResist = pr;
		ourObject.SetAgility(Agility);
		Dodge = dodge;
			

    }
    public void ApplyStatModifier(StatModifier modifier, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(HandleStatModifier(modifier, monoBehaviour));
    }
	private IEnumerator HandleStatModifier(StatModifier modifier, MonoBehaviour monoBehaviour)
	{
	    // Find the existing active modifier with the same spellName
	    StatModifier existingModifier = activeModifiers.Find(x => x.Spell == modifier.Spell);
	    if (existingModifier != null)
	    {
	        // Remove the existing modifier's effect
	        ChangeStat(existingModifier.TargetStat, -existingModifier.Value);
	        // Remove the existing modifier from the activeModifiers list
	        activeModifiers.Remove(existingModifier);
	    }
	    activeModifiers.Add(modifier);
	    ChangeStat(modifier.TargetStat, modifier.Value);
	    yield return new WaitForSeconds(modifier.Duration);
	    ChangeStat(modifier.TargetStat, -modifier.Value);
	    activeModifiers.Remove(modifier);
	}
    private void ChangeStat(StatModifier.Stat stat, int value)
    {
        switch (stat)
        {
            case StatModifier.Stat.Agility:
                Agility += value;
				ourObject.SetAgility(Agility);
                break;
            case StatModifier.Stat.Strength:
                Strength += value;
                break;
            case StatModifier.Stat.Fortitude:
                Fortitude += value;
                break;
            case StatModifier.Stat.Arcana:
                Arcana += value;
                break;
        }
    }
}
public abstract class MovingObject : NetworkBehaviour
{
	//Integrations
	//EndIntegrations
	[SyncVar]
	[SerializeField] public bool friendly;
	private const string CastingQ = "CastingQ";
    private const string CastingE = "CastingE";
    private const string CastingR = "CastingR";
    private const string CastingF = "CastingF";
    private const string Selected = "Selected";
	public NavMeshAgent agent;
	Color32 RedColorRef = new Color32 (208,70,72, 255);
	Color32 YellowColorRef = new Color32 (218, 212, 94, 255);
	Color32 GrayColorRef = new Color32 (128, 128, 128, 255);
	Color32 GreenColorRef = new Color32(58, 255, 0, 255);
	[SyncVar]
	public MovingObject Target;
	[SerializeField] GameObject ArrowPrefab;
	[SerializeField] GameObject SpellCasterAutoAttackPrefab;
    public static UnityEvent HoverNoise = new UnityEvent();
	[SerializeField] private Sprite TombStoneSprite;
	[SerializeField] private Sprite BloodyDeathSprite;
	[SyncVar]
	[SerializeField] public bool Dying = false;
    public Color originalColor;
    public LayerMask blockingLayer;         //OLD PHASE THIS OUT NOT BEING USED Layer on which collision will be checked.
	public LayerMask mobCollisionLayer;		//New collision detection layer for movingobjects
    [SerializeField] public CircleCollider2D circleCollider2D;       //The BoxCollider2D component attached to this object.
    private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
	public AudioMgr audioMgr;
	private AudioSource aud;
	public Sprite offSprite;
	public Sprite mainSprite;
    //Game Mechanics Variables
	[SerializeField] public int TIER;
	[SerializeField] public float EXPERIENCE;
	[SerializeField] public int CLASSPOINTS;
	[SerializeField] public int minDmgMH;                  //minimum damage on a hit
    [SerializeField] public int maxDmgMH;                  //maximum damage on a hit
	//resistances
	[SerializeField] public int FireResist;
	[SerializeField] public int ColdResist;
	[SerializeField] public int MagicResist;
	[SerializeField] public int DiseaseResist;
	[SerializeField] public int PoisonResist;
	[SerializeField] public int parry;
	[SerializeField] public int strength;
	[SerializeField] public int agility;
	[SerializeField] public int arcana;
	[SerializeField] public int fortitude;
	[SerializeField] public int armor;                   //armor points to remove from damage received
	[SyncVar] public int Agility;
	[SerializeField] public string mobType;
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, int, string>  TakeDamageCharacter = new UnityEvent<NetworkConnectionToClient, int, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, string>  DeathCharacter = new UnityEvent<NetworkConnectionToClient, string>();
    [SerializeField] public static UnityEvent<MovingObject>  RemoveCharReserve = new UnityEvent<MovingObject>();
    [SerializeField] public static UnityEvent<MovingObject>  RemoveFogParticipant = new UnityEvent<MovingObject>();
    [SerializeField] public static UnityEvent<MovingObject>  TargetWindowSet = new UnityEvent<MovingObject>();
    [SerializeField] public static UnityEvent<MovingObject>  TargetWasSet = new UnityEvent<MovingObject>();
    [SerializeField] public static UnityEvent<MovingObject, MovingObject, string, Vector2>  MovedToCast = new UnityEvent<MovingObject, MovingObject, string, Vector2>();
    [SerializeField] public static UnityEvent<MovingObject, Match>  DeadChar = new UnityEvent<MovingObject, Match>();
    [SerializeField] public static UnityEvent<MovingObject, Match>  UnStealthedChar = new UnityEvent<MovingObject, Match>();
    [SerializeField] public static UnityEvent<MovingObject>  CancelCast = new UnityEvent<MovingObject>();

	public Vector2 _TargetAcquired;
	public bool BonusFireWeapon= false;
	public int BonusFireEffect = 0;
	public bool BonusColdWeapon= false;
	public bool FrozenColdWeapon= false;
	public int BonusColdEffect = 0;
	public bool BonusMagicWeapon= false;
	public int BonusMagicEffect = 0;
	public bool BonusPoisonWeapon= false;
	public int BonusPoisonEffect = 0;
	public bool BonusDiseaseWeapon= false;
	public int BonusDiseaseEffect = 0;
	public bool BonusLeechWeapon= false;
	public int BonusLeechEffect = 0;
	public bool BonusDragonWeapon= false;
	public int BonusDragonEffect = 0;
	[SerializeField] public int dodge;
	[SerializeField] public int penetration;
	[SerializeField] public int penetrationOH;
	[SerializeField] public int healingIncrease = 0;
	[SerializeField] public int healingReceivedIncrease = 0;
	[SerializeField] public int healingReduction = 0;
	[SerializeField] public bool duelWielding = false;
	[SerializeField] public bool ThreatMod = false;
	[SerializeField] public float ThreatModifier = 0;
	[SerializeField] public int minDmgOH;                  //minimum damage on a hit
    [SerializeField] public int maxDmgOH;                  //maximum damage on a hit
	[SerializeField] public bool shield = false;
	[SerializeField] public int shieldValue = 0;
	[SerializeField] public int shieldChance = 0;
	[SyncVar]
	[SerializeField] public int attackDelay;
	public Vector2 Origin;
	public Vector2 PublicOrigin;
	[SyncVar]
	[SerializeField] public string SpellQ = "Empty";
	[SyncVar]
	[SerializeField] public bool SpellQCoolDown = false;
	[SyncVar]
	[SerializeField] public float CooldownQ = 0f;
	[SyncVar]
	[SerializeField] public string SpellE = "Empty";
	[SyncVar]
	[SerializeField] public bool SpellECoolDown = false;
	[SyncVar]
	[SerializeField] public float CooldownE = 0f;
	[SyncVar]
	[SerializeField] public string SpellR = "Empty";
	[SyncVar]
	[SerializeField] public bool SpellRCoolDown = false;
	[SyncVar]
	[SerializeField] public float CooldownR = 0f;
	[SyncVar]
	[SerializeField] public string SpellF = "Empty";
	[SyncVar]
	[SerializeField] public bool SpellFCoolDown = false;
	[SyncVar]
	[SerializeField] public float CooldownF = 0f;
   	public Transform ctSliderTransformParent;
	public Transform ctSliderTransform;
	[SerializeField] public Slider ctSlider;
	public Image ctImage;
	//HealthBar variables
	public Transform healthBarTransformParent;
	public Transform healthBarTransform;
	[SerializeField] public Slider healthBarSlider;
	public Image hpImage;
	public Transform magicPointBarTransformParent;
	public Transform magicPointBarTransform;
	[SerializeField] public Slider magicPointBarSlider;
	public Image mpImage;
	//State Variables
	//True while character moves one space
	[SyncVar]
	[SerializeField] public bool LerpInProgress = false;
	//True while character is moving
	[SyncVar]
	[SerializeField] public bool moving = false;
	[SyncVar]
	[SerializeField] public bool Casting = false;
	//True while character is doing an attack bump
	//True until movingToAttack completely finishes;
	[SerializeField] public float moveTime = .15f;
	public TurnManager curatorTM;
    private string deathhexColor = "8B0000";
    private string normalHithexColor = "FFFFFF";
    private string criticalHitHexColor = "E3CA00";
	private string hpTrapHexColor = "FF0F00";
	private string mpTrapHexColor = "004CFF";//also the blue color we need fo rpsrite
	[SyncVar] [SerializeField] public bool Snared = false;
	[SyncVar] [SerializeField] public bool Mesmerized = false;
	[SyncVar] [SerializeField] public bool Feared = false;
	[SyncVar] [SerializeField] public bool Stunned = false;
	[SyncVar] [SerializeField] public bool Slowed = false;
	[SyncVar] [SerializeField] public bool Silenced = false;
	[SyncVar] [SerializeField] public bool Stealth = false;
	[SyncVar]
    [SerializeField] public bool Energized = false;
	[SyncVar]
    [SerializeField] public float stamina = 0f; 
	[SyncVar]
	[SerializeField] public int max_hp;                  //hp and mp variables
	[SyncVar]
    [SerializeField] public int cur_hp;
	[SyncVar]
	[SerializeField] public int max_mp;                  //hp and mp variables
	[SyncVar]
    [SerializeField] public int cur_mp;
    [SerializeField] GameObject PopUpTextPrefab;
	[SerializeField] private int attackDelayEnemy;
    [SerializeField] GameObject SelectedCircle;
    [SerializeField] GameObject TargetCircle;
	private Animator animator;
	[SerializeField] float attackRange;
	private StatsHandler statsHandler;
	Coroutine AnimatingSpriteCO;
	[SyncVar]
    [SerializeField] public bool RadiusLock = false;
	public float GetAttacKRange(){
		return attackRange;
	}
	[Server]
	public void SetAgility(int agility){
		if(GetComponent<PlayerCharacter>()){
			Agility = agility;
		}
	}
	[Command]
	public void CmdStopMoving(){
		if(agent.enabled){
			agent.isStopped = true;
			agent.ResetPath();
		}
		moving = false;
		RadiusLock = false;
	}
	[Command]
	public void CmdRemoveTarget(){
		if(Target != null){
			print($"Removing target {Target.gameObject.name} from {this.gameObject.name}");
		}
		Target = null;
		TargetTargetterResetBool();
	}
	[Command]
	public void CmdClearAllStopMoving(){
		if(Target != null){
			print($"Removing target {Target.gameObject.name} from {this.gameObject.name}");
		}
		Target = null;
		if(agent.enabled){
			agent.isStopped = true;
		}
		moving = false;
		RadiusLock = false;
		TargetTargetterResetBool();
	}
	[TargetRpc]
	void TargetTargetterResetBool(){
		CombatPartyView.instance.ResetTargetWindow();
	}
	public bool SelectedCircleActive(){
		return SelectedCircle.activeInHierarchy;
	}
	public bool TargetCircleActive(){
		return TargetCircle.activeInHierarchy;
	}
	public int GetDodge(){
		int dodge = statsHandler.Dodge;
		int agil = statsHandler.Agility;
		dodge += agil/20;
		return dodge;
	}
	public int GetDodgeEnemy(){
		int dodge = statsHandler.Dodge;
		return dodge;
	}
	public int GetStrength(){
		return statsHandler.Strength;
	}
	public int GetAgility(){
		return statsHandler.Agility;
	}
	public int GetFortitude(){
		return statsHandler.Fortitude;
	}
	public int GetArcana(){
		return statsHandler.Arcana;
	}
	public int GetArmor(){
		return statsHandler.Armor;
	}
	public int GetMagicResist(){
		return statsHandler.MagicResist;
	}
	public int GetFireResist(){
		return statsHandler.FireResist;
	}
	public int GetColdResist(){
		return statsHandler.ColdResist;
	}
	public int GetDiseaseResist(){
		return statsHandler.DiseaseResist;
	}
	public int GetPoisonResist(){
		return statsHandler.PoisonResist;
	}
	public Vector3 lastPosition;
	public float accumulatedDistance;
	float startingRadius = .5f;
	float startingSpeed = .5f;
	float startingAcceleration = .5f;
	float startingStoppingDistance = .5f;

	public override void OnStartClient()
	{
	    base.OnStartClient();
		animator = GetComponent<Animator>();
	
	    // Remove the range detector collider
		Mob mob = GetComponent<Mob>();
		if(!mob){
			return;
		}
	    CircleCollider2D[] circleColliders = GetComponents<CircleCollider2D>();
    	foreach (CircleCollider2D circleCollider in circleColliders)
    	{
			if(circleCollider.radius > 1f){
    	    	circleCollider.enabled = false;
			}
    	}
	}
	protected virtual void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
		startingRadius = agent.radius;
		startingSpeed = agent.speed;
		startingAcceleration = agent.acceleration;
		startingStoppingDistance = agent.stoppingDistance;

	}
	//Protected, virtual functions can be overridden by inheriting classes.
    protected virtual void Start ()
    {
		if(isServer)
		{
			Mob.MobDiedRemovePossibleTarget.AddListener(ProcessMobDeath);
			MovingObject.DeadChar.AddListener(ProcessCharacterDeath);
			if(GetComponent<Mob>()){
				cur_hp = fortitude;
				max_hp = fortitude;
				cur_mp = (int)(arcana / 7.0f);
				max_mp = (int)(arcana / 7.0f);
				attackDelayEnemy = 100;
				dodge += agility / 20;
				
			}
			statsHandler = new StatsHandler();
			lastPosition = agent.transform.position;
			rightFace = false;
    		accumulatedDistance = 0;
			
		}
		//if(!isServer && GetComponent<PlayerCharacter>()){
		//	StartCoroutine(VisionSpark());
		//}
		if(!isServer)
		{
			ScenePlayer.TargetHighlightReset.AddListener(UnTargettedMO);
			MovingObject.TargetWindowSet.AddListener(CheckTargetCircle);
			if(GetComponent<Mob>()){
				AnimatingSpriteCO = StartCoroutine(AnimatingSprite());

			}
		}	
    }
	public bool charging = false;
	[Server]
	public void ServerMOCharge(MovingObject target){

		StopATTACKINGTOMOVE();
		agent.isStopped = true;
    	agent.ResetPath();
		charging = true;
		agent.isStopped = false;
		agent.acceleration = 25f;
        agent.speed = 8f;
		agent.radius = .125f;
		StartCoroutine(ChargingUnit(target));
	}
	[Command]
	public void CmdMoveToCast(MovingObject target, string mode, float rangeToCast, Vector2 mousePosition){
		ServerMoveToCast(target, mode, rangeToCast, mousePosition);
	}
	Coroutine MoveToCast;
	[Server]
	public void ServerMoveToCast(MovingObject target, string mode, float rangeToCast, Vector2 mousePosition){
		StopATTACKINGTOMOVE();
		StopCASTINGTOMOVE();
		Casting = false;
		RpcCancelMOCast();
		agent.ResetPath();
		print($"{rangeToCast} is max range of this spell");
		MoveToCast = StartCoroutine(MovingToCast(target, mode, rangeToCast, mousePosition));
	}
	private bool isWalking = false;
	public bool rightFace;
	protected virtual void Update()
	{
		if (isServer && Energized && !Dying && agent.enabled)
		{
			bool isAtDestination = !agent.pathPending && agent.remainingDistance <= .6f;// agent.stoppingDistanc;
			if (isAtDestination != !isWalking) // State has changed
			{
				isWalking = !isAtDestination; // Update the state
				RpcUpdateWalkingState(isWalking); // Notify all clients
			}
	
			if (agent.hasPath && agent.enabled)
			{
				if (moving && Casting)
				{
					ServerStopCasting();
				}
				Vector3 movementDirection = agent.destination - transform.position;
				bool newRightFace = movementDirection.x >= 0;
				//bool newRightFace = movementDirection.x >= 0;
				if (newRightFace != rightFace)
				{
					rightFace = newRightFace;
					RpcUpdateFacingDirection(newRightFace);
				}
				accumulatedDistance += Vector3.Distance(agent.transform.position, lastPosition); // Update the accumulated distance
                if (accumulatedDistance >= 1f)
                {
                    accumulatedDistance = 0; // Reset the accumulated distance
                    Vector3 updateLocation = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f, 0);
                    NewFogUpdate(updateLocation); // Run NewFogUpdate
                }
                lastPosition = agent.transform.position; // Update the last position
            }
				if (!agent.hasPath && agent.enabled || agent.velocity.sqrMagnitude == 0f && agent.enabled){
                	RadiusLock = false;
                    if (moving) // If the agent was previously moving
                    {
                        moving = false;
                        //agent.radius *= 2;
                        //syncAgentRadius = agent.radius;
                        // If agent has moved at least 1 unit since last update
                        accumulatedDistance = 0; // Reset the accumulated distance
	                	lastPosition = agent.transform.position; // Update the last position
                    }
					Vector3 updateLocation = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f, 0);
                    NewFogUpdate(updateLocation); // Run NewFogUpdate
                }
            }
		if(!isServer && Energized){
			healthBarSlider.value = (float)cur_hp / (float)max_hp;
			magicPointBarSlider.value = (float)cur_mp / (float)max_mp;
			PlayerCharacter PC = GetComponent<PlayerCharacter>();
			Mob mob = GetComponent<Mob>();
			if(Stealth){
				ClientHide();
			}
			if(PC){
				if(PC.assignedPlayer == ScenePlayer.localPlayer){
					if(stamina < 0)
        			{
        				ctSlider.value = stamina/-100f;
						ctImage.color = YellowColorRef;
        			}
        			else
        			{
        			    ctSlider.value = stamina/250f;
        				ctImage.color = GrayColorRef;
        			}
				} else {
					if(stamina < 0)
        			{
        				ctSlider.value = stamina/-100f;

						ctImage.color = GreenColorRef;
						
        			}
        			else
        			{
        			    ctSlider.value = stamina/250f;
        				ctImage.color = GrayColorRef;
        			}
				}
			}
			if(mob){
				if(stamina < 0)
        		{
        			ctSlider.value = stamina/-100f;
					ctImage.color = RedColorRef;
        		}
        		else
        		{
        		    ctSlider.value = stamina/250f;
        			ctImage.color = GrayColorRef;
        		}
			}
		}
		if(!RadiusLock && agent.enabled){
			if(agent.radius != startingRadius)
			{
				agent.radius = startingRadius;

			}
		} else if (agent.enabled) {
			if(agent.radius == startingRadius){

				agent.radius /= 3.5f;
			}
		}
	}
	[Command]
	public void CmdCancelSpell	(){
		if(Casting){
			ServerStopCasting();
		}
	}
	[Server]
	public void ServerStopCasting(){
		Casting = false;
		RpcCancelMOCast();
	}
	[ClientRpc]
	void RpcCancelMOCast(){
		CancelCast.Invoke(this);
	}
	void ProcessCharacterDeath(MovingObject deadMO, Match match){
		
		if(deadMO == this) { return; }
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		Mob mob = GetComponent<Mob>();
		if(pc){
			if(match == pc.assignedMatch){
				if(Target == deadMO){
					StopATTACKINGMob();
					Target = null;
					//need to tell client to remove
				}
			}
		} else if(mob){
			if(match == mob.assignedMatch){
				if(mob.threatList.ContainsKey(deadMO)){
					mob.threatList.Remove(deadMO);
				}
				//if(Target == deadMO){
				//	StopATTACKINGMob();
				//	Target = null;
				//	MovingObject newTarget = mob.GetHighestThreat();
				//	if(newTarget)
				//	{
				//		mob.Target = newTarget;
				//	}
				//}
				mob.DamageTaken();

			}
		}
	}
	void ProcessMobDeath(MovingObject deadMO, Match match){
		
		if(!deadMO){
			return;
		}
		if(deadMO == this){
			return;
		}
		if(Dying){
			return;
		}
		if(!Target){
			return;
		}
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		if(pc){
			if(match == pc.assignedMatch){
				if(Target == deadMO){
					StopATTACKINGMob();
					Target = null;
					//need to tell client to remove
				}
			}
		}
	}
	
	Coroutine ATTACKING;
	public Coroutine GetATTACKING(){
		return ATTACKING;
	}
	[Server]
	public void SetATTACKING(MovingObject target){

		Mob mob = GetComponent<Mob>();
		if(agent.enabled){
			agent.ResetPath();
		}
		if(Casting){
			Casting = false;
			if(agent.enabled){
				agent.isStopped = false;
			}
			RpcCancelMOCast();
		}
		if(agent.enabled){
			agent.isStopped = false;
		}
		if(mob){
			MovingObject newTarget = mob.GetHighestThreat();
			if(newTarget){
				ATTACKING = StartCoroutine(AttackWithUnit(newTarget));
			}
		} else {
			ATTACKING = StartCoroutine(AttackWithUnit(target));
		}
	}
	void StopATTACKING(MovingObject target, Match match){
		Mob mob = GetComponent<Mob>();
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		bool sameMatch = false;
		if(pc){
			if(match == pc.assignedMatch){
				sameMatch = true;
			}

		} else if(mob) {
			if(match == mob.assignedMatch){
				sameMatch = true;
			}
		}
		if(sameMatch){
			if(Target != null){
				if(Target == target){
					if(ATTACKING != null){
						StopCoroutine(ATTACKING);
						moving = false;
						agent.isStopped = true;
						Target = null;
					}
				}
			}
		}
	}
	public void StopATTACKINGTOMOVE(){
		if(ATTACKING != null){
			StopCoroutine(ATTACKING);
			ATTACKING = null;
		}
		//moving = false;
		//agent.isStopped = true;
	}
	public void StopCASTINGTOMOVE(){
		if(MoveToCast != null){
			StopCoroutine(MoveToCast);
			MoveToCast = null;
		}
		//moving = false;
		//agent.isStopped = true;
	}
	public void StopATTACKINGMob(){
		if(ATTACKING != null){
			StopCoroutine(ATTACKING);
			ATTACKING = null;
		}
	}
	public IEnumerator MovingToCast(MovingObject target, string mode, float spellRange, Vector2 mousePosition){
		if(target){
			if(target != Target){
				Target = target;
			}
			ServerMoveToTargetPosition(target.transform.position);
			float checkTime = .1f;
	    	while (true)
	    	{
				if(Target == null || target.Dying || target.Stealth){
					if(agent.enabled){
						agent.isStopped = true;
					}
					moving = false;
					agent.isStopped = false;
					if(GetATTACKING() != null)
        			{
        			    StopATTACKINGTOMOVE();
        			}
        			RadiusLock = false;
					yield break;
				}
	    	    // Iterate through each unit
	    	    // Compute the distance to the target
	    	    float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
				Debug.Log($"Distance to Target: {distanceToTarget}, Spell Range: {spellRange}");
	    	    if (distanceToTarget <= spellRange)
	    	    {
	    	        // If it is, stop moving and start attacking
					agent.isStopped = true;
					agent.ResetPath();
					TargetCastSpell(target, mode, mousePosition);
					RpcUpdateWalkingState(false);
					yield break;
	    	    }
	    	    yield return new WaitForSeconds(checkTime);
	    	}
		} else {

		}
		
		
	}
	[TargetRpc]
	void TargetCastSpell(MovingObject target, string mode, Vector2 mousePosition){
		//MovedToCast.Invoke(this, target, mode );
		print($"Mode for auto cast TargetCastSpell is {mode}");
		ScenePlayer.localPlayer.AutoCastForCharacter(this, target, mode, mousePosition);
	}
	public IEnumerator ChargingUnit(MovingObject target){
		if(target != Target){
			Target = target;
		}
		ServerMoveToTargetPosition(target.transform.position);
		float checkTime = .1f;
	    while (true)
	    {
			if(Target == null || target.Dying){
				if(agent.enabled){
					agent.isStopped = true;
				}
				agent.speed = startingSpeed;
				agent.acceleration = startingAcceleration;
				charging = false;
				moving = false;
				agent.isStopped = false;
				if(GetATTACKING() != null)
        		{
        		    StopATTACKINGTOMOVE();
        		}
        		moving = false;
        		RadiusLock = false;
				yield break;
			}
	        // Iterate through each unit
	        // Compute the distance to the target
	        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
	        if (distanceToTarget <= attackRange)
	        {
				agent.speed = startingSpeed;
				agent.acceleration = startingAcceleration;
				charging = false;
	            // If it is, stop moving and start attacking
				agent.isStopped = true;
    			agent.ResetPath();
				if( target != null){
				if(!target.Dying){
					Vector3 movementDirection = target.transform.position - transform.position;
					bool newRightFace = movementDirection.x >= 0;
					if (newRightFace != rightFace)
					{
						rightFace = newRightFace;
						RpcUpdateFacingDirection(newRightFace);
					}
					bool dmgDealt = true;
					float criticalValue = 0;
					int value = curatorTM.GetAutoAttack(this, target);
					if(attackRange <= 3){
            			StartCoroutine(BumpTowards(target.transform.position));
					} else {
						curatorTM.PlayerSpawnRangedAttack(this, target, attackRange);
					}
					bool dodged = curatorTM.GetDodge(target);
            	    if(dodged){
						dmgDealt = false;
            	        target.RpcSpawnPopUpDodged();
            	    }
            	    bool parried = curatorTM.GetParry(target);
            	    if(parried && !dodged){
						dmgDealt = false;
            	        target.RpcSpawnPopUpParried();
            	    }
					bool blockedAll = false;
            	    bool blocked = curatorTM.GetBlock(target);
            	    if(!parried && !dodged && blocked){
            	        value = value - target.shieldValue;
            	        if(value <= 0){
							blockedAll = true;
							dmgDealt = false;
            	            target.RpcSpawnPopUpBlocked();
            	        }
            	    }
            	    if(value <= 0 && !parried && !dodged && !blockedAll){
						dmgDealt = false;
            	        target.RpcSpawnPopUpAbsorbed();
            	    }
					int threat = value;
                    if(BonusColdWeapon){
                        target.ApplyStatChange("Agility", 30f, 20, "FrozenGreatsword", false);
                        target.DamageDealt(this, BonusColdEffect, 0f, false, BonusColdEffect, COLDDAMAGECOLOR);
                    }
                    if(BonusFireWeapon){
                        target.DamageDealt(this, BonusFireEffect, 0f, false, BonusFireEffect, FIREDAMAGECOLOR);
                    }
                    if(BonusPoisonWeapon){
                        target.DamageDealt(this, BonusPoisonEffect, 0f, false, BonusPoisonEffect, POISONDAMAGECOLOR);
                    }
                    if(BonusDiseaseWeapon){
                        target.DamageDealt(this, BonusDiseaseEffect, 0f, false, BonusDiseaseEffect, DISEASEDAMAGECOLOR);
                    }
                    if(BonusMagicWeapon){
                        target.DamageDealt(this, BonusMagicEffect, 0f, false, BonusMagicEffect, MAGICDAMAGECOLOR);
                    }
					if(BonusLeechWeapon){
                	    target.DamageDealt(this, BonusLeechEffect, 0f, false, BonusLeechEffect, MAGICDAMAGECOLOR);
                	    cur_hp = cur_hp + BonusLeechEffect;
						if(cur_hp > max_hp){
							cur_hp = max_hp;
						}
                	}
                    if(ThreatMod){
                        threat = (int)(threat * ThreatModifier);
                    }
					bool wasStealthed = false;
            	    if(dmgDealt){
						if(Stealth){
							value *= 2;
							Stealth = false;
							wasStealthed = true;
						}
            	        target.DamageDealt(this, value, criticalValue, true, threat, null);
            	    }
					if(duelWielding){
						int valueOH = 0;
						bool dmgDealtOH = true;
                        valueOH = curatorTM.GetAutoAttackOffhand(this, target);
						if(wasStealthed){
							valueOH *= 2;
						}
						bool dodgedOH = curatorTM.GetDodge(target);
            	    	if(dodgedOH){
							dmgDealtOH = false;
            	    	    target.RpcSpawnPopUpDodged();
            	    	}
            	    	bool parriedOH = curatorTM.GetParry(target);
            	    	if(parriedOH && !dodgedOH){
							dmgDealtOH = false;
            	    	    target.RpcSpawnPopUpParried();
            	    	}
						bool blockedAllOH = false;
            	    	bool blockedOH = curatorTM.GetBlock(target);
            	    	if(!parriedOH && !dodgedOH && blockedOH){
            	    	    valueOH -= target.shieldValue;
            	    	    if(valueOH <= 0){
								blockedAllOH = true;
								dmgDealtOH = false;
            	    	        target.RpcSpawnPopUpBlocked();
            	    	    }
            	    	}
            	    	if(valueOH <= 0 && !parriedOH && !dodgedOH && !blockedAllOH){
							dmgDealtOH = false;
            	    	    target.RpcSpawnPopUpAbsorbed();
            	    	}
                    	int threatOH = valueOH;
						if(ThreatMod){
							threatOH = (int)(threatOH * ThreatModifier);
                    	}
						if(dmgDealtOH){
            	        	target.DamageDealt(this, valueOH, criticalValue, true, threatOH, null);
						}
                    }
		    	}
			}
				agent.radius = startingRadius;
				SetATTACKING(target);
				yield break;
	        }
	        yield return new WaitForSeconds(checkTime);
	    }
	}
	public IEnumerator AttackWithUnit(MovingObject target)
	{
		Mob mob = GetComponent<Mob>();
		if(target != Target){
			Target = target;
		}
		float checkTime = .5f;
	    while (true)
	    {
			checkTime = .25f;
			if(charging){
				checkTime = .1f;
			}
			if(Target == null || target.Dying || target.Stealth){
				if(agent.enabled){
					agent.isStopped = true;
				}
				moving = false;
				if(mob){
					if(mob.threatList.ContainsKey(target)){
						mob.threatList.Remove(target);
					}
					mob.TransitionToState(new Mob.OnGuardState(), "OnGuardState", mob.curatorTM); 
				}
				yield break;
			}
	        // Iterate through each unit
	        // Compute the distance to the target
	        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
	        // Check if the unit is within attack range
			//if(mob){
			//	float distanceCheck = Vector2.Distance(mob.Origin, mob.transform.position);
			//	if(distanceCheck > 10f ){
			//		print($"Mob was out of range {distanceCheck} was our distance, resetting mob");
            //	    mob.ResettingMob(); 
			//		yield break;
			//	} else {
			//		print($"Mob was in range {distanceCheck} was our distance");
			//	}
			//}
			
	        if (distanceToTarget <= attackRange)
	        {
				
				moving = false;
            	RadiusLock = false;

	            // If it is, stop moving and start attacking
				if(agent.enabled){
					agent.isStopped = true;
				}
				RpcUpdateWalkingState(false);
				if(stamina == -100f && HasLineOfSight(transform.position, target.transform.position)){
	            	StartAttacking(target);
					//if(mob){
					//	if(cur_mp > 0){
					//		//check spells
					//		if(CooldownQ == 0f || CooldownE == 0f || CooldownR == 0f || CooldownF == 0f){
					//			List<string> availableSpells = new List<string>();
                	//			if(CooldownQ == 0f) availableSpells.Add(SpellQ);
                	//			if(CooldownE == 0f) availableSpells.Add(SpellE);
                	//			if(CooldownR == 0f) availableSpells.Add(SpellR);
                	//			if(CooldownF == 0f) availableSpells.Add(SpellF);
                	//			if(availableSpells.Count > 0) {
                	//			    int randomIndex = UnityEngine.Random.Range(0, availableSpells.Count);
                	//			    string spellPicked = availableSpells[randomIndex];
					//				//cast spell
                	//			}
					//		} else {
	            	//			StartAttacking(target);
					//		}
					//	} else {
	            	//		StartAttacking(target);
					//	}
					//} else {
	            	//	StartAttacking(target);
					//}
				}
	        }
	        else
	        {
				if(distanceToTarget > attackRange){
					moving = true;
					if(agent.enabled){
						agent.isStopped = false;
					}
					Stealth = false;
            		RadiusLock = true;
					Vector3 updateLocation = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f, 0);
        			NewFogUpdate(updateLocation); // Run NewFogUpdate
	            	//ServerMoveToTargetPosition(target.transform.position);
					Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
    				// Find a position that is 0.6 units away from the target in the direction of the current transform
    				Vector3 destination = target.transform.position - directionToTarget * 0.6f;
    				// Move to the new destination instead of directly to the target
    				ServerMoveToTargetPosition(destination);
				}
	            // If it's not, make sure it's still moving towards the target
	        }
	        // Wait until next frame
	        yield return new WaitForSeconds(checkTime);
	    }
	}
	
	[Server]
	void StartAttacking(MovingObject target){
		Mob mob = GetComponent<Mob>();
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		Vector3 updateLocation = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f, 0);
        NewFogUpdate(updateLocation); // Run NewFogUpdate
		if( target != null){
				if(!target.Dying){
					Vector3 movementDirection = target.transform.position - transform.position;
					bool newRightFace = movementDirection.x >= 0;
					if (newRightFace != rightFace)
					{
						rightFace = newRightFace;
						RpcUpdateFacingDirection(newRightFace);
					}
					bool dmgDealt = true;
					float criticalValue = 0;
					int value = curatorTM.GetAutoAttack(this, target);
					if(attackRange <= 3){
            			StartCoroutine(BumpTowards(target.transform.position));
					} else {
						curatorTM.PlayerSpawnRangedAttack(this, target, attackRange);
					}
					bool dodged = curatorTM.GetDodge(target);
            	    if(dodged){
						dmgDealt = false;
            	        target.RpcSpawnPopUpDodged();
            	    }
            	    bool parried = curatorTM.GetParry(target);
            	    if(parried && !dodged){
						dmgDealt = false;
            	        target.RpcSpawnPopUpParried();
            	    }
					bool blockedAll = false;
            	    bool blocked = curatorTM.GetBlock(target);
            	    if(!parried && !dodged && blocked){
            	        value = value - target.shieldValue;
            	        if(value <= 0){
							blockedAll = true;
							dmgDealt = false;
            	            target.RpcSpawnPopUpBlocked();
            	        }
            	    }
            	    if(value <= 0 && !parried && !dodged && !blockedAll){
						dmgDealt = false;
            	        target.RpcSpawnPopUpAbsorbed();
            	    }
					int threat = value;
                    if(BonusColdWeapon){
                        target.ApplyStatChange("Agility", 30f, 20, "FrozenGreatsword", false);
                        target.DamageDealt(this, BonusColdEffect, 0f, false, BonusColdEffect, COLDDAMAGECOLOR);
                    }
                    if(BonusFireWeapon){
                        target.DamageDealt(this, BonusFireEffect, 0f, false, BonusFireEffect, FIREDAMAGECOLOR);
                    }
                    if(BonusPoisonWeapon){
                        target.DamageDealt(this, BonusPoisonEffect, 0f, false, BonusPoisonEffect, POISONDAMAGECOLOR);
                    }
                    if(BonusDiseaseWeapon){
                        target.DamageDealt(this, BonusDiseaseEffect, 0f, false, BonusDiseaseEffect, DISEASEDAMAGECOLOR);
                    }
                    if(BonusMagicWeapon){
                        target.DamageDealt(this, BonusMagicEffect, 0f, false, BonusMagicEffect, MAGICDAMAGECOLOR);
                    }
					if(BonusLeechWeapon){
                	    target.DamageDealt(this, BonusLeechEffect, 0f, false, BonusLeechEffect, MAGICDAMAGECOLOR);
                	    cur_hp = cur_hp + BonusLeechEffect;
						if(cur_hp > max_hp){
							cur_hp = max_hp;
						}
                	}
                    if(ThreatMod){
                        threat = (int)(threat * ThreatModifier);
                    }
					bool wasStealthed = false;
            	    if(dmgDealt){
						if(Stealth){
							value *= 2;
							Stealth = false;
							wasStealthed = true;
						}
            	        target.DamageDealt(this, value, criticalValue, true, threat, null);
            	    }
					if(duelWielding){
						int valueOH = 0;
						bool dmgDealtOH = true;
                        valueOH = curatorTM.GetAutoAttackOffhand(this, target);
						if(wasStealthed){
							valueOH *= 2;
						}
						bool dodgedOH = curatorTM.GetDodge(target);
            	    	if(dodgedOH){
							dmgDealtOH = false;
            	    	    target.RpcSpawnPopUpDodged();
            	    	}
            	    	bool parriedOH = curatorTM.GetParry(target);
            	    	if(parriedOH && !dodgedOH){
							dmgDealtOH = false;
            	    	    target.RpcSpawnPopUpParried();
            	    	}
						bool blockedAllOH = false;
            	    	bool blockedOH = curatorTM.GetBlock(target);
            	    	if(!parriedOH && !dodgedOH && blockedOH){
            	    	    valueOH -= target.shieldValue;
            	    	    if(valueOH <= 0){
								blockedAllOH = true;
								dmgDealtOH = false;
            	    	        target.RpcSpawnPopUpBlocked();
            	    	    }
            	    	}
            	    	if(valueOH <= 0 && !parriedOH && !dodgedOH && !blockedAllOH){
							dmgDealtOH = false;
            	    	    target.RpcSpawnPopUpAbsorbed();
            	    	}
                    	int threatOH = valueOH;
						if(ThreatMod){
							threatOH = (int)(threatOH * ThreatModifier);
                    	}
						if(dmgDealtOH){
            	        	target.DamageDealt(this, valueOH, criticalValue, true, threatOH, null);
						}
                    }
					if(pc){
						ChargeSwingDelay();
					} else {
						mob.AddStaminaMob(mob.GetAttackDelayEnemy());
					}
					return;
		    	}
			}
	}
	public float GetStartSpeed(){
		return startingSpeed;
	}
	public float GetStartAcceleration(){
		return startingAcceleration;
	}
	[Server]
	public void ServerMoveToTargetPosition(Vector3 targetPosition){
		if(agent.enabled){
			agent.isStopped = false;
			agent.ResetPath();
			agent.SetDestination(new Vector3(targetPosition.x, targetPosition.y, transform.position.z));
		}
	}
	[Server]
	public void ServerMoveToTarget(MovingObject target){
		if(agent.enabled){
			agent.isStopped = false;
			agent.ResetPath();
			agent.SetDestination(target.transform.position);
		}
	}
	[Server]
	public void ServerMoveToAttackTarget(MovingObject target){
		if(agent.enabled){
			agent.isStopped = false;
			agent.ResetPath();
			agent.SetDestination(target.transform.position);
		}
	}
	void CheckTargetCircle(MovingObject moCheck){

		if(moCheck != this){
			//print($"{gameObject.name} was NOT moCheck!!");
			UnTargettedMO();
			return;
		}
		//print($"{gameObject.name} was ******** moCheck!!");

	}
	[Server]
	public IEnumerator SetAbilityCoolDownQ(float duration, bool setup){
		//print($"Starting Cooldown for {SpellQ} at {duration}");
        SpellQCoolDown = true;
        CooldownQ = duration;
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		if(pc){
			if(!setup){
				DateTime endTime = DateTime.Now.AddSeconds(duration); // Adding duration in seconds to current time
				CharacterCooldownListItem coolie = (new CharacterCooldownListItem {
					SpellnameFull = SpellQ,
					Value = endTime.ToString(),
					Position = "SPELLQ",
					PKey = "COOLDOWNQ"
				});
				print($"{coolie.SpellnameFull} is the spell we are putting on cooldown");
				pc.assignedPlayer.ServerCooldownSave(pc.CharID, coolie);
			}		
		}
        while (CooldownQ > 0f)
        {
            CooldownQ -= Time.deltaTime;
            yield return null;
        }
        CooldownQ = 0f;
        SpellQCoolDown = false;
    }
	[Server]
	public IEnumerator SetAbilityCoolDownE(float duration, bool setup){
		//print($"Starting Cooldown for {SpellE} at {duration}");
        SpellECoolDown = true;
        CooldownE = duration;
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		if(pc){
			if(!setup){
				DateTime endTime = DateTime.Now.AddSeconds(duration); // Adding duration in seconds to current time
				CharacterCooldownListItem coolie = (new CharacterCooldownListItem {
					SpellnameFull = SpellE,
					Value = endTime.ToString(),
					Position = "SPELLE",
					PKey = "COOLDOWNE"
				});
				print($"{coolie.SpellnameFull} is the spell we are putting on cooldown");
				pc.assignedPlayer.ServerCooldownSave(pc.CharID, coolie);
			}		
		}
        while (CooldownE > 0f)
        {
            CooldownE -= Time.deltaTime;
            yield return null;
        }
        CooldownE = 0f;
        SpellECoolDown = false;
    }
	[Server]
	public IEnumerator SetAbilityCoolDownR(float duration, bool setup){
		//print($"Starting Cooldown for {SpellR} at {duration}");
        SpellRCoolDown = true;
        CooldownR = duration;
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		if(pc){
			if(!setup){
				DateTime endTime = DateTime.Now.AddSeconds(duration); // Adding duration in seconds to current time
				CharacterCooldownListItem coolie = (new CharacterCooldownListItem {
					SpellnameFull = SpellR,
					Value = endTime.ToString(),
					Position = "SPELLR",
					PKey = "COOLDOWNR"
				});
				print($"{coolie.SpellnameFull} is the spell we are putting on cooldown");
				pc.assignedPlayer.ServerCooldownSave(pc.CharID, coolie);
			}		
		}
        while (CooldownR > 0f)
        {
            CooldownR -= Time.deltaTime;
            yield return null;
        }
        CooldownR = 0f;
        SpellRCoolDown = false;
    }
	[Server]
	public IEnumerator SetAbilityCoolDownF(float duration, bool setup){
		//print($"Starting Cooldown for {SpellF} at {duration}");
        SpellFCoolDown = true;
        CooldownF = duration;
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		if(pc){
			if(!setup){
				DateTime endTime = DateTime.Now.AddSeconds(duration); // Adding duration in seconds to current time
				CharacterCooldownListItem coolie = (new CharacterCooldownListItem {
					SpellnameFull = SpellF,
					Value = endTime.ToString(),
					Position = "SPELLF",
					PKey = "COOLDOWNF"
				});
				print($"{coolie.SpellnameFull} is the spell we are putting on cooldown");
				pc.assignedPlayer.ServerCooldownSave(pc.CharID, coolie);
			}		
		}
        while (CooldownF > 0f)
        {
            CooldownF -= Time.deltaTime;
            yield return null;
        }
        CooldownF = 0f;
        SpellFCoolDown = false;
    }
	Coroutine mesmerizeCoroutine;
	[Server]
	public void MesmerizeThis(float duration){
		if(mesmerizeCoroutine != null){
			StopCoroutine(mesmerizeCoroutine);
		}
		StopATTACKINGMob();
		Mesmerized = true;
		mesmerizeCoroutine = StartCoroutine(MOMesmerized(duration));
	}
	public IEnumerator MOMesmerized(float duration){
		Mesmerized = true;
		Mob mob = GetComponent<Mob>();
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		agent.isStopped = true;
		if(mob){
			//print($"{mob.NAME} was mesmerized for {duration} seconds");
		} else {
			if(pc){
				//print($"{pc.CharacterName} was mesmerized for {duration} seconds");
			}
		}
    	float elapsedTime = 0;
    	while (elapsedTime < duration && Mesmerized)
    	{
    	    elapsedTime += Time.deltaTime;
    	    yield return null;
    	}
		agent.isStopped = false;
    	Mesmerized = false;
		mob.DamageTaken();
	}
	//interrupt code
	[Server]
	public void Interrupted(TurnManager curator){
		if(curator == curatorTM){
			//kill cast on enemy
			Casting = false;
			RpcCancelMOCast();
		}
	}
	[Server]
    public void ApplyStatModifier(StatModifier modifier)
    {
        statsHandler.ApplyStatModifier(modifier, this);
    }
	[Server]
	public void StartingHideCounter(float value){
		StartCoroutine(HideCounter(value));
	}
	IEnumerator HideCounter(float value){
		float timer = value;
		while(Stealth && timer > 0){
			yield return new WaitForSeconds(1f); // Pause for 1 second
        	timer--; // Decrement the timer
		}
		Stealth = false;
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		if(pc){
			UnStealthedChar.Invoke(this, pc.assignedMatch);
		}
		RpcUnhide();
	}
	[ClientRpc]
	public void RpcUnhide(){
		Debug.Log("RpcUnhide called.");
		ClientUnhide();
	}
	[ClientRpc]
	public void RpcHideCast(){
		Debug.Log("RpcHideCast called.");
		ClientHide();
	}
	public void ClientUnhide(){
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if(spriteRenderer.color != originalColor){
        	spriteRenderer.color = originalColor;
		}
	}
	public void ClientHide(){
		Color Hidden = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if(spriteRenderer.color != Hidden){
        	spriteRenderer.color = Hidden;
		}
	}
	public int GetAttackDelayEnemy(){
		return attackDelayEnemy;
	}
	[Server]
	public void ServerHOT(float duration, List<int> values){
		
		StartCoroutine(ServerHealOverTime(duration, values));
	}
	[Server]
	public void ServerDOT(float duration, List<int> values, MovingObject spellOwner){
		
		StartCoroutine(ServerDamageOverTime(duration, values, spellOwner));
	}
	IEnumerator ServerDamageOverTime(float duration, List<int> values, MovingObject spellOwner)
{
    int numberOfValues = values.Count;
    // The time interval is now 1 second
    float timeInterval = 3f;  
	Mob mob = GetComponent<Mob>();
	PlayerCharacter charCheck = GetComponent<PlayerCharacter>();


    for (int i = 0; i < numberOfValues; i++)
    {
        this.DamageDealt(spellOwner, values[i], 0, false, values[i], null);
        // Wait for the calculated time interval before checking the next value
        yield return new WaitForSeconds(timeInterval);
    }
}
	IEnumerator ServerHealOverTime(float duration, List<int> values)
{
        //print("ServerHealOverTime Moving before turn manager");
    int numberOfValues = values.Count;
    // The time interval is now 1 second
    float timeInterval = 3f;  
	PlayerCharacter pc = GetComponent<PlayerCharacter>();
    for (int i = 0; i < numberOfValues; i++)
    {
        //print("Hot tick Moving before turn manager");
        cur_hp += values[i];
        if(cur_hp > max_hp)
        {
            cur_hp = max_hp;
        }
		if(pc){
			curatorTM.HOTTICK(pc);
		}
        // Wait for the calculated time interval before checking the next value
        yield return new WaitForSeconds(timeInterval);
    }
}
	[ClientRpc]
	public void RpcAnimateSpell(string spell, MovingObject target, int value){
		ItemAssets.Instance.CastingSpellAnimation(spell, this, value, target);
	}
	[ClientRpc]
	public void RpcAnimateOverTime(string spell, List<int> value, float duration, MovingObject target){
		ItemAssets.Instance.CastingSpellAnimationOverTime(spell, this, value, target, duration);
	}
	[ClientRpc]
	public void RpcAnimateSpellSelfCasted(string spell, MovingObject selfCastedObj){
		ItemAssets.Instance.CastingSpellAnimationSelfCasted(spell, selfCastedObj);
	}
	[ClientRpc]
	public void RpcAnimateEnemySpell(string spell, MovingObject targetPosition){
		ItemAssets.Instance.CastingSpellAnimationEnemy(spell, this, targetPosition);
	}
	
	IEnumerator AnimatingSprite(){
		SpriteRenderer sRend = GetComponent<SpriteRenderer>();
		while(true){
			sRend.sprite = mainSprite;
			yield return new WaitForSeconds(.5f);
			sRend.sprite = offSprite;
			yield return new WaitForSeconds(.5f);
		}
	}
	
	[Server]
	public void SetStatsServer(){
		//print($"Movingobject {gameObject.name} is setting stats and has {dodge} dodge chance and {agility} agility");
		statsHandler.SetInitialStats(this, agility, strength, fortitude, arcana, armor, MagicResist, FireResist, ColdResist, DiseaseResist, PoisonResist, dodge);
	} 
	/*

	[Server]
	public IEnumerator EnergyUpdater(){
		//print($"ENERGIZEDDDDDDD this mob {gameObject.name}");
		PlayerCharacter pcCheck = GetComponent<PlayerCharacter>();
		Mob mob = GetComponent<Mob>();
		float rechargeTime = .5f;
		float haste = 0f;
		Energized = true;
        stamina = 0f;
		int dodge = 0;
		if(mob){
			dodge = mob.dodge;
		}
		if(pcCheck){
			ClientSparkVision(pcCheck.assignedPlayer.OurNode.GetVision());
		}
		statsHandler.SetInitialStats(this, agility, strength, fortitude, arcana, armor, MagicResist, FireResist, ColdResist, DiseaseResist, PoisonResist, dodge);
		while (Energized)
    	{
    	    if (!moving && !Casting && !Mesmerized && !Stunned && !Feared)
    	    {
				if(mob){
					if(GetAgility() > 101){
						haste = Mathf.Floor((GetAgility() - 100) / 2);
					} else {
						haste = 0f;
					}
				}
				if(pcCheck){
					if(GetAgility() > 101){
						haste = Mathf.Floor((GetAgility() - 100) / 2);
					} else {
						haste = 0f;
					}
				}
				
				rechargeTime = .5f / (1f + (haste / 100f));
				stamina -= 5f;
        		stamina = Mathf.Clamp(stamina, -100f, 250f);
				if(stamina <= 0){
    	            RpcClientCheckColor();
    	        } else {
					RpcChangeToGray();
				}
				
				if(mob != null && stamina == -100f ){
					if(mob.PatrolPath == null){

						if(mob.Resetting){
							moving = true;
							curatorTM.ResetSingleMob(mob);
						}
						if(mob.threatList.Count == 0 && stamina == -100f && !mob.Resetting && mob.Searching ){
							foreach(var pc in curatorTM.GetPCList()){
								if(AreTilesWithinVision(mob.Vision, pc.transform.position, mob.transform.position) && HasLineOfSight(pc.transform.position, mob.transform.position)){
									curatorTM.AggroEntireGroup(mob.groupNumber, mob);
									break;
								}
							}
						}
						if(mob.threatList.Count > 0 && stamina == -100f && mob.SwitchFlip && !mob.Resetting && !mob.Dying ){
							mob.SwitchFlip = false;
							curatorTM.EnemyAction(mob.assignedMatch, mob);
						}
					} else {
						//add patrol logic
						curatorTM.PatrolMovement(mob.assignedMatch, mob);
					}
				}
				yield return new WaitForSeconds(rechargeTime);
    	    } else {
				yield return null;
			}
    	}
    }
	*/
	[Server]
	public void ServerRangedProcess(MovingObject shooter){
		print($"ServerRangedProcess {shooter.name} shot as {gameObject.name}");
		float distance = Vector2.Distance(shooter.transform.position, transform.position);
        float duration = distance / 1f * 0.05f;
		//RpcClientRangedAttack(shooter, duration);
	}
	/*
	[ClientRpc]
	public void RpcClientRangedAttack(MovingObject shooter){
		print($"RpcClientRangedAttack {shooter.name} shot as {gameObject.name}");

		float distance = Vector2.Distance(shooter.transform.position, transform.position);
        float duration = distance / 1f * 0.05f;
		print("spawning arrow on client!");
		AudioMgr sound = GetComponent<AudioMgr>();
		sound.PlaySound("bow draw");
		GameObject arrowHit = Instantiate(ArrowPrefab, shooter.transform.position, Quaternion.identity);
		Vector2 direction = transform.position - shooter.transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 225f; // -90f to adjust for sprite pointing downwards
    	arrowHit.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		arrowHit.GetComponent<ArrowProjectile>().TravelToTarget(new Vector2(transform.position.x, transform.position.y), duration);
	}
	*/
	[ClientRpc]
	public void RpcClientRangedAttack(MovingObject target){
		print($"RpcClientRangedAttack {target.name} shot as {gameObject.name}");

		float distance = Vector2.Distance(target.transform.position, transform.position);
        float duration = distance / 1f * 0.05f;
		print("spawning arrow on client!");
		AudioMgr sound = GetComponent<AudioMgr>();
		sound.PlaySound("bow draw");
		GameObject arrowHit = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
		//Vector2 direction = transform.position - target.transform.position;
		Vector2 direction = target.transform.position - transform.position;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 225f; // -90f to adjust for sprite pointing downwards
    	arrowHit.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		arrowHit.GetComponent<ArrowProjectile>().TravelToTarget(target, duration);
	}
	[ClientRpc]
	public void RpcClientCasterAuto(MovingObject target){
		print($"RpcClientCasterAuto {target.name} shot as {gameObject.name}");
		float distance = Vector2.Distance(target.transform.position, transform.position);
        float duration = distance / 1f * 0.05f;
		AudioMgr sound = GetComponent<AudioMgr>();
		GameObject magicAuto = Instantiate(SpellCasterAutoAttackPrefab, transform.position, Quaternion.identity);
		magicAuto.transform.SetParent(transform);
		Vector2 direction = target.transform.position - transform.position;
		//Vector2 direction = transform.position - target.transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 225f; // -90f to adjust for sprite pointing downwards
    	magicAuto.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		magicAuto.transform.position = new Vector3(transform.position.x, transform.position.y -.25f, 0);
		magicAuto.GetComponent<SpellCasterAA>().TravelToTarget(target, duration);
	}
	/*
	public bool HasLineOfSight(Vector2 start, Vector2 end)
    {
        int x0 = Mathf.FloorToInt(start.x);
        int y0 = Mathf.FloorToInt(start.y);
        int x1 = Mathf.FloorToInt(end.x);
        int y1 = Mathf.FloorToInt(end.y);
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            Vector2 current = new Vector2(x0 + .5f, y0 + .5f);
            if (curatorTM.GetComponent<Pathfinding>().reservationWalls.Contains(current)){
                return false;
            }
            if (x0 == x1 && y0 == y1){
                return true;
            }
            int e2 = 2 * err;
            if (e2 > -dy){
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx){
                err += dx;
                y0 += sy;
            }
        }
    }
	*/
	public bool HasLineOfSight(Vector2 start, Vector2 end)
    {
        int x0 = Mathf.FloorToInt(start.x);
        int y0 = Mathf.FloorToInt(start.y);
        int x1 = Mathf.FloorToInt(end.x);
        int y1 = Mathf.FloorToInt(end.y);
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            Vector2 current = new Vector2(x0 + .5f, y0 + .5f);
            if (curatorTM.reservationWalls.Contains(current)){
                return false;
            }
            if (x0 == x1 && y0 == y1){
                return true;
            }
            int e2 = 2 * err;
            if (e2 > -dy){
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx){
                err += dx;
                y0 += sy;
            }
        }
    }
	bool AreTilesWithinVision(float vision, Vector2 tile1, Vector2 tile2)
	{
	    float xDiff = Mathf.Abs(tile1.x - tile2.x);
	    float yDiff = Mathf.Abs(tile1.y - tile2.y);
	    return xDiff <= vision && yDiff <= vision;
	}
	bool AreTilesWithinSix(Vector2 tile1, Vector2 tile2)
	{
	    float xDiff = Mathf.Abs(tile1.x - tile2.x);
	    float yDiff = Mathf.Abs(tile1.y - tile2.y);
	    return xDiff <= 5 && yDiff <= 5;
	}
	bool MeleeRange(Vector2 tile1, Vector2 tile2)
	{
	    float xDiff = Mathf.Abs(tile1.x - tile2.x);
	    float yDiff = Mathf.Abs(tile1.y - tile2.y);
	    return xDiff <= 1 && yDiff <= 1;
	}
	bool AreTilesWithinTen(Vector2 tile1, Vector2 tile2)
	{
	    float xDiff = Mathf.Abs(tile1.x - tile2.x);
	    float yDiff = Mathf.Abs(tile1.y - tile2.y);
	    return xDiff <= 10 && yDiff <= 10;
	}
	//[ClientRpc]
	//public void RpcClientCheckColor(){
	//	ClientChangeBackToOriginal();
	//}
	//[ClientRpc]
    //public void RpcChangeToGray()
    //{
    //    ClientChangeToGray();
    //}
	public void ClientChangeBackToOriginal(){
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if(spriteRenderer.color != originalColor){
        	spriteRenderer.color = originalColor;
		}
	}
	public void ClientChangeToGray(){
		Color gray = new Color(0.5f, 0.5f, 0.5f, 1.0f);
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if(spriteRenderer.color == originalColor){
        	spriteRenderer.color = gray;
		}
	}
	[TargetRpc]
	public void TargetPositionCheck(Vector3 posCheck){
		PositionChecker(posCheck);
	}
	void PositionChecker(Vector3 posCheck){
		Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
		//print($"Checking {gameObject.name} to see if it has same position as server which is {posCheck} and clients is {currentPosition}");
		Vector2 ProperPosition = new Vector2(
            Mathf.Floor(currentPosition.x) + 0.5f,
            Mathf.Floor(currentPosition.y) + 0.5f
        );
		if(currentPosition == ProperPosition){
			return;
		} else {
			transform.position = posCheck;
			FoggyWar fogofWar = GameObject.Find("FogMachine").GetComponent<FoggyWar>();
			//print($"{posCheck} is the new target position that should be the char pos in foggywar");

			fogofWar.UpdateFogOfWar(this.gameObject, transform.position);
		}
	}
	public IEnumerator DelaySetup()
	{
		yield return new WaitForSeconds(.05f);
		//SetUpCharacter();
		
	}
	public void SetUpCharacter(TurnManager turnManager, Vector2 origin, Vector2 publicorigin)
	{
		if(isServer)
		{
            Origin = origin;
			PublicOrigin = publicorigin;
			if(!GetComponent<PlayerCharacter>()){
				//print($"Mobs Origin is {Origin}");
			}
			curatorTM = turnManager;
			//Material mat = GetComponent<Renderer>().material;
			//mat = new Material(mat); 
        	//rb2D = GetComponent <Rigidbody2D> ();                                               		//Get a component reference to this object's Rigidbody2D
			mainSprite = transform.GetComponent<SpriteRenderer>().sprite;
		}
	}
	
	/*
	if(stamina < 0)
        		{
        			ctSlider.value = stamina/-100f;
					ctImage.color = new Color32 (218, 212, 94, 255);
        		}
        		else
        		{
        		    ctSlider.value = stamina/250f;
        			ctImage.color = new Color32 (208,70,72, 255);
        		}
	*/
	[Server]
	public void ChargeSwingDelay()
    {
        stamina += attackDelay;
    }
	[Server]
	public void ChargeSpellDelay(float value)
    {
        stamina += value;
    }
	[Server]
	public void ChargeMoveDelay(float charge)
    {
        stamina += charge;
    }
	public float SpellRange(MovingObject caster, string mode){
        int lvl = caster.GetComponent<PlayerCharacter>().Level;
        float range = 0f;
        float baseRange = 0f;
        string _spellname = string.Empty;
        if(mode == CastingQ){
            _spellname = caster.SpellQ;
        }
        if(mode == CastingE){
            _spellname = caster.SpellE;
        }
        if(mode == CastingR){
            _spellname = caster.SpellR;
        }
        if(mode == CastingF){
            _spellname = caster.SpellF;
        }
        var nameMatch = System.Text.RegularExpressions.Regex.Match(_spellname, @"^\D*");
        string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
        int _spellRank = 1;
        // Extract spell rank
        var rankMatch = System.Text.RegularExpressions.Regex.Match(_spellname, @"\d+$");
        if (rankMatch.Success) {
            _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
        }
        if (spell == "Aimed Shot")
            baseRange = 6f;
        if (spell == "Bandage Wound")
            baseRange = 1.25f;
        if (spell == "Head Shot")
            baseRange = 5f;
        if (spell == "Silence Shot")
            baseRange = 5f;
        if (spell == "Crippling Shot")
            baseRange = 5f;
        if (spell == "Dash")
            baseRange = 3f;
        if (spell == "Identify Enemy")
            baseRange = 5f;
        if (spell == "Double Shot")
            baseRange = 6f;
        if (spell == "Fire Arrow")
            baseRange = 6f;
        if (spell == "Penetrating Shot")
            baseRange = 6f;
        if (spell == "Sleep")
            baseRange = 5f;

        // Enchanter Skills
        if (spell == "Mesmerize")
            baseRange = 5f;
        if (spell == "Haste")
            baseRange = 5f;
        if (spell == "Root")
            baseRange = 5f;
        if (spell == "Invisibility")
            baseRange = 5f;
        if (spell == "Rune")
            baseRange = 5f;
        if (spell == "Slow")
            baseRange = 5f;
        if (spell == "Magic Sieve")
            baseRange = 5f;
        if (spell == "Aneurysm")
            baseRange = 5f;
        if (spell == "Gravity Stun")
            baseRange = 5f;
        if (spell == "Weaken")
            baseRange = 5f;
        if (spell == "Resist Magic")
            baseRange = 5f;
        if (spell == "Purge")
            baseRange = 5f;
        if (spell == "Charm")
            baseRange = 5f;
        if (spell == "Mp Transfer")
            baseRange = 1.25f;

        // Fighter Skills
        if (spell == "Charge")
            baseRange = 5f;
        if (spell == "Bash")
            baseRange = 1.25f;
        if (spell == "Intimidating Roar")
            baseRange = 1.25f;
        if (spell == "Protect")
            baseRange = 1.25f;
        if (spell == "Knockback")
            baseRange = 1.25f;
        if (spell == "Throw Stone")
            baseRange = 5f;
        if (spell == "Heavy Swing")
            baseRange = 1.25f;
        if (spell == "Taunt")
            baseRange = 1.25f;
        // Priest Skills
        if (spell == "Holy Bolt")
            baseRange = 5f;
        if (spell == "Heal")
            baseRange = 5f;
        if (spell == "Cure Poison")
            baseRange = 5f;
        if (spell == "Dispel")
            baseRange = 5f;
        if (spell == "Fortitude")
            baseRange = 5f;
        if (spell == "Turn Undead")
            baseRange = 5f;
        if (spell == "Undead Protection")
            baseRange = 3f;
        if (spell == "Smite")
            baseRange = 5f;
        if (spell == "Shield Bash")
            baseRange = 1.25f;
        if (spell == "Greater Heal")
            baseRange = 3f;
        if (spell == "Group Heal")
            baseRange = 4f;
        if (spell == "Resurrect")
            baseRange = 5f;

        // Rogue Skills
        if (spell == "Shuriken")
            baseRange = 5f;
        if (spell == "Picklock")
            baseRange = 1.25f;
        if (spell == "Steal")
            baseRange = 1.25f;
        if (spell == "Tendon Slice")
            baseRange = 1.25f;
        if (spell == "Backstab")
            baseRange = 1.25f;
        if (spell == "Blind")
            baseRange = 5f;
        if (spell == "Poison")
            baseRange = 1.25f;
        // Wizard Skills
        if (spell == "Ice")
            baseRange = 5f;
        if (spell == "Fire")
            baseRange = 5f;
        if (spell == "Blizzard")
            baseRange = 6f;
        if (spell == "Magic Burst")
            baseRange = 1.25f;
        if (spell == "Teleport")
            baseRange = 7f;
        if (spell == "Meteor Shower")
            baseRange = 7f;
        if (spell == "Spell Crit")
            baseRange = 0f;
        if (spell == "Ice Block")
            baseRange = 1.25f;
        if (spell == "Ice Blast")
            baseRange = 5f;
        if (spell == "Incinerate")
            baseRange = 5f;
        if (spell == "Brain Freeze")
            baseRange = 5f;
        if (spell == "Light")
            baseRange = 5f;
        if (spell == "Magic Missile")
            baseRange = 5f;

        // Druid Skills
        if (spell == "Rejuvination")
            baseRange = 5f;
        if (spell == "Swarm Of Insects")
            baseRange = 5f;
        if (spell == "Thorns")
            baseRange = 5f;
        if (spell == "Nature's Protection")
            baseRange = 5f;
        if (spell == "Strength")
            baseRange = 5f;
        if (spell == "Snare")
            baseRange = 5f;
        if (spell == "Engulfing Roots")
            baseRange = 5f;
        if (spell == "Shapeshift")
            baseRange = 1.25f;
        if (spell == "Tornado")
            baseRange = 5f;
        if (spell == "Chain Lightning")
            baseRange = 5f;
        if (spell == "Greater Rejuvination")
            baseRange = 5f;
        if (spell == "Solar Flare")
            baseRange = 7f;
        if (spell == "Evacuate")
            baseRange = 4f;

        // Paladin Skills
        if (spell == "Holy Swing")
            baseRange = 1.25f;
        if (spell == "Divine Armor")
            baseRange = 5f;
        if (spell == "Flash Of Light")
            baseRange = 5f;
        if (spell == "Undead Slayer")
            baseRange = 0f;
        if (spell == "Stun")
            baseRange = 5f;
        if (spell == "Celestial Wave")
            baseRange = 5f;
        if (spell == "Angelic Shield")
            baseRange = 1.25f;
        if (spell == "Cleanse")
            baseRange = 5f;
        if (spell == "Consecrated Ground")
            baseRange = 5f;
        if (spell == "Divine Wrath")
            baseRange = 1.25f;
        if (spell == "Cover")
            baseRange = 1.25f;
        if (spell == "Shackle")
            baseRange = 5f;
        if (spell == "Lay On Hands")
            baseRange = 1.25f;
        range = (baseRange * ((_spellRank - 1) * .004f) + baseRange);
        
        
        return range;
    }
	 public float GetSpellCooldown(string spell, int _spellRank){
		
        float cooldownSeconds = 0f;
    	float cooldownSecondsBase = 0f;
		// Archer Skills
		if (spell == "Aimed Shot")
		    cooldownSecondsBase = 30f;
		if (spell == "Bandage Wound")
		    cooldownSecondsBase = 15 * 60f;
		if (spell == "Head Shot")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Silence Shot")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Crippling Shot")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Dash")
		    cooldownSecondsBase = 6 * 60f;
		if (spell == "Identify Enemy")
		    cooldownSecondsBase = 2 * 7 * 24 * 60 * 60f;
		if (spell == "Track")
		    cooldownSecondsBase = 1 * 24 * 60 * 60f;
		if (spell == "Fire Arrow")
		    cooldownSecondsBase = 4 * 60f;
		if (spell == "Penetrating Shot")
		    cooldownSecondsBase = 4 * 60f;
		if (spell == "Sleep")
		    cooldownSecondsBase = 4 * 60f;
		
		// Enchanter Skills
		if (spell == "Mesmerize")
		    cooldownSecondsBase = 20f;
		if (spell == "Haste")
		    cooldownSecondsBase = 30f;
		if (spell == "Root")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Invisibility")
		    cooldownSecondsBase = 60f;
		if (spell == "Rune")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Slow")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Magic Sieve")
		    cooldownSecondsBase = 1 * 60 * 60f;
		if (spell == "Aneurysm")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Gravity Stun")
		    cooldownSecondsBase = 4 * 60f;
		if (spell == "Weaken")
		    cooldownSecondsBase = 10 * 60f;
		if (spell == "Resist Magic")
		    cooldownSecondsBase = 30f;
		if (spell == "Purge")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Charm")
		    cooldownSecondsBase = 15 * 60f;
		if (spell == "Mp Transfer")
		    cooldownSecondsBase = 1 * 60f;
		
		// Fighter Skills
		if (spell == "Charge")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Bash")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Intimidating Roar")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Protect")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Knockback")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Throw Stone")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Heavy Swing")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Taunt")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Block")
		    cooldownSecondsBase = 4 * 60f;
		if (spell == "Tank Stance")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Offensive Stance")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Critical Strike")
		    cooldownSecondsBase = 0;
		
		// Priest Skills
		if (spell == "Holy Bolt")
		    cooldownSecondsBase = 30f;
		if (spell == "Heal")
		    cooldownSecondsBase = 14f;
		if (spell == "Cure Poison")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Dispel")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Fortitude")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Turn Undead")
		    cooldownSecondsBase = 10 * 60f;
		if (spell == "Undead Protection")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Smite")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Shield Bash")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Greater Heal")
		    cooldownSecondsBase = 20f;
		if (spell == "Group Heal")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Regeneration")
		    cooldownSecondsBase = 5 * 60f;
		if (spell == "Resurrect")
		    cooldownSecondsBase = 24 * 60 * 60f;
		// Rogue Skills
		if (spell == "Shuriken")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Hide")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Picklock")
		    cooldownSecondsBase = 20f;
		if (spell == "Steal")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Detect Traps")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Tendon Slice")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Backstab")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Rush")
		    cooldownSecondsBase = 20 * 60f;
		if (spell == "Blind")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Poison")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Sneak")
		    cooldownSecondsBase = 5 * 60f;
		// Wizard Skills
		if (spell == "Ice")
		    cooldownSecondsBase = 20f;
		if (spell == "Fire")
		    cooldownSecondsBase = 20f;
		if (spell == "Fireball")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Light")
		    cooldownSecondsBase = 10f;
		if (spell == "Magic Missile")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Spell Crit")
		    cooldownSecondsBase = 0;
		if (spell == "Ice Block")
		    cooldownSecondsBase = 10 * 60f;
		if (spell == "Ice Blast")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Blizzard")
		    cooldownSecondsBase = 4 * 60f;
		if (spell == "Magic Burst")
		    cooldownSecondsBase = 4 * 60f;
		if (spell == "Teleport")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Meteor Shower")
		    cooldownSecondsBase = 4 * 60f;
		if (spell == "Brain Freeze")
		    cooldownSecondsBase = 5 * 60f;
		if (spell == "Incinerate")
		    cooldownSecondsBase = 5 * 60f;
		
		// Druid Skills
		if (spell == "Rejuvination")
		    cooldownSecondsBase = 10f;
		if (spell == "Swarm Of Insects")
		    cooldownSecondsBase = 20f;
		if (spell == "Thorns")
		    cooldownSecondsBase = 10f;
		if (spell == "Nature's Protection")
		    cooldownSecondsBase = 30f;
		if (spell == "Strength")
		    cooldownSecondsBase = 10f;
		if (spell == "Snare")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Engulfing Roots")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Shapeshift")
		    cooldownSecondsBase = 2 * 60 * 60f;
		if (spell == "Tornado")
		    cooldownSecondsBase = 3 * 60f;
		if (spell == "Chain Lightning")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Greater Rejuvination")
		    cooldownSecondsBase = 25f;
		if (spell == "Solar Flare")
		    cooldownSecondsBase = 5 * 60f;
		if (spell == "Evacuate")
		    cooldownSecondsBase = 15 * 60f;
		
		// Paladin Skills
		if (spell == "Holy Swing")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Divine Armor")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Flash Of Light")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Stun")
		    cooldownSecondsBase = 2 * 60f;
		if (spell == "Celestial Wave")
		    cooldownSecondsBase = 3 * 60f;
		if (spell == "Angelic Shield")
		    cooldownSecondsBase = 5 * 60f;
		if (spell == "Cleanse")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Consecrated Ground")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Divine Wrath")
		    cooldownSecondsBase = 5 * 60f;
		if (spell == "Cover")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Root")
		    cooldownSecondsBase = 1 * 60f;
		if (spell == "Lay On Hands")
		    cooldownSecondsBase = 24 * 60 * 60f;

		cooldownSeconds = cooldownSecondsBase * (1 - _spellRank * 0.005f);
        return cooldownSeconds;
    }
	public const string FIREDAMAGECOLOR = "FF4500";
    public const string POISONDAMAGECOLOR = "32CD32";
    public const string DRAGONDAMAGECOLOR = "FF8C00";
    public const string LEECHDAMAGECOLOR = "8B0000";
    public const string MAGICDAMAGECOLOR = "9370DB";
    public const string COLDDAMAGECOLOR = "00BFFF";
    public const string DISEASEDAMAGECOLOR = "8B4513";
	[ClientRpc]
	public void RpcSpawnPopUp(float value, bool criticalStrike){
		//animator.SetBool("IsAttacking", true);
		AudioMgr sound = GetComponent<AudioMgr>();
		string amount = value.ToString();
		GameObject spawnTextPopUp = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        AbilityPopUp abilityDisplay = spawnTextPopUp.GetComponent<AbilityPopUp>();
		if(!criticalStrike){
        	abilityDisplay.AbilityPopUpBuild(amount, normalHithexColor);
			sound.PlaySound("blunt hit");
		} else {
        	abilityDisplay.AbilityPopUpBuild(amount, criticalHitHexColor);
			sound.PlaySound("Critical");
		}
	}
	[ClientRpc]
	public void RpcSpawnTrapDmg(int health, int mana){
		if(health > 0){
			GameObject spawnTextPopUpHP = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        	AbilityPopUp abilityDisplayHP = spawnTextPopUpHP.GetComponent<AbilityPopUp>();
        	abilityDisplayHP.TrapPopUpHp(health, hpTrapHexColor);
		}
		
		if(mana > 0){
			GameObject spawnTextPopUpMP = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        	AbilityPopUp abilityDisplayMP = spawnTextPopUpMP.GetComponent<AbilityPopUp>();
        	abilityDisplayMP.TrapPopUpHp(mana, mpTrapHexColor);
		}
	}
	[ClientRpc]
	public void RpcSpawnPopUpDodged(){
		AudioMgr audio = GetComponent<AudioMgr>();
		audio.PlaySound("Dodge");
		GameObject spawnTextPopUp = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        AbilityPopUp abilityDisplay = spawnTextPopUp.GetComponent<AbilityPopUp>();
        abilityDisplay.AbilityPopUpBuild("Dodged", normalHithexColor);
	}
	[ClientRpc]
	public void RpcSpawnPopUpParried(){
		AudioMgr audio = GetComponent<AudioMgr>();
		audio.PlaySound("Parry");
		GameObject spawnTextPopUp = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        AbilityPopUp abilityDisplay = spawnTextPopUp.GetComponent<AbilityPopUp>();
        abilityDisplay.AbilityPopUpBuild("Parried", normalHithexColor);
	}
	[ClientRpc]
	public void RpcSpawnPopUpBlocked(){
		AudioMgr audio = GetComponent<AudioMgr>();
		audio.PlaySound("Block");
		GameObject spawnTextPopUp = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        AbilityPopUp abilityDisplay = spawnTextPopUp.GetComponent<AbilityPopUp>();
        abilityDisplay.AbilityPopUpBuild("Blocked", normalHithexColor);
	}
	[ClientRpc]
	public void RpcSpawnPopUpAbsorbed(){
		AudioMgr audio = GetComponent<AudioMgr>();
		audio.PlaySound("moving");
		GameObject spawnTextPopUp = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        AbilityPopUp abilityDisplay = spawnTextPopUp.GetComponent<AbilityPopUp>();
        abilityDisplay.AbilityPopUpBuild("Absorbed", normalHithexColor);
	}
	[ClientRpc]
	public void RpcSpawnMagicalEffectDmg(string value, string hexColorCode){
		//print($"Spawning magical {hexColorCode}");
		AudioMgr audio = GetComponent<AudioMgr>();
		//audio.PlaySound("moving");
		GameObject spawnTextPopUp = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        AbilityPopUp abilityDisplay = spawnTextPopUp.GetComponent<AbilityPopUp>();
        abilityDisplay.MagicalWeapoNEffect(value, hexColorCode);
	}
	
	[ClientRpc]
	public void RpcSpawnDeath(){
		Mob mobCheck = GetComponent<Mob>();
		AudioMgr audio = GetComponent<AudioMgr>();
		CircleCollider2D[] circleColliders = GetComponents<CircleCollider2D>();
    	foreach (CircleCollider2D circleCollider in circleColliders)
    	{
    	    circleCollider.enabled = false;
    	}
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if(AnimatingSpriteCO != null){
				StopCoroutine(AnimatingSpriteCO);
			}
		if (spriteRenderer != null)
    	{
    	    //spriteRenderer.enabled = false; // Hide the sprite
    	    if (mobCheck != null)
    	    {
    	        spriteRenderer.sprite = BloodyDeathSprite;
	        	spriteRenderer.sortingOrder = 9; // Change the sorting order 
    	        if (TIER == 1) { audio.PlaySound("SmallDeath"); }
    	        if (TIER == 2) { audio.PlaySound("MediumDeath"); }
    	        if (TIER == 3) { audio.PlaySound("LargeDeath"); }
				if(CombatPartyView.instance.GetSelected() == mobCheck){
        		    CombatPartyView.instance.TurnOffSelectedWindow();
					UnselectedMO();
        		}
        		if(CombatPartyView.instance.GetTarget() == mobCheck){
        		    CombatPartyView.instance.TurnOffTarget(mobCheck);
					UnTargettedMO();
        		}
				UnTargettedMO();
				UnselectedMO();
    	    }
    	}
		gameObject.layer = LayerMask.NameToLayer("Death");
		healthBarSlider.gameObject.SetActive(false);
    	magicPointBarSlider.gameObject.SetActive(false);
    	ctSlider.gameObject.SetActive(false);
		//GameObject spawnTextPopUp = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        //AbilityPopUp abilityDisplay = spawnTextPopUp.GetComponent<AbilityPopUp>();
        //abilityDisplay.DeathPopUp("Death", deathhexColor);
		BoxCollider2D deathCollider = GetComponent<BoxCollider2D>();
		if(deathCollider){
			Destroy(deathCollider);
		}
	}
	[Server]
	public void ApplyStatChange(string stat, float duration, int value, string spellName, bool buff){
		if(!buff){
			value = value * -1;
		}
		StatModifier statChange = null;
		if(stat == "Strength"){
			statChange = new StatModifier(StatModifier.Stat.Strength, value, duration, spellName, buff);
		}
		if(stat == "Agility"){
			statChange = new StatModifier(StatModifier.Stat.Agility, value, duration, spellName, buff);
		}
		if(stat == "Fortitude"){
			statChange = new StatModifier(StatModifier.Stat.Fortitude, value, duration, spellName, buff);
		}
		if(stat == "Arcana"){
			statChange = new StatModifier(StatModifier.Stat.Arcana, value, duration, spellName, buff);
		}
		if(stat == "Armor"){
			statChange = new StatModifier(StatModifier.Stat.Armor, value, duration, spellName, buff);
		}
		if(stat == "MagicResistance"){
			statChange = new StatModifier(StatModifier.Stat.MagicResistance, value, duration, spellName, buff);
		}
		if(stat == "PoisonResistance"){
			statChange = new StatModifier(StatModifier.Stat.PoisonResistance, value, duration, spellName, buff);
		}
		if(stat == "DiseaseResistance"){
			statChange = new StatModifier(StatModifier.Stat.DiseaseResistance, value, duration, spellName, buff);
		}
		if(stat == "ColdResistance"){
			statChange = new StatModifier(StatModifier.Stat.ColdResistance, value, duration, spellName, buff);
		}
		if(stat == "FireResistance"){
			statChange = new StatModifier(StatModifier.Stat.FireResistance, value, duration, spellName, buff);
		}
		ApplyStatModifier(statChange);
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		if(pc){
			ClientUpdateStatChanges(stat, duration, value, spellName, buff);
		}
	}
	[ClientRpc]
	void ClientUpdateStatChanges(string stat, float duration, int value, string spellName, bool buff){
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		pc.AddBuff(stat, duration, value, spellName, buff);
	}
	
	[Server]
public void DamageDealt(MovingObject hittingObject, int value, float criticalValue, bool showPopUp, float threat, string magical)
{
	if (Dying)
		return;

	Mob mob = this.GetComponent<Mob>();
	PlayerCharacter charCheck = GetComponent<PlayerCharacter>();

	bool critical = false; //Random.value >= criticalValue;
	cur_hp -= value;
	
	if (Mesmerized)
		Mesmerized = false;
		agent.isStopped = false;

	if (!string.IsNullOrEmpty(magical))
	{
		//print("This was a magical pop-up from an NFT doing some damage");
		RpcSpawnMagicalEffectDmg(value.ToString(), magical);
	}

	if (showPopUp)
	{
		if (value <= 0)
			RpcSpawnPopUpAbsorbed();
		else
			RpcSpawnPopUp(value, critical);
	}

	if (cur_hp <= 0)
	{
		ServerStopCasting();
		if (mob)
		{
			//RpcSpawnDeath();
			mob.Die();
			return;
		}
		else
		{
			cur_hp = 0;
			if(charCheck){
				if(!Dying){
					Dying = true;
					charCheck.DeathEXP();
					StopATTACKINGMob();
					agent.isStopped = true;
					//agent.enabled = false;
					CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
					circleCollider.enabled = false;
					DeathCharacter.Invoke(charCheck.assignedPlayer.connectionToClient, charCheck.CharID);
					RemoveCharReserve.Invoke(this);
					//RevokePlayerAuthority();
					RpcCharDeath();
					DeadChar.Invoke(this, charCheck.assignedMatch);
					
					Target = null;
				}
			}
		}
	} else {
		if(charCheck){
	        TakeDamageCharacter.Invoke(charCheck.assignedPlayer.connectionToClient, cur_hp, charCheck.CharID);
		}
	}
	

	if (mob != null && !hittingObject.Dying && !Dying)
	{
		if(!mob.threatList.ContainsKey(hittingObject)){
			mob.threatList.Add(hittingObject, threat);
		} else {
			mob.threatList[hittingObject] += threat;
		}
		mob.DamageTaken();
	}
		//StartCoroutine(mob.AddingThreatNewmod(hittingObject, threat));
}
[Server]
void RevokePlayerAuthority()
{
    NetworkIdentity identity = this.GetComponent<NetworkIdentity>();

    // Ensure it's running on server, has authority, and is assigned to a client
    if (NetworkServer.active && identity.hasAuthority && identity.connectionToClient != null)
    {
        identity.RemoveClientAuthority();
    }
}
	[ClientRpc]
	public void RpcCharDeath(){
		AudioMgr audio = GetComponent<AudioMgr>();
		animator.enabled = false;
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer != null)
    	{
			//if(AnimatingSpriteCO != null){
			//	StopCoroutine(AnimatingSpriteCO);
			//}
			//Play animation for death so it goes from cloud tomb to flashing tomb yeah babyyyy
    	    spriteRenderer.sprite = TombStoneSprite;
	        spriteRenderer.sortingOrder = 9; // Change the sorting order 

    	    audio.PlaySound("CharDeath");
    	}
		gameObject.layer = LayerMask.NameToLayer("Death");
		healthBarSlider.gameObject.SetActive(false);
    	magicPointBarSlider.gameObject.SetActive(false);
    	ctSlider.gameObject.SetActive(false);
		GameObject spawnTextPopUp = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        AbilityPopUp abilityDisplay = spawnTextPopUp.GetComponent<AbilityPopUp>();
        abilityDisplay.DeathPopUp("Death", deathhexColor);
		StartCoroutine(DelayedVisionDeath());
		ScenePlayer.localPlayer.RemoveSelectedCharacter(this.gameObject);
		UnTargettedMO();
		UnselectedMO();
	}
	IEnumerator DelayedVisionDeath(){
		yield return new WaitForSeconds(2f);
		RemoveFogParticipant.Invoke(this);
	}
	[ClientRpc]
	public void RpcMobEXPCPDisplay(int CP, int EXP, int tier){
		Mob mob = GetComponent<Mob>();
		if(mob){
			GameObject spawnTextPopUp = Instantiate(PopUpTextPrefab, transform.position, Quaternion.identity);
        	AbilityPopUp abilityDisplay = spawnTextPopUp.GetComponent<AbilityPopUp>();
        	abilityDisplay.EXPCPPopUP(CP, EXP);
		}
	}
	//[Server]
	//public void DamageDealtToPlayer(int value){
	//	bool critical = Random.value >= 0.5f;
	//	
	//	cur_hp = cur_hp - value;
	//	if(cur_hp <= 0){
	//		cur_hp = max_hp;
	//	}
	//	if(value <= 0){
	//		RpcSpawnPopUpAbsorbed();
	//	} else {
	//		RpcSpawnPopUp(value, critical);
	//	}
	//}
	//[Server]
	//public void HealingReceived(int value){
	//	cur_hp = cur_hp + value;
	//	if(cur_hp > max_hp){
	//		cur_hp = max_hp;
	//		return;
	//	}
	//}
	[ClientRpc]
	public void NewFogUpdate(Vector2 newspot)
	{
		if(newspot == null){
			return;
		}
		Mob mob = GetComponent<Mob>();
		GameObject fogMachine = GameObject.Find("FogMachine");
		if(fogMachine){
			FoggyWar fogofWar = fogMachine.GetComponent<FoggyWar>();
			if(fogofWar){
				if(mob){
					fogofWar.UpdateMob(mob);
				} else {
					fogofWar.UpdateFogOfWar(this.gameObject, newspot);
				}
			}
		}
	}
	[ClientRpc]
	void RpcUpdateWalkingState(bool isWalking)
	{
		if(GetComponent<Mob>()){
			return;
		}
	    // This will be executed on all clients, synchronizing the animation state
	    animator.SetBool("IsWalking", isWalking);
	}
	[ClientRpc]
	public void RpcUpdateFacingDirection(bool rightFacing)
{
	SpriteRenderer sRend = GetComponent<SpriteRenderer>();
	sRend.flipX = rightFacing; // Flip if facing left
}
	[ClientRpc]
	public void RpcPlayWalkingSound(){
		AudioMgr audio = GetComponent<AudioMgr>();
		audio.PlaySound("moving");
	}
    protected void Movement (Vector3 end)
    {
        StartCoroutine(LerpPosition(end, moveTime));
    }
    //Function used to move this game object to the target position smoothly over a number of seconds
    IEnumerator LerpPosition(Vector2 targetPosition, float duration)
    {
		LerpInProgress = true;
        float time = 0;
        Vector2 startPosition = transform.position;
        ////print($"{this.gameObject.name} player is moving to position: {targetPosition} from {startPosition}");
        while (time < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
		LerpInProgress = false;
		//print($"{targetPosition} is the new target position that should be the char pos in foggywar");
		if(GetComponent<PlayerCharacter>()){
			//NewFogUpdate(targetPosition);
		} else {
			Mob mob = GetComponent<Mob>();
			if(mob){
				mob.ClientMobFog();
			}
		}
    }
    //used for a slight bumping animation towards an adjacent target
	[Server]
    public IEnumerator BumpTowards(Vector2 end)
    {
		//RpcPlaySwingSound();
		float radius = agent.radius;
		agent.radius = 0;
		//agent.enabled = false;
		Vector2 start = transform.position;
		Vector2 target = new Vector2((end.x - start.x)*.1f, (end.y - start.y)*.1f);
		transform.position = start + target;
		yield return new WaitForSeconds(moveTime);
		//agent.enabled = true;
		transform.position = start;
		agent.radius = radius;
    }
	[ClientRpc]
	void RpcPlaySwingSound(){
		AudioMgr sound = GetComponent<AudioMgr>();
		sound.PlaySound("blunt hit");
	}
	public void SelectedMO(){
        SelectedCircle.SetActive(true);
    }
    public void UnselectedMO(){
        SelectedCircle.SetActive(false);
    }
	public void TargettedMO(){
        TargetCircle.SetActive(true);
		TargetWindowSet.Invoke(this);
    }
    public void UnTargettedMO(){
        TargetCircle.SetActive(false);
    }
	[Command]
	public void CmdSetTarget(MovingObject target){
		Target = target;
		print($"Setting target {Target.gameObject.name} for {this.gameObject.name}");
		TargetSendNewTarget(target);
	}
	[TargetRpc]
	void TargetSendNewTarget(MovingObject target){
		TargetWasSet.Invoke(target);
	}
   private Coroutine Lerping;
public bool isMoving = false;
public float travelTime = .75f;
	public void SetPath(List<Node> newPath)
{
    // Stop the current pathfinding coroutine if it's running
    if (Lerping != null)
    {
        StopCoroutine(Lerping);
    }

    // Start the new path
    isMoving = true;
    Lerping = StartCoroutine(FollowPath(newPath));
}
[Server]
    public void ProcessAOESpell(string spell, int spellRank, int cost, Vector2 mousePosition, bool offensive){
        //CastingAOEHostileSpell
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		Mob mob = GetComponent<Mob>();
		Match match = null;
		if(pc){
			match = pc.assignedMatch;
		}
		if(mob){
			match = mob.assignedMatch;
		}
        if(offensive){
            curatorTM.CastingAOEHostileSpell(match, this, mousePosition, spell, spellRank, cost);
        } else {
            curatorTM.CastingAOEFriendlySpell(match, this, mousePosition, spell, spellRank, cost);
        }
    }
[Server]
	public void ProcessSpellCast(string mode, MovingObject castingCharacter, MovingObject target, int cost){
		string _spellname = string.Empty;
		if(mode == CastingQ){
            _spellname = SpellQ;
        }
        if(mode == CastingE){
            _spellname = SpellE;
        }
        if(mode == CastingR){
            _spellname = SpellR;
        }
        if(mode == CastingF){
            _spellname = SpellF;
        }
		PlayerCharacter pc = GetComponent<PlayerCharacter>();
		Mob mob = GetComponent<Mob>();
		Match match = null;
		if(pc){
			match = pc.assignedMatch;
		}
		if(mob){
			match = mob.assignedMatch;
		}
		var nameMatch = System.Text.RegularExpressions.Regex.Match(_spellname, @"^\D*");
        string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
        int _spellRank = 1;
        // Extract spell rank
        var rankMatch = System.Text.RegularExpressions.Regex.Match(_spellname, @"\d+$");
        if (rankMatch.Success) {
            _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
        }
        print($"{spell} and its rank is {_spellRank}");
		//Spell will determine what is cast
		//DPS Tier 1
		//Archer
		if(spell == "Aimed Shot"){
		    curatorTM.UpdatePlayerCastedOffensiveSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Bandage Wound"){
			curatorTM.UpdatePlayerCastedHealSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		//Wizard
		if(spell == "Ice"){
			curatorTM.UpdatePlayerCastedOffensiveSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Fire"){
			curatorTM.UpdatePlayerCastedOffensiveSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		//Rogue
		if(spell == "Shuriken"){
			curatorTM.UpdatePlayerCastedOffensiveSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Hide"){
			curatorTM.UpdatePlayerSelfCasted(match, this, spell, _spellRank, cost);
		    return;
		}
		//Priest
		if(spell == "Holy Bolt"){
			curatorTM.UpdatePlayerCastedOffensiveSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Heal"){
			curatorTM.UpdatePlayerCastedHealSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		//Druid
		if(spell == "Swarm Of Insects"){
			curatorTM.UpdatePlayerCastedOffensiveSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Rejuvination"){
			curatorTM.UpdatePlayerCastedHealSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		//Paladin
		if(spell == "Holy Swing"){
			curatorTM.UpdatePlayerCastedOffensiveSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Divine Armor"){
			curatorTM.UpdatePlayerSelfCasted(match, this, spell, _spellRank, cost);
		    return;
		}
		//Fighter
		if(spell == "Charge"){
			curatorTM.UpdatePlayerCastedOffensiveSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Bash"){
			curatorTM.UpdatePlayerCastedOffensiveSpellSingleTargetDPS(match, this, target, spell, _spellRank, cost);
		    return;
		}
		//Enchanter
		if(spell == "Mesmerize"){
			curatorTM.UpdateCrowdControlSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Haste"){
			curatorTM.UpdateBuffSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		
		/*
		if(spell == "Bandage Wound"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Head Shot"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Silence Shot"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Crippling Shot"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Dash"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Identify Enemy"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Track"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Fire Arrow"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Penetrating Shot"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Sleep"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Perception"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Double Shot"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Natures Precision"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		
		
		// Enchanter Spells
		if(spell == "Mesmerize"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Haste"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Root"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Invisibility"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Rune"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Slow"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Magic Sieve"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Aneurysm"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Gravity Stun"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Weaken"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Resist Magic"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Purge"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Charm"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Mp Transfer"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		
		// Fighter Spells
		if(spell == "Charge"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Bash"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Intimidating Roar"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Protect"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Knockback"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Throw Stone"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Heavy Swing"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Taunt"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Block"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Tank Stance"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Offensive Stance"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Critical Strike"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Riposte"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		
		// Priest Spells
		if(spell == "Holy Bolt"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Heal"){
		    curatorTM.UpdatePlayerCastedHealSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Cure Poison"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Dispel"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Fortitude"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Turn Undead"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Critical Heal"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Undead Protection"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Smite"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Shield Bash"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Greater Heal"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Group Heal"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Regeneration"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Resurrect"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		
		// Rogue Spells
		if(spell == "Shuriken"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Hide"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Picklock"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Steal"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Detect Traps"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Tendon Slice"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Backstab"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Rush"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Blind"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Treasure Finding"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Ambidexterity"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Poison"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Sneak"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		
		// Wizard Spells
		if(spell == "Ice"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Fire"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Blizzard"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Magic Burst"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Fireball"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Light"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Magic Missile"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Spell Crit"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Ice Block"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Ice Blast"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Incinerate"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Brain Freeze"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Teleport"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Meteor Shower"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		
		// Druid Spells
		if(spell == "Rejuvination"){
		    curatorTM.UpdatePlayerCastedHealSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Swarm Of Insects"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Thorns"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Nature's Protection"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Strength"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Snare"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Engulfing Roots"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Shapeshift"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Tornado"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Chain Lightning"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Greater Rejuvination"){
		    curatorTM.UpdatePlayerCastedHealSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Solar Flare"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Evacuate"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		// Paladin Spells
		if(spell == "Holy Swing"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Divine Armor"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Flash Of Light"){
		    curatorTM.UpdatePlayerCastedHealSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Undead Slayer"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Celestial Wave"){
		    curatorTM.UpdatePlayerCastedHealSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Angelic Shield"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Cleanse"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Consecrated Ground"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Divine Wrath"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Cover"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Shackle"){
		    curatorTM.UpdatePlayerCastedOffensiveSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		if(spell == "Lay On Hands"){
		    curatorTM.UpdatePlayerCastedHealSpell(match, this, target, spell, _spellRank, cost);
		    return;
		}
		*/
	}
	public int GetSpellCost(string spell){
		print($"Getting cost of {spell}");
        int cost = 0;
        //Tier  **************

        // Archer Skills
        if (spell == "Aimed Shot")
            return 1;
        if (spell == "Bandage Wound")
            return 1;
        // Enchanter Skills
        if (spell == "Mesmerize")
            return 1;
        if (spell == "Haste")
            return 1;
        // Fighter Skills
        if (spell == "Charge")
            return 1;
        if (spell == "Bash")
            return 1;
        // Priest Skills
        if (spell == "Holy Bolt")
            return 1;
        if (spell == "Heal")
            return 1;
        // Rogue Skills
        if (spell == "Shuriken")
            return 1;
        if (spell == "Hide")
            return 1;
        // Wizard Skills
        if (spell == "Ice")
            return 1;
        if (spell == "Fire")
            return 1;
        // Druid Skills
        if (spell == "Rejuvination")
            return 1;
        if (spell == "Swarm Of Insects")
            return 1;
        // Paladin Skills
        if (spell == "Holy Swing")
            return 1;
        if (spell == "Divine Armor")
            return 1;
        //Tier 2 **************
        // Archer Skills
        if (spell == "Head Shot")
            return 1;
        if (spell == "Silence Shot")
            return 1;
        if (spell == "Crippling Shot")
            return 1;
        if (spell == "Dash")
            return 1;
        if (spell == "Identify Enemy")
            return 1;
        if (spell == "Track")
            return 1;
        // Enchanter Skills
        if (spell == "Root")
            return 1;
        if (spell == "Invisibility")
            return 1;
        if (spell == "Rune")
            return 1;
        if (spell == "Slow")
            return 2;
        if (spell == "Magic Sieve")
            return 1;
        if (spell == "Aneurysm")
            return 2;
        // Fighter Skills
        if (spell == "Intimidating Roar")
            return 1;
        if (spell == "Protect")
            return 1;
        if (spell == "Knockback")
            return 1;
        if (spell == "Throw Stone")
            return 1;
        if (spell == "Heavy Swing")
            return 1;
        if (spell == "Taunt")
            return 1;
        // Priest Skills
        if (spell == "Cure Poison")
            return 1;
        if (spell == "Dispel")
            return 1;
        if (spell == "Fortitude")
            return 1;
        if (spell == "Turn Undead")
            return 2;
        if (spell == "Critical Heal")
            return 0;
        if (spell == "Undead Protection")
            return 1;
        // Rogue Skills
        if (spell == "Picklock")
            return 1;
        if (spell == "Steal")
            return 1;
        if (spell == "Detect Traps")
            return 1;
        if (spell == "Tendon Slice")
            return 1;
        if (spell == "Backstab")
            return 1;
        if (spell == "Rush")
            return 1;
        // Wizard Skills
        if (spell == "Fireball")
            return 2;
        if (spell == "Light")
            return 1;
        if (spell == "Magic Missile")
            return 1;
        if (spell == "Spell Crit")
            return 0;
        if (spell == "Ice Block")
            return 2;
        if (spell == "Ice Blast")
            return 2;
        // Druid Skills
        if (spell == "Thorns")
            return 1;
        if (spell == "Nature's Protection")
            return 2;
        if (spell == "Strength")
            return 1;
        if (spell == "Snare")
            return 1;
        if (spell == "Engulfing Roots")
            return 1;
        if (spell == "Shapeshift")
            return 2;
        // Paladin Skills
        if (spell == "Flash Of Light")
            return 1;
        if (spell == "Undead Slayer")
            return 0;
        if (spell == "Stun")
            return 1;
        if (spell == "Celestial Wave")
            return 2;
        if (spell == "Angelic Shield")
            return 1;
        if (spell == "Cleanse")
            return 1;
        //Tier 3 **************
        // Archer Skills
        if (spell == "Fire Arrow")
            return 2;
        if (spell == "Penetrating Shot")
            return 2;
        if (spell == "Sleep")
            return 2;
        if (spell == "Perception")
            return 0;
        // Enchanter Skills
        if (spell == "Gravity Stun")
            return 4;
        if (spell == "Weaken")
            return 2;
        if (spell == "Resist Magic")
            return 2;
        if (spell == "Purge")
            return 1;
        // Fighter Skills
        if (spell == "Block")
            return 2;
        if (spell == "Tank Stance")
            return 1;
        if (spell == "Offensive Stance")
            return 1;
        if (spell == "Critical Strike")
            return 0;
        // Priest Skills
        if (spell == "Smite")
            return 2;
        if (spell == "Shield Bash")
            return 1;
        if (spell == "Greater Heal")
            return 2;
        if (spell == "Group Heal")
            return 4;
        // Rogue Skills
        if (spell == "Blind")
            return 1;
        if (spell == "Treasure Finding")
            return 0;
        if (spell == "Ambidexterity")
            return 0;
        if (spell == "Poison")
            return 1;
        // Wizard Skills
        if (spell == "Blizzard")
            return 6;
        if (spell == "Magic Burst")
            return 1;
        if (spell == "Teleport")
            return 1;
        if (spell == "Meteor Shower")
            return 8;
        // Druid Skills
        if (spell == "Staff Specialization")
            return 0;
        if (spell == "Tornado")
            return 5;
        if (spell == "Chain Lightning")
            return 4;
        if (spell == "Greater Rejuvination")
            return 2;
        // Paladin Skills
        if (spell == "Consecrated Ground")
            return 1;
        if (spell == "Divine Wrath")
            return 2;
        if (spell == "Cover")
            return 1;
        if (spell == "Root")
            return 1;
        //Tier 4 ****************
        // Archer Skills
        if (spell == "Double Shot")
            return 2;
        if (spell == "Natures Precision")
            return 1;
        // Enchanter Skills
        if (spell == "Charm")
            return 4;
        if (spell == "Mp Transfer")
            return 6;
        // Fighter Skills
        if (spell == "Riposte")
            return 0;
        if (spell == "Double Attack")
            return 0;
        // Priest Skills
        if (spell == "Regeneration")
            return 2;
        if (spell == "Resurrect")
            return 10;
        // Rogue Skills
        if (spell == "Double attack")
            return 0;
        if (spell == "Sneak")
            return 1;
        // Wizard Skills
        if (spell == "Incinerate")
            return 4;
        if (spell == "Brain Freeze")
            return 4;
        // Druid Skills
        if (spell == "Solar Flare")
            return 3;
        if (spell == "Evacuate")
            return 5;
        // Paladin Skills
        if (spell == "Lay On Hands")
            return 3;
        if (spell == "Double Attack")
            return 0;
        return cost;
    }
    IEnumerator FollowPath(List<Node> path){
		while(stamina > 0){
			yield return null;
		}
        Node currentNode = null;
        int pathLength = path.Count;
        for (int i = 0; i < pathLength; i++)
        {
            currentNode = path[i];
             if (CurrentNode != null)
            {
                CurrentNode.occupant = null; // Remove ourselves from the current node
            }

            CurrentNode = currentNode;
            CurrentNode.occupant = this; // Set ourselves as the new node's occupant
            Debug.Log("FollowPath: Starting to move to a new node.");
            Vector2 start = transform.position;
            Vector2 end = currentNode.position;
            float t = 0;
            while (t < 1)
            {
                if (!isMoving)
                {
                    Debug.Log("FollowPath: Movement was interrupted, breaking from the coroutine.");
                    yield break;
                }
                //NewFogUpdate(currentNode.position);
                t += Time.deltaTime / travelTime;
                transform.position = Vector2.Lerp(start, end, t);
                yield return null;
            }
        }
        Debug.Log("FollowPath: Finished following the path.");
        isMoving = false;
    }
public Node CurrentNode;
}}