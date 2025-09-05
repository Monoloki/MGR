using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimulationStatsLogger : MonoBehaviour
{
    public float logInterval = 1f;
    private float timer = 0f;
    private int deaths = 0;
    private int births = 0;
    private int predatorKills = 0;
    private string filePath;
    private int treesEaten = 0;
    private int treesSpawned = 0;


    void Start() {
        string basePath = Path.Combine(Application.dataPath, "simulation_stats");
        string extension = ".csv";
        int index = 0;
        filePath = basePath + extension;
        while (File.Exists(filePath)) {
            index++;
            filePath = basePath + $"_{index}" + extension;
        }
        Debug.Log(filePath);
        File.WriteAllText(filePath, "Time,Herbivore_Male,Herbivore_Female,Predator_Male,Predator_Female,Deaths,Births,PredatorKills,TreesEaten,TreesSpawned,MaxGeneration," +
            "HerbivoreMale_Tezyzna,HerbivoreMale_Poped,HerbivoreMale_Zwin,HerbivoreMale_Atrak,HerbivoreMale_Perc,HerbivoreMale_Sila," +
            "HerbivoreFemale_Tezyzna,HerbivoreFemale_Poped,HerbivoreFemale_Zwin,HerbivoreFemale_Atrak,HerbivoreFemale_Perc,HerbivoreFemale_Sila," +
            "PredatorMale_Tezyzna,PredatorMale_Poped,PredatorMale_Zwin,PredatorMale_Atrak,PredatorMale_Perc,PredatorMale_Sila," +
            "PredatorFemale_Tezyzna,PredatorFemale_Poped,PredatorFemale_Zwin,PredatorFemale_Atrak,PredatorFemale_Perc,PredatorFemale_Sila\n");

        Human.OnDeath += OnHumanDeath;
        ReproduceState.OnBirth += OnHumanBirth;
        PredatorFoodState.OnPredatorKill += OnPredatorKill;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= logInterval)
        {
            timer = 0f;
            LogStats();
            deaths = 0;
            births = 0;
        }
    }

    void OnDestroy() {
        Human.OnDeath -= OnHumanDeath;
        ReproduceState.OnBirth -= OnHumanBirth;
        PredatorFoodState.OnPredatorKill -= OnPredatorKill;
    }

    private void LogStats() {
        int herbivoreMale = 0, herbivoreFemale = 0, predatorMale = 0, predatorFemale = 0;
        float[] herbivoreMaleGenes = new float[6];
        float[] herbivoreFemaleGenes = new float[6];
        float[] predatorMaleGenes = new float[6];
        float[] predatorFemaleGenes = new float[6];
        int maxGeneration = 0;

        Human[] humans = FindObjectsOfType<Human>();
        foreach (var h in humans) {
            if (h.genome == null || h.genome.genes == null || h.genome.genes.Length < 6)
                continue;

            // Liczenie maksymalnego pokolenia
            if (h.generation > maxGeneration)
                maxGeneration = h.generation;

            if (!h.predator && h.gender == GENDER.male) {
                herbivoreMale++;
                for (int i = 0; i < 6; i++) herbivoreMaleGenes[i] += h.genome.genes[i];
            }
            if (!h.predator && h.gender == GENDER.female) {
                herbivoreFemale++;
                for (int i = 0; i < 6; i++) herbivoreFemaleGenes[i] += h.genome.genes[i];
            }
            if (h.predator && h.gender == GENDER.male) {
                predatorMale++;
                for (int i = 0; i < 6; i++) predatorMaleGenes[i] += h.genome.genes[i];
            }
            if (h.predator && h.gender == GENDER.female) {
                predatorFemale++;
                for (int i = 0; i < 6; i++) predatorFemaleGenes[i] += h.genome.genes[i];
            }
        }

        // Oblicz œrednie
        for (int i = 0; i < 6; i++) {
            if (herbivoreMale > 0) herbivoreMaleGenes[i] /= herbivoreMale;
            if (herbivoreFemale > 0) herbivoreFemaleGenes[i] /= herbivoreFemale;
            if (predatorMale > 0) predatorMaleGenes[i] /= predatorMale;
            if (predatorFemale > 0) predatorFemaleGenes[i] /= predatorFemale;
        }

        string line = $"{Time.time:F2},{herbivoreMale},{herbivoreFemale},{predatorMale},{predatorFemale},{deaths},{births},{predatorKills},{treesEaten},{treesSpawned},{maxGeneration}";

        // Dodaj œrednie geny do linii
        for (int i = 0; i < 6; i++) line += $",{herbivoreMaleGenes[i]:F3}";
        for (int i = 0; i < 6; i++) line += $",{herbivoreFemaleGenes[i]:F3}";
        for (int i = 0; i < 6; i++) line += $",{predatorMaleGenes[i]:F3}";
        for (int i = 0; i < 6; i++) line += $",{predatorFemaleGenes[i]:F3}";

        line += "\n";
        File.AppendAllText(filePath, line);
        Debug.Log("Logged stats");
        predatorKills = 0;
        treesEaten = 0;
        treesSpawned = 0;
    }

    private void OnPredatorKill() {
        predatorKills++;
    }

    private void OnHumanDeath()
    {
        deaths++;
    }

    private void OnHumanBirth()
    {
        births++;
    }
    public void OnTreeEaten() {
        treesEaten++;
    }

    public void OnTreeSpawned() {
        treesSpawned++;
    }
}
