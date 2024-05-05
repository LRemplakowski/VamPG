using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Kalagaan
{
	namespace HairDesignerExtension
	{
		public class RandomMotion : MonoBehaviour
		{

			public Transform m_reference;
			public Vector3 m_area;

			public Vector3 m_target;
			public float m_speed = 1f;


			void NewTarget()
			{
				m_target.x = Random.value * m_area.x * 2f - m_area.x;
				m_target.y = Random.value * m_area.y * 2f - m_area.y;
				m_target.z = Random.value * m_area.z * 2f - m_area.z;

				if (m_reference != null)
				{
					m_target += m_reference.position;
				}
			}

			void Start()
			{
				NewTarget();
			}
			
			void Update()
			{

				Vector3 delta = (m_target - transform.position);

				float speed = m_speed;
				


				if (delta.magnitude<= Time.deltaTime * speed)
				{
					NewTarget();
				}
				else
				{
					transform.position += delta.normalized * Time.deltaTime * speed;
				}

				transform.Rotate(Vector3.up, Time.deltaTime * 50f,Space.World);

			}
		}
	}
}