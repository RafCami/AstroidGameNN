using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    float movementSpeed = 5;
    string movement = "stop";
    int score = 0;
    private bool alive;
    private bool initialized = false;
    private NeuralNetwork net;
    private Transform asteroid1;
    private Transform asteroid2;
    //GameObject explosion;


    public void Init(NeuralNetwork nnet)
    {
        this.net = nnet;
        asteroid1 = transform;
        asteroid2 = transform;
        this.initialized = true;
        this.alive = true;
    }
    void Update()
    {
        if (initialized == true)
        {
            float[] inputs = new float[2];
            //inputs[0] = Vector2.Distance(transform.position, asteroid1.position) / 4.4f;
            //inputs[1] = Vector2.Distance(transform.position, asteroid2.position) / 4.4f;
            inputs[0] = asteroid1.position.x - transform.position.x;
            inputs[1] = asteroid2.position.x - transform.position.x;

            float[] output = net.FeedForward(inputs);
            //output[0] = (output[0] / net.WeightsTotal());
            if (Convert.ToInt64(output[0]) == 1)
            {
                this.movement = "right";
            }
            else if (Convert.ToInt32(output[0]) == -1)
            {
                this.movement = "left";
            }
            else
            {
                this.movement = "stop";
            }

            Move();

        }
    }

    public void StartMovingRight()
    {
        movement = "right";
        Debug.Log("Right");
    }

    public void StartMovingLeft()
    {
        movement = "left";
        Debug.Log("Left");
    }

    public void StopMoving()
    {
        movement = "stop";
    }

    public void Move()
    {
        Vector3 totalMovement;
        if (movement == "right")
        {
            totalMovement = Vector3.right * movementSpeed * Time.deltaTime;
        }
        else if (movement == "left")
        {
            totalMovement = Vector3.left * movementSpeed * Time.deltaTime;
        }
        else
        {
            totalMovement = Vector3.zero;
        }
        Vector3 appliedMovement = transform.position + totalMovement;

        if (appliedMovement.x < -7.4f || appliedMovement.x > 7.4f)
        {
            totalMovement = Vector3.zero;
        }
        transform.Translate(totalMovement);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Asteroid")
        {
            //Debug.Log("BOTS");
            //explosion = Instantiate(explosion, transform.position, Quaternion.identity);
            //Invoke("TimerExplosion", 0.5f);
            gameObject.SetActive(false);
            this.net.SetFitness(this.score);
            this.alive = false;
            //Debug.Log($"Your score is: {score}");
            //Debug.Log("Game Over");
        }
    }

    //void TimerExplosion()
    //{
    //    Destroy(explosion);
    //}

    public void IncreaseScore()
    {
        score++;
    }

    public void SetAsteroids([CanBeNull] Transform asteroid1, [CanBeNull] Transform asteroid2)
    {
        if (asteroid1 != null)
        {
            this.asteroid1 = asteroid1;
        }
        if (asteroid2 != null)
        {
            this.asteroid2 = asteroid2;
        }
    }

    public void SetAsteroids(List<GameObject> asteroids)
    {
        if (asteroids.Count == 0)
        {
            asteroid1 = transform;
            asteroid2 = transform;
        }
        else if (asteroids.Count == 1)
        {
            asteroid1 = asteroids[0].transform;
            asteroid2 = transform;
        }
        else
        {
            if (Vector2.Distance(transform.position, asteroids[0].transform.position) < Vector2.Distance(transform.position, asteroids[1].transform.position))
            {
                asteroid1 = asteroids[0].transform;
                asteroid2 = asteroids[1].transform;
            }
            else
            {
                asteroid1 = asteroids[1].transform;
                asteroid2 = asteroids[0].transform;
            }
        }
        for (int i = 2; i < asteroids.Count; i++)
        {
            if (Vector2.Distance(transform.position, asteroids[i].transform.position) < Vector2.Distance(transform.position, asteroid1.position))
            {
                asteroid2 = asteroid1;
                asteroid1 = asteroids[i].transform;
            }
            else if (Vector2.Distance(transform.position, asteroids[i].transform.position) < Vector2.Distance(transform.position, asteroid2.position))
            {
                asteroid2 = asteroids[i].transform;
            }
        }
    }
    public bool IsAlive()
    {
        return this.alive;
    }

}
