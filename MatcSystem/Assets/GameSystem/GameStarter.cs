using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
	UnityGame game;

	// Use this for initialization
	void Awake()
	{
		useGUILayout = false;
		//Debug.Log("Compile Version: " + GolfConfig.Instance.version);
	}

	void Start()
	{
	
		DontDestroyOnLoad(GameObject.Find("GlobalObject"));
		
	}

	// Update is called once per frame
	void Update()
	{
		game.Update();
	}

	void FixedUpdate()
	{
		game.FixedUpdate();
	}

	void OnLevelWasLoaded(int level)
	{
		game.OnChangeLevelFinish(level);
	}

	void OnApplicationQuit()
	{
		
			game.OnApplicationQuit();
	}

	 void OnApplicationPause(bool pause)
	{
		
			game.OnApplicationPause(pause);
	}
}
