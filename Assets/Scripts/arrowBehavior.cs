using UnityEngine;
using System.Collections;

public class arrowBehavior : CustomBehaviour {

	// This arrow moves at a constant speed horizontally, but the vertical
	// speed is allowed to vary.  This simplifies the math somewhat, and makes
	// it easier to reason about to some degree -- arrows shot at targets twice
	// as far away take twice as long to get there -- at the cost of being
	// somewhat unrealistic at long ranges (they take off like friggin
	// missiles).
	
	float m_speed;
	Vector3 m_targetPoint;
	Vector3 m_velocity;
	float accuracy;

	public GameObject targetMarker;
	
	// force of gravity -- 9.8 is the "real" value, but setting this higher
	// makes the arrows arc up more and come down faster, looks kinda cool
	public float Gravity = 9.8f;
	
	public void ShootSelf(Vector3 myTarget, float speed) {
	
		accuracy = 4.0f;
		m_targetPoint = new Vector3 (myTarget.x + Random.Range(-accuracy,accuracy), myTarget.y , myTarget.z + Random.Range(-accuracy, accuracy));
		//m_targetPoint = myTarget;
		m_speed = speed;
		
		// the direction is a little arrow pointing at the target point
		Vector3 directionToTarget = (m_targetPoint - transform.position).normalized;
		Debug.DrawRay(transform.position, directionToTarget * 4, Color.red, 5); // show it for 5 seconds
		m_velocity = directionToTarget * m_speed;

		// computing the vertical velocity to make sure it stays in the air long enough to hit the target
		float timeOfFlight = Vector3.Distance(m_targetPoint, transform.position) / speed;
		// 100% ganked from http://gamedev.stackexchange.com/questions/17467/calculating-velocity-needed-to-hit-target-in-parabolic-arc
		// the internet will atrophy all our abilities to figure things out on our own
		m_velocity.y = 0.5f * Gravity * timeOfFlight + (m_targetPoint.y - transform.position.y)/timeOfFlight;

		var target = Instantiate(targetMarker, m_targetPoint, Quaternion.identity);
		Destroy(target, timeOfFlight);
		Destroy (this.gameObject, timeOfFlight);

	}

	// Update is called once per frame
	void Update () {

		// let's treat the xz motion (horizontal) totally separately from the
		// y (vertical), so we'll create two variables and set them separately
		Vector3 horizontalVelocity;
		Vector3 verticalVelocity;
		
		// the horizontal velocity is the same as it was when the arrow was
		// initially shot, we're not modeling atmospheric drag, because,
		// they're frikkin arrows yo
		horizontalVelocity = new Vector3(m_velocity.x, 0, m_velocity.z);
		Debug.DrawRay(transform.position, horizontalVelocity, Color.yellow);
		
		// the vertical velocity is where we subtract the gravity (you could
		// also do this by using a rigidbody, you just have to make sure the
		// gravity value used for Unity's physics is the same as the gravity
		// value used for the initial calculation)
		verticalVelocity = new Vector3(0, m_velocity.y, 0);
		verticalVelocity = verticalVelocity - (new Vector3(0, Gravity, 0) * Time.deltaTime);
		Debug.DrawRay(transform.position, verticalVelocity, Color.green);
		
		// the actual velocity is just the sum of the two components
		m_velocity = horizontalVelocity + verticalVelocity;
		Debug.DrawRay(transform.position, horizontalVelocity, Color.white);
		
		// here we could set the velocity on a rigidbody, or we can do MANUAL
		// PHYSICS and compute the position delta ourselves
		transform.position = transform.position + (m_velocity * Time.deltaTime);
		
		// make the arrow point along the velocity using one of the various
		// functions on Quaternion that do this sort of thing
		var rotation = Quaternion.LookRotation(m_velocity);
		// .. unfortunately because the janky ass cylinder points the wrong
		// way by default we have to add a special factor to this --
		// wouldn't be a problem with a correctly-made model
		transform.rotation = rotation * Quaternion.Euler(90, 0, 0);
		
		
	}
	
	void OnTriggerEnter (Collider other) {
		
		
		
	}
}
