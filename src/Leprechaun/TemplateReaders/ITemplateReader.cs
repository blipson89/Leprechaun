using Leprechaun.Model;
using Rainbow.Storage;

namespace Leprechaun.TemplateReaders
{
	public interface ITemplateReader
	{
		TemplateInfo[] GetTemplates(params TreeRoot[] rootPaths);
	}
}