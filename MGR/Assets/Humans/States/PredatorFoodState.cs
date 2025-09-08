using System.Collections;
using UnityEngine;

public class PredatorFoodState : IState {
    private readonly Human character;
    private GameObject targetFood;
    private Human targetPrey;
    private Vector3 searchTarget;
    private float attackInterval = 1.0f;
    private float attackTimer = 0f;
    private float attackDamage = 20f;
    public static event System.Action OnPredatorKill;

    public PredatorFoodState(Human character) {
        this.character = character;
        // Si³a (5): attackDamage
        if (character.genome != null && character.genome.genes != null && character.genome.genes.Length >= 6)
            attackDamage = Mathf.Lerp(3f, 10f, character.genome.genes[5]);
        else
            attackDamage = 20f; // domyœlna wartoœæ
    }

    public void Enter() {
        Debug.Log("Entering Search Food State");
        FindTarget();
    }

    public void Execute() {
        if (!character.NeedsFood()) {
            character.ChangeState(new IdleState(character));
            return;
        }

        FindTarget();

        if (targetFood != null) {
            // PodejdŸ do jedzenia
            character.transform.position = Vector3.MoveTowards(
                character.transform.position,
                targetFood.transform.position,
                character.patrolSpeed * Time.deltaTime
            );

            if (Vector3.Distance(character.transform.position, targetFood.transform.position) < 0.1f) {
                if (character == null) return;
                character.EatFood(targetFood);
                character.ChangeState(new IdleState(character));
            }
 
        }
        else if (targetPrey != null) {
            // PodejdŸ do ofiary
            character.transform.position = Vector3.MoveTowards(
                character.transform.position,
                targetPrey.transform.position,
                character.patrolSpeed * Time.deltaTime
            );

            if (Vector3.Distance(character.transform.position, targetPrey.transform.position) < 1f) {
                // Atakuj w interwa³ach
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackInterval) {
                    attackTimer = 0f;
                    targetPrey.LoseHp(attackDamage);
                    if (targetPrey.health <= 0) {
                        OnPredatorKill?.Invoke();
                        character.EatFood(targetPrey.gameObject);
                        targetPrey = null;
                        character.ChangeState(new IdleState(character));
                    }
                }
            }
        }
        else {
            character.transform.position = Vector3.MoveTowards(
            character.transform.position,
            searchTarget,
            character.patrolSpeed * Time.deltaTime
        );

            // Jeœli osi¹gniêto cel, wybierz nowy punkt
            if (Vector3.Distance(character.transform.position, searchTarget) < 0.1f) {
                searchTarget = character.GenerateRandomPointWithinBounds();
            }
        }
 
    }

    public void Exit() {
        Debug.Log("Exiting Search Food State");
    }

    private void FindTarget() {
        // Szukaj le¿¹cego jedzenia
        targetFood = character.FindClosestObjectWithTag("EdibleFood");

        // Szukaj najbli¿szego roœlino¿ercy
        targetPrey = FindClosestHerbivore();

        // Priorytet: najpierw atakuj roœlino¿ercê, potem jedzenie
        if (targetFood != null) {
            targetPrey = null;
        }
    }

    private Human FindClosestHerbivore() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(character.transform.position, character.range);
        Human closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var col in colliders) {
            Human other = col.GetComponent<Human>();
            if (other != null && !other.predator && other != character && other.health > 0) {              
                float distance = Vector3.Distance(character.transform.position, other.transform.position);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closest = other;
                }
            }
        }
        return closest;
    }
}