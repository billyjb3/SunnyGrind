using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour 
{
	public int level = 1;
	public int baseHealth = 100;
	private int health;
	public int hp;
	public int baseDamage = 10;
	private int damage;
	public Image healthbar;
	public float speed;
	public int trendInterval;
	private int trendCount;
	private GameObject spawner;
	public GameObject[] drops;
	public float[] dropProbabilities;
	private Rigidbody2D enemy;
	private GameObject target;
	private bool facingRight = true;
	private float targetPadding = 1.5f;
	private GameObject player;
	private GameManager gameManager;


	void Start () 
	{
		
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		player = gameManager.getPlayer ();
		enemy = GetComponent<Rigidbody2D> ();
		trendCount = trendInterval;
		bool dir = Random.value >= .5;
		if (dir)
			moveRight ();
		else
			moveLeft ();
		health = baseHealth + (baseHealth / 10) * (level - 1);
		hp = health;
		damage = baseDamage + (baseDamage / 10) * (level - 1);
	}
	public void setLevel(int level)
	{
		this.level = level;
		health = baseHealth + (baseHealth / 10) * (level - 1);
		hp = health;
		damage = baseDamage + (baseDamage / 10) * (level - 1);
	}

	public int getDamage()
	{
		return damage;
	}

	private void moveRight()
	{
		bool move = false;
		if (!facingRight)
			move = true;
		else
		{
			if (enemy.velocity.magnitude < speed / 2)
				move = true;
		}

		if (move)
		{
			enemy.velocity = new Vector2 (speed, 0);
			float sx = enemy.GetComponent<Transform> ().localScale.x;
			sx = Mathf.Abs (sx);
			float sy = enemy.GetComponent<Transform> ().localScale.y;
			enemy.GetComponent<Transform>().localScale = new Vector3 (sx, sy, 1);
			facingRight = true;
		}

	}
	private void moveLeft()
	{
		bool move = false;
		if (facingRight)
			move = true;
		else
		{
			if (enemy.velocity.magnitude < speed / 2)
				move = true;
		}
		if (move)
		{
			enemy.velocity = new Vector2 (-speed, 0);
			float sx = enemy.GetComponent<Transform> ().localScale.x;
			sx = Mathf.Abs (sx);
			float sy = enemy.GetComponent<Transform> ().localScale.y;
			enemy.GetComponent<Transform>().localScale = new Vector3 (-sx, sy, 1);
			facingRight = false;
		}
	}

	private void Die()
	{
		spawner.GetComponent<SpawnBoxScript>().enemyKilled ();
		for(int i = 0; i < drops.Length; i++)
		{
			float roll = Random.value;
			if(roll <= dropProbabilities[i])
			{
				Instantiate (drops [i], enemy.GetComponent<Transform>().position, enemy.GetComponent<Transform> ().rotation);
			}
		}
		GameObject.Find ("GameManager").GetComponent<GameManager>().EnemyKilled (this.gameObject);
	}

	// Update is called once per frame
	void Update () 
	{
		healthbar.fillAmount = (float)hp / health;
		if (hp <= 0)
			Die ();
		if(trendCount > 0)
			trendCount--;
		if (target != null)
		{
			//target is right of enemy + padding space
			if (GetComponent<Transform> ().position.x + targetPadding < target.GetComponent<Transform> ().position.x)
				moveRight ();
			else if (GetComponent<Transform> ().position.x - targetPadding > target.GetComponent<Transform> ().position.x)
				moveLeft ();
		} 
		else if (trendCount == 0)
		{
			trendCount = trendInterval;
			bool dir = Random.value >= .5;
			if (dir)
				moveRight ();
			else
				moveLeft ();
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "PlayerProjectile" )
		{
			ProjectileScript ps = col.GetComponent<ProjectileScript> ();
			if (!ps.hasDamaged (this.gameObject.GetInstanceID ()))
			{
				hp -= (int)ps.getDamage ();
				ps.addDamagedID (this.gameObject.GetInstanceID ());
				Destroy (col.gameObject, 1);
				target = player;
			}
		}
	}

	public void setSpawner(GameObject spawner)
	{
		this.spawner = spawner;
	}
}
