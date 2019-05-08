using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour 
{
	private Slider healthbar;
	private Slider manabar;
	private Text goldText;
	private Text healthPotionText;
	private Text manaPotionText;
	private Image baseAttackImage;
	private Image skillAttackImage;
	public GameObject player;
	public GameObject fireball;
	private Vector2 fireballRight = new Vector2 (25, 0);
	private Vector2 fireballLeft = new Vector2(-25, 0);
	private float fireballDamage;
	public Collider2D hitbox;
	private Animator animator;

	private bool facingRight = true;
	public int gold;
	public int health;
	public float hp;
	public int mana;
	public float mp;
	public float hpregen;
	public float mpregen;

	private int invincible = 0;
	private int hitInterval = 60; 

	private float baseAttackTime = 0;
	private float baseAttackInterval = 1; //in seconds
	private float skillAttackTime = 0;
	private float skillAttackInterval = 1;

	private int healthPotions = 10;
	private int manaPotions = 10;

	private int level = 1;
	private float xp = 0;
	private float xpToLevel = 100;
	private Text xpnumbers;
	private Image xpslider;
	private Text levelText;

	NinjaController.NinjaController nc;
	// Use this for initialization
	void Start () 
	{
		xpnumbers = GameObject.Find ("XPNumber").GetComponent<Text> ();
		xpslider = GameObject.Find ("XPSlider").GetComponent<Image> ();
		levelText = GameObject.Find ("Level").GetComponent<Text> ();
		goldText = GameObject.Find ("GoldAmount").GetComponent<Text> ();
		healthPotionText = GameObject.Find ("HealthPotionNumber").GetComponent<Text> ();
		manaPotionText = GameObject.Find ("ManaPotionNumber").GetComponent<Text> ();
		healthPotionText.text = healthPotions.ToString ();
		manaPotionText.text = manaPotions.ToString ();
		skillAttackImage = GameObject.Find ("SkillAttack").GetComponent<Image> ();
		baseAttackImage = GameObject.Find ("BaseAttack").GetComponent<Image>();
		healthbar = GameObject.Find ("HealthBar").GetComponent<Slider>();
		manabar = GameObject.Find ("ManaBar").GetComponent<Slider> ();
		animator = GetComponent<Animator> ();
		nc = GetComponent<NinjaController.NinjaController> ();
		InvokeRepeating ("Regen", .1f, .1f);
	}

	public void EnemyKilled(GameObject enemy)
	{
		xp += 10;
		if(xp >= xpToLevel)
		{
			xp -= xpToLevel;
			level++;
			levelText.text = "Level " + level.ToString ();
		}
		Destroy (enemy);
	}
	
	// called once every 1/10th of a second
	// put anything that needs to be timed by set time here
	private void Regen() 
	{
		if(skillAttackTime > 0)
		{
			skillAttackImage.fillAmount = (1 - skillAttackTime) / 1;
			skillAttackTime -= .1f;
		}
		if(skillAttackTime < 0)
		{
			skillAttackTime = 0;
			skillAttackImage.fillAmount = 1;
		}
		if (baseAttackTime > 0)
		{
			baseAttackImage.fillAmount = (1 - baseAttackTime) / 1;
			baseAttackTime -= .1f;
		}
		if (baseAttackTime < 0)
		{
			baseAttackTime = 0;
			baseAttackImage.fillAmount = 1;
		}
			
		if (hp != health)
			hp += hpregen/600;
		if (mp != mana)
			mp += mpregen/600;
		if (hp > health)
			hp = health;
		if (mp > mana)
			mp = mana;
	}

	// Update is called once per frame
	void Update ()
	{
		xpnumbers.text = "(" + xp.ToString () + " / " + xpToLevel.ToString () + " )";
		xpslider.fillAmount = xp / xpToLevel;
		goldText.text = gold.ToString ();
		healthbar.value = hp / health;
		manabar.value = mp / mana;

		if (baseAttackTime >= 0)
		if (invincible > 0)
			invincible--;
		
		bool right = Input.GetKeyDown (KeyCode.D) || Input.GetKey (KeyCode.RightArrow);
		bool left = Input.GetKeyDown (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow);
		bool baseAttack = Input.GetKeyDown (KeyCode.E) || Input.GetKey (KeyCode.E);
		bool skillAttack = Input.GetKeyDown (KeyCode.Q) || Input.GetKey (KeyCode.Q);
		bool hpkey = Input.GetKeyDown (KeyCode.F);
		bool mpkey = Input.GetKeyDown (KeyCode.C);

		if (hpkey && healthPotions != 0 && hp != health)
		{
			hp += 20;
			healthPotions--;
			if (hp > health)
				hp = health;
			healthPotionText.text = healthPotions.ToString ();
		}

		if (mpkey && manaPotions != 0 && mp != mana)
		{
			mp += 20;
			manaPotions--;
			if (mp > mana)
				mp = mana;
			manaPotionText.text = manaPotions.ToString ();
		}

		if (right)
		{
			facingRight = true;
			transform.localScale = new Vector3 (1, 1, 1);
		}
		if (left)
		{
			facingRight = false;
			transform.localScale = new Vector3 (-1, 1, 1);
		}


		// regular fireball attack. no manna usage. 1shot/sec
		if (baseAttack && baseAttackTime <= 0)
			fireballSkill ();
		if (skillAttack && skillAttackTime <= 0 && mp >= 20)
			tripleFireballSkill ();
	}

	private float getFireballDamage()
	{
		return 10f * level;
	}
	private void fireballSkill()
	{
		animator.SetTrigger ("Attack");
		baseAttackImage.fillAmount = 0;
		baseAttackTime = baseAttackInterval;
		GameObject fired = Instantiate (fireball, player.transform.position, player.transform.rotation);
		Rigidbody2D projectile = fired.GetComponent<Rigidbody2D> ();
		fired.GetComponent<ProjectileScript> ().setDamage (getFireballDamage ());
		if (facingRight)
			projectile.velocity = fireballRight;
		else
		{
			projectile.transform.localScale = new Vector3 (-1, 1, 1);
			projectile.velocity = fireballLeft;
		}
	}

	private void tripleFireballSkill()
	{
		animator.SetTrigger ("Attack");
		mp -= 20;
		skillAttackImage.fillAmount = 0;
		skillAttackTime = skillAttackInterval;
		float verticalSpacing = .4f;
		float horizontalSpacing = .8f;
		float damage = getFireballDamage ();
		if(facingRight)
		{
			GameObject fired1 = Instantiate (fireball, player.transform.position + new Vector3(horizontalSpacing, 0, 0), player.transform.rotation);
			GameObject fired2 = Instantiate (fireball, player.transform.position + new Vector3 (0, verticalSpacing, 0), player.transform.rotation);
			GameObject fired3 = Instantiate (fireball, player.transform.position + new Vector3 (0, -verticalSpacing, 0), player.transform.rotation);
			fired1.GetComponent<Rigidbody2D> ().velocity = fireballRight;
			fired2.GetComponent<Rigidbody2D> ().velocity = fireballRight;
			fired3.GetComponent<Rigidbody2D> ().velocity = fireballRight;
			fired1.GetComponent<ProjectileScript> ().setDamage (damage);
			fired2.GetComponent<ProjectileScript> ().setDamage (damage);
			fired3.GetComponent<ProjectileScript> ().setDamage (damage);
		}
		else
		{
			GameObject fired1 = Instantiate (fireball, player.transform.position + new Vector3(-horizontalSpacing, 0, 0), player.transform.rotation);
			GameObject fired2 = Instantiate (fireball, player.transform.position + new Vector3 (0, verticalSpacing, 0), player.transform.rotation);
			GameObject fired3 = Instantiate (fireball, player.transform.position + new Vector3 (0, -verticalSpacing, 0), player.transform.rotation);
			fired1.transform.localScale = new Vector3 (-1, 1, 1);
			fired2.transform.localScale = new Vector3 (-1, 1, 1);
			fired3.transform.localScale = new Vector3 (-1, 1, 1);
			fired1.GetComponent<Rigidbody2D> ().velocity = fireballLeft;
			fired2.GetComponent<Rigidbody2D> ().velocity = fireballLeft;
			fired3.GetComponent<Rigidbody2D> ().velocity = fireballLeft;
			fired1.GetComponent<ProjectileScript> ().setDamage (damage);
			fired2.GetComponent<ProjectileScript> ().setDamage (damage);
			fired3.GetComponent<ProjectileScript> ().setDamage (damage);
		}
	}

	public void Damage(int damage)
	{
		hp -= damage;
		if (hp < 0)
			hp = 0;
		if(facingRight)
		{
			nc.SimAddForce (new Vector2 (-200000, 10000));
		}
		else
		{
			nc.SimAddForce (new Vector2 (200000, 10000));
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		string tag = col.gameObject.tag;
		if(tag == "Enemy" && invincible == 0)
		{
			Damage (col.GetComponent<EnemyController>().getDamage());
			invincible = hitInterval;
		}
		else if(tag == "Item")
		{
			GameObject item = col.gameObject;
			if (item.name == "Coin(Clone)")
			{
				gold += item.GetComponent<CoinScript> ().value;
				Destroy (item);
			}
			if(item.name == "HealthPotion(Clone)")
			{
				healthPotions++;
				healthPotionText.text = healthPotions.ToString ();
				Destroy (item);
			}
			if(item.name == "ManaPotion(Clone)")
			{
				manaPotions++;
				manaPotionText.text = manaPotions.ToString ();
				Destroy (item);
			}

		}
	}

}
