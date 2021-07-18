using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

		private Animator anim;
		private CharacterController controller;

		public float speed;
		public float turnSpeed;
		private Vector3 moveDirection = Vector3.zero;
		public float gravity;

		void Start () {
			controller = GetComponent <CharacterController>();
			anim = gameObject.GetComponentInChildren<Animator>();
		}

		void Update (){
			// if (Input.GetKey ("w")) {
			// 	anim.SetInteger ("AnimationPar", 1);
			// }  else {
			// 	anim.SetInteger ("AnimationPar", 0);
			// }
			anim.SetInteger ("AnimationPar", 1);

			if(controller.isGrounded){
				moveDirection = transform.forward * speed; //* Input.GetAxis("Vertical")
			}

			float turn = Input.GetAxis("Horizontal");
			transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
			controller.Move(moveDirection * Time.deltaTime);
			moveDirection.y -= gravity * Time.deltaTime;
		}
}
