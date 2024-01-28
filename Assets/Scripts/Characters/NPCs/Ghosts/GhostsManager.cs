using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostsManager : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private PlayerManager player { get; set; }
    public PlayerManager Player { get => player; }

    [field: Header("Ghosts settings")]
    [field: SerializeField] public Transform spawnPosition { get; set; }
    [field: SerializeField] public int maxGhosts { get; set; } = 20;
    [field: SerializeField] private Poolable ghostPrefab { get; set; }
    private ObjectPool ghostPool { get; set; }

    private Coroutine reactivateGhostsCo { get; set; }

    IEnumerator Start() {
        ghostPool = ObjectPool.CreatePool(ghostPrefab, maxGhosts, $"__GhostsPool__", transform.GetChild(0).transform);

        yield return new WaitForSeconds(1f);

        // Enable all ghosts on pool list.
        for (int i = 0; i < ghostPool.availableObjects.Count; i++) {
            Poolable ghostTemp = ghostPool.availableObjects[i];
            if (ghostTemp.gameObject.activeInHierarchy)
                continue;

            ghostTemp.transform.position = spawnPosition.position;
            ghostTemp.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void ReactivateGhosts() {
        if (reactivateGhostsCo == null) 
            StartCoroutine(ReactivateGhostsCo());
    }

    IEnumerator ReactivateGhostsCo() {
        for (int i = 0; i < 20; i++) {
            ghostPool?.GetFromPool();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
