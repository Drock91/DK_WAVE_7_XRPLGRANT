using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
namespace dragon.mirror{
public class TrapDrop : NetworkBehaviour
{
    Match assignedMatch;
    [SerializeField] Animator animator;
    [SerializeField] Sprite beginningSprite;
    [SerializeField] Sprite endingSprite;
    [SerializeField] SpriteRenderer sRend;
    public static UnityEvent TrapNoise = new UnityEvent();
    public static UnityEvent HoverNoise = new UnityEvent();
    private string NAME = "Trap";
    bool hpTrap = true;
    bool mpTrap = false;
    bool debuffTrap = false;
    int tier = 1;
    bool Sprung = false;
    [Server]
    public void SetMatch(Match match, int _tier, bool hp, bool mp, bool debuff){
        assignedMatch = match;
        tier = _tier;
        hpTrap = hp;
        mpTrap = mp;
        debuffTrap = debuff;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isServer){
            PlayerCharacter pc = other.gameObject.GetComponent<PlayerCharacter>();
            if (pc && !Sprung)
            {
                Sprung = true;
                int hpAmount = 0;
                if(hpTrap){
                    hpAmount = tier;
                }
                int mpAmount = 0;
                if(mpTrap){
                    mpAmount = tier;
                }
                pc.ServerTrap(hpAmount, mpAmount, debuffTrap);
                RpcAnimateTrap();
                StartCoroutine(DelayedDestroy());
            }
        }
    }
    [ClientRpc]
    void RpcAnimateTrap(){
        animator.SetBool("IsSprung", true);
        TrapNoise.Invoke();
    }
    IEnumerator DelayedDestroy(){
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    public void OnMouseEnter(){
		if (isServer)
    	{
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
    }
    public void OnMouseExit(){
		if(isServer){
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