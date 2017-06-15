using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    //-------API - Métodos------------

	// Use this for initialization
	protected virtual void Start ()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
	}

    //--------Métodos custom-----------------

    // Move: sirve para moverse a traves de parametros X y Y además de conseguir el raycast por parametro como resultado.
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        //vectores: el inicio como posición actual y la posicion final que sería la incial + las coordenadas pasadas por parametro.
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        //Se desactiva el box collider de este objeto para evitar que colisione con el mismo.
        boxCollider.enabled = false;
        //aplicamos raycast desde la posicion inicial hacia el final en el layer de bloqueo.
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        //Si no pegó con nada significa que debe moverse, de lo contrario no podrá moverse.
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    //Método de movimiento utilizando epsilon que significa un número muy cercano a 0 pero no es 0, esto hace la transición suave.
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Utilizamos el vector para conseguir la posicion a la que debe moverse el rigidBody2D.
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            //actualiza información de la distancia restante
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }
    protected abstract void OnCantMove<T>(T component)
        where T : Component;

    protected virtual void AttemptMove <T> (int xDir, int yDir) 
        where T : Component
    {
        
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);
        if (hit.transform == null)
        {
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }

    }
        

}
