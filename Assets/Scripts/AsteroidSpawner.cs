using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{

    public GameObject asteroid;
    List<GameObject> asteroids = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawning", 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < asteroids.Count; i++)
        {
            if (asteroids[i].transform.position.y < -20)
            {
                Destroy(asteroids[i]);
                asteroids.RemoveAt(i);
                GameObject spaceship = GameObject.Find("Spaceship");
                if (spaceship != null)
                {
                    spaceship.GetComponent<Spaceship>().IncreaseScore();
                }
                
            }
        }
    }

    void Spawning()
    {
        float x = Random.Range(-9, 9);
        float y = Random.Range(8, 10);


        asteroids.Add(Instantiate(asteroid, new Vector2(x, y), Quaternion.identity));
    }
        



}
