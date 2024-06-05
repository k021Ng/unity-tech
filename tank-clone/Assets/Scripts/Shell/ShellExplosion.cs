using UnityEngine;
using System.Collections;

public class ShellExplosion : MonoBehaviour {
	public LayerMask m_TankMask;
	public ParticleSystem m_ExplosionParticels;
	public AudioSource m_ExplosionAudio;
	public float m_MaxDamage = 100.0f;
	public float m_ExplosionForce = 1000f;
	public float m_MaxLifeTime = 2f;
	public float m_ExplosionRadius = 5f;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, m_MaxLifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);
		for (int i = 0; i < colliders.Length; i++) {
			Rigidbody targetRigidbody = colliders [i].GetComponent<Rigidbody> ();
			if (!targetRigidbody)
				continue;

			targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

			TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();
			if (!targetHealth)
				continue;

			float damage = CalculateDamage (targetRigidbody.position);
			targetHealth.TakeDamage (damage);
		}

		m_ExplosionParticels.transform.parent = null;
		m_ExplosionParticels.Play ();

		m_ExplosionAudio.Play ();

		Destroy (m_ExplosionParticels.gameObject, m_ExplosionParticels.duration);
		Destroy (gameObject);
	}

	private float CalculateDamage(Vector3 targetPosition)
	{
		Vector3 explosionToTarget = targetPosition - transform.position;
		float explosionDistance = explosionToTarget.magnitude;
		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
		float damage = relativeDistance * m_MaxDamage;
		damage = Mathf.Max (0f, damage);
		return damage;
	}
}
