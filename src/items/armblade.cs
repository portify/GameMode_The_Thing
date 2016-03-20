//////////
// item //
//////////
datablock ItemData(armbladeItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "Add-Ons/Weapon_Sword/sword.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Sword";
	iconName = "Add-Ons/Weapon_Sword/icon_sword";
	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	 // Dynamic properties defined by the scripts
	image = armbladeImage;
	canDrop = true;
};
////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(armbladeImage)
{
	// Basic Item properties
	shapeFile = "Add-Ons/Weapon_Sword/sword.dts";
	emap = true;

	// Specify mount point & offset for 3rd person, and eye offset
	// for first person rendering.
	mountPoint = 0;
	offset = "0 0 0";

	// When firing from a point offset from the eye, muzzle correction
	// will adjust the muzzle vector to point to the eye LOS point.
	// Since this weapon doesn't actually fire from the muzzle point,
	// we need to turn this off.  
	correctMuzzleVector = false;

	eyeOffset = "0.7 1.2 -0.25";

	// Add the WeaponImage namespace as a parent, WeaponImage namespace
	// provides some hooks into the inventory system.
	className = "WeaponImage";

	// Projectile && Ammo.
	item = armbladeItem;
	ammo = " ";
	projectile = swordProjectile;
	projectileType = Projectile;

	//melee particles shoot from eye node for consistancy
	melee = true;
	doRetraction = false;
	//raise your arm up or not
	armReady = true;

	//casing = " ";
	doColorShift = true;
	colorShiftColor = "0.8 0.2 0.2 1.000";

	// Images have a state system which controls how the animations
	// are run, which sounds are played, script callbacks, etc. This
	// state system is downloaded to the client so that clients can
	// predict state changes and animate accordingly.  The following
	// system supports basic ready->fire->reload transitions as
	// well as a no-ammo->dryfire idle state.

	// Initial start up state
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.5;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = swordDrawSound;

	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;

	stateName[2]			= "PreFire";
	stateScript[2]                  = "onPreFire";
	stateAllowImageChange[2]        = false;
	stateTimeoutValue[2]            = 0.1;
	stateTransitionOnTimeout[2]     = "Fire";

	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateTimeoutValue[3]            = 0.2;
	stateFire[3]                    = true;
	stateAllowImageChange[3]        = false;
	stateSequence[3]                = "Fire";
	stateScript[3]                  = "onFire";
	stateWaitForTimeout[3]		= true;

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "Fire";

	
	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateTimeoutValue[5]            = 0.2;
	stateAllowImageChange[5]        = false;
	stateWaitForTimeout[5]		= true;
	stateSequence[5]                = "StopFire";
	stateScript[5]                  = "onStopFire";

	raycastEnabled = 1;
	raycastRange = 4;
	raycastFromEye = true;
	directDamage = 25;
	directDamageType = $DamageType::Sword;
	raycastHitExplosion = hammerProjectile;
};

function armbladeImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, armattack);
}

function armbladeImage::onStopFire(%this, %obj, %slot)
{	
	%obj.playthread(2, root);
}

function KnifeImage::damage(%this, %obj, %col, %position, %normal)
{
	%damage = %this.directDamage;
	if (%col.getType() & $TypeMasks::playerObjectType)
	{
		%dot = vectorDot(%obj.getForwardVector(), %col.getForwardVector());
		if (%dot > 0)
			%damage = 100;
	}
	%col.damage(%obj, %position, %damage, %damageType);
}