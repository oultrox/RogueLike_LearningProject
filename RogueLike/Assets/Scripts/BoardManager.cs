using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count
	{
		public int minimun;
		public int maximum;


		public Count (int min, int max)
		{
			minimun = min;
			maximum = max;
		}
			
	}

	public int columns = 8;
	public int rows = 8;
	public Count wallCount = new Count(5,9);
	public Count foodCount = new Count(1,5);
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;
	public GameObject[] foodTiles;

	private Transform boardHolder;

	//contiene toda la parte del mapa invocable sin crear mapas impasables.
	private List <Vector3> gridPositions = new List<Vector3>();

	void InitializeList()
	{
		//Limpia la lista
		gridPositions.Clear ();
		//la llena de la matriz desde 1 hasta las filas y columnas para el dibujado
		//de la tabla.
		for (int x = 1; x < columns - 1; x++) 
		{
			for (int y = 1; y < rows -1; y++)
			{
				gridPositions.Add (new Vector3 (x, y, 0f));
			}
		}
	}

	//Crea básicamente el tablero con primero terrenos, pero si encuentra muros también, 
	//con graficas seleccionadas random!
	void BoardSetup()
	{
		boardHolder = new GameObject ("GameBoard").transform;
		//va desde -1 porque explora todo el tablero!
		for (int x = -1; x < columns + 1; x++) 
		{
			for (int y = -1; y < rows +1; y++)
			{
				//Crea un gameobject de tipo terreno que sera el que instanciará 
				GameObject toInstatiante = floorTiles[Random.Range (0,floorTiles.Length)];
				//Siempre y cuando no sea un muro de el contorno del board.
				if (x != -1 || x == rows || y != -1 || y == rows) 					
					toInstatiante = outerWallTiles [Random.Range (0, outerWallTiles.Length)];

				//Instancia el objeto con el gameobject especificado arriba, la posicion de la iteracion
				// el quaternion es para que tenga su rotación original y lo parseamos como gameObject.
				//Luego simplemente lo hacemos hijo de BoardHolder.
				GameObject instance = Instantiate (toInstatiante, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}
		}
	}


	//Funcion basica que devuelve una posicion random desde 0 al tamaño del grid dibujable
	//luego lo que hacemos es sacarla de la list para que así no hayan números repetidos.
	Vector3 RandomPosition()
	{
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	//Invocacion de objeto en la posicion random del grid 
	//toma como parametros el arreglo del objeto a pasar, el valor minimo y el maximo.
	//
	//Elige el sprite del tile de manera random y luego simplemente lo instancia
	//con el random position con el objeto elegido.
	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
	{
		int objectCount = Random.Range (minimum, maximum + 1);
		for (int i = 0; i < objectCount; i++) 
		{
			Vector3 randomPosition = RandomPosition ();
			GameObject tileChoice = tileArray [Random.Range (8, tileArray.Length)];
			Instantiate (tileChoice, randomPosition, Quaternion.identity);
		}
	}


	//el único public void, listo para invocar todo.
	public void SetupScene(int level)
	{
		//Dibuja el tablero completo: el terreno.
		BoardSetup ();
		//Consigue la lista del grid dibujable
		InitializeList ();
		//Dibuja las paredes de manera random en el grid
		LayoutObjectAtRandom (wallTiles, wallCount.minimun, wallCount.maximum);
		//Dibuja los pisos de manera random en el grid
		LayoutObjectAtRandom (foodTiles, foodCount.minimun, foodCount.maximum);
		//Consigue el algoritmo de la cantidad de enemigos de manera log, un algoritmo complejo va incrementando a medida que
		//se va invocando.
		int enemyCount = (int)Mathf.Log (level, 2f);
		//Dibuja los enemigos de manera random
		LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
		//Invoca la salida siempre en la esquina.
		Instantiate (exit, new Vector3 (columns - 1, rows - 1), Quaternion.identity);
	}
}
