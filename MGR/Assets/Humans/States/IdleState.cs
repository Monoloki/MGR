using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState {
    private readonly Human character;

    public IdleState(Human character) {
        this.character = character;
    }

    public void Enter() {
       // Debug.Log("Entering Idle State");
    }   

    public void Execute() {
        if (character.NeedsFood()) {
            character.ChangeState(new SearchFoodState(character));
        }
        else if (character.NeedsWater()) {
            character.ChangeState(new SearchWaterState(character));
        }
        else {
            character.ChangeState(new PatrolState(character));
        }
    }

    public void Exit() {
       // Debug.Log("Exiting Idle State");
    }
}
