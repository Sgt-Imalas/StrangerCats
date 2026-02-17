
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
 
 
public class Starfield : MonoBehaviour
{
    public int MaxStars = 200;
    public float StarSize = 0.1f;
    public float StarSizeRange = 0.5f;
    public float FieldWidth = 25f;
    public float FieldHeight = 25f;
    public float MaxStarDistance = 30f;
    private float MaxStarDistanceSq;
    public float MaxStarClipDistance = 40f;
    private float MaxStarClipDistanceSq; 
    public bool Colorize = false;
    public Camera MainCam;

    public float yDiff = 10f;
    public float xDiff = 10f;

    public GameObject player;
    private Vector3 offset;

    private ParticleSystem ps;
    ParticleSystem.Particle[] Stars;

    private void Start()
    {
        MaxStarDistanceSq = MaxStarDistance * MaxStarDistance;
        MaxStarClipDistanceSq = MaxStarClipDistance * MaxStarClipDistance;
        ps = transform.GetComponent<ParticleSystem>();
        offset = transform.position - player.transform.position;
        ps.GetComponent<Renderer>().sortingLayerName = "Background";
    }
	private void Awake()
	{

		CreateStars();
	}


	private void CreateStars()
    {
        Stars = new ParticleSystem.Particle[MaxStars];        

        for (int i = 0; i < MaxStars; i++)
        {
            Stars[i].position = GetRandomInRectangle(FieldWidth, FieldHeight);
            Stars[i].startSize = StarSize;
            Stars[i].startColor = new Color(255, 255, 255, 1);
        }
    }


    void Update()
    {
        for (int i = 0; i < MaxStars; i++)
        {
             float tempX = Stars[i].position.x, tempY= Stars[i].position.y, tempZ = Stars[i].position.z;

            if (tempX<transform.position.x-FieldWidth) tempX = tempX + FieldWidth*2;
            else if(tempX > transform.position.x + FieldWidth) tempX = tempX - FieldWidth * 2;

            if (tempY < transform.position.y - FieldHeight) tempY = tempY + FieldHeight * 2;
            else if (tempY > transform.position.y + FieldHeight) tempY = tempY - FieldHeight * 2;


            Stars[i].position = new Vector3(tempX, tempY, tempZ);


            /*  
            if ((Stars[i].position - player.transform.position).sqrMagnitude <= MaxStarDistanceSq)
            {
                Stars[i].startSize = Random.Range(StarSize, StarSizeRange);
                //Customisation of stars
            }
            */
        }
        ps.SetParticles(Stars, Stars.Length);
        transform.position = player.transform.position ;
    }

    // GetRandomInRectangle
    //----------------------------------------------------------
    // Get a random value within a certain rectangle area
    //
    Vector3 GetRandomInRectangle(float width, float height)
    {
        float x = Random.Range(player.transform.position.x - width, player.transform.position.x + width);

        float y = Random.Range(player.transform.position.y - width, player.transform.position.y + width);

        float z = Random.Range(0,20);

        return new Vector3(x, y, z);
    }



}
