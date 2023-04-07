﻿namespace GameJam;

[Prefab]
public partial class ShackComponent : StructureComponent
{
	public override void Initialize() 
	{
		GameMgr.GoblinPerSecond += 1d / 60d;
	}

	public override void OnDestroy()
	{
		GameMgr.GoblinPerSecond -= 1d / 60d;
	}

	TimeUntil nextGoblin = 5f;

	[Event.Tick.Server]
	public void GenerateGoblin()
	{
		if ( nextGoblin )
		{
			nextGoblin = 60f;

			if ( GameMgr.GoblinArmy.Count() < GameMgr.MaxArmySize )
			{
				var gob = BaseNPC.FromPrefab( "prefabs/npcs/goblin.prefab" );
				if ( gob.IsValid() )
				{
					gob.Position = Entity.Position + Entity.Rotation.Backward * 100f;
					GameMgr.GoblinArmy.Add( gob );
				}
			}
		}
	}
}