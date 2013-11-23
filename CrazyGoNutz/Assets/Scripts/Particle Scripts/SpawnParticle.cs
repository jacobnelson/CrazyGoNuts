using UnityEngine;
using System.Collections;

public class SpawnParticle : MonoBehaviour 
{
	// Static
	static private SpawnParticle spawnParticle = null;
	
	// Particle System Prefabs
	public GameObject moodParticleSystem = null;
	
	
	// Initialization
	void Awake () 
	{
		spawnParticle = this;
	}
	
	void Start()
	{
		//SpawnParticle.SpawnMoodParticle( new Vector3(0,3,0), MoodTextures.textures.happy);
	}
	
	// Methods
	static public void SpawnMoodParticle(Vector3 where, Texture2D texture)
	{
		GameObject partsys = null;
		partsys = Instantiate(spawnParticle.moodParticleSystem, where, Quaternion.identity) as GameObject;
		partsys.renderer.material.mainTexture = texture;
	}
}
