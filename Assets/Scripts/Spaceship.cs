using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    float movementSpeed = 5;
    string movement = "stop";
    int lives = 3;
    int score = 0;

    public GameObject explosion;

    void Update()
    {
        if (movement == "right" || movement == "left")
        {
            Move();
        }
        else
        {
            StopMoving();
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
        else
        {
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Asteroid(Clone)")
        {
            if (lives > 0)
            {
                lives--;
                gameObject.transform.position = new Vector2(0, -4);
                Debug.Log("Lives: " + lives);
            }
            else
            {
                explosion = Instantiate(explosion, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                Debug.Log($"Your score is: {score}");
                Debug.Log("Game Over");
            }
        }
    }

    public void IncreaseScore()
    {
        score++;
    }


}
