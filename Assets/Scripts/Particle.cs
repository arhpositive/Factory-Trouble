using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour
{
    private ParticleSystem _particleSys;

	// Use this for initialization
	void Start ()
	{
	    _particleSys = gameObject.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (!_particleSys.IsAlive())
	    {
	        Destroy(gameObject);
	    }
	}
}
