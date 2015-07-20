﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
	public float MoveSpeed = 10f;
	public float Gravity = 21f;
	public float TerminalVelocity = 20f;
	public float VerticalVelocity;
	public Vector3 MoveVector;
	public float JumpSpeed = 6f;

	private CharacterController CharacterController;

	private void Awake()
	{
		CharacterController = GetComponent<CharacterController>();
	}

	public void Move()
	{
		SnapAlignCharacterWithCamera();
		ProcessMotion();
	}

	private void ProcessMotion()
	{
		MoveVector = transform.TransformDirection(MoveVector);

		if (MoveVector.magnitude > 1)
			MoveVector = Vector3.Normalize(MoveVector);

		MoveVector *= MoveSpeed;

		MoveVector = new Vector3(MoveVector.x, VerticalVelocity, MoveVector.z);
		ApplyGravity();

		CharacterController.Move(MoveVector * Time.deltaTime);
	}

	private void SnapAlignCharacterWithCamera()
	{
		if (MoveVector.x != 0 || MoveVector.z != 0)
		{
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
		}
	}

	private void ApplyGravity()
	{
		if (MoveVector.y > -TerminalVelocity)
			MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);

		if (CharacterController.isGrounded && MoveVector.y < -1)
			MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
	}

	public void Jump()
	{
		if(CharacterController.isGrounded)
		{
			VerticalVelocity = JumpSpeed;
		}
	}

}
