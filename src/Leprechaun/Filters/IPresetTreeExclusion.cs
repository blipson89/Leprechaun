namespace Leprechaun.Filters
{
	public interface IPresetTreeExclusion
	{
		bool Evaluate(string itemPath);
		string Description { get; }
	}
}
