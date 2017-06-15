using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    //Variables
    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    //----------- Métodos custom --------
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (!skipMove)
        {
            base.AttemptMove<T>(xDir, yDir);
            skipMove = true;  
        }
        else
        {
            skipMove = false;
        }
        
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            //si la coordenada del player (target) es mayor que la del enemigo (transform) entonces yDir será 1 (arriba)
            //sino será -1 (abajo) usando matemática booleana.
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            //Si la posicion del player X es mayor que la del enemigo entonces el valor será 1 (derecha) sino,
            //será -1 (izquierda).
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        hitPlayer.LoseFood(playerDamage);
        animator.SetTrigger("enemyAttack");

    }

}
