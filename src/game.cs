if (!isObject(GameRoundCleanup))
	new SimSet(GameRoundCleanup);
package TheThingGamePackage
{
	function Armor::onDisabled(%this, %obj)
	{
		if (isObject(%obj.client))
		{
			%obj.client.stopViewingInventory();
			if (%obj.client.inDefaultGame() && isObject(GameRoundCleanup))
			{
				%obj.inhibitRemoveBody = true;
				GameRoundCleanup.add(%obj);
			}
		}

		Parent::onDisabled(%this, %obj);
	}

	function Player::removeBody(%this)
	{
		if (!%this.inhibitRemoveBody)
			Parent::removeBody(%this);
	}

	function GameConnection::onDeath(%client, %sourceObject, %sourceClient, %damageType, %damLoc)
	{
		if (%client.miniGame != $DefaultMiniGame)
			return Parent::onDeath(%client, %sourceObject, %sourceClient, %damageType, %damLoc);

		if (%sourceObject.sourceObject.isBot)
		{
			%sourceClientIsBot = 1;
			%sourceClient = %sourceObject.sourceObject;
		}

		%player = %client.player;

		if (isObject(%player))
		{
			%player.setShapeName("", 8564862);
			if (isObject(%player.tempBrick))
			{
				%player.tempBrick.delete();
				%player.tempBrick = 0;
			}
			%player.isBody = true;
			%player.client = 0;
		}
		else
			warn("WARNING: No player object in GameConnection::onDeath() for client '" @ %client @ "'");

		if (isObject(%client.camera) || isObject(%player))
		{ // this part of the code isn't accurate
			if (%client.getControlObject() != %client.camera)
			{
				%client.camera.setMode("Corpse", %player);
				%client.camera.setControlObject(0);
				%client.setControlObject(%client.camera);
			}
		}

		%client.player = 0;
		%client.corpse = %player;

		if ($Damage::Direct[$damageType] && getSimTime() - %player.lastDirectDamageTime < 100 && %player.lastDirectDamageType !$= "")
			%damageType = %player.lastDirectDamageType;

		if (%damageType == $DamageType::Impact && isObject(%player.lastPusher) && getSimTime() - %player.lastPushTime <= 1000)
			%sourceClient = %player.lastPusher;

		%message = '%2 killed %1';

		if (%sourceClient == %client || %sourceClient == 0)
		{
			%message = $DeathMessage_Suicide[%damageType];
			%client.corpse.suicide = true;
		}
		else
			%message = $DeathMessage_Murder[%damageType];

		// removed mid-air kills code here
		// removed mini-game kill points here

		%clientName = %client.getPlayerName();

		if (isObject(%sourceClient))
			%sourceClientName = %sourceClient.getPlayerName();
		else if (isObject(%sourceObject.sourceObject) && %sourceObject.sourceObject.getClassName() $= "AIPlayer")
			%sourceClientName = %sourceObject.sourceObject.name;
		else
			%sourceClientName = "";

		echo("\c4" SPC %sourceClientName SPC "killed" SPC %clientName);
		// removed mini-game checks here
		// removed death message print here
		// removed %message and %sourceClientName arguments
		messageClient(%client, 'MsgYourDeath', '', %clientName, '', '');//%client.miniGame.respawnTime);

		commandToClient(%client, 'CenterPrint', '', 1);
		%client.miniGame.checkLastManStanding();
	}

	function MiniGameSO::reset(%this, %client)
	{
		Parent::reset(%this, %client);

		if (%this.owner != 0 || %this.lastResetTime != getSimTime())
			return;

		if (%this.numMembers == 0)
			return;

		if (isObject(GameRoundCleanup))
			GameRoundCleanup.deleteAll();

		%thingClient = %this.member[getRandom(%this.numMembers - 1)];

		%thingPlayer = %thingClient.player;
		%thingPlayer.changeDataBlock(TheThingData);

		for (%i = 0; %i < %this.numMembers; %i++)
		{
			%member = %this.member[%i];
			%player = %member.player;

			if (%member != %thingClient)
			{
				messageClient(%member, '', "\c5You are a human this round. Eliminate The Thing to win!");
				// Give starter weapons here

				%player.addTool(Colt1911Item);
				%player.addTool(MagazineItem_45ACP_x7);
				%player.addTool(MagazineItem_45ACP_x7);
			}
			else
			{
				messageClient(%member, '', "\c0You are be The Thing this round! Absorb all the humans to win!");
				%player.addTool(Colt1911Item);
			}
		}

		%group = nameToID("BrickGroup_888888");

		%maxItemPool = -1;
		%maxAmmoPool = -1;

		%itemPool[%maxItemPool++] = "0";
		%itemPool[%maxItemPool++] = "Colt1911Item";
		%itemPool[%maxItemPool++] = "M1GarandItem";
		%itemPool[%maxItemPool++] = "ColtWalkerItem";
		%itemPool[%maxItemPool++] = "Remington870Item";
		%itemPool[%maxItemPool++] = "RevolverItem";
		%itemPool[%maxItemPool++] = "ThompsonItem";
		%itemPool[%maxItemPool++] = "MicroUziItem";

		%ammoPool[%maxAmmoPool++] = "0";
		%ammoPool[%maxAmmoPool++] = "MagazineItem_45ACP_x7";
		%ammoPool[%maxAmmoPool++] = "Bullet357PackItem";
		%ammoPool[%maxAmmoPool++] = "BulletBuckshotPackItem";
		%ammoPool[%maxAmmoPool++] = "MagazineItem_45ACP_x20_SMG";
		%ammoPool[%maxAmmoPool++] = "MagazineItem_3006_x8";
		%ammoPool[%maxAmmoPool++] = "MagazineItem_MicroUzi";
		%ammoPool[%maxAmmoPool++] = "MagazineItem_MicroUziExtended";

		for (%i = 0; %i < %group.NTObjectCount["_itemSpawn"]; %i++)
		{
			%brick = %group.NTObject["_itemSpawn", %i];
			%brick.setItem(%itemPool[getRandom(%maxItemPool)]);
		}

		for (%i = 0; %i < %group.NTObjectCount["_ammoSpawn"]; %i++)
		{
			%brick = %group.NTObject["_ammoSpawn", %i];
			%brick.setItem(%ammoPool[getRandom(%maxAmmoPool)]);
		}
	}

	function MiniGameSO::checkLastManStanding(%this)
	{
		if (%this.owner != 0 || isEventPending(%this.resetSchedule))
			return Parent::checkLastManStanding(%this);

		%livingThing = false;
		%livingHuman = false;

		for (%i = 0; %i < %this.numMembers; %i++)
		{
			%member = %this.member[%i];

			if (isObject(%member.player))
			{
				if (%member.player.getDataBlock() == nameToID("TheThingData"))
					%livingThing = true;
				else
					%livingHuman = true;
			}
		}

		if (!%livingHuman)
		{
			if (!%livingThing)
				%this.messageAll('', "\c5All humans have perished. The Thing may be gone, but it has still succeeded.");
			else
				%this.messageAll('', "\c5All humans have perished. The Thing has succeeded.");
		}
		else if (!%livingThing)
			%this.messageAll('', "\c5The Thing has perished. The humans are successful.");
		else
			return 0;

		%this.messageAll('', "\c5Resetting the game in five seconds.");
		%this.scheduleReset(5000);

		return 0;
	}
};

activatePackage("TheThingGamePackage");
