using UnityEngine;
using System.Collections;

public class BannerSetupScript : MonoBehaviour 
{
	public GUIStyle bannerStyle;
	public Texture2D bannerTexture;
	

	void Awake ()
	{
		Banner.style = bannerStyle;
		Banner.bannerTexture = bannerTexture;
	}
}
