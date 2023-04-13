using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.CompositeCollider2D;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{

    public GameObject asteroid;
    public GameObject spaceshipPrefab;
    public TMPro.TextMeshProUGUI highScoreTmp;
    public TMPro.TextMeshProUGUI generationTmp;
    [Range(0.1f,100)]
    public float timeScale = 1;

    private bool isTraining = false;
    private int populationSize = 200;
    private int generation = 0;
    private int highScore = 0;
    private int[] layers = new int[] { 2, 50, 100, 100, 1 };
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
        Time.timeScale = timeScale;
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
                int genHighScore = Convert.ToInt32(networks.Last().GetFitness());
                if (genHighScore > highScore)
                {
                    highScore = genHighScore;
                    highScoreTmp.text = $"High Score: {highScore} (gen {generation})";
                }
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
            generationTmp.text = $"Generation: {generation}";
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
                        //Debug.Log($"Remaining ships: {spaceships.Count}");
                        for (int j = 0; j < spaceships.Count; j++)
                        {
                            //Debug.Log($"Checking ship index {j}");
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
        float x;
        float y = Random.Range(8, 10);
        int edge = Random.Range(1, 10);
        if (edge <= 2)
        {
            if (edge == 1)
            {
                x = Random.Range(-8, -7);
            }
            else
            {
                x = Random.Range(7, 8);
            }
        }
        else
        {
            x = Random.Range(-9, 9);
        }


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

        networks = new List<NeuralNetwork>();
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
            GameObject go = Instantiate(spaceshipPrefab, new Vector2(0, -3.75f), Quaternion.identity);
            Spaceship ship = go.GetComponent<Spaceship>();
            //Spaceship ship = ((GameObject)Instantiate(spaceshipPrefab, new Vector3(0, -3.75f, 0), spaceshipPrefab.transform.rotation)).GetComponent<Spaceship>();
            ship.Init(networks[i]);
            spaceships.Add(ship);
        }
    }

}
