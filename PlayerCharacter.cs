using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCharacter : MovingObject
{
	
	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(SetUpPrefab());
	}
	
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
		//StartCoroutine(SetUpPrefab());


		
		
			
    }
	public IEnumerator SetUpPrefab()
	{
		yield return new WaitForSeconds(.1f);
		PrefabCharacterSetup();
		
	}
	public void PrefabCharacterSetup()
	{	
		
		//transform.name = jsonString
		
		if(transform.name == "Fighter(Clone)" || transform.name == "BlankCharacter(Clone)")
			abilityList.Add(abilityMgr.GetAbility("charge"));
		
		if(transform.name == "Priest")
			abilityList.Add(abilityMgr.GetAbility("heal"));

		if (transform.name == "Mage(Clone)")
		{
			abilityList.Add(abilityMgr.GetAbility("fire"));
			abilityList.Add(abilityMgr.GetAbility("IceStorm"));
			abilityList.Add(abilityMgr.GetAbility("FireStorm"));
		}
		if (transform.name == "Enchanter")
		{
			abilityList.Add(abilityMgr.GetAbility("Mesmerize"));
		}
		//load up gear and stats
	}

    // Update is called once per frame
    protected override void Update()
    {
		base.Update();
		
		if(cur_hp <= 0)
			Destroy(gameObject);
    }
	
	
	void OnMouseEnter()
    {
		//sfxMgr.ShineObject(this.transform.GetComponent<Renderer>().material);
		//foreach(Ability a in abilityList)
			//Debug.Log(a.name + "," + a.cooldown);
    }
	
}
