using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.Rendering.HableCurve;

public class Human : MonoBehaviour {
    public float hunger = 100f; // Aktualny poziom g³odu
    public float thirst = 100f; // Aktualny poziom pragnienia
    public float maxHunger = 100f; // Maksymalny poziom g³odu
    public float maxThirst = 100f; // Maksymalny poziom pragnienia
    public float foodConsumptionRate = 0.1f; // Tempo spadku parametrów
    public float waterConsumptionRate = 1f; // Tempo spadku parametrów
    public float patrolSpeed = 2f; // Prêdkoœæ poruszania siê podczas patrolowania
    public float range = 0.1f; // Zasiêg wykrywania jedzenia i wody
    public float movingRange = 5f;

    public Color gizmoColor = Color.green;
    public int segments = 50; // Liczba segmentów do narysowania okrêgu (im wiêcej, tym g³adszy)


    private IState currentState;

    private void Start() {
        ChangeState(new PatrolState(this)); // Rozpocznij w stanie patrolowania
    }

    private void Update() {
        hunger -= foodConsumptionRate * Time.deltaTime;
        thirst -= waterConsumptionRate * Time.deltaTime;

        if (hunger <= 0 || thirst <= 0) {
            Debug.Log("Character has died.");
            //Destroy(gameObject); // Postaæ umiera, jeœli g³ód lub pragnienie wynosi 0
            return;
        }

        currentState?.Execute(); // Wykonaj bie¿¹cy stan
    }

    public void ChangeState(IState newState) {
        currentState?.Exit(); // WyjdŸ ze starego stanu
        currentState = newState;
        currentState.Enter(); // WejdŸ w nowy stan
    }

    public GameObject FindClosestObjectWithTag(string tag) {
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

    public bool NeedsFood() => hunger / maxHunger < 0.5f; // Czy postaæ jest bardzo g³odna
    public bool NeedsWater() => thirst / maxThirst < 0.5f; // Czy postaæ jest bardzo spragniona
    public bool IsWithinRange(Vector3 target) => Vector3.Distance(transform.position, target) <= range; // Czy obiekt jest w zasiêgu

    // Przyk³adowa metoda do symulacji znalezienia wody
    public void DrinkWater() {
        thirst = maxThirst;
        Debug.Log("Character drank water.");
    }

    // Przyk³adowa metoda do symulacji znalezienia jedzenia
    public void EatFood() {
        hunger = maxHunger;
        Debug.Log("Character ate food.");
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
}







