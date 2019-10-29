using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperController : MonoBehaviour
{
    public LayerMask m_mask;

    public float force;
    public float forceRange;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PushFromCenter()
    {
        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(transform.position, forceRange, m_mask);

        foreach (Collider2D collider in overlappingColliders)
        {
            collider.GetComponent<Rigidbody2D>().AddForce(collider.transform.position - transform.position * force, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PushFromCenter();
    }
}
