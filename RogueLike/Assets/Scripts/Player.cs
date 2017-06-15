using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {


    //Variables
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;


    private Animator animator;
    private int food;

    //------------API - Métodos------------------------------

    //Inicio, método sobreescrito de la herencia de MovingObject.
	protected override void Start () {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food " + food;
        base.Start();
	}

    //se ejecuta 1 vez por frame
    void Update()
    {
        if (!GameManager.instance.playersTurn)
        {
            return;
        }
        
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        //Esto es para prevenir diagonales, o se mueve horizontalmente, o verticalmente.
        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    //Colisión 2D que dependiendo del tag del objeto ejecutará distintivos métodos.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }

        if (collision.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            collision.gameObject.SetActive(false);
        }
        if (collision.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            collision.gameObject.SetActive(false);
        }
    }

    //------------------Métodos custom de esta clase------------------------------------

    //Método sobeescrito de la herencia para intentar moverse.
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food " + food;

        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;

        CheckIfGameOver();

        //Aquí termina el turno del player.
        GameManager.instance.playersTurn = false;
    }

    //chequea si no tienes comida y termina el juego de ser cierto.
    private void CheckIfGameOver()
    {
        if (food<=0)
        {
            GameManager.instance.GameOver();
        }
    }

    //Método sobreescrito de la herencia para cuando no puede moverse por encontrarse
    //con una "pared" (asumiendo que el metodo abstracto se encontró con una pared).
    //y ejecuta una animación de corte en el player.
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    //reinicio del nivel
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //pérdida de comida y chequea si perdió 
    public void LoseFood (int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

}
