using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    float movementSpeed = 5;
    string movement = "stop";
    int score = 0;
    private bool initialized = false;
    private NeuralNetwork net;
    private Transform astroid1;
    private Transform astroid2;
    private GameObject explosion;


    void init(NeuralNetwork nnet)
    {
        this.net = nnet;
        astroid1 = transform;
        astroid2 = transform;
        this.initialized = true;
    }
    void Update()
    {
        if (initialized == true)
        {
            float[] inputs = new float[2];
            inputs[0] = Vector2.Distance(transform.position, astroid1.position);
            inputs[1] = Vector2.Distance(transform.position, astroid2.position);

            
            float[] output = net.FeedForward(inputs);
            if (Convert.ToInt32(output[0]) == 1)
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
        if (movement == "right")
        {
            transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        }
        if (movement == "left")
        {
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("BOTS");
        if (other.gameObject.tag == "Asteroid")
        {
            explosion = Instantiate(explosion, transform.position, Quaternion.identity);
            Invoke("TimerExplosion", 0.5f);
            gameObject.SetActive(false);
            this.net.SetFitness(this.score);
            Debug.Log($"Your score is: {score}");
            Debug.Log("Game Over");
        }
    }

    void TimerExplosion()
    {
        Destroy(explosion);
    }

    public void IncreaseScore()
    {
        score++;
    }

    public void ClosestAstroids(Transform astroid1, Transform astroid2)
    {
        this.astroid1 = astroid1;
        this.astroid2 = astroid2;
    }


}
