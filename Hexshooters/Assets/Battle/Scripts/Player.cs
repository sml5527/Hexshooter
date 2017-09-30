﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
	public List<Attributes> weaknesses = new List<Attributes>();
	public List<Attributes> strengths = new List<Attributes>();
    public int health;
	public Transform spell;
	public Weapon_Types weapon;
	public List<Object> Chamber = new List<Object>();
	public FieldManager field;
	public bool reload;
	public int PlayerNum;
	private bool yAxisInUse = false;
	private bool xAxisInUse = false;
	Text pHealth;
	public int armorWeakness;
	public StatusManager myStatus;
	protected string atkbutton;
	protected string abilButton1;
	protected string abilButton2;
	public string stat;
	bool breakImmune; //flag to ensure that every water shotgun spell doesn't endlessly apply break
	int stackDmg;
	public Sprite[] playerImages;
	public Sprite[] playerPortaits;
	bool buttonPresed;
	bool allowShot =false;
	public bool hit;
	public bool heal;
	public Image playerDisplay;
	public Image topSpell;
	public Sprite defaultSlot;
	StatusEffect moveLag;
	StatusEffect shotLag;
	public bool basic;
	bool canMove = true;
	int shotLimiter = 0;
	int heldNum =0;

    // Use this for initialization
    void Start () 
	{
		basic = false;

		setPlayer ();

		GameObject p2 = GameObject.FindGameObjectWithTag("Player2");
		if (p2 != null || PlayerNum != 1)
		{
			field = GameObject.FindGameObjectWithTag ("FieldManager").GetComponent<FieldManagerPVP> ();
		} 
		else
		{
			field = GameObject.FindGameObjectWithTag ("FieldManager").GetComponent<FieldManager> ();
		}
		reload = true;
        	health = 100;

    }

	public void weaponAbility(Weapon_Types weaponUsed)
	{
		switch (weaponUsed)
		{
			case Weapon_Types.Revolver:
				if (abilButton1 != null && abilButton2 != null)
				{
					if (Input.GetButtonDown (abilButton1))
					{
						chamberLeft ();
					}
					if (Input.GetButtonDown (abilButton2))
					{
						chamberRight ();
					}
				}
			break;
			case Weapon_Types.Rifle:
				//Fusion
			break;
			case Weapon_Types.Shotgun:
				
			break;
			case Weapon_Types.Gatling:
				if (shotLimiter == 10)
				{
					if (Input.GetButton ("Ability 1_P1") || Input.GetButton ("Ability 2_P1"))
					{
						if (!myStatus.IsAffected (StatusType.Disabled) && !myStatus.IsAffected (StatusType.ShotLag))
						{
							GameObject go = (GameObject)Instantiate (Resources.Load ("Rapid"), new Vector2 (transform.position.x, transform.position.y), Quaternion.identity);
					
							////get thething component on your instantiated object
							Spell mything = go.GetComponent<Spell> ();
					
							////set a mmber variable (must be PUBLIC)
							mything.weaponUsed = weapon; 
							mything.PlayerNum = PlayerNum;
						}
						shotLimiter = 0;
						allowShot = false;
						moveLag = new StatusEffect (.7f);
						moveLag.m_type = StatusType.Bound;
						myStatus.AddEffect (moveLag);
					}
				}
				else if(shotLimiter !=10)
				{
					shotLimiter++;
				}
			break;
			case Weapon_Types.Canegun:

			break;
			case Weapon_Types.Bow:

			break;
		}
	}
	// Update is called once per frame
	public void playerUpdate () 
	{
		canMove = true;
		updateCurrentSpell ();
		if(Chamber.Count >=0)
			field.updateChamberImages (PlayerNum);
		pHealth.text = health.ToString();
		buttonPresed = false;

		weaponAbility(weapon);

		statusAtrributes ();

		if (!myStatus.IsAffected( StatusType.Bound) && !myStatus.IsAffected( StatusType.MoveLag) && !myStatus.IsAffected( StatusType.Bubbled))
		{
			movement ();
		}

		fire();

		if (Chamber.Count == 0 && field.Handful.Count > 0)
		{
			reload = true;
		}

		if (hit)
		{
			GetComponent<SpriteRenderer> ().color = Color.red;
			hit = false;
		}
		else if (heal)
		{
			GetComponent<SpriteRenderer> ().color = Color.blue;
			heal = false;
		}
		else
		{
			GetComponent<SpriteRenderer>().color = Color.white;
		}
	}

	void fire()
	{
		if (allowShot)
		{
			if (Input.GetAxisRaw (atkbutton) > 0 && Chamber.Count > 0)
			{
				if (!buttonPresed)
				{
					buttonPresed = true;
					if (!myStatus.IsAffected (StatusType.Disabled)&& !myStatus.IsAffected( StatusType.ShotLag))
					{
						initiateSpell ();
					}
					allowShot = false;
				}
			}
			if (Input.GetAxisRaw (atkbutton) > 0 && Chamber.Count == 0 && basic)
			{
				if (!buttonPresed)
				{
					buttonPresed = true;
					if (!myStatus.IsAffected (StatusType.Disabled)&& !myStatus.IsAffected( StatusType.ShotLag))
					{
						GameObject go = (GameObject)Instantiate(Resources.Load ("Basic"),new Vector2(transform.position.x,transform.position.y),Quaternion.identity);

						////get the thing component on your instantiated object
						Spell mything = go.GetComponent<Spell>();

						////set a member variable (must be PUBLIC)
						mything.weaponUsed = weapon; 
						mything.PlayerNum = PlayerNum;
						shotLag = new StatusEffect (0.5f);
						shotLag.m_type = StatusType.ShotLag;
						//myStatus.AddEffect (shotLag);
					}
					allowShot = false;
				}
			}
		}
		if (Input.GetAxisRaw (atkbutton) == 0)
			allowShot = true;
	}
	
	void movement()
	{
		bool inboundsX = false;
		bool inboundsY = false;
		bool moveRight = true;
		bool moveLeft  = true;
		bool moveUp  = true;
		bool moveDown = true;

		float Horizontal = 0.0f;
		float vertical = 0.0f;
		string playerArea = "";
		string enemyArea = "";

		if (PlayerNum == 1)
		{
			if (GameObject.FindGameObjectWithTag("Player2") != null && PlayerNum == 1)
			{
				Horizontal = Input.GetAxisRaw ("Horizontal_P1");
				vertical = Input.GetAxisRaw ("Vertical_P1");
			} 
			else
			{
				Horizontal = Input.GetAxisRaw ("Horizontal_Solo");
				vertical = Input.GetAxisRaw ("Vertical_Solo");
			}
			playerArea = "playerZone";
			enemyArea = "enemyZone";
		} else if (PlayerNum == 2)
		{
			Horizontal = Input.GetAxisRaw ("Horizontal_P2");
			vertical = Input.GetAxisRaw ("Vertical_P2");
			playerArea = "enemyZone";
			enemyArea = "playerZone";
		} 
		else
		{
			Horizontal = Input.GetAxisRaw ("Horizontal_P1");
			vertical = Input.GetAxisRaw ("Vertical_P1");
			playerArea = "playerZone";
			enemyArea = "enemyZone";
		}
		//Checks for Left and RIght Movement
		if (Horizontal > 0) 
		{
			if (!xAxisInUse)
			{
				xAxisInUse = true;
				Collider2D[] hitColliders = Physics2D.OverlapCircleAll (new Vector2 (transform.position.x + 1, transform.position.y), 0.2f);
				//Checks whether or not something is in the way or if the desired spot is within the player area.
				foreach (Collider2D c in hitColliders)
				{
					if (c.gameObject.tag == playerArea)
					{
						inboundsX = true;
					}
					if (c.gameObject.tag == "Obstacle")
					{
						if(!c.gameObject.GetComponent<Obstacle>().canPass)
							moveRight = false;
					}
				
					if (c.gameObject.tag == enemyArea)
					{
						moveRight = false;
					}
				
					if (c.gameObject.tag == "enemy")
					{
						moveRight = false;
					}
				}
				//Performs the movement if possible
				if (inboundsX)
				if (moveRight)
				{
					transform.position = new Vector2 (transform.position.x + 1, transform.position.y);
					moveLag = new StatusEffect (0.1f);
					moveLag.m_type = StatusType.Bound;
					//myStatus.AddEffect (moveLag);
				}
			}
		} 
		else if (Horizontal < 0) 
		{
			if (!xAxisInUse)
			{
				xAxisInUse = true;
				Collider2D[] hitColliders = Physics2D.OverlapCircleAll (new Vector2 (transform.position.x - 1, transform.position.y), 0.2f);
				foreach (Collider2D c in hitColliders)
				{
					//Checks whether or not something is in the way or if the desired spot is within the player.
					if (c.gameObject.tag == playerArea)
					{
						inboundsX = true;
					}
					if (c.gameObject.tag == "Obstacle")
					{
						if(!c.gameObject.GetComponent<Obstacle>().canPass)
						moveLeft = false;
					}
				
					if (c.gameObject.tag == enemyArea)
					{
						moveLeft = false;
					}
				
					if (c.gameObject.tag == "enemy")
					{
						moveLeft = false;
					}
				}
				//Performs the movement if possible
				if (inboundsX)
				if (moveLeft)
				{
					transform.position = new Vector2 (transform.position.x - 1, transform.position.y);
					moveLag = new StatusEffect (0.1f);
					moveLag.m_type = StatusType.Bound;
					//myStatus.AddEffect (moveLag);
				}
			}
		}
		//Checks for Up and Down Movement
		if (vertical > 0)
		{
			if (!yAxisInUse)
			{
				yAxisInUse = true;
				Collider2D[] hitColliders = Physics2D.OverlapCircleAll (new Vector2 (transform.position.x, transform.position.y + 1), 0.2f);
				foreach (Collider2D c in hitColliders)
				{
					//Checks whether or not something is in the way or if the desired spot is within the player.
					if (c.gameObject.tag == playerArea)
					{
						inboundsY = true;
					}
					if (c.gameObject.tag == "Obstacle")
					{
						if(!c.gameObject.GetComponent<Obstacle>().canPass)
							moveUp = false;
					}
				
					if (c.gameObject.tag == enemyArea)
					{
						moveUp = false;
					}
				
					if (c.gameObject.tag == "enemy")
					{
						moveUp = false;
					}
				}
				//Performs the movement if possible
				if (inboundsY)
				if (moveUp)
				{
					transform.position = new Vector2 (transform.position.x, transform.position.y + 1);
					moveLag = new StatusEffect (0.1f);
					moveLag.m_type = StatusType.Bound;
					//myStatus.AddEffect (moveLag);
				}
			} 
		} else if (vertical < 0)
		{
			if (!yAxisInUse)
			{
				yAxisInUse = true;
				Collider2D[] hitColliders = Physics2D.OverlapCircleAll (new Vector2 (transform.position.x, transform.position.y - 1), 0.2f);
				foreach (Collider2D c in hitColliders)
				{
					//Checks whether or not something is in the way or if the desired spot is within the player.
					if (c.gameObject.tag == playerArea)
					{
						inboundsY = true;
					}
					if (c.gameObject.tag == "Obstacle")
					{
						if(!c.gameObject.GetComponent<Obstacle>().canPass)
							moveDown = false;
					}
				
					if (c.gameObject.tag == enemyArea)
					{
						moveDown = false;
					}
				
					if (c.gameObject.tag == "enemy")
					{
						moveDown = false;
					}
				}
				//Performs the movement if possible
				if (inboundsY)
				if (moveDown)
				{
					transform.position = new Vector2 (transform.position.x, transform.position.y - 1);
					moveLag = new StatusEffect (0.1f);
					moveLag.m_type = StatusType.Bound;
					//myStatus.AddEffect (moveLag);
				}
			}
		}
		if (PlayerNum == 1)
		{
			if (GameObject.FindGameObjectWithTag("Player2") != null && PlayerNum == 1)
			{
				if (Input.GetAxisRaw ("Vertical_P1") == 0)
				{
					yAxisInUse = false;
				}
				if (Input.GetAxisRaw ("Horizontal_P1") == 0)
				{
					xAxisInUse = false;
				}
			} 
			else
			{
				if (Input.GetAxisRaw ("Vertical_Solo") == 0)
				{
					yAxisInUse = false;
				}
				if (Input.GetAxisRaw ("Horizontal_Solo") == 0)
				{
					xAxisInUse = false;
				}
			}

		} 
		else
		{
			if (Input.GetAxisRaw ("Horizontal_P2") == 0)
			{
				xAxisInUse = false;
			}
			if (Input.GetAxisRaw ("Vertical_P2") == 0)
			{
				yAxisInUse = false;
			}
		}
	}
	void initiateSpell()
	{
		GameObject go = (GameObject)Instantiate(Chamber[0],new Vector2(transform.position.x,transform.position.y),Quaternion.identity);

		Spell mything = go.GetComponent<Spell>();
		mything.weaponUsed = weapon; 
		mything.PlayerNum = PlayerNum;
		mything.setDescription (mything.weaponUsed);
		if (!mything.holdable)
		{
			////get the thing component on your instantiated object
			
			////set a member variable (must be PUBLIC)
			Chamber.RemoveAt (0);
			shotLag = new StatusEffect (0.1f);
			shotLag.m_type = StatusType.ShotLag;
			//myStatus.AddEffect (shotLag);
		}
		else
		{
			while (Input.GetAxisRaw (atkbutton) > 0 && buttonPresed)
			{
				mything.transform.position = transform.position;
				if (heldNum == mything.holdFactor)
				{
					heldNum = 0;
					if (mything.currentholdLevel < mything.holdLevelMax)
						mything.currentholdLevel++;
				}
				else
				{
					heldNum++;
				}
			}
			Chamber.RemoveAt (0);
			if(mything.currentholdLevel>0)
				mything.damage = mything.damage * mything.currentholdLevel;
		}
	}

	void updateCurrentSpell()
	{
		if (Chamber.Count > 0)
		{
			Sprite rune = ((GameObject)Resources.Load (Chamber [0].name)).GetComponent<Spell> ().bulletImage;
			if (rune != null)
			{
				topSpell.sprite = rune;
			}
		}
		else
		{
			topSpell.sprite = defaultSlot;
		}
	}
	public void takeDamage(int damage, Attributes[] effects) //created for "break" status
	{
        AudioSource hitSound = this.gameObject.GetComponent<AudioSource>();
        hitSound.Play();

		int multipliers = 1;
		if (myStatus.IsAffected(StatusType.Break))
		{
			multipliers *= 2;
		}
		if (myStatus.IsAffected(StatusType.Shield))
		{
			multipliers /= 2;
		}
		if (myStatus.IsAffected(StatusType.Stacking))
		{
			this.health -= stackDmg;
			stackDmg++;
		}
		else
		{
			stackDmg = 0;
		}

		foreach (Attributes a1 in weaknesses)
		{
			foreach (Attributes b1 in effects)
			{
				if (b1 == a1)
					multipliers *= 2;
			}
		}
		foreach (Attributes c1 in strengths)
		{
			foreach (Attributes d1 in effects)
			{
				if (d1 == c1)
					multipliers /= 2;
			}
		}
		this.health -= damage* multipliers + stackDmg;

		if (damage * multipliers + stackDmg > 0)
			hit = true;
		else
			if (damage * multipliers + stackDmg < 0)
				heal = true;
	}

	public void updatePlayerImage()
	{
		switch (weapon)
		{
			case Weapon_Types.Revolver:
				GetComponent<SpriteRenderer> ().sprite = playerImages [0];
				if (playerDisplay != null)
				{
					playerDisplay.sprite = playerPortaits [0];
					playerDisplay.color = new Color (255, 255, 255, 255);
				}
			break;
			case Weapon_Types.Rifle:
				GetComponent<SpriteRenderer> ().sprite = playerImages [0];
			break;
			case Weapon_Types.Shotgun:
				GetComponent<SpriteRenderer> ().sprite = playerImages [1];
				if (playerDisplay != null)
				{
					playerDisplay.sprite = playerPortaits [1];
					playerDisplay.color = new Color (255, 255, 255, 255);
				}
			break;
			case Weapon_Types.Gatling:
				GetComponent<SpriteRenderer> ().sprite = playerImages [0];
			break;
			case Weapon_Types.Canegun:
				GetComponent<SpriteRenderer> ().sprite = playerImages [0];
			break;
			case Weapon_Types.Bow:
				GetComponent<SpriteRenderer> ().sprite = playerImages [0];
			break;
		}
	}
	public void activeUpdate()
	{
			pHealth.text = health.ToString();
	}
	void chamberLeft()
	{
		if (Chamber.Count > 1)
		{
			Object temp = Chamber [0];
			Chamber.RemoveAt (0);
			Chamber.Add (temp);
			updateCurrentSpell ();
			field.updateChamberImages (PlayerNum);
			allowShot = false;
		}
	}
	void chamberRight()
	{
		if (Chamber.Count > 1)
		{
			Object temp = Chamber [Chamber.Count - 1];
			Chamber.RemoveAt (Chamber.Count - 1);
			Chamber.Insert (0, temp);
			updateCurrentSpell ();
			field.updateChamberImages (PlayerNum);
			allowShot = false;
		}
	}
	void setPlayer()
	{
		if (PlayerNum == 1)
		{
			playerDisplay = GameObject.Find ("PlayerImage").GetComponent<Image> ();
		}
		else
		{				
			playerDisplay = GameObject.Find ("PlayerImage_2").GetComponent<Image> ();
		}
		hit = false;
		if (GameObject.Find ("CharSelect") != null)
		{
			if (PlayerNum == 1)
			{
				weapon = GameObject.Find ("CharSelect").GetComponent<CharSelect> ().p1Weap;
			}
			else
			{				
				weapon = GameObject.Find ("CharSelect").GetComponent<CharSelect> ().p2Weap;
			}
		}
		myStatus = GetComponent<StatusManager>();
		if (PlayerNum == 1)
		{
			if (GameObject.FindGameObjectWithTag("Player2") != null && PlayerNum == 1)
			{
				atkbutton = "Fire_P1";
				abilButton1 = "Ability 1_P1";
				abilButton2 = "Ability 2_P1";
			} 
			else
			{
				atkbutton = "Fire_Solo";
				abilButton1 = "Ability 1_P2";
				abilButton2 = "Ability 2_P2";
			}

		}
		else
		{
			atkbutton = "Fire_P2";
			abilButton1 = "Ability 1_P2";
			abilButton2 = "Ability 2_P2";
		}

		if (PlayerNum == 1)
		{
			pHealth = GameObject.Find("PlayerHealth").GetComponent<Text>();
		}
		else
		{
			pHealth = GameObject.Find("PlayerHealth_2").GetComponent<Text>();
		}
	}
	public void statusAtrributes()
	{
		if(myStatus.IsAffected (StatusType.Bubbled))
		{
			bool weak = false;
			foreach (Attributes a in weaknesses)
			{
				if (a == Attributes.Electric)
				{
					weak = true;
				}
			}
			if (!weak)
			{
				weaknesses.Add (Attributes.Electric);
			}
		}
		else
		{
			int deleter = -1;
			for (int i = 0; i < weaknesses.Count ; i++)
			{
				if (weaknesses[i] == Attributes.Electric)
				{
					deleter = i;
				}
			}
			if (deleter != -1)
			{
				weaknesses.RemoveAt (deleter);
			}
		}
	}
}
