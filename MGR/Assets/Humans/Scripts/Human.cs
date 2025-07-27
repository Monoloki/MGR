using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Human : MonoBehaviour {
    public float hunger = 100f; // Aktualny poziom g³odu
    public float thirst = 100f; // Aktualny poziom pragnienia
    public float energy = 100f;
    public float health = 100f;
    public float maxHealth = 100f;
    public float maxHunger = 100f; // Maksymalny poziom g³odu
    public float maxThirst = 100f; // Maksymalny poziom pragnienia
    public float maxEnergy = 100f;
    public float foodConsumptionRate = 0.1f; // Tempo spadku parametrów
    public float waterConsumptionRate = 1f; // Tempo spadku parametrów
    public float energyConsumption = 0.1f; // Tempo spadku parametrów
    public float patrolSpeed = 2f; // Prêdkoœæ poruszania siê podczas patrolowania
    public float range = 0.1f; // Zasiêg wykrywania jedzenia i wody
    public float movingRange = 5f;
    public bool predator = false;
    public GENDER gender = GENDER.male;
    public TMP_Text stausText;

    [SerializeField] private GameObject meat;
    [SerializeField] private Image healthBar;
    [SerializeField] private Genome genome;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color[] preyGenderColor;
    [SerializeField] private Color[] predatorGenderColor;
    public Color gizmoColor = Color.green;
    private int segments = 50; // Liczba segmentów do narysowania okrêgu (im wiêcej, tym g³adszy)
    private IState currentState;

    private void Start() {
        ChangeState(new PatrolState(this)); // Rozpocznij w stanie patrolowania
        gender = (GENDER)Random.Range(0, 2);
        SetGenderColor();
    }

    private void Update() {
        hunger -= foodConsumptionRate * Time.deltaTime;
        thirst -= waterConsumptionRate * Time.deltaTime;
        energy -= waterConsumptionRate * Time.deltaTime;

        CheckForHealthLose();
        CheckForDeath();


        currentState?.Execute(); // Wykonaj bie¿¹cy stan
    }

    private void CheckForHealthLose(){
        if (hunger <= 0) {
            LoseHp( 1 * Time.deltaTime);
        }

        if (thirst <= 0) {
            LoseHp(5 * Time.deltaTime);
        }
    }

    public void LoseHp(float hpToLose) {
        health -= hpToLose;
        healthBar.fillAmount = health / 100;
    }

    private void CheckForDeath() {
        if (health <= 0) {
            Debug.Log("Character has died.");
            Die();
            Destroy(gameObject);
            return;
        }
    }

    public void ChangeState(IState newState) {
        stausText.text = newState.ToString();
        currentState?.Exit(); // WyjdŸ ze starego stanu
        currentState = newState;
        currentState.Enter(); // WejdŸ w nowy stan
    }

    public bool CanIEatIt(GameObject targetFood) {
        Human edible;
        targetFood.TryGetComponent<Human>(out edible);

        if (edible != null && predator) {
            return true;
        }
        return false;
        
    }

    virtual public GameObject FindClosestObjectWithTag(string tag) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders) {

            if (collider.CompareTag(tag)) {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closest = collider.gameObject;
                }
            }
        }
        return closest;
    }

    public void SetGenderColor() {

        if (predator) {
            sprite.color = predatorGenderColor[(int)gender];
        }
        else {
            sprite.color = preyGenderColor[(int)gender];
        }
    }

    public bool NeedsFood() => hunger / maxHunger < 0.5f; // Czy postaæ jest bardzo g³odna

    public bool ExtreameStarving() => hunger / maxHunger < 0.1f; // Krytyczny poziom godu
    public bool NeedsWater() => thirst / maxThirst < 0.5f; // Czy postaæ jest bardzo spragniona

    public bool ExtreameThirst() => thirst / maxThirst < 0.1f; // Krytyczny poziom pragnienia

    public bool IsWithinRange(Vector3 target) => Vector3.Distance(transform.position, target) <= range; // Czy obiekt jest w zasiêgu

    // Przyk³adowa metoda do symulacji znalezienia wody
    public void DrinkWater() {
        thirst = maxThirst; 
    }

    // Przyk³adowa metoda do symulacji znalezienia jedzenia
    public void EatFood(GameObject food) {
        hunger = maxHunger;

        if (predator) {
            Destroy(food);
        }
    }

    public Vector3 GenerateRandomPointWithinBounds() {
        Bounds mapBoundaries;
        // ZnajdŸ obiekt z tagiem Map i pobierz komponent klasy Map
        GameObject mapObject = GameObject.FindGameObjectWithTag("Map");
        if (mapObject != null) {
            Map map = mapObject.GetComponent<Map>();
            if (map != null) {
                mapBoundaries = map.boundries.bounds; // Pobierz granice z klasy Map
                float randomX = Random.Range(mapBoundaries.min.x, mapBoundaries.max.x);
                float randomY = Random.Range(mapBoundaries.min.y, mapBoundaries.max.y);
                return new Vector3(randomX, randomY, transform.position.z);
            }
            else {
                Debug.LogError("No 'Map' component found on the object with tag 'Map'.");
            }
        }
        else {
            Debug.LogError("No object with tag 'Map' found in the scene!");
        }
        return new Vector3(0,0,0);
        
    }

    private void OnDrawGizmos() {
        Gizmos.color = gizmoColor;
        DrawCircle(range, segments);
    }

    private void DrawCircle(float radius, int segments) {
        float angleStep = 360f / segments;

        Vector3 previousPoint = transform.position + new Vector3(radius, 0, 0); // Startowy punkt okrêgu
        for (int i = 1; i <= segments; i++) {
            float angle = angleStep * i;
            float rad = Mathf.Deg2Rad * angle;

            // Oblicz nastêpny punkt na okrêgu
            Vector3 newPoint = transform.position + new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0);

            // Rysuj liniê miêdzy poprzednim a nowym punktem
            Gizmos.DrawLine(previousPoint, newPoint);

            previousPoint = newPoint; // Zaktualizuj poprzedni punkt
        }
    }

    public void Die() {
        Instantiate(meat, transform.position, Quaternion.identity);
    }
}







