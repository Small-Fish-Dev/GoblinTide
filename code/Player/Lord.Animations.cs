﻿namespace GameJam;

public partial class Lord
{
	public void SimulateAnimations()
	{
		// note(gio): made the animations a bit more speedy - nicer to look at imo
		SetAnimParameter( "move_x", Velocity.Dot( Rotation.Forward ) * 1.5f );
		SetAnimParameter( "move_y", Velocity.Dot( Rotation.Right ) * 1.5f );
	}
}
