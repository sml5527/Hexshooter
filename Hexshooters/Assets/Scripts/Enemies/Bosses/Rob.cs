﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rob : AIBase {

    private int attackCounter;
    public int ATTACK_TIMEOUT = 2;
    public int recentWaterCounter = 0;//If greater than zero, water spell was recently used.
    public const int RECENT_WATER_RESET = 4; //what recentWaterCounter resets to.

    public const int RAGE_ATTACK_TIMEOUT = 1;
    
    private bool canAttack;//If true, the enemy can currently attack, if false, cannot for some reason.
    public Ransom myBrother = null;

    // Bandit1 moves every second.
    public override float TIME_PER_ACTION
    {
        get
        {
            return 1.0f;
        }
    }

    public float modify_TIME_PER_ACTION;
    public const float RAGE_TIME_PER_ACTION = 0.5f;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        attackCounter = 0;
        minX = 5;
        maxX = 8;

        minY = 0;
        maxY = 4;

        myBrother = GameObject.FindObjectOfType<Ransom>();
        Initialize(6, 2);
        modify_TIME_PER_ACTION = TIME_PER_ACTION;
    }

    public override void Update()
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
                    if (Move(Direction.Up) && myBrother && !myBrother.myStatus.IsAffected(StatusType.Bound))
                    {
                        myBrother.Move(Direction.Up);
                        myBrother.movedByBrother = true;
                    }
                }
                else if (decision < 0.80f)//15%
                {
                    if (Move(Direction.Down) && myBrother && !myBrother.myStatus.IsAffected(StatusType.Bound))
                    {
                        myBrother.Move(Direction.Down);
                        myBrother.movedByBrother = true;
                    }
                }
                else if (decision < 0.85f)//5%
                {
                    if (Move(Direction.Right) && myBrother && !myBrother.myStatus.IsAffected(StatusType.Bound))
                    {
                        myBrother.Move(Direction.Right);
                        myBrother.movedByBrother = true;
                    }
                }
                else if (decision < 0.90f)//5%
                {
                    if (Move(Direction.Left) && myBrother && !myBrother.myStatus.IsAffected(StatusType.Bound))
                    {
                        myBrother.Move(Direction.Left);
                        myBrother.movedByBrother = true;
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
                    Move(Direction.Up);
                }
                else if (decision < 0.7f)//35%
                {
                    Move(Direction.Down);
                }
                else if (decision < 0.8f)//10%
                {
                    Move(Direction.Left);
                }
                else if (decision < 0.9f)//10%
                {
                    Move(Direction.Right);
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
            if (shieldRand > (float)myBrother.myEnemy.Health() / 100.0f)
            {
                Debug.LogError("Ice shield spell in Rob Attack() unimplemented.");
            }
            else
            {
                // Use wind spell
                Debug.LogError("Wind spell in Rob Attack() unimplemented.");
            }
        }
        else
        {
            float shieldRand = UnityEngine.Random.Range(0, 1.0f);
            if (myBrother && shieldRand > (float)myBrother.myEnemy.Health() / 100.0f)
            {
                Debug.LogError("Ice shield spell in Rob Attack() unimplemented.");
            }
            else
            {
                float rand = UnityEngine.Random.Range(0, 1.0f);
                if (rand < 0.5f)//50%
                {
                    // Use wind spell
                    Debug.LogError("Wind spell in Rob Attack() unimplemented.");
                }
                else
                {
                    // Use water spell
                    Debug.LogError("Water spell in Rob Attack() unimplemented.");
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
