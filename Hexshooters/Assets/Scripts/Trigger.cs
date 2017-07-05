﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

	[Header("Script Settings")]
	public string script;
	public bool postBattle;
	public string postBattleScript;

	[Header("Battle Settings")]
	public string background;
	public string scenario;
	public bool scriptedLoss;

	[Header("Scene Changing Settings")]
	public bool battle;
	public bool exit;
	public bool postBattleExit;

	[Header("Activation Settings")]
	public bool repeatable;
	public bool touch;
	public bool interacted;
	public bool active;

	[Header("Reward Settings")]
	public RewardType preRewardChoice;
	public RewardType postRewardChoice;
	public bool postRewardActivator;
	public bool postReward;
	public string postRewardedItem;
	public bool preRewardActivator;
	public bool preReward;
	public string preRewardedItem;

	[Header("Boundary")]
	public bool left;
	public bool right;
	public bool up;
	public bool down;
	public bool passable;

	[HideInInspector]
	public OverPlayer op;
	public enum RewardType {None, Character, Spell, Weapon};

	void Start()
	{
		op = GameObject.Find ("OverPlayer").GetComponent<OverPlayer> ();
	}
	public void battleResults()
	{
		if (postBattle)
		{
			op.dialog.Load (postBattleScript);
		}
		if (postReward)
		{
			if (postRewardChoice == RewardType.Character)
			{
				if(postRewardedItem == "John")
				{
					op.charUnlock = true;
				}
			}
		}
	}
	public void preBattle()
	{
		if (preReward)
		{
			if (preRewardChoice == RewardType.Character)
			{
				if(preRewardedItem == "John")
				{
					op.charUnlock = true;
				}
			}
		}
	}
	public void activateOthers(Trigger[] others)
	{
		foreach (Trigger t in others)
		{
			t.active = true;
		}
	}
	public void deactivateOthers(Trigger[] others)
	{
		foreach (Trigger t in others)
		{
			t.active = false;
		}
	}
	public void unblockBoundaries(Trigger[] others)
	{
		foreach (Trigger t in others)
		{
			t.passable = true;
		}
	}
}