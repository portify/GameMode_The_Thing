datablock PlayerData(OrdinaryPlayerData : PlayerStandardArmor)
{
	uiName = "Ordinary Player";
	canJet = false;
	firstPersonOnly = true;
	maxTools = 7;
	mass = 110;
};

datablock PlayerData(TheThingData : PlayerStandardArmor)
{
	uiName = "The Thing";
	canJet = false;
	maxDamage = 75;
	maxTools = 3;
	mass = 110;
};

function TheThingData::onTrigger(%this, %obj, %slot, %state)
{
	Parent::onTrigger(%this, %obj, %slot, %state);
	if(%slot == 4 && %state)
	{
		%start = %obj.getEyePoint();
		%end = vectorAdd(%start, vectorScale(%obj.getEyeVector(), 6));

		%mask = $TypeMasks::All ^ ($TypeMasks::FxBrickAlwaysObjectType | $TypeMasks::StaticShapeObjectType);
		%ray = containerRayCast(%start, %end, %mask, %obj);
		if (%ray)
			%pos = getWords(%ray, 1, 3);
		else
			%pos = %b;
		initContainerRadiusSearch(%pos, 0.2,
			$TypeMasks::playerObjectType | $TypeMasks::CorpseObjectType);

		while (isObject(%col = containerSearchNext()))
		{
			if (%col.isBody)
			{
				%corpse = %col;
				break;
			}
		}
		if (isObject(%corpse) && vectorDist(%corpse.getPosition(), %pos) < 2)
		{
			if (!%corpse.absorbed)
			{
				//TODO: Corpse DNA pool AKA people the changeling can disguise as
				//Stuff
				%obj.client.setAppearance(%corpse.appearance);
				%obj.setShapeName(%corpse.PlayerName, 8564862);
				messageClient(%obj.client, '', "\c2You have succesfully absorbed the corpse of" SPC %corpse.PlayerName);

				%corpse.absorbed = true;

				%corpse.hideNode("ALL");
				%corpse.unHideNode("headSkin");
				%corpse.unHideNode("femchest");
				%corpse.unHideNode("rarmslim");
				%corpse.unHideNode("larmslim");
				%corpse.unHideNode("lhand");
				%corpse.unHideNode("rhand");
				%corpse.unHideNode("pants");
				%corpse.unHideNode("rshoe");
				%corpse.unHideNode("lshoe");
				%corpse.setFaceName("ASCIITerror");
				%corpse.setDecalName("");
				%corpse.setNodeColor("ALL", "0 0 0 1");
			}
			else
			{
				if (isObject(%obj.client))
					messageClient(%obj.client, '', "The corpse is already absorbed!");
			}
		}
	}
}


package TheThingPlayerPackage
{
	function serverCmdLight(%this) //Another "interact" key for inventory stuff and other things
	{
		%player = %this.player;
		if (%player.getMountedImage(0))
		{
			for (%i = 0; %i < 4; %i++)
			{
				%image = %player.getMountedImage(%i);

				if (isObject(%image) && %image.onLight(%player, %i))
				{
					parent::serverCmdLight();
					return;
				}
			}
		}
		if (getSimTime() - %this.lastLightClick < 100) //a limit so server cannot be lagged out
			return;

		%this.lastLightClick = getSimTime();
		// %start = %player.getEyePoint();
		// %end = vectorAdd(%start, vectorScale(%player.getEyeVector(), 6));

		// %mask = $TypeMasks::All ^ ($TypeMasks::FxBrickAlwaysObjectType | $TypeMasks::StaticShapeObjectType);
		// %ray = containerRayCast(%start, %end, %mask, %player);
		// //Corpse looting/planting
		// if (%ray)
		// 	%pos = getWords(%ray, 1, 3);
		// else
		// 	%pos = %b;
		// initContainerRadiusSearch(%pos, 0.2,
		// 	$TypeMasks::playerObjectType | $TypeMasks::CorpseObjectType);

		// while (isObject(%col = containerSearchNext()))
		// {
		// 	if (%col.isBody)
		// 	{
		// 		%corpse = %col;
		// 		break;
		// 	}
		// }
		// if (isObject(%corpse) && vectorDist(%corpse.getPosition(), %pos) < 2)
		// {
		// 	%this.startViewingInventory(%corpse);
		// 	%player.playThread(2, "activate2");
		// }
	}
};

activatePackage("TheThingPlayerPackage");