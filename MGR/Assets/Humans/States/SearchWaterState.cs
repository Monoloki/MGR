using System;
using UnityEngine;

public class SearchWaterState : IState {
    private readonly Human character;
    private GameObject targetWater;
    private Vector3 searchTarget;

    public SearchWaterState(Human character) {
        this.character = character;
    }

    public void Enter() {
        FindWaterOrSetSearchTarget();
    }

    public void Execute() {
        if (!character.NeedsWater()) {
            character.ChangeState(new IdleState(character));
            return;
        }

        

        if (targetWater != null) {

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

            targetWater = character.FindClosestObjectWithTag("DrinkingWater");

            if (targetWater != null) return;         


            if (Vector3.Distance(character.transform.position, searchTarget) < 0.1f) {
                searchTarget = character.GenerateRandomPointWithinBounds();
            }
            else {
                character.transform.position = Vector3.MoveTowards(
                character.transform.position,
                searchTarget,
                character.patrolSpeed * Time.deltaTime
                );
            }
        }
    }

    public void Exit() {
        //Debug.Log("Exiting Search Water State");
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
