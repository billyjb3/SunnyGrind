using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour 
{
	private float damage;
	private ArrayList damaged = new ArrayList();

	public void OnTriggerEnter2D(Collider2D col)
	{
		string tag = col.gameObject.tag;
		if (tag != "Enemy" && tag != "Player")
		{
			explode ();
			Destroy (this.gameObject, 1);
		}
		if (tag == "Enemy")
			explode ();
			
	}

	public void explode()
	{
		GetComponent<Rigidbody2D> ().velocity -= GetComponent<Rigidbody2D> ().velocity / 2;
		Animator anime = GetComponent<Animator> ();
		anime.SetTrigger ("Explode");
	}

	public void setTag(string tag)
	{
		this.gameObject.tag = tag;
	}

	public void setDamage(float damage)
	{
		this.damage = damage;
	}
	public float getDamage()
	{
		return damage;
	}
	public void addDamagedID(int id)
	{
		damaged.Add (id);
	}
	public bool hasDamaged(int id)
	{
		if (damaged.Contains (id))
			return true;
		else
			return false;
	}
}




