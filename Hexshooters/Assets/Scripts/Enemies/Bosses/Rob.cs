﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rob : Enemy {

    private int attackCounter;
    public int ATTACK_TIMEOUT = 2;
    public int recentWaterCounter = 0;//If greater than zero, water spell was recently used.
    public const int RECENT_WATER_RESET = 4; //what recentWaterCounter resets to.

    public const int RAGE_ATTACK_TIMEOUT = 1;
    
    private bool canAttack;//If true, the enemy can currently attack, if false, cannot for some reason.
    public Ransom myBrother = null;
    

    public float modify_TIME_PER_ACTION;
    public const float RAGE_TIME_PER_ACTION = 0.5f;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        attackCounter = 0;

        myBrother = GameObject.FindObjectOfType<Ransom>();
        //Initialize(6, 2);
        modify_TIME_PER_ACTION = TIME_PER_ACTION;
    }

    public override void enemyUpdate()
    {
        if (myStatus.IsAffected(StatusType.Slow) || myStatus.IsAffected(StatusType.Freeze))
        {
            timer -= Time.deltaTime * FROZEN_MULTIPLIER;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0.0f)
        {
            timer += modify_TIME_PER_ACTION;
            if (!isInitialized)
            {
                Debug.LogError(this.gameObject.name + " is an enemy that Initialize() was never called on.  Wherever you created this enemy, call Initialize on it.");
            }
            AIStep();
        }
		if (health <= 0)
		{
			MarkedForDeletion = true;
		}
    }

    public override void AIStep()
    {
        // The decision that affects the behaviour tables.
        float decision = UnityEngine.Random.Range(0, 1.0f);

        // Counts down until the next attack is available.
        if (attackCounter > 0)
        {
            attackCounter--;
        }
        if(recentWaterCounter > 0)
        {
            recentWaterCounter--;
        }

        // Checking to make sure nothing is preventing attacking.
        canAttack = (attackCounter == 0) && !myStatus.IsAffected(StatusType.Disabled);//Being disabled prevents attacking

        // This is where the actual action happens.
        if (!myStatus.IsAffected(StatusType.Bound))
        {

            if (canAttack)
            {
                if (decision < 0.5f)//50%
                {
                    Attack();
                }
                else if (decision < 0.65f)//15%
                {
                    if (movePossible(Direction.Up) == 2)
                    {
                        if (Move(Direction.Up) && myBrother && !myBrother.myStatus.IsAffected(StatusType.Bound))
                        {
                            if (myBrother.movePossible(Direction.Up) == 2)
                            {
                                myBrother.Move(Direction.Up);
                                myBrother.movedByBrother = true;
                            }
                        }
                    }
                }
                else if (decision < 0.80f)//15%
                {
                    if (movePossible(Direction.Down) == 2)
                    {
                        if (Move(Direction.Down) && myBrother && !myBrother.myStatus.IsAffected(StatusType.Bound))
                        {
                            if (myBrother.movePossible(Direction.Down) == 2)
                            {
                                myBrother.Move(Direction.Down);
                                myBrother.movedByBrother = true;
                            }
                        }
                    }
                }
                else if (decision < 0.85f)//5%
                {
                    if (movePossible(Direction.Right) == 2)
                    {
                        if (Move(Direction.Right) && myBrother && !myBrother.myStatus.IsAffected(StatusType.Bound))
                        {
                            if (myBrother.movePossible(Direction.Right) == 2)
                            {
                                myBrother.Move(Direction.Right);
                                myBrother.movedByBrother = true;
                            }
                        }
                    }
                }
                else if (decision < 0.90f)//5%
                {
                    if (movePossible(Direction.Left) == 2)
                    {
                        if (Move(Direction.Left) && myBrother && !myBrother.myStatus.IsAffected(StatusType.Bound))
                        {
                            if (myBrother.movePossible(Direction.Left) == 2)
                            {
                                myBrother.Move(Direction.Left);
                                myBrother.movedByBrother = true;
                            }
                        }
                    }
                }
                else//10%
                {
                    //Do nothing.
                }
            }
            else
            {
                if (decision < 0.35f) //35%
                {
                    if (movePossible(Direction.Up) == 2)
                    {
                        Move(Direction.Up);
                    }
                }
                else if (decision < 0.7f)//35%
                {
                    if (movePossible(Direction.Down) == 2)
                    {
                        Move(Direction.Down);
                    }
                }
                else if (decision < 0.8f)//10%
                {
                    if (movePossible(Direction.Left) == 2)
                    {
                        Move(Direction.Left);
                    }
                }
                else if (decision < 0.9f)//10%
                {
                    if (movePossible(Direction.Right) == 2)
                    {
                        Move(Direction.Right);
                    }
                }
                else//10%
                {
                    //Do nothing.
                }
            }

        }
        else// Bound loop, can't move but still tries to attack.
        {
            if (canAttack)
            {
                Attack();
            }
        }

        if (myBrother)
        {
            myBrother.AIStep();
        }
    }

    public void Attack()
    {
        if(myBrother && myBrother.recentFireCounter > 0)
        {
            float shieldRand = UnityEngine.Random.Range(0, 1.0f);
            if (shieldRand > (float)myBrother.GetComponent<Enemy>().Health() / 100.0f)
            {
				// Use ice spell
				GameObject go = (GameObject)Instantiate(Resources.Load("Ice"),new Vector2(transform.position.x,transform.position.y),Quaternion.identity);

				////get the thing component on your instantiated object
				Spell mything = go.GetComponent<Spell>();

				////set a member variable (must be PUBLIC)
				mything.weaponUsed = 3; 
				mything.PlayerNum = 2;
            }
            else
            {
				// Use wind spell
				GameObject go = (GameObject)Instantiate(Resources.Load("Wind"),new Vector2(transform.position.x,transform.position.y),Quaternion.identity);

				////get the thing component on your instantiated object
				Spell mything = go.GetComponent<Spell>();

				////set a member variable (must be PUBLIC)
				mything.weaponUsed = 1; 
				mything.PlayerNum = 2;
            }
        }
        else
        {
            float shieldRand = UnityEngine.Random.Range(0, 1.0f);
            if (myBrother && shieldRand > (float)myBrother.GetComponent<Enemy>().Health() / 100.0f)
            {
				// Use ice spell
				GameObject go = (GameObject)Instantiate(Resources.Load("Ice"),new Vector2(transform.position.x,transform.position.y),Quaternion.identity);

				////get the thing component on your instantiated object
				Spell mything = go.GetComponent<Spell>();

				////set a member variable (must be PUBLIC)
				mything.weaponUsed = 3; 
				mything.PlayerNum = 2;
            }
            else
            {
                float rand = UnityEngine.Random.Range(0, 1.0f);
                if (rand < 0.5f)//50%
                {
                    // Use wind spell
					GameObject go = (GameObject)Instantiate(Resources.Load("Wind"),new Vector2(transform.position.x,transform.position.y),Quaternion.identity);

					////get the thing component on your instantiated object
					Spell mything = go.GetComponent<Spell>();

					////set a member variable (must be PUBLIC)
					mything.weaponUsed = 1; 
					mything.PlayerNum = 2;
                }
                else
                {
                    // Use water spell
					// Use wind spell
					GameObject go = (GameObject)Instantiate(Resources.Load("Water"),new Vector2(transform.position.x,transform.position.y),Quaternion.identity);

					////get the thing component on your instantiated object
					Spell mything = go.GetComponent<Spell>();

					////set a member variable (must be PUBLIC)
					mything.weaponUsed = 3; 
					mything.PlayerNum = 2;
                    recentWaterCounter = RECENT_WATER_RESET;
                }
            }

        }

        attackCounter = ATTACK_TIMEOUT;

    }

    public void Die()
    {
        if (myBrother)
        {
            myBrother.myBrother = null;
            myBrother.ATTACK_TIMEOUT = Ransom.RAGE_ATTACK_TIMEOUT;
            myBrother.modify_TIME_PER_ACTION = Ransom.RAGE_TIME_PER_ACTION;

        }
    }

    public void OnDestroy()
    {
        Die();
    }
}
