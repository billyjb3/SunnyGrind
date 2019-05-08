using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoxScript : MonoBehaviour 
{
	public GameObject enemy;
	public int enemyLevel = 1;
	public int maxEnemies = 1;
	public float spawnDelay = 5;
	public int enemiesPerSpawn = 1;

	private Transform position;
	private int enemyCount = 0;

	// Use this for initialization
	void Start () 
	{
		position = GetComponent<Transform>();
		SpriteRenderer placeholder = GetComponent<SpriteRenderer> ();
		placeholder.enabled = false;
		InvokeRepeating ("Spawn", 0, spawnDelay);
	}
	private void Spawn()
	{
		for(int i = 0; i < enemiesPerSpawn; i++)
		{
			if(enemyCount < maxEnemies)
			{
				GameObject spawned = Instantiate (enemy, position.position, position.rotation);
				spawned.GetComponent<EnemyController>().setSpawner (this.gameObject);
				spawned.GetComponent<EnemyController>().setLevel (enemyLevel);
				enemyCount++;
			}
		}
	}
	public void enemyKilled()
	{
		enemyCount--;
	}

}
