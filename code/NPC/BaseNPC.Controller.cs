﻿namespace GameJam;

public partial class BaseNPC
{
	[Net] public float WishSpeed { get; private set; } = 0f;
	[Net] public Vector3 Direction { get; set; } = Vector3.Zero;
	public Vector3 WishVelocity => Direction.Normal * WishSpeed;
	public Rotation WishRotation => Rotation.LookAt( Direction, Vector3.Up );

	public virtual void ComputeMotion()
	{
		if ( Direction != Vector3.Zero )
			WishSpeed = WalkSpeed;
		else
			WishSpeed = 0f;

		Velocity = Vector3.Lerp( Velocity, WishVelocity, Time.Delta * 5f )
			.WithZ( Velocity.z );

		var pushOffset = Vector3.Zero;

		foreach ( var toucher in touchingEntities )
		{
			if ( toucher == null || !toucher.IsValid )
				continue;

			var direction = (Position - toucher.Position).WithZ(0).Normal;
			var distance = Position.DistanceSquared( toucher.Position );
			var maxDistance = 500f;

			pushOffset = direction * Math.Max( maxDistance - distance, 0f ) * Time.Delta * 3f;
		}

		Velocity += pushOffset;
		Position += Velocity * Time.Delta;
	}

	internal List<BaseEntity> touchingEntities = new();

	public override void StartTouch( Entity other )
	{
		base.Touch( other );

		if ( !Game.IsServer ) return;

		if ( other is BaseEntity toucher && other.Tags.Has( "Pushable" ) )
			touchingEntities.Add( toucher );
	}


	public override void EndTouch( Entity other )
	{
		base.Touch( other );

		if ( !Game.IsServer ) return;

		if ( other is BaseEntity toucher && touchingEntities.Contains( toucher ) )
			touchingEntities.Remove( toucher );
	}
}

