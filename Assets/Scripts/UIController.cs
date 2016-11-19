using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

	private Controller controller;
	public string bestCar;
	public string curBestCar;
	public string roundBestCar;
	public float roundBestScore;
	public float curBestScore;
	public float bestScore;
	public int generationNumberOverall;
	public int lastGen;
	public int improvements;
	public int carsAlive;
	public string genomaBest;
	public string text;
	public int x;

	
	// Controllers temp
	public bool floorSeed;
	public bool carSeed;

	// Controllers
	public GameObject startMenu;
	public GameObject menuButton;
	public GameObject menu;

	// Use this for initialization
	void Start () {
		controller = Camera.main.GetComponent<Controller> ();

		carsAlive = 0;
		lastGen = 0;
		improvements = 0;
		bestScore = 0;
		curBestScore = 0;
		roundBestScore = 0;
		roundBestCar = "";
		bestCar = "";
		curBestCar = "";
		genomaBest = "";
	}

	// Update is called once per frame
	void Update () {
		carsAlive = controller.GetCarsAlive ();
		GameObject go = controller.GetFirstCar ();
		
		if (go != null) {
			controller.GetComponentInParent<SmoothFollow> ().target = go.transform;
			float curScore = go.GetComponent<Car>().GetScore();

			curBestScore = curScore;
			curBestCar = go.gameObject.name;

			if (roundBestScore < curScore) {
				roundBestScore = curScore;
				roundBestCar = go.gameObject.name;
			}
			if (roundBestScore > bestScore) {
				bestScore = curBestScore;
				generationNumberOverall = controller.GetGeneration();
				genomaBest = go.GetComponent<Car>().GetGenome().ToString();
				bestCar = go.gameObject.name;
				if (generationNumberOverall != lastGen) {
					improvements++;
					lastGen = generationNumberOverall;
				}
			}
		}
		text = "Statistics\n\n" +

			"Generations: " + controller.GetGeneration() + "\n\n" +
				System.String.Format("BestScore: {0:N1}", bestScore) + 
				" Gen: " + generationNumberOverall + 
				" BestCar: " + bestCar + "\n\n" + 
				System.String.Format("CurBestScore: {0:N1}", curBestScore) +
				" CurBestCar: " + curBestCar + "\n\n" +
				System.String.Format("RoundBestScore: {0:N1}", roundBestScore) +
				" RoundBestCar: " + roundBestCar + "\n\n" +
				"CarsAlive: " + carsAlive +
				" Improvements: " + improvements;
	}

	void OnGUI() {
		if (!controller.GetPause())
			GUI.Box (new Rect ( Screen.width - x - 10, 10, x, Screen.height - 20), text);
	}

	public void NextGeneration() {
		print (text);
		roundBestCar = "";
		roundBestScore = 0;
		curBestCar = "";
		curBestScore = 0;
		carsAlive = 0;
	}

	public void StartGame() {
		controller.StartGame (carSeed, floorSeed);
		controller.Pause ();

		// Change Screen
		startMenu.SetActive (false);
		menuButton.SetActive (true);
	}

	// Set if it will use prefab for floor
	public void SetFloorPrefab(int result) {
		floorSeed = result == 1;
	}

	// Set if it will use prefab for cars
	public void SetCarPrefab(int result) {
		carSeed = result == 1;
	}

	public void OpenMenu() {
		controller.Pause ();

		// Change Screen
		menuButton.SetActive (false);
		menu.SetActive (true);
	}

	public void QuitGame() {
		controller.Quit ();
		Start ();

		// Change Screen
		menu.SetActive (false);
		startMenu.SetActive (true);
	}

	public void Resume() {
		controller.Pause ();

		// Change Screen
		menuButton.SetActive (true);
		menu.SetActive (false);
	}
}
