using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneticAlgorithm {
    public static float mutationRate = 0.05f;

    public static float[] Crossover(float[] parentA, float[] parentB) {
        int length = parentA.Length;
        float[] child = new float[length];

        int crossoverPoint = Random.Range(0, length);

        for (int i = 0; i < length; i++) {
            if (i < crossoverPoint)
                child[i] = parentA[i];
            else
                child[i] = parentB[i];

            // Mutacja
            if (Random.value < mutationRate) {
                child[i] = Random.Range(0f, 1f); // losowy gen
            }
        }

        return child;
    }
}
