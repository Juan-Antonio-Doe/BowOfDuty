using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    public ObjectPool pool { get; set; }
    [field: SerializeField] public bool autoDisable { get; set; } = false;
    [field: SerializeField] public float timeToDisable { get; set; } = 1f;

    private void OnEnable()
    {
        if(autoDisable == true)
        {
            StartCoroutine(CRT_Disable());
        }
    }

    IEnumerator CRT_Disable()
    {
        yield return new WaitForSeconds(timeToDisable);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        pool?.AddToPool(this);
    }
}
