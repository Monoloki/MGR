using UnityEngine;

public class SearchWaterState : IState {
    private readonly Human character;
    private GameObject targetWater;
    private Vector3 searchTarget;

    public SearchWaterState(Human character) {
        this.character = character;
    }

    public void Enter() {
        Debug.Log("Entering Search Water State");
        FindWaterOrSetSearchTarget();
    }

    public void Execute() {
        if (!character.NeedsWater()) {
            character.ChangeState(new IdleState(character));
            return;
        }

        if (targetWater != null) {
            // Podejœcie do wody
            character.transform.position = Vector3.MoveTowards(
                character.transform.position,
                targetWater.transform.position,
                character.patrolSpeed * Time.deltaTime
            );

            if (Vector3.Distance(character.transform.position, targetWater.transform.position) < 0.1f) {
                // Woda zosta³a osi¹gniêta
                character.DrinkWater();
                //GameObject.Destroy(targetWater); // Opcjonalne usuniêcie obiektu wody
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
                FindWaterOrSetSearchTarget(); // ZnajdŸ wodê lub wybierz nowy cel
            }
        }
    }

    public void Exit() {
        Debug.Log("Exiting Search Water State");
    }

    private void FindWaterOrSetSearchTarget() {
        targetWater = character.FindClosestObjectWithTag("DrinkingWater");

        if (targetWater == null) {
            // Jeœli nie znaleziono wody, ustaw nowy losowy punkt poszukiwañ
            searchTarget = character.GenerateRandomPointWithinBounds();
            Debug.Log("No water found, moving to random search target.");
        }
        else {
            Debug.Log("Water found, moving to target.");
        }
    }
}
