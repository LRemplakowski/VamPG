using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CoverDetector : MonoBehaviour
{
    public delegate void OnLookForCoverFinished(Dictionary<GridElement, List<Cover>> positionsNearCover);
    public OnLookForCoverFinished onLookForCoverFinished;

    private Dictionary<GridElement, List<Cover>> elementsNearCover = new Dictionary<GridElement, List<Cover>>();
    private List<Collider> coverSources = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        Cover c = other.GetComponent<Cover>();
        if (!c)
            return;
        if (c.GetCoverQuality() != CoverQuality.None)
        {
            coverSources.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (coverSources.Contains(other))
        {
            coverSources.Remove(other);
        }
    }

    public bool GetIsNearCover()
    {
        return coverSources.Count > 0;
    }

    public void StartLookingForCover(List<GridElement> gridElementsInRange)
    {
        elementsNearCover = new Dictionary<GridElement, List<Cover>>();
        StartCoroutine(LookForCover(gridElementsInRange));
    }

    private IEnumerator LookForCover(List<GridElement> gridElements)
    {
        foreach (GridElement g in gridElements)
        {
            transform.position = g.transform.position;
            yield return new WaitForFixedUpdate();
            if (GetIsNearCover())
            {
                List <Cover> coversNearGridPosition = new List<Cover>();
                foreach (Collider c in coverSources)
                {
                    coversNearGridPosition.Add(c.GetComponent<Cover>());
                }
                elementsNearCover.Add(g, coversNearGridPosition);
            }
        }
        if (onLookForCoverFinished != null)
            onLookForCoverFinished.Invoke(elementsNearCover);
        StopCoroutine(LookForCover(gridElements));
    }
}
