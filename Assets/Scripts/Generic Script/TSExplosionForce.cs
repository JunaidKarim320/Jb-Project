using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSExplosionForce : MonoBehaviour {

	public float radius = 5.0F;
	public float power = 10.0F;
    [SerializeField] Transform powerRadius;

	void Start()
	{
		Vector3 explosionPos = powerRadius.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (hit.gameObject.tag != "Player")
            {
                if (rb != null)
                    rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
                
               // print("herte");
            }
			
		}
        Destroy(this.gameObject, 5f);
    }
}
