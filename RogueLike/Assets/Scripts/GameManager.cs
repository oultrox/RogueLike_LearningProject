using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public BoardManagerTutorial boardScript;
    //Singleton
    public static GameManager instance = null;
    
    //Variables publicas
    public int playerFoodPoints = 100;
    public float turnDelay = .1f;
    public float levelStartDealy = 2f;
    [HideInInspector] public bool playersTurn = true;

    //variables privadas
    private Text levelText;
    private GameObject levelImage;
    public List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    private int level = 0;
    //----------API - Métodos------------
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
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManagerTutorial>();
    }

    private void Update()
    {
        if (!playersTurn && !enemiesMoving && !doingSetup)
        {
            StartCoroutine(MoveEnemies());
        }
    }

    void OnEnable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled. 
        //Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    
    //--------Métodos custom-------------
    public void GameOver()
    {
        levelText.text = "After " + level + " days. you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    //This is called each time a scene is loaded.
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Add one to our level number.
        level++;//Call InitGame to initialize our level.
        InitGame();
    }

    void InitGame()
    {
        enemies.Clear();
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelImage.SetActive(true);
        levelText.text = "Day " + level;

        Invoke("HideLevelImage", levelStartDealy);
        boardScript.SetupScene(level);
        
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }


    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

	
}
