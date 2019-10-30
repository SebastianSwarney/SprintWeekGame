using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperController : MonoBehaviour
{
    public enum BumperStates
    {
        Inactive,
        Charging,
        Forcing
    }
    [HideInInspector]
    public BumperStates bumperStates = BumperStates.Inactive;

    private SpriteRenderer spriteRendererComponent;
    private float changingColorValue = 1.0f;

    public Color bumperColourA;
    public Color bumperColourB;

    [Tooltip("The ring that visually represents the force.")]
    public GameObject forceRing;
    [Tooltip("The ring's sprite renderer component.")]
    public SpriteRenderer ringSprite;
    private Vector3 originalForceRingSize;
    private Color originalRingColor;
    private float ringAlphaChangeRate = 1.0f;

    private int layerMask = 1 << 11;

    [Tooltip("The force at which the bumper pushes physics objects.")]
    public float force;
    [Tooltip("Time until bumper is activated.")]
    public float timeToActivation;
    private float elapsingTimeToActivation;
    private float elapsingCountDownTime;
    [Tooltip("The initial range at which the bumper applies force when activated.")]
    public float initialForceRange;
    private float increasingForceRange;
    [Tooltip("The duration that the force radius grows for.")]
    public float forceGrowthTime;
    private float elapsingForceGrowthTime;
    [Tooltip("Rate at which force range grows.")]
    public float forceRangeGrowthRate;

    // Start is called before the first frame update
    void Start()
    {
        spriteRendererComponent = gameObject.GetComponent<SpriteRenderer>();

        increasingForceRange = initialForceRange;
        elapsingForceGrowthTime = forceGrowthTime;
        elapsingTimeToActivation = timeToActivation;
        elapsingCountDownTime = timeToActivation / 3;

        originalForceRingSize = forceRing.transform.localScale;
        originalRingColor = ringSprite.color;
    }

    private void PushFromCenter()
    {
        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(transform.position, increasingForceRange,
            layerMask);

        foreach (Collider2D collider in overlappingColliders)
        {
            collider.GetComponentInParent<Rigidbody2D>().AddForce(collider.transform.position - transform.position *
                force, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {


        if (bumperStates == BumperStates.Charging)
        {
            elapsingTimeToActivation -= Time.fixedDeltaTime;
            changingColorValue -= .01f;
            spriteRendererComponent.color = new Color(spriteRendererComponent.color.r,
                Mathf.Lerp(0.0f, timeToActivation, elapsingTimeToActivation / timeToActivation),
                 Mathf.Lerp(0.0f, timeToActivation, elapsingTimeToActivation / timeToActivation), 1.0f);
        }

        if (elapsingTimeToActivation <= 0.0f)
        {
            bumperStates = BumperStates.Forcing;
            elapsingTimeToActivation = timeToActivation;
        }

        if (bumperStates == BumperStates.Forcing)
        {
            elapsingForceGrowthTime -= Time.fixedDeltaTime;
            increasingForceRange += forceRangeGrowthRate * Time.fixedDeltaTime;
            forceRing.transform.localScale += new Vector3(Mathf.Sqrt(forceRangeGrowthRate / 2) / 3.14f,
                Mathf.Sqrt(forceRangeGrowthRate / 2) / 3.14f, 0.0f) * Time.fixedDeltaTime;
            ringSprite.color = new Color(ringSprite.color.r, ringSprite.color.g, ringSprite.color.b,
                Mathf.Lerp(0.0f, forceGrowthTime, elapsingForceGrowthTime / forceGrowthTime));
            PushFromCenter();
        }

        if (elapsingForceGrowthTime <= 0.0f)
        {
            elapsingForceGrowthTime = forceGrowthTime;
            increasingForceRange = initialForceRange;
            forceRing.transform.localScale = originalForceRingSize;
            forceRing.gameObject.GetComponent<SpriteRenderer>().color = originalRingColor;
            ringAlphaChangeRate = 1.0f;
            spriteRendererComponent.color = Color.white;
            bumperStates = BumperStates.Inactive;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (bumperStates == BumperStates.Inactive)
        {
            bumperStates = BumperStates.Charging;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, increasingForceRange);
    }
}
