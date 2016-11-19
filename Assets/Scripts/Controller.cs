using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Controller : MonoBehaviour {

	public GameObject carPrefab;
	public GameObject floorPiece;
	public GameObject floor;
	public int numberFloor;
	public int numberCars;
	public int mutationRate;
	public int generationLimit;
	public float end;
	public float timeToDie; // evaluate need
	public List<GameObject> population;
	public List<GameObject> pastGeneration;
	private int generation;
	private GameObject populationRepository;

	private bool allowNextGeneration;
	public GameObject roadPrefab;
	public string[] carsPrefab;

	public bool pause;

	private UIController uiController;

	// Use this for initialization
	void Start () {
		uiController = GameObject.FindGameObjectWithTag ("UIController").GetComponent<UIController> ();
		pause = true;
	}

	// Update is called once per frame
	void Update () {

	}

	public void StartGame(bool carSeed, bool floorSeed) {
		allowNextGeneration = true;
		generation = 1;
		pastGeneration = new List<GameObject> ();
		populationRepository = new GameObject ("Population Repository");
		populationRepository.transform.Translate (-2.5f, 0, 0);

		// Check if floor is random or not
		if (floorSeed) {
			floor = Instantiate(roadPrefab);
			
			end = 231f;

		} else {
			GenerateFloor ();
		}

		// Check if car is random or not
		if (carSeed) {
			for (int i = 0; i < numberCars; i++) { // Generate Random cars
				population.Add (GeneratePremadeCar ("Car " + (i + 1), i));
			}
		} else {
			for (int i = 0; i < numberCars; i++) { // Generate Random cars
				population.Add(GenerateRandomCar ("Car " + (i + 1)));
			}
		}
	}

	private GameObject CreateCar(Genome genome, string name) {
		GameObject car = (GameObject)Instantiate (carPrefab);
		car.name = name;
		car.GetComponent<Car> ().CreateCar (genome);
		car.transform.parent = populationRepository.transform;
		return car;
	}

	private GameObject GenerateRandomCar(string name) {
		bool[] genome = GenerateGenome ();
		Genome aux = new Genome ();
		aux.SetGenome (genome);

		return CreateCar(aux, name);
	}

	private GameObject GeneratePremadeCar(string name, int carNum) {
		bool[] genome = new bool[74];
		for (int i = 0; i < 74; i++) {
			genome[i] = this.carsPrefab[carNum][i] == '1';
		}
		Genome aux = new Genome ();
		aux.SetGenome (genome);
		
		return CreateCar(aux, name);
	}

	private bool[] GenerateGenome() {
		bool[] b = new bool[74];
		
		for (int i = 0; i < 74; i++) {
			b [i] = UnityEngine.Random.Range (0, 2) == 1;
		}
		
		return b;
	}

	private void GenerateFloor() {
		GameObject floorPast = (GameObject)Instantiate (floorPiece);
		floorPast.transform.parent = floor.transform;
		floorPast.transform.localPosition = new Vector3 ();
		for (int i = 0; i < 10; i++) {
			GameObject newFloor = (GameObject)Instantiate(floorPiece);
			newFloor.transform.parent = floor.transform;
			newFloor.transform.position = floorPast.GetComponent<FloorPiece>().end.position;
			newFloor.transform.Translate(0.9f, 0, 0);
			floorPast = newFloor;
		}
		int range = 10;
		for (int i = 0; i < numberFloor; i++) {
			GameObject newFloor = (GameObject)Instantiate(floorPiece);
			newFloor.transform.parent = floor.transform;
			newFloor.transform.Rotate(new Vector3(0,0, Random.Range(-range, range)));
			newFloor.transform.position = floorPast.GetComponent<FloorPiece>().end.position;
			newFloor.transform.Translate(0.9f, 0, 0);
			floorPast = newFloor;

			if (i % 5 == 0 && range < 65) {
				range += 2;
			}
			end = newFloor.transform.position.x + 0.9f;
		}
	}

	public List<bool> Mutation(List<bool> element) {
		for (int i = 0; i < element.Count; i++) {
			if (Random.Range(1, 101) <= mutationRate)
				element[i] = !element[i];
		}
		return element;
	}

	private void CrossOver(Genome car1, Genome car2, out Genome newCar1, out Genome newCar2) {
		newCar1 = new Genome ();
		newCar2 = new Genome ();
		int cutPoint = Random.Range (1, newCar1.GetSize());

		// Past Genaration Car's Genome
		List<bool> aux1 = new List<bool> (car1.GetGenome ());
		List<bool> aux2 = new List<bool> (car2.GetGenome ());
		// New Genome
		List<bool> aux3 = new List<bool>();
		List<bool> aux4 = new List<bool>();

		aux3.AddRange(aux1.GetRange(0, cutPoint));
		aux3.AddRange(aux2.GetRange(cutPoint, aux2.Count - cutPoint));

		aux4.AddRange(aux2.GetRange(0, cutPoint));
		aux4.AddRange(aux1.GetRange(cutPoint, aux1.Count - cutPoint));

		aux3 = Mutation (aux3);
		aux4 = Mutation (aux4);

		newCar1.SetGenome (aux3.ToArray ());
		newCar2.SetGenome (aux4.ToArray ());
	}

	private void DestroyOldGeneration() { 
		for (int i = 0; i < pastGeneration.Count; i++) {
			if (!pastGeneration[i].activeSelf) {
				DestroyImmediate(pastGeneration[i].GetComponent<Car>().carSign);
				DestroyImmediate(pastGeneration[i]);
			}
		}
	}

	private void NextGeneration() {
		if (!allowNextGeneration)
			return;
		Debug.Log ("Next Generation");

		uiController.NextGeneration ();

		List<GameObject[]> selected = Selection.roulette (pastGeneration, 5);

		generation ++;

		List<Genome> newGeneration = new List<Genome> ();
		List<int> generationName = new List<int> ();

		for (int i = 0; i < selected.Count; i++) {
			Genome newCar1, newCar2;
			CrossOver(selected[i][0].GetComponent<Car>().GetGenome(), selected[i][1].GetComponent<Car>().GetGenome(), out newCar1, out newCar2);

			newGeneration.Add(newCar1);
			generationName.Add(generation);

			newGeneration.Add(newCar2);
			generationName.Add(generation);
	
			selected[i][0].GetComponent<Car>().ResetCar();
			selected[i][0].SetActive(true);
			population.Add(selected[i][0]);

			selected[i][1].GetComponent<Car>().ResetCar();
			selected[i][1].SetActive(true);
			population.Add(selected[i][1]);
		}

		DestroyOldGeneration ();

		for (int i = 0; i < newGeneration.Count; i++) {
			population.Add(CreateCar(newGeneration[i], "Car " + (i + 1) + "." + generationName[i]));
		}

		pastGeneration = new List<GameObject> ();

		if (generationLimit < generation)
			allowNextGeneration = false;
	}

	public void Died(GameObject car) {
		Debug.Log ("Car dead: " + car.name + " Score: " + car.GetComponent<Car> ().GetScore ());

		pastGeneration.Add (car);


		population.Remove (car);

		car.SetActive (false);

		if (population.Count == 0)
			NextGeneration ();
	}

	public void ReachedEnd() {
		allowNextGeneration = false;

	//	Pause ();

		for (int i = 0; i < population.Count; i++) {
			GameObject g = population[i];
			population.Remove(population[i]);
			DestroyImmediate(g);

		}

		for (int i = 0; i < pastGeneration.Count; i++) {
			DestroyImmediate(pastGeneration[i]);
		}
	}



	// Controllers Game
	public void Pause() {
		pause = !pause;

		for (int i = 0; i < population.Count; i++) {
			population[i].GetComponent<Rigidbody2D>().isKinematic = pause;
		}
	}

	public void Quit() {
		population.RemoveAll (restart);
		pastGeneration.RemoveAll (restart);
		DestroyImmediate (populationRepository);
		DestroyImmediate (floor);
		floor = new GameObject("Floor");
		floor.transform.position = new Vector3 (-15, -3.5f, 0);
	}
	
	private static bool restart(GameObject go)
	{
		return true;
	}

	// Getters and Setters
	public int GetGeneration() {
		return generation;
	}

	public GameObject GetFirstCar() {
		GameObject best = null;
		for (int i = 0; i < population.Count; i++) {
			if (best == null)
				best = population[i];
			else if (best.transform.position.x < population[i].transform.position.x) {
				best = population[i];
			}
		}
		return best;
	}

	public bool GetPause() {
		return this.pause;
	}

	public int GetCarsAlive() {
		return population.Count;
	}
}