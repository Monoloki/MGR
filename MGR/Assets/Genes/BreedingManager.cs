using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreedingManager : MonoBehaviour {
    public Genome parentA;
    public Genome parentB;

    public GameObject childPrefab;

    public void Breed() {
        float[] childGenes = GeneticAlgorithm.Crossover(parentA.genes, parentB.genes);

        GameObject child = Instantiate(childPrefab, transform.position + Vector3.right * 2, Quaternion.identity);
        Genome genome = child.GetComponent<Genome>();
        genome.SetGenes(childGenes);
    }
}
