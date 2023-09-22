using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Mirror;
namespace dragon.mirror{
[System.Serializable]
public class CharacterSaveData {
    public int CharHealth;
    public int CharMana;
    public float CharExperience;
    public float CharClassPoints;
    public string CharID;
    public CharacterSaveData() { }
    public CharacterSaveData(int health, int mana,float exp, float classPoints, string ID)
    {
        CharHealth = health;
        CharMana = mana;
        CharExperience = exp;
        CharClassPoints = classPoints;
        CharID = ID;
    }
}
public class Buff
{
    public string Stat { get; private set; }
    public float Duration { get; private set; }
    public int Value { get; private set; }
    public string SpellName { get; private set; }
    public bool IsBuff { get; private set; }

    public Buff(string stat, float duration, int value, string spellName, bool isBuff)
    {
        Stat = stat;
        Duration = duration;
        Value = value;
        SpellName = spellName;
        IsBuff = isBuff;
    }
}
public class PlayerCharacter : MovingObject
{
    public static UnityEvent<PlayerCharacter, ScenePlayer, Match, int> CharacterSprite = new UnityEvent<PlayerCharacter, ScenePlayer, Match, int>();
    public static UnityEvent<MovingObject> ResetSpells = new UnityEvent<MovingObject>();

    [SerializeField] public static UnityEvent<NetworkConnectionToClient, CharacterSaveData>  SaveCharacter = new UnityEvent<NetworkConnectionToClient, CharacterSaveData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, int, string>  TrapMP = new UnityEvent<NetworkConnectionToClient, int, string>();

    public static UnityEvent<string> CombatRefresh = new UnityEvent<string>();
	[SyncVar]
	[SerializeField] public string CharacterName;
	[SyncVar]
	[SerializeField] public string ClassType;
    [SyncVar]
    [SerializeField] public bool Bow = false;
	[SerializeField] public int Level;
    [SyncVar]
	[SerializeField] public bool SoFire;
	[SyncVar]
	[SerializeField] public string CharID;
    float PLAYEREXPERIENCE;
    float PLAYERCLASSPOINTS;

	public Match assignedMatch;
	[SyncVar] [SerializeField] public ScenePlayer assignedPlayer;
	private const string CastingQ = "CastingQ";
    private const string CastingE = "CastingE";
    private const string CastingR = "CastingR";
    private const string CastingF = "CastingF";
    private const string Selected = "Selected";
    [SerializeField] GameObject CombatPartyMemberBuilt;
    [SerializeField] GameObject CombatPartyMemberPrefab;
    [SerializeField] public static UnityEvent<string> ImproperCheckText = new UnityEvent<string>();

    //create the stamps
    private List<Buff> activeBuffs = new List<Buff>();
    public void AddBuff(string stat, float duration, int value, string spellName, bool buff){
        Buff newBuff = new Buff(stat, duration, value, spellName, buff);
        activeBuffs.Add(newBuff);
        
        ////print($"{newBuff.SpellName} has started at {Time.time}");
        StartCoroutine(RemoveBuffAfterDuration(newBuff));
    }
    
    private IEnumerator RemoveBuffAfterDuration(Buff buff){
        if (GetComponent<NetworkIdentity>().hasAuthority){
            // Invoke the event
            //CombatRefresh.Invoke(CharID);
        }
        yield return new WaitForSeconds(buff.Duration);
        activeBuffs.Remove(buff);
        ////print($"{buff.SpellName} has ended at {Time.time}");
        // Check if the client has authority
        if (GetComponent<NetworkIdentity>().hasAuthority){
            // Invoke the event
            //CombatRefresh.Invoke(CharID);
        }
    }
    public List<Buff> GetCharacterBuffList(){
        return activeBuffs;
    }
	protected override void Awake(){
		base.Awake();
	}
    protected override void Start(){
        base.Start();
		if(isServer){
            #if UNITY_SERVER
            PlayFabServer.SendEXPCP.AddListener(AcceptCPEXP);
            PlayFabServer.ENDMATCHMAKER.AddListener(SaveUnit);
            #endif
            ScenePlayer.ChangingMOSpellsMatch.AddListener(ChangingSpells);
			return;
		}
		ScenePlayer.BuildCombatPlayerUI.AddListener(BuildPlayerCharacterUICombat);
    }
    [Server]
    public void ServerTrap(int hp, int mp, bool debuff){
        if (Dying)
		return;
        cur_hp -= hp;
        cur_mp -= mp;
        if (cur_hp <= 0){
                ServerStopCasting();
	    		cur_hp = 0;
	    			if(!Dying){
	    				Dying = true;
	    				DeathEXP();
	    				StopATTACKINGMob();
	    				agent.isStopped = true;
	    				//agent.enabled = false;
	    				CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
	    				circleCollider.enabled = false;
	    				DeathCharacter.Invoke(assignedPlayer.connectionToClient, CharID);
	    				RemoveCharReserve.Invoke(this);
	    				//RevokePlayerAuthority();
	    				RpcCharDeath();
	    				DeadChar.Invoke(this, assignedMatch);
	    				Target = null;
	    			}
	    } else {
            if(hp > 0){
	            TakeDamageCharacter.Invoke(assignedPlayer.connectionToClient, cur_hp, CharID);
            }
            if(mp > 0){
	            TrapMP.Invoke(assignedPlayer.connectionToClient, cur_mp, CharID);
            }
            if(hp > 0 || mp > 0){
                RpcSpawnTrapDmg(hp, mp);
            }
	    }
    }
    [Server]
    void ChangingSpells(string ID, ScenePlayer CharOwner, SendSpellList spellList){
        if(ID == CharID && CharOwner == assignedPlayer){
            SpellQ = spellList.SpellQ;
            SpellE = spellList.SpellE;
            SpellR = spellList.SpellR;
            SpellF = spellList.SpellR;
            print($"{CharacterName} Q spell is {SpellQ}");
            print($"{CharacterName} E spell is {SpellE}");
            print($"{CharacterName} R spell is {SpellR}");
            print($"{CharacterName} F spell is {SpellF}");
            TargetResetSpell(spellList);
        }
    }
    [TargetRpc]
    void TargetResetSpell(SendSpellList spellList){
        ResetSpells.Invoke(this);
    }
    [Server]
    void AcceptCPEXP(Match match, float cp, float exp){
        if(Dying){
            return;
        }
        if(match == assignedMatch){
            PLAYERCLASSPOINTS += cp;
            PLAYEREXPERIENCE += exp;
        }
    }
    [Command]
    public void CmdUnlockDoor(Door door, Match match)
    {
        curatorTM.CuratorUnlockDoor(door, match);
    }
    [Command]
    public void CmdCloseDoor(Door door, Match match)
    {
        curatorTM.CuratorCloseDoor(door, match);
    }
    [Command]
    public void CmdBreakDoor(Door door, Match match)
    {
        curatorTM.CuratorBreakDoor(door, match);
    }

    [Command]
    public void CmdOpenDoor(Door door, Match match)
    {
        curatorTM.CuratorOpenDoor(door, match);
    }
    [Command]
    public void CmdOpenMainChest(MainChest mainchest){
        curatorTM.OpenChest(mainchest, assignedMatch);
    }
    [Command]
    public void CmdOpenMiniChest(MiniChest miniChest){
        curatorTM.LootMiniChest(miniChest, assignedMatch);
    }
    [Command]
    public void CmdPickUpArmor(ArmorDrop armorDrop){
        curatorTM.LootArmorRack(armorDrop, assignedMatch);
    }
    [Command]
    public void CmdPickUpWeapon(WeaponDrop weaponDrop){
        curatorTM.LootWeaponRack(weaponDrop, assignedMatch);
    }
	[Server]
	public void EnergySpark(MovingObject obj, bool inside){
		if(obj == this){
			StartCoroutine(EnergyUpdaterCharacter(inside));
		}
	}
	[Server]
	public IEnumerator EnergyUpdaterCharacter(bool inside){
		////print($"ENERGIZEDDDDDDD this character {gameObject.name}");
		float rechargeTime = .5f;
		float haste = 0f;
		Energized = true;
        stamina = 0f;
		ClientSparkVision(inside);
		SetStatsServer();
        agent.enabled = true;
        //StartCoroutine(UpdateAgentRoutine());
		while (Energized)
    	{
    	    if (!moving && !Casting && !Mesmerized && !Stunned && !Feared)
    	    {
					if(GetAgility() > 101){
						haste = Mathf.Floor((GetAgility() - 100) / 2);
					} else {
						haste = 0f;
					}
				rechargeTime = .5f / (1f + (haste / 100f));
				stamina -= 5f;
        		stamina = Mathf.Clamp(stamina, -100f, 250f);
				//if(stamina <= 0){
    	        //    RpcClientCheckColor();
    	        //} else {
				//	RpcChangeToGray();
				//}
				yield return new WaitForSeconds(rechargeTime);
    	    } else {
				yield return null;
			}
    	}
    }
    public void DeathEXP(){
        PLAYEREXPERIENCE *= .9f;
    }
    IEnumerator UpdateAgentRoutine() {
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

                                //agent.radius *= 2;
                                //syncAgentRadius = agent.radius;
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
                        //if (!moving)
                        //{
                        //    moving = true;
                        //    // Reduce the baseOffset by half when agent starts moving
                        //    agent.radius /= 2;
                        //    syncAgentRadius = agent.radius;
                        //}
                        //moving = true;
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
    [ClientRpc]
	public void ClientSparkVision(bool inside){
		StartCoroutine(VisionSpark(inside));
	}
    IEnumerator VisionSpark(bool inside){
		yield return new WaitForSeconds(2f);
		float RevealingRange = inside ? 4f : 7f;
		if (SoFire) {
		    RevealingRange = inside ? 6f : 9f;
		}
		FoggyWar fogofWar = GameObject.Find("FogMachine").GetComponent<FoggyWar>();
		if(!fogofWar){
			yield break;
		}
		//print($"Sending AddCharacter to {CharacterName} in the fog machaine for {RevealingRange} vision!!");
		fogofWar.AddCharacter(this.gameObject, RevealingRange);
		fogofWar.UpdateFogOfWar(this.gameObject, transform.position);
		//print($"Sending visionspark to {GetComponent<PlayerCharacter>().CharacterName}");
		//FogUpdate.Invoke(transform.position, RevealingRange);
		
	}
	[Server]
	public void AssignedPayerAndMatch(ScenePlayer player, Match match, string number){
		////print("assigning playercharacter its stats");
		assignedMatch = match;
		assignedPlayer = player;
             // Calculate the raw arcana value from the MPBonus

		StartCoroutine(SetUpPrefab(player, number));
	}
	[Server]
	IEnumerator SetUpPrefab(ScenePlayer player, string charID){
		yield return new WaitForSeconds(1f);
		PrefabCharacterSetup(player, charID);
	}
	[Server]
	void PrefabCharacterSetup(ScenePlayer player, string charID){
        string _class = string.Empty;
        string _CORE = string.Empty;
        int level = 1;
        float currentWeight = 0f;
        List<CharacterFullDataMessage> characterSheets = player.GetInformationSheets();
        foreach(var sheet in characterSheets){
            if(sheet.CharacterID == charID){
                foreach(var spellItem in sheet.CharSpellData){
                    if(spellItem.Key == "SPELLQ"){
                        SpellQ = spellItem.Value;
                    }
                    if(spellItem.Key == "SPELLE"){
                        SpellE = spellItem.Value;
                    }
                    if(spellItem.Key == "SPELLR"){
                        SpellR = spellItem.Value;
                    }
                    if(spellItem.Key == "SPELLF"){
                        SpellF = spellItem.Value;
                    }
                }
                foreach(var stat in sheet.CharStatData){
                    if(stat.Key == "CORE"){
                        _CORE = stat.Value;
                    }
                    if(stat.Key == "Class"){
                        _class = stat.Value;
                        ClassType = stat.Value;
                    }
                    if(stat.Key == "currentHP"){
                        cur_hp += int.Parse(stat.Value);
                        
                    }
                    if(stat.Key == "currentMP"){
                        cur_mp += int.Parse(stat.Value);
                        
                    }
                    if(stat.Key == "LVL"){
                        Level = int.Parse(stat.Value);
                        level = int.Parse(stat.Value);
                    }
                    if(stat.Key == "CharName"){
			        	CharacterName = stat.Value;
			        }
                    if(stat.Key == "EXP"){
			        	PLAYEREXPERIENCE = float.Parse(stat.Value);
			        }
                    if(stat.Key == "ClassPoints"){
			        	PLAYERCLASSPOINTS = float.Parse(stat.Value);
			        }
                    if(stat.Key == "CharacterID"){
			        	CharID = stat.Value;
			        }
                }
                if(sheet.CharCooldownData != null){
                    List<CharacterCooldownListItem> GarbageList = new List<CharacterCooldownListItem>();
                    foreach(var coolies in sheet.CharCooldownData){
                        print($"{coolies.Value} is when this cooldown {coolies.SpellnameFull} is due to expire");
                        if(coolies.Position == "SPELLQ"){
                            print($"{coolies.Value} was when we were supposed to be done with the spell {DateTime.Now} is time now");
                            if (DateTime.TryParse(coolies.Value, out DateTime endTime))
                            {
                                // Calculate the remaining time by subtracting the current time from the end time
                                TimeSpan remainingTime = endTime - DateTime.Now;
                                float newTime = (float)remainingTime.TotalSeconds;
                                print($"{newTime} is our newtime for this spell!! almos tthere!!");
                                if(newTime > 30f){
                                    StartCoroutine(SetAbilityCoolDownQ(newTime, true));
                                } else {
                                    GarbageList.Add(coolies);
                                    CooldownQ = 0f;
                                }
                            }
                        }
                        if(coolies.Position == "SPELLE"){
                            print($"{coolies.Value} was when we were supposed to be done with the spell {DateTime.Now} is time now");
                            if (DateTime.TryParse(coolies.Value, out DateTime endTime))
                            {
                                // Calculate the remaining time by subtracting the current time from the end time
                                TimeSpan remainingTime = endTime - DateTime.Now;
                                float newTime = (float)remainingTime.TotalSeconds;
                                print($"{newTime} is our newtime for this spell!! almos tthere!!");
                                if(newTime > 30f){
                                    StartCoroutine(SetAbilityCoolDownE(newTime, true));
                                } else {
                                    GarbageList.Add(coolies);
                                    CooldownE = 0f;
                                }
                            }
                        }
                        if(coolies.Position == "SPELLR"){
                            print($"{coolies.Value} was when we were supposed to be done with the spell {DateTime.Now} is time now");
                            if (DateTime.TryParse(coolies.Value, out DateTime endTime))
                            {
                                // Calculate the remaining time by subtracting the current time from the end time
                                TimeSpan remainingTime = endTime - DateTime.Now;
                                float newTime = (float)remainingTime.TotalSeconds;
                                print($"{newTime} is our newtime for this spell!! almos tthere!!");
                                if(newTime > 30f){
                                    StartCoroutine(SetAbilityCoolDownR(newTime, true));
                                } else {
                                    GarbageList.Add(coolies);
                                    CooldownR = 0f;
                                }
                            }
                        }
                        if(coolies.Position == "SPELLF"){
                            print($"{coolies.Value} was when we were supposed to be done with the spell {DateTime.Now} is time now");
                            if (DateTime.TryParse(coolies.Value, out DateTime endTime))
                            {
                                // Calculate the remaining time by subtracting the current time from the end time
                                TimeSpan remainingTime = endTime - DateTime.Now;
                                float newTime = (float)remainingTime.TotalSeconds;
                                print($"{newTime} is our newtime for this spell!! almos tthere!!");
                                if(newTime > 30f){
                                    StartCoroutine(SetAbilityCoolDownF(newTime, true));
                                } else {
                                    GarbageList.Add(coolies);
                                    CooldownF = 0f;
                                }
                            }
                        }
                    }
                    foreach(var garbageItem in GarbageList){
                        assignedPlayer.ServerCooldownRemove(CharID, garbageItem);
                    }
                }
                bool sofPresent = false;
                foreach(var charItem in sheet.CharInventoryData){
                    if(charItem.Value.GetEQUIPPED()){
                        if(charItem.Value.GetItemSpecificClass() == "Bow"){
                            //print($"{CharacterName} Had Bow");
                            Bow = true;
                        }
                        if(charItem.Value.GetItemName() == "Sword Of Fire"){
                            //print($"{CharacterName} Had SoFire");

                            sofPresent = true;
                            BonusFireWeapon = true;
                            BonusFireEffect = 1 * level;
                        }
                        if(charItem.Value.GetItemName() == "Acidic Axe"){
                            //print($"Had Axe");

                            BonusPoisonWeapon = true;
                            BonusPoisonEffect = 1 * level;
                        }
                        if(charItem.Value.GetItemName() == "Bow Of Power"){
                            //print($"Had Bow");

                            BonusMagicWeapon = true;
                            BonusMagicEffect = 1 * level;
                            Bow = true;
                        }
                        if(charItem.Value.GetItemName() == "Frozen Greatsword"){
                            //print($"Had BonusColdWeapon");

                            BonusColdWeapon = true;
                            BonusColdEffect = 2 * level;
                            FrozenColdWeapon = true;
                        }
                        if(charItem.Value.GetItemName() == "Greatspear Of Dragonslaying"){
                            //print($"Had BonusDragonWeapon");

                            BonusDragonWeapon = true;
                            BonusDragonEffect = 5 * level;
                        }
                        if(charItem.Value.GetItemName() == "Mace Of Healing"){
                            //print($"Had healingIncrease");

                            healingIncrease = 1 * level;
                        }
                        if(charItem.Value.GetItemName() == "Spear Of Dragonslaying"){
                            //print($"Had BonusDragonWeapon");

                            BonusDragonWeapon = true;
                            BonusDragonEffect = 2 * level;
                        }
                        if(charItem.Value.GetItemName() == "Staff Of Protection"){
                            //print($"Had armor increase");

                            armor += (1 * level);
                        }
                        if(charItem.Value.GetItemName() == "Thunder Infused Greathammer"){
                            //print($"Had BonusMagicWeapon");

                            BonusMagicWeapon = true;
                            BonusMagicEffect = 2 * level;
                        }
                        if(charItem.Value.GetItemName() == "Vampiric Dagger"){
                            //print($"Had leech!!");
                            
                            BonusLeechWeapon = true;
                            BonusLeechEffect = 1 + (level - 1) / 3;
                        }
                        if(charItem.Value.GetItemName() == "Venomous Greataxe"){
                            //print($"Had BonusPoisonWeapon and some big ass damage!!");

                            BonusPoisonWeapon = true;
                            BonusPoisonEffect = 2 * level;
                        }
                        if(charItem.Value.GetSTRENGTH_item() != null)
                        {
                            strength += int.Parse(charItem.Value.GetSTRENGTH_item());
                        }
                        if(charItem.Value.GetAGILITY_item() != null)
                        {
                            agility += int.Parse(charItem.Value.GetAGILITY_item());
                        }
                        if(charItem.Value.GetFORTITUDE_item() != null)
                        {
                            fortitude +=  int.Parse(charItem.Value.GetFORTITUDE_item());
                        }
                        if(charItem.Value.GetARCANA_item() != null)
                        {
                            arcana += int.Parse(charItem.Value.GetARCANA_item());
                        }
                        if(charItem.Value.GetArmor_item() != null)
                        {
                            armor += int.Parse(charItem.Value.GetArmor_item());
                        }
                        if(charItem.Value.GetMagicResist_item() != null)
                        {
                            MagicResist += int.Parse(charItem.Value.GetMagicResist_item());
                        }
                        if(charItem.Value.GetFireResist_item() != null)
                        {
                            FireResist += int.Parse(charItem.Value.GetFireResist_item());
                        }
                        if(charItem.Value.GetColdResist_item() != null)
                        {
                            ColdResist += int.Parse(charItem.Value.GetColdResist_item());
                        }
                        if(charItem.Value.GetDiseaseResist_item() != null)
                        {
                            DiseaseResist += int.Parse(charItem.Value.GetDiseaseResist_item());
                        }
                        if(charItem.Value.GetPoisonResist_item() != null)
                        {
                            PoisonResist += int.Parse(charItem.Value.GetPoisonResist_item());
                        }
                        if(charItem.Value.GetBlockChance() != "0" && !string.IsNullOrEmpty(charItem.Value.GetBlockChance())){
                            shield = true;
                            shieldChance = int.Parse(charItem.Value.GetBlockChance());
                        }
                        if(charItem.Value.GetBlockValue() != "0" && !string.IsNullOrEmpty(charItem.Value.GetBlockValue())){
                            shield = true;
                            shieldValue = int.Parse(charItem.Value.GetBlockValue());
                            ThreatMod = true;
                            ThreatModifier = 3.5f;
                        }
                        if(charItem.Value.GetDamageMin() != "0" && !string.IsNullOrEmpty(charItem.Value.GetDamageMin())){
                            if(charItem.Value.GetEQUIPPEDSLOT() == "Main-Hand"){
                                minDmgMH = int.Parse(charItem.Value.GetDamageMin());
                            }
                        }
                        if(charItem.Value.GetDamageMax() != "0" && !string.IsNullOrEmpty(charItem.Value.GetDamageMax())){
                            if(charItem.Value.GetEQUIPPEDSLOT() == "Main-Hand"){
                                maxDmgMH = int.Parse(charItem.Value.GetDamageMax());
                                if(!string.IsNullOrEmpty(charItem.Value.GetAttackDelay()))
                                {
                                    attackDelay += int.Parse(charItem.Value.GetAttackDelay());
                                }
                                if(!string.IsNullOrEmpty(charItem.Value.GetPenetration()))
                                {
                                    penetration = int.Parse(charItem.Value.GetPenetration());
                                }
                                if(!string.IsNullOrEmpty(charItem.Value.GetParry()))
                                {
                                    parry += int.Parse(charItem.Value.GetParry());
                                }
                            }
                        }
                        if(charItem.Value.GetDamageMin() != "0" && !string.IsNullOrEmpty(charItem.Value.GetDamageMin())){
                            if(charItem.Value.GetEQUIPPEDSLOT() == "Off-Hand"){
                                duelWielding = true;
                                minDmgOH = int.Parse(charItem.Value.GetDamageMin());
                            }
                        }
                        if(charItem.Value.GetDamageMax() != "0" && !string.IsNullOrEmpty(charItem.Value.GetDamageMax())){
                            if(charItem.Value.GetEQUIPPEDSLOT() == "Off-Hand"){
                                duelWielding = true;
                                maxDmgOH = int.Parse(charItem.Value.GetDamageMax());
                                if(!string.IsNullOrEmpty(charItem.Value.GetAttackDelay()))
                                {
                                    attackDelay += int.Parse(charItem.Value.GetAttackDelay());
                                }
                                if(!string.IsNullOrEmpty(charItem.Value.GetPenetration()))
                                {
                                    penetrationOH = int.Parse(charItem.Value.GetPenetration());
                                }
                                if(!string.IsNullOrEmpty(charItem.Value.GetParry()))
                                {
                                    parry += int.Parse(charItem.Value.GetParry());
                                }
                            }
                        }
                    }
                    currentWeight = currentWeight + (float.Parse(charItem.Value.GetWeight()) * charItem.Value.GetAmount());
                }
                if(sofPresent){
                    SoFire = true;
                } else {
                    SoFire = false;
                }
                //print($"{SoFire} is what SoFire is set to on {CharacterName}");
                break;
            }
        }
        (int baseStrength, int baseAgility, int baseFortitude, int baseArcana) = player.GetCharacterStats(_class, level, _CORE);
        strength = strength + baseStrength;
        agility = agility + baseAgility;
        fortitude = fortitude + baseFortitude;
        arcana = arcana + baseArcana;
        dodge = agility / 20;
        max_hp = fortitude;
        max_mp = arcana / 7;
		if(maxDmgMH == 0){
			maxDmgMH = strength/ 20;
		}
		if(minDmgMH == 0){
			minDmgMH = 1;
		}
	}
    public int ApplyAgilityReduction(int agility, int strength, float carriedWeight)
    {
        float agilityReduction = CalculateAgilityReduction(strength, carriedWeight);
        float reducedAgility = agility * (1 - agilityReduction);
        return (int)reducedAgility;
    }
    public float CalculateAgilityReduction(int strength, float carriedWeight)
    {
        float maxCarryCapacity = strength; // 1 strength = 1 pound carry cap max
        float agilityReduction = 0f;

        if (carriedWeight > maxCarryCapacity * 1.5) // Over 150% max weight cap
        {
            agilityReduction = 0.75f; // 75% reduction
        }
        else if (carriedWeight > maxCarryCapacity * 1.25) // Over 125% max weight cap
        {
            agilityReduction = 0.5f; // 50% reduction
        }
        else if (carriedWeight > maxCarryCapacity) // Over 100% max weight cap
        {
            agilityReduction = 0.25f; // 25% reduction
        }

        return agilityReduction;
    }
	public void OnMouseEnter(){
		if (isServer)
    	{
    	    return;
    	}
    	    MouseOverCombat mouseOverBox = GameObject.Find("MouseOverCombat").GetComponent<MouseOverCombat>();
    	    Canvas mouseOverBoxCanvas = mouseOverBox.GetComponent<Canvas>();
    	    mouseOverBoxCanvas.enabled = true;
    	    mouseOverBox.InjectName(CharacterName);
    	    mouseOverBox.transform.position = Input.mousePosition + new Vector3(100, 100, 0);
            HoverNoise.Invoke();
    }
    	public void OnMouseExit(){

		if(isServer){
			return;
		}
		MouseOverCombat mouseOverBox = GameObject.Find("MouseOverCombat").GetComponent<MouseOverCombat>();
       	Canvas mouseOverBoxCanvas = mouseOverBox.GetComponent<Canvas>();
	   	mouseOverBoxCanvas.enabled = false;
    	}
	[Command]
	public void CmdProjectedCastTile(Vector3 mousePosition, float duration){
		foreach(var player in assignedMatch.players){
			//MoveableTile castingTile = Instantiate()
			player.TargetShowCastTile(mousePosition, duration);
		}
	}
	[Command]
	public void CmdSelfTarget(string spellname, string Mode){
		var nameMatch = System.Text.RegularExpressions.Regex.Match(spellname, @"^\D*");
        string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
        int _spellRank = 1;
        // Extract spell rank
        var rankMatch = System.Text.RegularExpressions.Regex.Match(spellname, @"\d+$");
        if (rankMatch.Success) {
            _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
        }
		int cost = GetSpellCost(spell);
        if(cur_mp < cost){
			return;
		}
        float cooldown = GetSpellCooldown(spell, _spellRank);
        //print($"about to set the cooldown time {cooldown} for {CharacterName} using {spell} and the mode is {mode}");
        if(Mode == CastingQ){
            StartCoroutine(SetAbilityCoolDownQ(cooldown, false));
        }
        if(Mode == CastingE){
            StartCoroutine(SetAbilityCoolDownE(cooldown, false));
        }
        if(Mode == CastingR){
            StartCoroutine(SetAbilityCoolDownR(cooldown, false));
        }
        if(Mode == CastingF){
            StartCoroutine(SetAbilityCoolDownF(cooldown, false));
        }
		
		//Self target aoes
		//aoe list fighter intimidating roar 2x2,
        ServerSelfTargeter(spellname, cost);
	}
    [Server]
    void ServerSelfTargeter(string spellname, int cost){
        var nameMatch = System.Text.RegularExpressions.Regex.Match(spellname, @"^\D*");
        string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
        int _spellRank = 1;
        // Extract spell rank
        var rankMatch = System.Text.RegularExpressions.Regex.Match(spellname, @"\d+$");
        if (rankMatch.Success) {
            _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
        }
		curatorTM.UpdatePlayerSelfCasted(assignedPlayer.currentMatch, this, spell, _spellRank, cost);
    }
    float finalRange;
    public bool InSpellRange(MovingObject caster, MovingObject target, string mode){
        int lvl = caster.GetComponent<PlayerCharacter>().Level;
        float range = 0f;
        float baseRange = 0f;
        bool inRange = false;
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
            baseRange = 1.5f;
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
            baseRange = 1.5f;

        // Fighter Skills
        if (spell == "Charge")
            baseRange = 5f;
        if (spell == "Bash")
            baseRange = 1.5f;
        if (spell == "Intimidating Roar")
            baseRange = 1.5f;
        if (spell == "Protect")
            baseRange = 1.5f;
        if (spell == "Knockback")
            baseRange = 1.5f;
        if (spell == "Throw Stone")
            baseRange = 5f;
        if (spell == "Heavy Swing")
            baseRange = 1.5f;
        if (spell == "Taunt")
            baseRange = 1.5f;
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
            baseRange = 1.5f;
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
            baseRange = 1.5f;
        if (spell == "Steal")
            baseRange = 1.5f;
        if (spell == "Tendon Slice")
            baseRange = 1.5f;
        if (spell == "Backstab")
            baseRange = 1.5f;
        if (spell == "Blind")
            baseRange = 5f;
        if (spell == "Poison")
            baseRange = 1.5f;
        // Wizard Skills
        if (spell == "Ice")
            baseRange = 5f;
        if (spell == "Fire")
            baseRange = 5f;
        if (spell == "Blizzard")
            baseRange = 6f;
        if (spell == "Magic Burst")
            baseRange = 1.5f;
        if (spell == "Teleport")
            baseRange = 7f;
        if (spell == "Meteor Shower")
            baseRange = 7f;
        if (spell == "Spell Crit")
            baseRange = 0f;
        if (spell == "Ice Block")
            baseRange = 1.5f;
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
            baseRange = 1.5f;
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
            baseRange = 5f;
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
            baseRange = 1.5f;
        if (spell == "Cleanse")
            baseRange = 5f;
        if (spell == "Consecrated Ground")
            baseRange = 5f;
        if (spell == "Divine Wrath")
            baseRange = 1.5f;
        if (spell == "Cover")
            baseRange = 1.5f;
        if (spell == "Shackle")
            baseRange = 5f;
        if (spell == "Lay On Hands")
            baseRange = 1.5f;
        range = (baseRange * ((_spellRank - 1) * .004f) + baseRange);
        
        float distance = Vector2.Distance(caster.transform.position, target.transform.position);

        if(distance <= range)
        {
            inRange = true;
            
        }
        else
        {
            inRange = false;
        }
        print($"{baseRange} is the base range for the spell {spell}");
        finalRange = range;
        return inRange;
    }
    [Command]
    public void CmdCastAOESpell(string mode, Vector2 mousePosition){
        Casting = false;
        
        string _spell = string.Empty;
        if(mode == CastingQ){
            _spell = SpellQ;
            if(SpellQCoolDown){
                return;
            }
        }
        if(mode == CastingE){
            _spell = SpellE;
            if(SpellECoolDown){
                return;
            }
        }
        if(mode == CastingR){
            _spell = SpellR;
            if(SpellRCoolDown){
                return;
            }
        }
        if(mode == CastingF){
            _spell = SpellF;
            if(SpellFCoolDown){
                return;
            }
        }
        var nameMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"^\D*");
        string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
        int _spellRank = 1;
        // Extract spell rank
        var rankMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"\d+$");
        if (rankMatch.Success) {
            _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
        }
        int cost = GetSpellCost(spell);
        float cooldown = GetSpellCooldown(spell, _spellRank);
        if(cur_mp < cost){
			return;
		}
        //print($"about to set the cooldown time {cooldown} for {CharacterName} using {spell} and the mode is {mode}");
        if(mode == CastingQ){
            StartCoroutine(SetAbilityCoolDownQ(cooldown, false));
        }
        if(mode == CastingE){
            StartCoroutine(SetAbilityCoolDownE(cooldown, false));
        }
        if(mode == CastingR){
            StartCoroutine(SetAbilityCoolDownR(cooldown, false));
        }
        if(mode == CastingF){
            StartCoroutine(SetAbilityCoolDownF(cooldown, false));
        }
        bool hostile = !assignedPlayer.DetermineFriendly(spell);
        ProcessAOESpell(spell, _spellRank, cost, mousePosition, hostile);
    }
    
    
	[Command]
	public void CmdCastSpell(string mode, MovingObject target){
        // all single target or instant cast spells
		Casting = false;
        
        string _spell = string.Empty;
        if(mode == CastingQ){
            _spell = SpellQ;
            if(SpellQCoolDown){
                return;
            }
        }
        if(mode == CastingE){
            _spell = SpellE;
            if(SpellECoolDown){
                return;
            }
        }
        if(mode == CastingR){
            _spell = SpellR;
            if(SpellRCoolDown){
                return;
            }
        }
        if(mode == CastingF){
            _spell = SpellF;
            if(SpellFCoolDown){
                return;
            }
        }
        Vector3 movementDirection = target.transform.position - transform.position;
	    bool newRightFace = movementDirection.x >= 0;
	    if (newRightFace != rightFace)
	    {
	    	rightFace = newRightFace;
	    	RpcUpdateFacingDirection(newRightFace);
	    }
        if(!InSpellRange(this, target, mode)){
            print($"target {target.gameObject.name} was out of range of spell {_spell}");
            return;
        }
        print($"Made it to CmdCastSpell");
        var nameMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"^\D*");
        string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
        int _spellRank = 1;
        // Extract spell rank
        var rankMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"\d+$");
        if (rankMatch.Success) {
            _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
        }
        if(spell != "Resurrect"){
            if(target.Dying){
                TargetCannotCastOnDead();
                return;
            }
        }
		int cost = GetSpellCost(spell);
        float cooldown = GetSpellCooldown(spell, _spellRank);
        if(cur_mp < cost){
			return;
		}
        //print($"about to set the cooldown time {cooldown} for {CharacterName} using {spell} and the mode is {mode}");
        if(mode == CastingQ){
            StartCoroutine(SetAbilityCoolDownQ(cooldown, false));
        }
        if(mode == CastingE){
            StartCoroutine(SetAbilityCoolDownE(cooldown, false));
        }
        if(mode == CastingR){
            StartCoroutine(SetAbilityCoolDownR(cooldown, false));
        }
        if(mode == CastingF){
            StartCoroutine(SetAbilityCoolDownF(cooldown, false));
        }
		ProcessSpellCast(mode, this, target, cost);
		//AOE
		//Aoe list enchanter GravityStun 5x5, ResistMagic 5x5
		//Aoe list priest turn undead 6x6, Groupheal 2x2
		//aoe list wizard IceBlast 3x3, IceSpear 1x3, FireBall 3x3, Meteor shower 6x6, Blizzard 4x4
		//aoe list archer 
		//aoe list fighter
		//aoe list rogue 
		//Single target
		//curatorTM.UpdatePlayerCastedDmgSpell();
		//Find the spell cost of each spell d
		//RpcAnimateSpell(spell, mousePosition);

	}
    [TargetRpc]
    void TargetCannotCastOnDead(){
        ImproperCheckText.Invoke("Cannot cast on a dead target");
    }
	
	void BuildPlayerCharacterUICombat(PlayerCharacter player){
		if(player != this){
			return;
		}
		SpawnCombatPartyMemberPrefab();
        CombatPartyView.instance.AddOurUnits(this);
	}
    public GameObject SpawnCombatPartyMemberPrefab() {
		//print($"Building {CharacterName}'s combat UI build");
        CombatPartyMemberBuilt = Instantiate(CombatPartyMemberPrefab, CombatPartyView.instance.CombatPartyParent);
        CombatPartyMemberBuilt.GetComponent<CharacterCombatUI>().SetCharacter(this);
        //newUICombatCharacter.transform.SetSiblingIndex(player.playerIndex - 1);
        return CombatPartyMemberBuilt;
    }
    
	public void PlayCastSound(){
		AudioMgr sound = GetComponent<AudioMgr>();
		sound.PlayCastChantSound();
	}
    // Update is called once per frame
    protected override void Update()
    {
		base.Update();

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(isServer){
            return;
        }
        PortalOVM portalOVM = other.gameObject.GetComponent<PortalOVM>();
        if(portalOVM){
            assignedPlayer.PortalOVM();
        }
    }
    public void UpdateCombatUI(string stat, float duration, int value, string spellName, bool buff){
        //this is the new buff we want to build
        // its coming from playercharacter which came from turn manager
        // lets build the stamps here for the buffs and debuffs that wear off over time and fade out
        
        CombatPartyMemberBuilt.GetComponent<CharacterCombatUI>().ReceiveBuff(stat, duration, value, spellName, buff);
    }
    void SaveUnit(Match match){
		if(match == assignedMatch){
            CharacterSaveData savingGame = new CharacterSaveData(cur_hp, cur_mp, PLAYEREXPERIENCE, PLAYERCLASSPOINTS, CharID);
            print($"Killing {CharacterName}, sending its data to be saved at server!! EXP {PLAYEREXPERIENCE} CP {PLAYERCLASSPOINTS} HP {cur_hp} MP {cur_mp}");
            SaveCharacter.Invoke(assignedPlayer.connectionToClient, savingGame);
			//Destroy(this.gameObject);
		}
	}


	Sprite Load( string imageName, string spriteName)
    {
        Sprite[] all = Resources.LoadAll<Sprite>( imageName);
 
        foreach( var s in all)
        {
            if (s.name == spriteName)
            {
                return s;
            }
        }
        return null;
    }


    public void AskToMove(){

    }
}}
