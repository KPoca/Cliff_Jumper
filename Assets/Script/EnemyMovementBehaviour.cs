using UnityEngine;

public class EnemyMovementBehaviour : MonoBehaviour
{
    Rigidbody2D enemyRigidbody;
    BoxCollider2D enemyWallCollider;
    EnemyStatus enemyStatus;
    FlashColor flashColor;

    [SerializeField] float enemyMovementSpeed = 5000f;
    float moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyWallCollider = GetComponent<BoxCollider2D>();
        enemyStatus = GetComponent<EnemyStatus>();
        flashColor = GetComponent<FlashColor>();

        moveDirection = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        FlipEnemy();
        Die();
    }

    void Move()
    {
        Vector2 enemyVelocity = new Vector2(moveDirection * enemyMovementSpeed, enemyRigidbody.linearVelocity.y);
        enemyRigidbody.linearVelocity = enemyVelocity;
    }

    void FlipEnemy()
    {
        if (enemyWallCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            transform.localScale = new Vector2(-transform.localScale.x, 1f);
            moveDirection = -moveDirection;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Arrow")){
            flashColor.FlashRed();
        } 
    }

    void Die(){
        if (enemyStatus.GetHP() <= 0){
            Destroy(gameObject);
        }
    }
}
