﻿namespace GameJam;

public partial class Lord
{
	[Net] public BaseEntity PointingAt { get; set; } = null;
	[Net] public Vector3 PointingPosition { get; set; } = 0f;
	public List<BaseNPC> CurrentlyCommanding { get; set; } = new();
	public void SimulateCommanding()
	{
		if ( !Game.IsServer ) return;
		if ( Pointing )
		{
			PointingAt = FindBestPointedAt();


			DebugOverlay.Sphere( PointingPosition, 30f, Color.Red, Time.Delta );

			if ( Input.Pressed( InputButton.PrimaryAttack ) )
			{
				if ( PointingAt is BaseNPC npc )
				{
					if ( npc.Faction == Faction )
					{
						if ( CurrentlyCommanding.Contains( npc ) )
							RemoveFromCommanding( npc );
						else
							AddToCommanding( npc );
					}
				}
			}
		}
	}

	public void AddToCommanding( BaseNPC npc )
	{
		CurrentlyCommanding.Add( npc );
		npc.Kill();
	}

	public void RemoveFromCommanding( BaseNPC npc )
	{
		CurrentlyCommanding.Remove( npc );
	}

	internal BaseEntity FindBestPointedAt()
	{
		var preciseTrace = Trace.Ray( Camera.Position, Camera.Position + Camera.Rotation.Forward * 500f )
				.Size( 15f )
				.Ignore( this )
				.Run();

		if ( preciseTrace.Entity is BaseEntity preciseEntity )
		{
			PointingPosition = preciseEntity.Position;
			return preciseEntity;
		}

		var outerTrace = Trace.Ray( Camera.Position, Camera.Position + Camera.Rotation.Forward * 500f )
				.Size( 30f )
				.Ignore( this )
				.Run();

		if ( outerTrace.Entity is BaseEntity outerEntity )
		{
			PointingPosition = outerEntity.Position;
			return outerEntity;
		}

		var closestEntityToPrecise = Entity.All
			.OfType<BaseEntity>()
			.Where( x => x.Position.DistanceSquared( preciseTrace.HitPosition ) <= 900f )
			.OrderBy( x => x.Position.DistanceSquared( preciseTrace.HitPosition ) )
			.FirstOrDefault();

		if ( closestEntityToPrecise.IsValid() )
		{
			PointingPosition = closestEntityToPrecise.Position;
			return closestEntityToPrecise;
		}

		PointingPosition = preciseTrace.HitPosition;

		return null;
	}
}
