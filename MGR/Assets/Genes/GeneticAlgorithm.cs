using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneticAlgorithm {
    // --- Parametry mutacji ---
    public static float mutationChance = 0.3f;  // Prawdopodobieñstwo, ¿e dziecko w ogóle zostanie zmutowane
    public static float perGeneModeShare = 0.7f;  // Szansa na tryb "per gen" (reszta = jeden gen)
    public static float smallSigma = 0.05f; // si³a ma³ej mutacji gaussowskiej
    public static float bigJumpShare = 0.2f;  // udzia³ du¿ych skoków wœród mutacji

    public static float[] Crossover(float[] parentA, float[] parentB) {
        if (parentA == null || parentB == null)
            throw new System.ArgumentNullException();
        if (parentA.Length != parentB.Length)
            throw new System.ArgumentException("Rodzice musz¹ mieæ ten sam rozmiar.");

        int length = parentA.Length;
        var child = new float[length];

        // --- Crossover ---
        for (int i = 0; i < length; i++) {
            child[i] = (Random.value < 0.5f) ? parentA[i] : parentB[i];
        }

        // --- Mutacja ---
        if (Random.value < mutationChance) {
            bool perGeneMode = Random.value < perGeneModeShare;

            if (perGeneMode) {
                // Mutacja per gen (ka¿dy gen osobno)
                for (int i = 0; i < length; i++) {
                    child[i] = Mathf.Clamp01(MutateHybrid(child[i]));
                }
            }
            else {
                // Mutacja tylko jednego losowego genu
                int idx = Random.Range(0, length);
                child[idx] = Mathf.Clamp01(MutateHybrid(child[idx]));
            }
        }

        return child;
    }

    // Hybrydowa mutacja: ma³a gaussowska czêœciej, reset rzadziej
    private static float MutateHybrid(float gene) {
        if (Random.value < bigJumpShare) {
            return Random.value; // reset
        }
        else {
            return gene + NextGaussian() * smallSigma;
        }
    }

    // Box–Muller: standardowa N(0,1)
    private static float NextGaussian() {
        float u1 = Mathf.Max(1e-7f, Random.value);
        float u2 = Random.value;
        float r = Mathf.Sqrt(-2f * Mathf.Log(u1));
        float theta = 2f * Mathf.PI * u2;
        return r * Mathf.Cos(theta);
    }
}
