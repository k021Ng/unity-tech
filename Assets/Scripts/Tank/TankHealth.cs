using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour {
	public float m_StartingHealth = 100f;
	public Slider m_Slider;
	public Image m_FillImage;
	public Color m_FullHealthColor = Color.green;
	public Color m_ZeroHealthColor = Color.red;
	public GameObject m_ExplosionPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
