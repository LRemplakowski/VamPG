using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Characters.Formation
{
    public class FormationElement : MonoBehaviour
    {
        [SerializeField]
        private float circleRadius = .5f;
        private const float scaleAdjustment = 0.4f;
        [SerializeField]
        private SpriteRenderer circleRenderer;
        [SerializeField]
        private Color color;

        internal Creature character;

        private void Start()
        {
            if (circleRenderer == null)
                circleRenderer = GetComponentInChildren<SpriteRenderer>(true);
            if (circleRenderer != null)
                circleRenderer.transform.localScale = new Vector3(circleRadius * scaleAdjustment, circleRadius * scaleAdjustment, 1f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, circleRadius);
        }

        internal void MoveOwnerToElement()
        {
            character.ClearAllActions();
            if (character != null)
                character.Move(this.transform.position);
            else
                Debug.LogError("Formation element with no character assigned! " + this.gameObject);
        }    
    }
}