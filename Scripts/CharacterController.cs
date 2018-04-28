using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour {

	[System.Serializable]
	public class MoveSettings {
		public float forwardVel = 12;
		public float strafeVel = 12;
		public float rotateVel = 100;
		public float jumpVel = 25;
		public float distToGrounded = 0.1f;
		public LayerMask ground;
	}

	[System.Serializable]
	public class PhysSettings {
		public float downAccel = 0.75f;  
	}

	[System.Serializable]
	public class InputSettings  {
		public float inputDelay = 0.1f;
		public string FORWARD_AXIS = "Vertical";
		public string TURN_AXIS = "Horizontal";
		public string JUMP_AXIS = "Jump";
		public string STRAFE_AXIS = "Strafe";
	}

	[System.Serializable]
	public class TextSettings {
		public Text textRotacion;
		public Text textPosicion;
		public Text textLibre;
	}

	public MoveSettings moveSetting = new MoveSettings();
	public PhysSettings physSetting = new PhysSettings();
	public InputSettings inputSetting = new InputSettings ();
	public TextSettings textSetting = new TextSettings ();

	Vector3 velocity = Vector3.zero;
	Quaternion targetRotation;
	Rigidbody rBody;
	float forwardInput, turnInput,  jumpInput, strafeInput;

	public Quaternion TargetRotation {
		get { return targetRotation; }
	}

	bool Grounded() {
		return Physics.Raycast(transform.position, Vector3.down, moveSetting.distToGrounded, moveSetting.ground);
	}
		

	// Use this for initializatio n
	void Start () {
		targetRotation = transform.rotation;
		if (GetComponent<Rigidbody> ())
			rBody = GetComponent<Rigidbody> ();
		else
			Debug.LogError ("The character needs a rigid body.");

		forwardInput = turnInput = jumpInput = 0;
		setText ();

	}

	void GetInput() {
		forwardInput = Input.GetAxis (inputSetting.FORWARD_AXIS); //interpolated
		turnInput = Input.GetAxis (inputSetting.TURN_AXIS); //interpolated	
		jumpInput = Input.GetAxisRaw (inputSetting.JUMP_AXIS); //non-interpolated
		strafeInput = Input.GetAxis (inputSetting.STRAFE_AXIS); // interpolated
	}

	// Update is called once per frame
	void Update () {
		GetInput ();
		Turn();
		setText ();
	}

	void setText() {
		textSetting.textPosicion.text = "Posicion: " + transform.position.ToString();
		textSetting.textRotacion.text = "Rotacion: " + transform.rotation.ToString();
		textSetting.textLibre.text = "Grounded: " + Grounded ().ToString();
	}

	void FixedUpdate() {
		Run ();
		Strafe ();
		Jump ();
		rBody.velocity = transform.TransformDirection(velocity);
	}

	void Run() {
		if (Mathf.Abs(forwardInput) > inputSetting.inputDelay) {
			//move
			velocity.z = moveSetting.forwardVel * forwardInput;
		} else {
			//zero velocity
			velocity.z = 0;
		}
	}

	void Turn() {
		if (Mathf.Abs (turnInput) > inputSetting.inputDelay) {
			targetRotation *= Quaternion.AngleAxis (moveSetting.rotateVel * turnInput * Time.deltaTime, Vector3.up);
		}
		transform.rotation = targetRotation;
	}

	void Strafe() {
		if (Mathf.Abs (strafeInput) > inputSetting.inputDelay) {
			velocity.x = moveSetting.strafeVel * strafeInput;
		} else {
			velocity.x = 0;
		}
	}

	void Jump() {
		if (jumpInput > 0 && Grounded ()) {
			//jump
			velocity.y = moveSetting.jumpVel;
		} else if (jumpInput == 0 && Grounded ()) {
			//zero out our velocity.y
			velocity.y = 0;
		} else {
			//decrease velocity.y
			velocity.y -= physSetting.downAccel;
		}
	}

}
