namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public abstract class AbstractImageRenderer : AbstractRenderer
	{
		protected IMap2D<IColor> _image;

		public IMap2D<IColor> Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
			}
		}
	}
}
