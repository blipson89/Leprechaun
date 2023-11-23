using System;
using System.Diagnostics;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{FullTypeName} ({Id})")]
	public class RenderingCodeGenerationMetadata : BaseCodeGenerationMetadata
	{
		
		private readonly RenderingInfo _renderingInfo;

		public RenderingCodeGenerationMetadata(RenderingInfo renderingInfo, string fullTypeName, string rootNamespace):
			base(fullTypeName, rootNamespace)
		{
			_renderingInfo = renderingInfo;
		}
		public virtual Guid Id => _renderingInfo.Id;

		public virtual string Name => _renderingInfo.Name;
	}
}