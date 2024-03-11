using Godot;
using System;

public partial class Movement : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
    [Export]
    public float SprintSpeed = 10.0f;
    [Export]
    public float CrouchSpeed = 2.3f;
    
    [Export]
	public float JumpVelocity = 5.0f;
	[Export]
	public float MouseSensitivity = 0.3f;
	[Export]
	public float gravity = 9.8f;
	[Export]
	public float lerp_speed = 10.0f;

    private float currentSpeed = 0f;
    private float crouching_depth = -0.7f;


    private Node3D head;
	Vector3 direction = Vector3.Zero;
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

	private Vector3 Lerp(Vector3 from, Vector3 to, float weight)
	{
        float retX = Mathf.Lerp(from.X, to.X, weight);
        float retY = Mathf.Lerp(from.Y, to.Y, weight);
        float retZ = Mathf.Lerp(from.Z, to.Z, weight);
        return new Vector3(retX, retY,retZ);
    }

    public override void _PhysicsProcess(double delta)
	{
		var fdelta = (float)delta;

		Vector3 velocity = Velocity;
        head.Position = new Vector3(head.Position.X, 1.95f, head.Position.Z);
        // Add the gravity.
        if (!IsOnFloor())
			velocity.Y -= gravity * fdelta;


		if (Input.IsActionPressed("Crouch"))
		{
			var a = 1.95f + crouching_depth;
			currentSpeed = CrouchSpeed;
			head.Position = new Vector3(head.Position.X, Mathf.Lerp(head.Position.Y,a, fdelta*lerp_speed), head.Position.Z);
		}
		else
		{
			if (Input.IsActionPressed("Sprint"))
			{
				head.Position = new Vector3(head.Position.X, 1.95f, head.Position.Z);
				currentSpeed = SprintSpeed;
			}
			else
			{
				currentSpeed = Speed;
			}

			// Handle Jump.
			if (Input.IsActionJustPressed("Jump") && IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
        

        direction = Lerp(direction,(Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized(), fdelta * lerp_speed); 

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * currentSpeed;
			velocity.Z = direction.Z * currentSpeed;
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
