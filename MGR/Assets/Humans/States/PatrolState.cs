using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState {
    private readonly Human character;
    private Bounds mapBoundaries; // Granice mapy jako obiekt Bounds
    private Vector3 patrolTarget;

    public PatrolState(Human character) {
        this.character = character;
    }

    public void Enter() {
        Debug.Log("Entering Patrol State");
        SetNewPatrolTarget();
    }

    public void Execute() {
        // Przemieszczanie si� do celu
        character.transform.position = Vector3.MoveTowards(
            character.transform.position,
            patrolTarget,
            character.patrolSpeed * Time.deltaTime
        );

        // Je�li osi�gni�to cel, wybierz nowy punkt
        if (Vector3.Distance(character.transform.position, patrolTarget) < 0.1f) {
            SetNewPatrolTarget();
        }

        // Je�li posta� jest g�odna lub spragniona, zmie� stan na Idle
        if (character.NeedsFood() || character.NeedsWater()) {
            character.ChangeState(new IdleState(character));
        }
    }

    public void Exit() {
        Debug.Log("Exiting Patrol State");
    }

    private void SetNewPatrolTarget() {
        // Generowanie punktu w granicach mapy
        patrolTarget = character.GenerateRandomPointWithinBounds();
    }
}
