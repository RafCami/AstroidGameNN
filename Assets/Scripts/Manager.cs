using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public GameObject asteroid;
    public GameObject spaceship;

    private bool isTraining = false;
    private int populationSize = 10;
    private int generation = 0;
    private int[] layers = new int[] { 2, 10, 10, 1 };
    private List<NeuralNetwork> networks = new List<NeuralNetwork>();
    private List<GameObject> asteroids = new List<GameObject>();
    private List<Spaceship> spaceships = new List<Spaceship>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isTraining == false)
        {
            //on first gen create networks, else process existing networks
            if (generation == 0)
            {
                InitNetworks();
            }
            else
            {
                networks.Sort();
                for (int i = 0; i < populationSize / 2; i++)
                {
                    networks[i] = new NeuralNetwork(networks[i + (populationSize / 2)]);
                    networks[i].Mutate();

                    //reset neuron values by creating new network with same weights
                    networks[i + (populationSize / 2)] = new NeuralNetwork(networks[i + (populationSize / 2)]);
                }

                for (int i = 0; i < populationSize; i++)
                {
                    networks[i].SetFitness(0f);
                }
            }

            //start training next gen
            generation++;
            isTraining = true;
            GenerateSpaceships();
            InvokeRepeating("AsteroidSpawning", 0, 1);
        }
        else
        {
            //check if any spaceships are still alive
            bool allDead = true;
            for (int i = 0; i < spaceships.Count; i++)
            {
                if (spaceships[i].IsAlive() == true)
                {
                    allDead = false;
                    break;
                }
            }
            
            //when no ships remain stop training and destroy all remeaining asteroids
            if (allDead == true)
            {
                StopTraining();
            }
            else
            {
                //remove asteroids that have left the screen
                for (int i = 0; i < asteroids.Count; i++)
                {
                    //Asteroid is out of bounds, destroy it and increase score of living spaceships
                    if (asteroids[i].transform.position.y < -20)
                    {
                        Destroy(asteroids[i]);
                        asteroids.RemoveAt(i);
                        for (int j = 0; i < spaceships.Count; j++)
                        {
                            if (spaceships[j].IsAlive() == true)
                            {
                                spaceships[j].IncreaseScore();
                            }
                        }
                    }
                }
                //give each living spaceship its 2 nearest asteroids
                for (int i = 0; i < spaceships.Count; i++)
                {
                    if (spaceships[i].IsAlive() == true)
                    {
                        spaceships[i].SetAsteroids(asteroids);
                    }
                }
            }
        }
    }

    void AsteroidSpawning()
    {
        float x = Random.Range(-9, 9);
        float y = Random.Range(8, 10);


        asteroids.Add(Instantiate(asteroid, new Vector2(x, y), Quaternion.identity));
    }

    void StopTraining()
    {
        isTraining = false;
        CancelInvoke("AsteroidSpawning");
        for (int i = 0; i < asteroids.Count; i++)
        {
            Destroy(asteroids[i]);
            asteroids.RemoveAt(i);
        }
    }

    void InitNetworks() 
    {
        //population has to be even
        if (populationSize % 2 != 0)
        {
            populationSize++;
        }
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork nn = new NeuralNetwork(layers);
            nn.Mutate();
            networks.Add(nn);
        }
    }

    void GenerateSpaceships()
    {
        //destroy old spaceships, safety check
        if (spaceships != null)
        {
            for (int i = 0; i < spaceships.Count; i++)
            {
                Destroy(spaceships[i].gameObject);
            }
        }
        spaceships = new List<Spaceship>();

        //create spaceships
        for (int i = 0; i < populationSize; i++)
        {
            GameObject go = Instantiate(spaceship, new Vector2(0, -3.75f), Quaternion.identity);
            Spaceship s = go.GetComponent<Spaceship>();
            s.Init(networks[i]);
            spaceships.Add(s);
        }
    }

}
