using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

namespace CloudberryKingdomMonoWindows
{
	/*public class Video
	{
		public TimeSpan Duration;
	}*/

	public class VideoPlayer : IDisposable
	{
		public bool IsLooped;

		public float Volume;

		public VideoPlayer()
		{
		}

		public void Play(Video video)
		{
		}

		public Texture2D GetTexture()
		{
			return null;
		}

		#region IDisposable implementation

		public void Dispose ()
		{
		}

		#endregion
	}
}

