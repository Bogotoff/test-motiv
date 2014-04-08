using UnityEngine;
using System.Collections;

public class BonusTrigger: MonoBehaviour
{
    public int score          = 10;
    public bool destroyOnTake = false;
    public float rotateSpeed  = 0f;
    public bool rotateX       = false;
    public bool rotateY       = false;
    public bool rotateZ       = true;

    private Transform _cachedTransform = null;

    void Start()
    {
        _cachedTransform = gameObject.transform;

        if (rotateSpeed > 0) {
            if (rotateX) {
                _cachedTransform.Rotate(Vector3.right, Random.Range(1, 180));
            }

            if (rotateY) {
                _cachedTransform.Rotate(Vector3.up, Random.Range(1, 180));
            }

            if (rotateZ) {
                _cachedTransform.Rotate(Vector3.forward, Random.Range(1, 180));
            }
        }
    }

    void Update()
    {
        if (rotateSpeed > 0) {
            float f = rotateSpeed * Time.deltaTime;

            if (rotateX) {
                _cachedTransform.Rotate(Vector3.right, f);
            }

            if (rotateY) {
                _cachedTransform.Rotate(Vector3.up, f);
            }

            if (rotateZ) {
                _cachedTransform.Rotate(Vector3.forward, f);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.CompareTo("Player") == 0) {
            other.gameObject.GetComponent<Player>().onBonus(score);
        }

        if (destroyOnTake) {
            Destroy(gameObject);
        }
    }
}
