using Godot;
using Godot.NativeInterop;
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
	public float lerp_speed = 7.0f;

	private float default_head_y = 1.04f;
    private float currentSpeed = 0f;
    private float crouching_depth = -0.5f;

    [Export]
    private float bobFrequency = 2.0f;
	[Export]
	private float bobAmplitude = 0.08f;

	float t_bob = 0.0f;

    private Node3D head;
	private Camera3D headCamera;
	private CollisionShape3D crch_collision;
    private CollisionShape3D stand_collision;
	private RayCast3D player_cast;

    Vector3 direction = Vector3.Zero;
    public override void _Ready()
    {
        base._Ready();
		head = this.GetNode<Node3D>("Head");
		headCamera = head.GetNode<Camera3D>("HeadCamera");
        player_cast = this.GetNode<RayCast3D>("player_ray_cast");
		crch_collision = this.GetNode<CollisionShape3D>("crouching_collision_shape");
        stand_collision = this.GetNode<CollisionShape3D>("standing_collisin_shape");
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
		var fdelta = (float)delta;

        Vector3 velocity = Velocity;
        // Add the gravity.
        if (!IsOnFloor())
			velocity.Y -= gravity * fdelta;

		if (Input.IsActionPressed("Crouch"))
		{
			var a = default_head_y + crouching_depth;
			currentSpeed = CrouchSpeed;
			head.Position = new Vector3(head.Position.X, Mathf.Lerp(head.Position.Y,a, fdelta*lerp_speed), head.Position.Z);
			stand_collision.Disabled = true;
			crch_collision.Disabled = false;
		}
		else if(!player_cast.IsColliding())
		{
            stand_collision.Disabled = false;
            crch_collision.Disabled = true;
            head.Position = new Vector3(head.Position.X, 
				Mathf.Lerp(head.Position.Y, default_head_y, fdelta * lerp_speed), head.Position.Z);

            if (Input.IsActionPressed("Sprint"))
			{
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
        

        direction = direction.Lerp((Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized(), fdelta * lerp_speed); 

		if (direction != Vector3.Zero)
		{
			if (!IsOnFloor()) 
			{
                velocity.X = Mathf.Lerp(velocity.X,direction.X * (currentSpeed/2f), fdelta * 2.0f);
                velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * (currentSpeed/2f), fdelta * 2.0f);
            } 
			else
			{
				velocity.X = direction.X * currentSpeed;
				velocity.Z = direction.Z * currentSpeed;
			}
		}
		else
		{
			velocity.Z = 0.0f; //Mathf.MoveToward(Velocity.Z, 0, Speed);
			velocity.X = 0.0f; //Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;


		var floatIsOnFloor = IsOnFloor() ? 1f : 0f;

        t_bob += fdelta * velocity.Length() * floatIsOnFloor;
		headCamera.Position = HeadBob(t_bob);

        MoveAndSlide();
	}

	private Vector3 HeadBob(float time)
	{
		var pos = Vector3.Zero;

		pos.Y = MathF.Sin(time * bobFrequency) * bobAmplitude;
		pos.X = MathF.Cos(bobFrequency / 2) * bobAmplitude;
		return pos;

	}
}
