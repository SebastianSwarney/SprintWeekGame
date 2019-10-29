using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperController : MonoBehaviour
{
    private SpriteRenderer spriteRendererComponent;
    private float redValue = 1.0f;

    public GameObject forceRing;
    private Vector3 originalForceRingSize;

    int layerMask = 1 << 0;

    [Tooltip("The force at which the bumper pushes physics objects.")]
    public float force;

    [Tooltip("The initial range at which the bumper applies force when activated.")]
    public float initialForceRange;
    private float increasingForceRange;
    public float forceRangeGrowthRate;

    private bool forceActivated;
    private bool triggered;

    public float timeToActivation;
    private float elapsingTimeToActivation;

    public float forceGrowthTime;
    private float elapsingForceGrowthTime;

    // Start is called before the first frame update
    void Start()
    {
        spriteRendererComponent = gameObject.GetComponent<SpriteRenderer>();

        increasingForceRange = initialForceRange;
        elapsingForceGrowthTime = forceGrowthTime;
        elapsingTimeToActivation = timeToActivation;

        originalForceRingSize = forceRing.transform.localScale;
    }

    private void PushFromCenter()
    {
        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(transform.position, increasingForceRange, layerMask);

        foreach (Collider2D collider in overlappingColliders)
        {
            collider.GetComponentInParent<Rigidbody2D>().AddForce(collider.transform.position - transform.position * force, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (triggered == true)
        {
            elapsingTimeToActivation -= Time.fixedDeltaTime;
            redValue -= .01f;
            spriteRendererComponent.color = new Color(1.0f, redValue, redValue);
        }

        if (elapsingTimeToActivation <= 0.0f)
        {
            forceActivated = true;         
            elapsingTimeToActivation = timeToActivation;
        }

        if (forceActivated == true)
        {
            elapsingForceGrowthTime -= Time.fixedDeltaTime;
            increasingForceRange += forceRangeGrowthRate * Time.fixedDeltaTime;
            forceRing.transform.localScale += new Vector3(Mathf.Sqrt(forceRangeGrowthRate) / 3.14f / 2, 
                Mathf.Sqrt(forceRangeGrowthRate) / 3.14f / 2, 0.0f) * Time.fixedDeltaTime;
            //forceRing.gameObject.GetComponent<SpriteRenderer>().color = new Color( 0.1f;
            PushFromCenter();
        }

        if (elapsingForceGrowthTime <= 0.0f)
        {
            elapsingForceGrowthTime = forceGrowthTime;
            increasingForceRange = initialForceRange;
            forceRing.transform.localScale = originalForceRingSize;
            spriteRendererComponent.color = Color.white;
            forceActivated = false;
            triggered = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggered = true;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, increasingForceRange);
    }
}
