using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameVars
{
	public static float GRAVITY = 3.3f;

	public static Rect WATER_BOUND = new Rect(-9,-11,18,9);
	public static Rect FISH_SPAWN = new Rect(-12,-12,24,9);
	
	public static float SPAWNINTERVAL = 0.45f;


	public static float SCOREINTERVAL = 0.6f;
	public static float SCOREPENALTY = -2f;
	public static float FISH_SCORE = 14f;
	
	public static int MAXFISH = 20;
	public static int MINFISH = 7;
	public static int MINDEBRIS = 5;
	public static int MAXDEBRIS = 15;

	public static float WATERB = 0.4f;

	public static float HOOKSPEED = 5f;
	public static float HOOKIMMUNE = 1f;
	public static float BOAT_PUSH_FORCE = 5f;


	public static float FISH_SPEED_MIN = 4.0f;
	public static float FISH_SPEED_MAX = 8.0f;
	public static float FISH_PUSH_FORCE = 8.0f;

	public static float DEBRIS_SPEED_MIN = -0.3f;
	public static float DEBRIS_SPEED_MAX = -0.9f;
	public static float DEBRIS_PUSH = 2.0f;

	public static float CLOUD_SPEED_MIN = 0.1f;
	public static float CLOUD_SPEED_MAX = 2.0f;
	public static float CLOUD_PUSH = 2.0f;
}