using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
	UnityGame game;

	
	void Awake()
	{
		//useGUILayout = false;
		//Debug.Log("Compile Version: " + GolfConfig.Instance.version);
	}

	void Start()
	{
	
		DontDestroyOnLoad(this);
		var gameClasses = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(UnityGame)));
		foreach (var g in gameClasses)
		{
			var attr = g.GetCustomAttribute(typeof(GameAttribute), false);
			if (attr != null)
			{
				game = Activator.CreateInstance(g, new object[] { }) as UnityGame;
				break;
			}
		}
		if (game == null)
		{
			Debug.Log("No Game Class Define");
			return;
		}
		game?.Start(this);
	}

	
	void Update()
	{
		game?.Update();
	}

	void FixedUpdate()
	{
		game?.FixedUpdate();
	}

	void OnLevelWasLoaded(int level)
	{
		game?.OnChangeLevelFinish(level);
	}

	void OnApplicationQuit()
	{
		
			game?.OnApplicationQuit();
	}

	 void OnApplicationPause(bool pause)
	{
		
			game?.OnApplicationPause(pause);
	}
}
