using System;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//using DirectShowLib;

namespace SeeSharp.Xna.Video
{
    /// <summary>
    /// Describes the state of a video player
    /// </summary>
    public enum VideoState
    {
        Playing,
        Paused,
        Stopped
    }

    /// <summary>
    /// Enables Video Playback in Microsoft XNA
    /// </summary>
    public class VideoPlayer : ISampleGrabberCB, IDisposable
    {
        #region Media Type GUIDs
        private Guid MEDIATYPE_Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
        private Guid MEDIASUBTYPE_RGB24 = new Guid(0xe436eb7d, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);
        private Guid FORMAT_VideoInfo = new Guid(0x05589f80, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);
        #endregion

        #region Private Fields
        /// <summary>
        /// The Main FilterGraph Com Object
        /// </summary> 
        private FilterGraph fg = null;

        /// <summary>
        /// The GraphBuilder interface ref
        /// </summary>
        private IGraphBuilder gb = null;

        /// <summary>
        /// The MediaControl interface ref
        /// </summary>
        private IMediaControl mc = null;

        /// <summary>
        /// The MediaEvent interface ref
        /// </summary>
        private IMediaEventEx me = null;

        /// <summary>
        /// The MediaPosition interface ref
        /// </summary>
        private IMediaPosition mp = null;

        /// <summary>
        /// The MediaSeeking interface ref
        /// </summary>
        private IMediaSeeking ms = null;

        /// <summary>
        /// Thread used to update the video's Texture2D data
        /// </summary>
        private Thread updateThread;

        /// <summary>
        /// Thread used to wait untill the video is complete, then invoke the OnVideoComplete EventHandler
        /// </summary>
        private Thread waitThread;

        /// <summary>
        /// The Video File to play
        /// </summary>
        private string filename;

        /// <summary>
        /// Is a new frame avaliable to update?
        /// </summary>
        private bool frameAvailable = false;

        /// <summary>
        /// Array to hold the raw data from the DirectShow video stream.
        /// </summary>
        private byte[] bgrData;

        /// <summary>
        /// The RGBA frame bytes used to set the data in the Texture2D Output Frame
        /// </summary>
        private byte[] videoFrameBytes;

        /// <summary>
        /// Private Video Width
        /// </summary>
        private int videoWidth = 0;

        /// <summary>
        /// Private Video Height
        /// </summary>
        private int videoHeight = 0;

        /// <summary>
        /// Private Texture2D to render video to. Created in the Video Player Constructor.
        /// </summary> 
        private Texture2D outputFrame;

        /// <summary>
        /// Average Time per Frame in milliseconds
        /// </summary>
        private long avgTimePerFrame;

        /// <summary>
        /// BitRate of the currently loaded video
        /// </summary>
        private int bitRate;

        /// <summary>
        /// Current state of the video player
        /// </summary>
        private VideoState currentState;

        /// <summary>
        /// Is Disposed?
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// Current time position
        /// </summary>
        private long currentPosition;

        /// <summary>
        /// Video duration
        /// </summary>
        private long videoDuration;

        /// <summary>
        /// How transparent the video frame is.
        /// Takes effect on the next frame after this is updated
        /// Max Value: 255 - Opaque
        /// Min Value: 0   - Transparent
        /// </summary>
        private byte alphaTransparency = 255;
        #endregion

        #region Public Properties
        /// <summary>
        /// Automatically updated video frame. Render this to the screen using a SpriteBatch.
        /// </summary>
        public Texture2D GetTexture()
        {
            return outputFrame;
        }

        /// <summary>
        /// Width of the loaded video
        /// </summary>
        public int VideoWidth
        {
            get
            {
                return videoWidth;
            }
        }

        /// <summary>
        /// Height of the loaded video
        /// </summary>
        public int VideoHeight
        {
            get
            {
                return videoHeight;
            }
        }

        /// <summary>
        /// Gets or Sets the current position of playback in seconds
        /// </summary>
        public double CurrentPosition
        {
            get
            {
                return (double)currentPosition / 10000000;
            }
            set
            {
                if (value < 0)
                    value = 0;

                if (value > Duration)
                    value = Duration;

                DsError.ThrowExceptionForHR(mp.put_CurrentPosition(value));
                currentPosition = (long)value * 10000000;
            }
        }

        /// <summary>
        /// Returns the current position of playback, formatted as a time string (HH:MM:SS)
        /// </summary>
        public string CurrentPositionAsTimeString
        {
            get
            {
                double seconds = (double)currentPosition / 10000000;

                double minutes = seconds / 60;
                double hours = minutes / 60;

                int realHours = (int)Math.Floor(hours);
                minutes -= realHours * 60;

                int realMinutes = (int)Math.Floor(minutes);
                seconds -= realMinutes * 60;

                int realSeconds = (int)Math.Floor(seconds);

                return (realHours < 10 ? "0" + realHours.ToString() : realHours.ToString()) + ":" + (realMinutes < 10 ? "0" + realMinutes.ToString() : realMinutes.ToString()) + ":" + (realSeconds < 10 ? "0" + realSeconds.ToString() : realSeconds.ToString());
            }
        }

        /// <summary>
        /// Total duration in seconds
        /// </summary>
        public double Duration
        {
            get
            {
                return (double)videoDuration / 10000000;
            }
        }

        /// <summary>
        /// Returns the duration of the video, formatted as a time string (HH:MM:SS)
        /// </summary>
        public string DurationAsTimeString
        {
            get
            {
                double seconds = (double)videoDuration / 10000000;

                double minutes = seconds / 60;
                double hours = minutes / 60;

                int realHours = (int)Math.Floor(hours);
                minutes -= realHours * 60;

                int realMinutes = (int)Math.Floor(minutes);
                seconds -= realMinutes * 60;

                int realSeconds = (int)Math.Floor(seconds);

                return (realHours < 10 ? "0" + realHours.ToString() : realHours.ToString()) + ":" + (realMinutes < 10 ? "0" + realMinutes.ToString() : realMinutes.ToString()) + ":" + (realSeconds < 10 ? "0" + realSeconds.ToString() : realSeconds.ToString());
            }
        }

        /// <summary>
        /// Currently Loaded Video File
        /// </summary>
        public string FileName
        {
            get
            {
                return filename;
            }
        }

        /// <summary>
        /// Gets or Sets the current state of the video player
        /// </summary>
        public VideoState CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                switch (value)
                {
                    case VideoState.Playing:
                        Play();
                        break;
                    case VideoState.Paused:
                        Pause();
                        break;
                    case VideoState.Stopped:
                        Stop();
                        break;
                }
            }
        }

        /// <summary>
        /// Event which occurs when the video stops playing once it has reached the end of the file
        /// </summary>
        public event EventHandler OnVideoComplete;

        /// <summary>
        /// Is Disposed?
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        /// <summary>
        /// Number of Frames Per Second in the video file.
        /// Returns -1 if this cannot be calculated.
        /// </summary>
        public int FramesPerSecond
        {
            get
            {
                if (avgTimePerFrame == 0)
                    return -1;

                float frameTime = (float)avgTimePerFrame / 10000000.0f;
                float framesPS = 1.0f / frameTime;
                return (int)Math.Round(framesPS, 0, MidpointRounding.ToEven);
            }
        }

        /// <summary>
        /// The number of milliseconds between each frame
        /// Returns -1 if this cannot be calculated
        /// </summary>
        public float MillisecondsPerFrame
        {
            get
            {
                if (avgTimePerFrame == 0)
                    return -1;

                return (float)avgTimePerFrame / 10000.0f;
            }
        }

        /// <summary>
        /// Gets or sets how transparent the video frame is.
        /// Takes effect on the next frame after this is updated
        /// Max Value: 255 - Opaque
        /// Min Value: 0   - Transparent
        /// </summary>
        public byte AlphaTransparency
        {
            get
            {
                return alphaTransparency;
            }
            set
            {
                alphaTransparency = value;
            }
        }
        #endregion

		public static bool Broken = false;

        #region Constructor
        /// <summary>
        /// Creates a new Video Player. Automatically creates the required Texture2D on the specificied GraphicsDevice.
        /// </summary>
        /// <param name="FileName">The video file to open</param>
        /// <param name="graphicsDevice">XNA Graphics Device</param>
        public VideoPlayer(string FileName, GraphicsDevice graphicsDevice)
        {
            try
            {
                // Set video state
                currentState = VideoState.Stopped;

                // Store Filename
                filename = FileName;

                // Open DirectShow Interfaces
                InitInterfaces();

                // Create a SampleGrabber Filter and add it to the FilterGraph
                SampleGrabber sg = new SampleGrabber();
                ISampleGrabber sampleGrabber = (ISampleGrabber)sg;
                DsError.ThrowExceptionForHR(gb.AddFilter((IBaseFilter)sg, "Grabber"));

                // Setup Media type info for the SampleGrabber
                AMMediaType mt = new AMMediaType();
                mt.majorType = MEDIATYPE_Video;     // Video
                mt.subType = MEDIASUBTYPE_RGB24;    // RGB24
                mt.formatType = FORMAT_VideoInfo;   // VideoInfo
                DsError.ThrowExceptionForHR(sampleGrabber.SetMediaType(mt));

                // Construct the rest of the FilterGraph
				var result = gb.RenderFile(Path.Combine("Content", filename + ".wmv"), null);
                DsError.ThrowExceptionForHR(result);

                // Set SampleGrabber Properties
                DsError.ThrowExceptionForHR(sampleGrabber.SetBufferSamples(true));
                DsError.ThrowExceptionForHR(sampleGrabber.SetOneShot(false));
                DsError.ThrowExceptionForHR(sampleGrabber.SetCallback((ISampleGrabberCB)this, 1));

                // Hide Default Video Window
                IVideoWindow pVideoWindow = (IVideoWindow)gb;
                DsError.ThrowExceptionForHR(pVideoWindow.put_AutoShow(OABool.False));

                // Create AMMediaType to capture video information
                AMMediaType MediaType = new AMMediaType();
                DsError.ThrowExceptionForHR(sampleGrabber.GetConnectedMediaType(MediaType));
                VideoInfoHeader pVideoHeader = new VideoInfoHeader();
                Marshal.PtrToStructure(MediaType.formatPtr, pVideoHeader);

                // Store video information
                videoHeight = pVideoHeader.BmiHeader.Height;
                videoWidth = pVideoHeader.BmiHeader.Width;
                avgTimePerFrame = pVideoHeader.AvgTimePerFrame;
                bitRate = pVideoHeader.BitRate;
                DsError.ThrowExceptionForHR(ms.GetDuration(out videoDuration));

                // Create byte arrays to hold video data
                videoFrameBytes = new byte[(videoHeight * videoWidth) * 4]; // RGBA format (4 bytes per pixel)
                bgrData = new byte[(videoHeight * videoWidth) * 3];         // BGR24 format (3 bytes per pixel)

                // Create Output Frame Texture2D with the height and width of the video
                outputFrame = new Texture2D(graphicsDevice, videoWidth, videoHeight, false, SurfaceFormat.Color);
            }
            catch
            {
				Broken = true;
                //throw new Exception("Unable to Load or Play the video file");
            }
        }
        #endregion

        #region DirectShow Interface Management
        /// <summary>
        /// Initialises DirectShow interfaces
        /// </summary>
        private void InitInterfaces()
        {
			try
			{
				fg = new FilterGraph();
				gb = (IGraphBuilder)fg;
				mc = (IMediaControl)fg;
				me = (IMediaEventEx)fg;
				ms = (IMediaSeeking)fg;
				mp = (IMediaPosition)fg;
			}
			catch
			{
				Broken = true;
			}
        }

		static int ConvertLinearVolumeToLogScale(float volume)
		{
			//double L = 10000.0;
			//double logval = L * ((Math.Pow(10.0, volume) - 1.0) / 9.0 - 1.0);
			//return (int)logval;

			volume = CloudberryKingdom.CoreMath.LerpRestrict(0.0f, 1.0f, volume);
			float val = CloudberryKingdom.CoreMath.LerpRestrict(-10000.0f, 0.0f, (float)Math.Pow(volume, .3f));
			return (int)val;
		}

        /// <summary>
        /// Closes DirectShow interfaces
        /// </summary>
        private void CloseInterfaces()
        {
			try
			{
				if (me != null)
				{
					DsError.ThrowExceptionForHR(mc.Stop());
					//0x00008001 = WM_GRAPHNOTIFY
					DsError.ThrowExceptionForHR(me.SetNotifyWindow(IntPtr.Zero, 0x00008001, IntPtr.Zero));
				}
				mc = null;
				me = null;
				gb = null;
				ms = null;
				mp = null;
				if (fg != null)
					Marshal.ReleaseComObject(fg);
				fg = null;
			}
			catch
			{
			}
        }
        #endregion

        #region Update and Media Control
		public static int UpdateCount = 0, CopyCount = 0;
        /// <summary>
        /// Updates the Output Frame data using data from the video stream. Call this in Game.Update().
        /// </summary>
        public void Update()
        {
			try
			{
				if (Broken)
				{
					UpdateCount++;
					return;
				}

				// Remove the OutputFrame from the GraphicsDevice to prevent an InvalidOperationException on the SetData line.
				if (outputFrame.GraphicsDevice.Textures[0] == outputFrame)
				{
					outputFrame.GraphicsDevice.Textures[0] = null;
				}

				_UpdateBuffer();

				// Set video data into the Output Frame
				outputFrame.SetData<byte>(videoFrameBytes);
				UpdateCount++;

				// Update current position read-out
				DsError.ThrowExceptionForHR(ms.GetCurrentPosition(out currentPosition));
			}
			catch
			{
				Broken = true;
			}
        }

        /// <summary>
        /// Starts playing the video
        /// </summary>
        public void Play()
        {
            if (currentState != VideoState.Playing)
            {
                // Create video threads
                //updateThread = new Thread(new ThreadStart(UpdateBuffer));
                //waitThread = new Thread(new ThreadStart(WaitForCompletion));

                // Start the FilterGraph
                DsError.ThrowExceptionForHR(mc.Run());

                // Start Threads
                //updateThread.Start();
                //waitThread.Start();

                // Update VideoState
                currentState = VideoState.Playing;				
            }
        }

		public void SetVolume(float volume)
		{
			try
			{
				var mixer = (IBasicAudio)fg;
				mixer.put_Volume(ConvertLinearVolumeToLogScale(volume));
			}
			catch
			{
			}
		}

        /// <summary>
        /// Pauses the video
        /// </summary>
        public void Pause()
        {
			try
			{
				// End threads
				if (updateThread != null)
					updateThread.Abort();
				updateThread = null;

				if (waitThread != null)
					waitThread.Abort();
				waitThread = null;

				// Stop the FilterGraph (but remembers the current position)
				DsError.ThrowExceptionForHR(mc.Stop());

				// Update VideoState
				currentState = VideoState.Paused;
			}
			catch
			{
				Broken = true;
			}
        }

        /// <summary>
        /// Stops playing the video
        /// </summary>
        public void Stop()
        {
			try
			{
				// End Threads
				if (updateThread != null)
					updateThread.Abort();
				updateThread = null;

				if (waitThread != null)
					waitThread.Abort();
				waitThread = null;

				if (Broken)
				{
					currentState = VideoState.Stopped;
					return;
				}

				// Stop the FilterGraph
				DsError.ThrowExceptionForHR(mc.Stop());

				// Reset the current position
				DsError.ThrowExceptionForHR(ms.SetPositions(0, AMSeekingSeekingFlags.AbsolutePositioning, 0, AMSeekingSeekingFlags.NoPositioning));

				// Update VideoState
				currentState = VideoState.Stopped;
			}
			catch
			{
				Broken = true;
			}
        }

        /// <summary>
        /// Rewinds the video to the start and plays it again
        /// </summary>
        public void Rewind()
        {
            Stop();
            Play();
        }
        #endregion

        #region ISampleGrabberCB Members and Helpers
        /// <summary>
        /// Required public callback from DirectShow SampleGrabber. Do not call this method.
        /// </summary>
        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
			if (Broken) return 0;

			try
			{
				lock (this)
				{
					// Copy raw data into bgrData byte array
					Marshal.Copy(pBuffer, bgrData, 0, BufferLen);

					// Flag the new frame as available
					frameAvailable = true;
				}
			}
			catch
			{
				Broken = true;
			}

            // Return S_OK
            return 0;
        }

        /// <summary>
        /// Required public callback from DirectShow SampleGrabber. Do not call this method.
        /// </summary>
        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            // Return S_OK
            return 0;
        }

		private void _UpdateBuffer()
		{
			if (Broken) return;

			try
			{
				lock (this)
				{
					if (frameAvailable)
					{
						int waitTime = avgTimePerFrame != 0 ? (int)((float)avgTimePerFrame / 10000) : 20;

						int samplePosRGBA = 0;
						int samplePosRGB24 = 0;

						for (int y = 0, y2 = videoHeight - 1; y < videoHeight; y++, y2--)
						{
							for (int x = 0; x < videoWidth; x++)
							{
								samplePosRGBA = (((y2 * videoWidth) + x) * 4);
								samplePosRGB24 = ((y * videoWidth) + x) * 3;

								videoFrameBytes[samplePosRGBA + 0] = bgrData[samplePosRGB24 + 2];
								videoFrameBytes[samplePosRGBA + 1] = bgrData[samplePosRGB24 + 1];
								videoFrameBytes[samplePosRGBA + 2] = bgrData[samplePosRGB24 + 0];
								videoFrameBytes[samplePosRGBA + 3] = alphaTransparency;
							}
						}
					}
				}
			}
			catch
			{
				Broken = true;
			}
		}

        /// <summary>
        /// Waits for the video to finish, then calls the OnVideoComplete event
        /// </summary>
        private void WaitForCompletion()
        {
			if (Broken) return;

            int waitTime = avgTimePerFrame != 0 ? (int)((float)avgTimePerFrame / 10000) : 20;

            try
            {
                while (videoDuration > currentPosition)
                {
                    Thread.Sleep(waitTime);
                }

                if (OnVideoComplete != null)
                    OnVideoComplete.Invoke(this, EventArgs.Empty);
            }
            catch
			{
			}
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Cleans up the Video Player. Must be called when finished with the player.
        /// </summary>
        public void Dispose()
        {
			if (Broken) return;

			try
			{
				isDisposed = true;

				Stop();
				CloseInterfaces();

				if (Broken)
				{
					outputFrame = null;
					return;
				}

				outputFrame.Dispose();
				outputFrame = null;
			}
			catch
			{
			}
        }
        #endregion
    }
}