using System;
using System.IO;
using Microsoft.Xna.Framework.Storage;

using CloudberryKingdom;

namespace EasyStorage
{
	// implements the synchronous file operations for the SaveDevice.
	public abstract partial class SaveDevice
	{
		/// <summary>
		/// Helper method to open a StorageContainer.
		/// </summary>
		private StorageContainer OpenContainer(string containerName)
		{
			// open the container from the device. while this is normally an async process, we
			// block until completion which makes it a synchronous operation for our uses.
            bool isTransferredFromOtherPlayer = false;
            try
            {
#if XDK
                IAsyncResult asyncResult = storageDevice.BeginOpenContainer(containerName, null, null);

                // Can't use this ovveride without causing problems with unreasonable exceptions being thrown.
                //IAsyncResult asyncResult = storageDevice.BeginOpenContainer(containerName, false, out isTransferredFromOtherPlayer, null, null);
#else
			    IAsyncResult asyncResult = storageDevice.BeginOpenContainer(containerName, null, null);
#endif
                asyncResult.AsyncWaitHandle.WaitOne();
                var container = storageDevice.EndOpenContainer(asyncResult);

                // Return a null if the container was transferred from another player.
                // (This is now always false, since we do not use the BeginOpenContainer override.
                if (isTransferredFromOtherPlayer) return null;

                return container;
            }
            catch (Exception e)
            {
                Tools.Write(e.Message);

                // Assume the data was loaded from a different user
                return null;
            }
		}

		/// <summary>
		/// Helper that just checks that IsReady is true and throws if it's false.
		/// </summary>
		private void VerifyIsReady()
		{
			if (!IsReady)
				throw new InvalidOperationException("StorageDevice is not valid.");
		}

		/// <summary>
		/// Saves a file.
		/// </summary>
		/// <param name="containerName">The name of the container in which to save the file.</param>
		/// <param name="fileName">The file to save.</param>
		/// <param name="saveAction">The save action to perform.</param>
		public void Save(string containerName, string fileName, FileAction saveAction)
		{
			VerifyIsReady();

			// lock on the storage device so that only one storage operation can occur at a time
			lock (storageDevice)
			{
				// open a container
				using (StorageContainer currentContainer = OpenContainer(containerName))
				{
                    if (currentContainer == null) return;

					// attempt the save
					using (var stream = currentContainer.CreateFile(fileName))
					{
						saveAction(stream);
					}
				}
			}
		}

		/// <summary>
		/// Loads a file.
		/// </summary>
		/// <param name="containerName">The name of the container from which to load the file.</param>
		/// <param name="fileName">The file to load.</param>
		/// <param name="loadAction">The load action to perform.</param>
		public void Load(string containerName, string fileName, FileAction loadAction)
		{
			VerifyIsReady();

			// lock on the storage device so that only one storage operation can occur at a time
			lock (storageDevice)
			{
				// open a container
				using (StorageContainer currentContainer = OpenContainer(containerName))
				{
                    if (currentContainer == null) return;

                    if (currentContainer.FileExists(fileName))
                    {
                        // attempt the load
                        using (var stream = currentContainer.OpenFile(fileName, FileMode.Open))
                        {
                            loadAction(stream);
                        }
                    }
				}
			}
		}

		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <param name="containerName">The name of the container from which to delete the file.</param>
		/// <param name="fileName">The file to delete.</param>
		public void Delete(string containerName, string fileName)
		{
			VerifyIsReady();

			// lock on the storage device so that only one storage operation can occur at a time
			lock (storageDevice)
			{
				// open a container
				using (StorageContainer currentContainer = OpenContainer(containerName))
				{
                    if (currentContainer == null) return;

					// attempt to delete the file
					if (currentContainer.FileExists(fileName))
					{
						currentContainer.DeleteFile(fileName);
					}
				}
			}
		}

		/// <summary>
		/// Determines if a given file exists.
		/// </summary>
		/// <param name="containerName">The name of the container in which to check for the file.</param>
		/// <param name="fileName">The name of the file.</param>
		/// <returns>True if the file exists, false otherwise.</returns>
		public bool FileExists(string containerName, string fileName)
		{
			VerifyIsReady();

			// lock on the storage device so that only one storage operation can occur at a time
			lock (storageDevice)
			{
				// open a container
				using (StorageContainer currentContainer = OpenContainer(containerName))
				{
                    if (currentContainer == null) return false;

					return currentContainer.FileExists(fileName);
				}
			}
		}

		/// <summary>
		/// Gets an array of all files available in a container.
		/// </summary>
		/// <param name="containerName">The name of the container in which to search for files.</param>
		/// <returns>An array of file names of the files in the container.</returns>
		public string[] GetFiles(string containerName)
		{
			return GetFiles(containerName, null);
		}

		/// <summary>
		/// Gets an array of all files available in a container.
		/// </summary>
		/// <param name="containerName">The name of the container in which to search for files.</param>
		/// <param name="pattern">A search pattern to use to find files.</param>
		/// <returns>An array of file names of the files in the container.</returns>
		public string[] GetFiles(string containerName, string pattern)
		{
			VerifyIsReady();

			// lock on the storage device so that only one storage operation can occur at a time
			lock (storageDevice)
			{
				// open a container
				using (StorageContainer currentContainer = OpenContainer(containerName))
				{
                    if (currentContainer == null) return new string[] { };

					return string.IsNullOrEmpty(pattern) ? currentContainer.GetFileNames() : currentContainer.GetFileNames(pattern);
				}
			}
		}
	}
}
