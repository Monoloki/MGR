
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Human : MonoBehaviour {
    public static event System.Action OnDeath;
    public float hunger = 100f; // Aktualny poziom g³odu
    public float thirst = 100f; // Aktualny poziom pragnienia
    public float energy = 100f;
    public float health = 100f;
    public float maxHealth = 100f;
    public float maxHunger = 100f; // Maksymalny poziom g³odu
    public float maxThirst = 100f; // Maksymalny poziom pragnienia
    public float maxEnergy = 100f;
    public float foodConsumptionRate = 1.2f; // Tempo spadku parametrów
    public float waterConsumptionRate = 2f; // Tempo spadku parametrów
    public float energyConsumption = 0.2f; // Tempo spadku parametrów
    public float patrolSpeed = 4f; // Prêdkoœæ poruszania siê podczas patrolowania
    public float range = 3f; // Zasiêg wykrywania jedzenia i wody
    public float movingRange = 10f;
    public bool predator = false;
    public GENDER gender = GENDER.male;
    public TMP_Text stausText;
    public int generation = 0;

    [SerializeField] private GameObject meat;
    [SerializeField] private Image healthBar;
    public Genome genome;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color[] preyGenderColor;
    [SerializeField] private Color[] predatorGenderColor;
    public Color gizmoColor = Color.green;
    private int segments = 50; // Liczba segmentów do narysowania okrêgu (im wiêcej, tym g³adszy)
    public IState currentState;
    public float reproduceCooldownTime = 5f; // czas oczekiwania po rozmna¿aniu
    public float reproduceCooldownTimer;


    public void ApplyGenome() {
        if (genome == null || genome.genes == null || genome.genes.Length < 6)
            return;

        // Tê¿yzna (0): maxHealth, foodConsumptionRate, waterConsumptionRate
        float tezyzna = genome.genes[0];
        maxHealth = Mathf.Lerp(80f, 200f, tezyzna); // np. 80-200
        health = maxHealth;
        foodConsumptionRate = Mathf.Lerp(2.5f, 0.8f, tezyzna); // im wiêksza tê¿yzna, tym wolniej spada g³ód
        waterConsumptionRate = Mathf.Lerp(3.5f, 1.0f, tezyzna); // im wiêksza tê¿yzna, tym wolniej spada pragnienie

        // Popêd (1): reproduceCooldownTimer
        float poped = genome.genes[1];

        if (gender == GENDER.male)
            reproduceCooldownTimer = Mathf.Lerp(60f, 30f, poped); // np. 60-10 sekund
        else
            reproduceCooldownTimer = Mathf.Lerp(90f, 50f, poped); // np. 60-10 sekund

        // Zwinnoœæ (2): patrolSpeed
        float zwin = genome.genes[2];
        patrolSpeed = Mathf.Lerp(1f, 4f, zwin);

        // Percepcja (4): range
        float percepcja = genome.genes[4];
        range = Mathf.Lerp(0.5f, 3f, percepcja);
    }

    private void Start() {
        ChangeState(new PatrolState(this)); // Rozpocznij w stanie patrolowania
        gender = (GENDER)Random.Range(0, 2);
        SetGenderColor();

        ApplyGenome();
    }

    public void ForceReproduceWith(Human partner) {
        if (!(currentState is ReproduceState)) {
            ChangeState(new ReproduceState(this, partner));
        }
    }

    private void Update() {
        hunger -= foodConsumptionRate * Time.deltaTime;
        thirst -= waterConsumptionRate * Time.deltaTime;
        CheckForHealthLose();
        CheckForDeath();

        if (reproduceCooldownTimer > 0f)
            reproduceCooldownTimer -= Time.deltaTime;

        if (currentState is PatrolState && IsReadyToReproduce()) {
            Human partner = FindReproducePartner();
            if (partner != null) {
                ChangeState(new ReproduceState(this, partner));
                partner.ForceReproduceWith(this);
            }
        }

        currentState?.Execute();
    }

    private Human FindReproducePartner() {
        Physics2D.queriesHitTriggers = true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D col in colliders) {
            Human other = col.GetComponent<Human>();
            if (other != null &&
                other != this &&
                other.predator == this.predator &&
                other.gender != this.gender &&
                other.IsReadyToReproduce()) {
                return other;
            }
        }
        return null;
    }

    private bool CanReproduce() {
        Physics2D.queriesHitTriggers = true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D col in colliders) {
            Human other = col.GetComponent<Human>();
            if (other != null &&
                other != this &&
                other.predator == this.predator &&
                other.gender != this.gender &&
                other.IsReadyToReproduce() &&
                this.IsReadyToReproduce()) {
                return true;
            }
        }
        return false;
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
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject obj in objects) {
            if (obj == this.gameObject || obj.TryGetComponent<Human>(out Human human)) // Pomijaj samego siebie
                continue;

            float dist = Vector3.Distance(obj.transform.position, currentPos);
            if (dist < minDist) {
                closest = obj;
                minDist = dist;
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

        if (food.tag == "EdibleFood") {
            food.TryGetComponent<Human> (out Human humanfood);
            if (humanfood != null) {
                humanfood.Die();
            }
            else {
                Destroy(food);
            }
        }
        else {
            Destroy(food);
        }
    }

    public Vector3 GenerateRandomPointWithinBounds() {
        Bounds mapBoundaries;
        GameObject mapObject = GameObject.FindGameObjectWithTag("Map");
        if (mapObject != null) {
            Map map = mapObject.GetComponent<Map>();
            if (map != null) {
                mapBoundaries = map.boundries.bounds;
                Vector3 randomPoint;
                int safety = 0;
                do {
                    float randomX = Random.Range(mapBoundaries.min.x, mapBoundaries.max.x);
                    float randomY = Random.Range(mapBoundaries.min.y, mapBoundaries.max.y);
                    randomPoint = new Vector3(randomX, randomY, transform.position.z);
                    safety++;
                    if (safety > 20) break; // zabezpieczenie przed nieskoñczon¹ pêtl¹
                } while (Vector3.Distance(transform.position, randomPoint) < 1f);
                return randomPoint;
            }
            else {
                Debug.LogError("No 'Map' component found on the object with tag 'Map'.");
            }
        }
        else {
            Debug.LogError("No object with tag 'Map' found in the scene!");
        }
        return new Vector3(0, 0, 0);
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

    public bool IsReadyToReproduce() {
        return reproduceCooldownTimer <= 0f && (currentState is IdleState || currentState is PatrolState);
    }

    public void Die() {
        Instantiate(meat, transform.position, Quaternion.identity);
        OnDeath?.Invoke();

        Destroy(gameObject);
    }
}







