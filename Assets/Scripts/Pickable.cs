using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public bool picked;

    public void bePicked(Transform newParent)
    {
        picked = true;
        StartCoroutine(HandlePick(newParent));
    }
	
    IEnumerator HandlePick(Transform newParent)
    {
        yield return new WaitForSeconds(1.5f);
        transform.parent = newParent;
        transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
