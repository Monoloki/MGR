using UnityEngine;

public class SearchFoodState : IState {
    private readonly Human character;
    private GameObject targetFood;
    private Vector3 searchTarget;

    public SearchFoodState(Human character) {
        this.character = character;
    }

    public void Enter() {
        Debug.Log("Entering Search Food State");
        FindFoodOrSetSearchTarget();
    }

    public void Execute() {
        if (!character.NeedsFood()) {
            character.ChangeState(new IdleState(character));
            return;
        }

        if (targetFood != null) {
            // Podej�cie do jedzenia
            character.transform.position = Vector3.MoveTowards(
                character.transform.position,
                targetFood.transform.position,
                character.patrolSpeed * Time.deltaTime
            );

            if (Vector3.Distance(character.transform.position, targetFood.transform.position) < 0.1f) {
                // Jedzenie zosta�o osi�gni�te
                character.EatFood();
                //GameObject.Destroy(targetFood); // Opcjonalne usuni�cie obiektu jedzenia
                character.ChangeState(new IdleState(character));
            }
        }
        else {
            // Kontynuuj szukanie w losowym punkcie
            character.transform.position = Vector3.MoveTowards(
                character.transform.position,
                searchTarget,
                character.patrolSpeed * Time.deltaTime
            );

            if (Vector3.Distance(character.transform.position, searchTarget) < 0.1f) {
                FindFoodOrSetSearchTarget(); // Znajd� jedzenie lub wybierz nowy cel
            }
        }
    }

    public void Exit() {
        Debug.Log("Exiting Search Food State");
    }

    private void FindFoodOrSetSearchTarget() {
        targetFood = character.FindClosestObjectWithTag("EdibleFood");

        if (targetFood == null) {
            // Je�li nie znaleziono jedzenia, ustaw nowy losowy punkt poszukiwa�
            searchTarget = character.GenerateRandomPointWithinBounds();
            Debug.Log("No food found, moving to random search target.");
        }
        else {
            Debug.Log("Food found, moving to target.");
        }
    }
}
