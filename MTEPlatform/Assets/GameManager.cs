using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public GameObject pauseMenu;
	public GameObject playerPrefab;
	[HideInInspector]
	public GameObject player;
	public GameObject playerSpawn;
	private bool paused = false;
	// Use this for initialization
	void Start () 
	{
		Resume ();
		player = Instantiate (playerPrefab, playerSpawn.transform.position, playerSpawn.transform.rotation);
	}
	
	// Update is called once per frame
	void Update () 
	{
		bool esc = Input.GetKeyDown (KeyCode.Escape);

		if (esc)
		{
			if (paused)
				Resume();
			else
				Pause ();
		}
			
		if(player.GetComponent<PlayerScript>().hp <= 0)
		{
			player.transform.position = playerSpawn.transform.position;
			player.GetComponent<PlayerScript>().hp = player.GetComponent<PlayerScript> ().health;
		}
	}

	public void EnemyKilled(GameObject enemy)
	{
		player.GetComponent<PlayerScript>().EnemyKilled (enemy);
	}

	public void Pause()
	{
		paused = true;
		pauseMenu.SetActive (true);
		Time.timeScale = 0f;
	}
	public void Resume()
	{
		paused = false;
		pauseMenu.SetActive (false);
		Time.timeScale = 1f;
	}
	public void Quit()
	{
		SceneManager.LoadScene ("StartMenu");
	}
	public GameObject getPlayer()
	{
		return player;
	}
}
