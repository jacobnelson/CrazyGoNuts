using UnityEngine;
using System.Collections;

public class BannerSetupScript : MonoBehaviour 
{
	public GUIStyle bannerStyle;
	public Texture2D bannerTexture;
	public Texture2D bannerblueTexture;
	

	void Awake ()
	{
		Banner.style = bannerStyle;
		Banner.bannerTexture = bannerTexture;
		Banner.bannerblueTexture = bannerblueTexture;
	}
}
