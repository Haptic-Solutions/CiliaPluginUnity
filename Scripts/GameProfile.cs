using System.Collections.Generic;

public class Options
{
	public Options()
	{

	}
}
public class Effect
{
	public int EffectID;
	public double LoopTime;
	public double FrameDuration;
	public List<List<uint>> EffectColors;
	public Options Options;
	public Effect()
	{
		EffectColors = new List<List<uint>>();
		Options = new Options();
	}
}
public class LoadProfile
{
	public string ProfileName;
	public uint DefaultGroupID;
	public List<string> Groups;
	public List<List<object>> Scents;
	public Effect Effect;

	public LoadProfile()
	{
		Groups = new List<string>();
		Scents = new List<List<object>>();
		Effect = new Effect();
	}
}
public class LoadProfileJSON
{
	public LoadProfile LoadProfile;

	public LoadProfileJSON()
	{
		LoadProfile = new LoadProfile();
	}
}





