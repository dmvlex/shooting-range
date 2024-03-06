using Godot;
using System;

public partial class Movement : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	[Export]
	public float MouseSensitivity = 0.3f;
	[Export]
	public float gravity = 9.8f;
	private Node3D head;
	
	public override void _Ready()
    {
        base._Ready();
		head = this.GetNode<Node3D>("Head");
		Input.MouseMode = Input.MouseModeEnum.Captured;
    }

   public override void _Input(InputEvent @event)
    {
        base._Input(@event);

		//rotate the head
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			Vector3 rotDeg = RotationDegrees;
			rotDeg.Y -= eventMouseMotion.Relative.X * MouseSensitivity;
			RotationDegrees = rotDeg;

			rotDeg = head.RotationDegrees;
			rotDeg.X -= eventMouseMotion.Relative.Y * MouseSensitivity;
			rotDeg.X = Mathf.Clamp(rotDeg.X,-89,89);

			head.RotationDegrees = rotDeg;
		}
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.Z = 0.0f; //Mathf.MoveToward(Velocity.Z, 0, Speed);
			velocity.X = 0.0f; //Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
