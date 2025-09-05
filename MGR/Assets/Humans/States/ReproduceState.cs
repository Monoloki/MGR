using UnityEngine;

public class ReproduceState : IState {
    public static event System.Action OnBirth;
    private Human human;
    private Human partner;
    private bool hasSpawnedChild = false;
    private float reproduceCooldown = 5f;
    private float timer = 0f;

    public ReproduceState(Human human, Human partner) {
        this.human = human;
        this.partner = partner;
    }

    public void Enter() {
        timer = 0f;
        hasSpawnedChild = false;
    }

    public void Execute() {
        timer += Time.deltaTime;
        if (!hasSpawnedChild && timer >= reproduceCooldown) {
            if (IsBothInReproduceState()) {
                SpawnChild(human, partner);
                hasSpawnedChild = true;
                human.reproduceCooldownTimer = human.reproduceCooldownTime;
                partner.reproduceCooldownTimer = partner.reproduceCooldownTime;
                human.ChangeState(new PatrolState(human));
                partner.ChangeState(new PatrolState(partner));
            }
        }
    }

    public void Exit() {
        // Mo¿esz dodaæ efekt zakoñczenia rozmna¿ania
    }

    private bool IsBothInReproduceState() {
        return partner != null && partner.currentState is ReproduceState;
    }

    private void SpawnChild(Human parent1, Human parent2) {
        Debug.Log("child spawned");
        GameObject child = Object.Instantiate(parent1.gameObject, parent1.transform.position, Quaternion.identity);
        Human childHuman = child.GetComponent<Human>();

        // Pobierz genomy rodziców
        Genome genome1 = parent1.GetComponent<Genome>();
        Genome genome2 = parent2.GetComponent<Genome>();

        // Stwórz geny dziecka na podstawie rodziców z mutacj¹
        float[] childGenes;
        if (genome1 != null && genome2 != null) {
            childGenes = GeneticAlgorithm.Crossover(genome1.genes, genome2.genes);
        }
        else {
            // fallback: losowe geny
            childGenes = new float[6];
            for (int i = 0; i < 6; i++)
                childGenes[i] = Random.Range(0f, 1f);
        }

        // Przypisz geny dziecku
        Genome childGenome = child.GetComponent<Genome>();
        if (childGenome == null)
            childGenome = child.AddComponent<Genome>();
        childGenome.SetGenes(childGenes);

        // Ustaw pozosta³e parametry
        childHuman.gender = (Random.value > 0.5f) ? GENDER.male : GENDER.female;
        childHuman.predator = parent1.predator;
        childHuman.SetGenderColor();
        childHuman.reproduceCooldownTimer = childHuman.reproduceCooldownTime; // Dziecko nie mo¿e siê rozmna¿aæ na starcie
        childHuman.ApplyGenome();

        OnBirth?.Invoke();
    }
}