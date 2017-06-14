using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public BoardManagerTutorial boardScript;
    //Singleton
    public static GameManager instance = null;

    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;


    private int level = 3;
    //API - Métodos
    void Awake()
    {
        //Chequea si ya existe una instancia de tipo GameManager porque debe 
        //ser sólo 1 por escena.
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Hace que el objeto no se destruya al cargar una nueva escena.
        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManagerTutorial>();
        InitGame();
    } 

    public void GameOver()
    {
        enabled = false;
    }
        
    void InitGame()
    {
        boardScript.SetupScene(level);
    }
	
}
