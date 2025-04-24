using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    BoxCollider2D arrowHitBox;
    Rigidbody2D arrowRigidbody;
       

    [SerializeField] float arrowSpeed;
    [SerializeField] float arrowLifetime;


    void Start()
    {
        arrowRigidbody = GetComponent<Rigidbody2D>();
        arrowHitBox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Fly();
        Invoke("Disappear", 0.7f);
    }

    void Fly()
    {
        arrowRigidbody.linearVelocity = new Vector2(transform.localScale.x * arrowSpeed, 0f);
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Enemy")){
            EnemyStatus enemyStatus = other.GetComponent<EnemyStatus>();
            if (enemyStatus != null){
                enemyStatus.SetHP(enemyStatus.GetHP() - 1);
                int currentHealth = enemyStatus.GetHP();
                Debug.Log("This enemy has " + currentHealth + " HP left!");
            }

            Destroy(gameObject);
        }

        Destroy(gameObject); 
    }

    void Disappear()
    {
        Destroy(gameObject);
    }
}
