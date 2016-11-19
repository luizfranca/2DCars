using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Selection : MonoBehaviour {

	public static List<GameObject[]> roulette(List<GameObject> population, int pairParents) {
		List<GameObject[]> result = new List<GameObject[]> ();

		List<List<float>> aux = new List<List<float>> ();

		population = population.OrderByDescending (go => go.GetComponent<Car> ().GetScore ()).ToList();

		float accomulatedFitness = 0;
		for (int i = 0; i < population.Count; i++) {
			accomulatedFitness += population[i].GetComponent<Car>().GetScore();

			aux.Add(new List<float>());
			aux[i].Add(i);
			aux[i].Add(population[i].GetComponent<Car>().GetScore());
			aux[i].Add(accomulatedFitness);
		}

		for (int i = 0; i < pairParents; i++) {
			// parent 1
			float num1 = Random.Range(0f, aux[aux.Count - 1][2]);
			int index1 = GetNext(aux, num1);
			GameObject go1 = population[(int) aux[index1][0]];
			aux.Remove(aux[index1]);

			// parent 2
			float num2 = Random.Range(0f, aux[aux.Count - 1][2]);
			int index2 = GetNext(aux, num2);
			GameObject go2 = population[(int) aux[index2][0]];
			aux.Remove(aux[index2]);

			result.Add(new GameObject[] { go1, go2 } );
		}
		return result;
	}
	
	// Auxiliar functions
	private static int GetNext(List<List<float>> table, float number){
		int index = 0;
		for (int i = 0; i < table.Count; i++) {
			if (table[i][2] <= number) {
				index = (int) table[i][0];
				break;
			}
		}
		return index;
	}
}