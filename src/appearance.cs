function GameConnection::getAppearance(%this)
{
	%list = %list TAB %this.faceName;
	%list = %list TAB %this.decalName;
	%list = %list TAB %this.lleg;
	%list = %list TAB %this.secondPack;
	%list = %list TAB %this.pack;
	%list = %list TAB %this.rarmColor;
	%list = %list TAB %this.chest;
	%list = %list TAB %this.hipColor;
	%list = %list TAB %this.hat;
	%list = %list TAB %this.hatColor;
	%list = %list TAB %this.accentColor;
	%list = %list TAB %this.lHand;
	%list = %list TAB %this.larmColor;
	%list = %list TAB %this.chestColor;
	%list = %list TAB %this.packColor;
	%list = %list TAB %this.secondPackColor;
	%list = %list TAB %this.rleg;
	%list = %list TAB %this.rHand;
	%list = %list TAB %this.rhandColor;
	%list = %list TAB %this.lArm;
	%list = %list TAB %this.rlegColor;
	%list = %list TAB %this.headColor;
	%list = %list TAB %this.accent;
	%list = %list TAB %this.hip;
	%list = %list TAB %this.rArm;
	%list = %list TAB %this.lhandColor;
	%list = %list TAB %this.llegColor;
	return %list;
}

function GameConnection::setAppearance(%this, %list)
{
	talk(%list);
	// %this.setFaceName(getField(%list, 0));
	// %this.setDecalName(getField(%list, 1));
	// %this.unHideNode(getField(%list, 2));
	// %this.unHideNode(getField(%list, 3));
	// %this.unHideNode(getField(%list, 4));
	// %this.rarmColor = getField(%list, 5));
	// %this.unHideNode(getField(%list, 6));
	// %this.hipColor = getField(%list, 7));
	// %this.unHideNode(getField(%list, 8));
	// %this.hatColor = getField(%list, 9));
	// %this.accentColor = getField(%list, 10));
	// %this.unHideNode(getField(%list, 11));
	// %this.larmColor = getField(%list, 12));
	// %this.chestColor = getField(%list, 13));
	// %this.packColor = getField(%list, 14));
	// %this.secondPackColor = getField(%list, 15));
	// %this.unHideNode(getField(%list, 16));
	// %this.unHideNode(getField(%list, 17));
	// %this.rhandColor = getField(%list, 18));
	// %this.unHideNode(getField(%list, 19));
	// %this.rlegColor = getField(%list, 20));
	// %this.headColor = getField(%list, 21));
	// %this.unHideNode(getField(%list, 22));
	// %this.unHideNode(getField(%list, 23));
	// %this.unHideNode(getField(%list, 24));
	// %this.lhandColor = getField(%list, 25));
	// %this.llegColor = getField(%list, 26));
}