using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    PlayerMovement playerMovement;
    Rigidbody2D playerRigidbody;
    Animator playerAnimation;
    SpriteRenderer spriteRenderer;
    [SerializeField] int HP = 3;
    [SerializeField] TextMeshProUGUI healthText;

    [SerializeField] float kickBack = 20f;
    [SerializeField] LayerMask hazardLayers;
    [SerializeField] float iFrameDuration;

    [SerializeField] PhysicsMaterial2D noFriction;
    [SerializeField] PhysicsMaterial2D hasFriction;
    

    int playerLayer;
    bool isInvincible = false;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthText = FindAnyObjectByType<TextMeshProUGUI>();

        playerLayer = gameObject.layer;
    }

    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvincible) return;

        if (collision.gameObject.CompareTag("Enemy") || ((1 << collision.gameObject.layer) & hazardLayers) != 0) {
            if (HP > 1) {
                DeductHealth(1);
                DmgKickBack();
                StartCoroutine(TemporalIFrame());
            } else {
                DeductHealth(1);
                DmgKickBack();
                TriggerDeath();
                DisableCollision(); 
            }
            
        }
    }

    void DeductHealth(int value)
    {
        HP -= value;
        if (HP < 0) HP = 0;
        healthText.text = "x " + HP;
    }

    void TriggerDeath()
    {
        playerMovement.SetIsDeath(true);
        playerAnimation.SetTrigger("onTakingDmg");
    }

    void DmgKickBack()
    {
        playerRigidbody.linearVelocity = new Vector2(-transform.localScale.x * kickBack, kickBack);
        playerRigidbody.sharedMaterial = hasFriction;
    }

    void DisableCollision()
    {
        for (int i = 0; i < 32; i++){
            if ((hazardLayers & (1 << i)) != 0) {
                Physics2D.IgnoreLayerCollision(playerLayer, i, true);
            }
        }
    }

    IEnumerator TemporalIFrame()
    {
        isInvincible = true;

        for (int i = 0; i < 32; i++){
            if ((hazardLayers & (1 << i)) != 0) {
                Physics2D.IgnoreLayerCollision(playerLayer, i, true);
            }
        }

        StartCoroutine(IFrameFlickering());
        yield return new WaitForSeconds(iFrameDuration);

        for (int i = 0; i < 32; i++){
            if ((hazardLayers & (1 << i)) != 0) {
                Physics2D.IgnoreLayerCollision(playerLayer, i, false);
            }
        }

        isInvincible = false;
    }

    IEnumerator IFrameFlickering()
    {
        float timeElapsed = 0f;
        while (iFrameDuration > timeElapsed){
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.2f);
            timeElapsed += 0.3f;
        }
    }


    public void SetHP(int value){ HP = value; }
    public int GetHP(){ return HP; }
}