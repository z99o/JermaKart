using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipper : MonoBehaviour
{
    // Start is called before the first frame update
    public float frequency;
    public SpriteRenderer sprite;
    public ParticleSystem hearts;
    void Start()
    {
        StartCoroutine(C_Fliper());
    }

    IEnumerator C_Fliper()
    {
        yield return new WaitForSeconds(frequency);
        sprite.flipX = !sprite.flipX;
        StartCoroutine(C_Fliper());
    }
    private void OnTriggerEnter(Collider other)
    {
        hearts.Play();
    }
}
