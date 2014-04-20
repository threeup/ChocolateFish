using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public enum GameTaskPriority
{
	LOW,
	MED,
	TOP,
}

public class GameTask : IComparable
{
	public delegate void RunDelegate(string name);
	
	public GameTaskPriority priority;
	public float timeToExecute;
	public float delay;
	public float period;
	public string param;
	public RunDelegate run;
	
	public GameTask(float p_timeToExecute, RunDelegate p_run, string p_param)
	{
		priority = GameTaskPriority.MED;
		timeToExecute = p_timeToExecute;
		run = p_run;
		param = p_param;
	}

	public int CompareTo(object obj) {
        if (obj == null) return 1;

        GameTask otherTask = obj as GameTask;
        if (otherTask == null) return -1;

        if (this.priority != otherTask.priority)
        {
        	return this.priority.CompareTo(otherTask.priority);
        }

        return this.timeToExecute.CompareTo(otherTask.timeToExecute);
        
    }
}

public class Scheduler : BasicSingleton<Scheduler>
{
	private SortedDictionary<GameTask, string> taskDictionary;
	private List<GameTask> delList;

    bool _initialized = false;
	void Awake()
	{
        Initialize();
    }
	public void Initialize()
	{
		taskDictionary = new SortedDictionary<GameTask, string>();
		delList = new List<GameTask>();
		_initialized = true;
	}

	public bool IsInitialized()
	{
		return _initialized;
	}

	public void AddTask(float p_timeToExecute, GameTask.RunDelegate p_run, string p_param)
	{
		GameTask newTask = new GameTask(p_timeToExecute, p_run, p_param);
		taskDictionary.Add(newTask, "");
	}

	public void Update()
	{
		float deltaTime = Time.deltaTime;
		foreach(KeyValuePair<GameTask, String> kvp in taskDictionary)
		{
			GameTask task = kvp.Key;
			task.timeToExecute -= deltaTime;
			if (task.timeToExecute < 0)
			{
				task.run(task.param);
				Debug.Log(task);
				delList.Add(task);
			}
		}
		foreach(GameTask task in delList)
		{
			taskDictionary.Remove(task);
		}
	}
}