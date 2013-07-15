using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace StaticSiteGenerator
{
	public static class S3ObjectExtension
	{
		public static bool IsFolder(this S3Object Object)
		{
			bool LastCharIsSlash = Object.Key[Object.Key.Length - 1] == '/';
			
			return LastCharIsSlash;
		}

		public static bool IsFolder(this GetObjectResponse Object)
		{
			bool LastCharIsSlash = Object.Key[Object.Key.Length - 1] == '/';

			return LastCharIsSlash;
		}
	}

	class Program
	{
		//const string SiteRoot = "http://www.pwnee.com.s3.amazonaws.com";
		const string SiteRoot = "http://www.pwnee.com";
		const string RootKeyword = "✪Root✪";
		const string PostRootKeyword = "✪PostRoot✪";
		const string BlogPreviewCut = "✪PreviewCut✪";
		const string BlogPreviewCut_Replace = "<!-- Preview Cut -->";

		const string LogPath = "Log.txt";
		const string MainBucket = "www.pwnee.com";
		const string LocalRelativePath = "";
		static string LocalPath = Path.Combine(LocalRelativePath, MainBucket);

		static DateTime LastUploadSynch = GetLastUploadSynch();

		static AmazonS3 s3Client;
		
		static MD5CryptoServiceProvider Crypto = new MD5CryptoServiceProvider();


		static void Log(string s, params object[] args)
		{
			s = string.Format(s, args);
			
			File.AppendAllText(LogPath, s + "\n");
			Console.WriteLine(s);
		}

		static void LogWithTime(string s, params object[] args)
		{
			s = string.Format(s, args);
			Log(s + string.Format(" Time: {0}", DateTime.Now));
		}

		static void Initialize()
		{
			Log("\n\n-----------------------------------------------------");
			LogWithTime("Running program.");

			var AWSAccessKey = "AKIAIAZCDKXQKAGFHGEA";
			var AWSSecretKey = "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";

			s3Client = AWSClientFactory.CreateAmazonS3Client(AWSAccessKey, AWSSecretKey, RegionEndpoint.USEast1);
		}

		/// <summary>
		/// Get a list of all objects in a bucket.
		/// </summary>
		/// <param name="BucketName">The name of the bucket.</param>
		/// <returns></returns>
		static List<S3Object> GetBucketObjects(string BucketName)
		{
			ListObjectsRequest request = new ListObjectsRequest();
			request.BucketName = BucketName;
			ListObjectsResponse response = s3Client.ListObjects(request);

			return response.S3Objects;
		}

		static Dictionary<string, S3Object> GetBucketDictionary(string BucketName)
		{
			var list = GetBucketObjects(BucketName);

			Dictionary<string, S3Object> dict = new Dictionary<string, S3Object>();

			foreach (var obj in list)
			{
				dict.Add(obj.Key, obj);
			}

			return dict;
		}

		/// <summary>
		/// Write out the name, size, and last modified time of all objects in a bucket.
		/// </summary>
		/// <param name="BucketName">The name of the bucket.</param>
		static void ListBucketObjects(string BucketName)
		{
			foreach (S3Object o in GetBucketObjects(BucketName))
			{
				Console.WriteLine("{0}\t{1}\t{2}", o.Key, o.Size, o.LastModified);
			}
		}

		static void DownloadObject(S3Object Object, string FileDestination)
		{
			DownloadObject(Object.BucketName, Object.Key, FileDestination, Object.LastModified);
		}
		static void DownloadObject(string BucketName, string Key, string FileDestinationPath, string LastModified = null)
		{
			GetObjectRequest request = new GetObjectRequest();
			request.BucketName = BucketName;
			request.Key        = Key;
			GetObjectResponse response = s3Client.GetObject(request);

			if (response.IsFolder())
			{
				EnsureDirectory(FileDestinationPath);
			}
			else
			{
				EnsureDirectory(Path.GetDirectoryName(FileDestinationPath));

				bool UpToDate = false;
				if (LastModified != null)
				{
					if (File.Exists(FileDestinationPath))
					{
						if (HashCompare(FileDestinationPath, response.ETag))
							UpToDate = true;
					}
				}

				if (!UpToDate)
				{
					response.WriteResponseStreamToFile(FileDestinationPath);
				}
			}
		}

		static void EnsureFolderOnS3(string BucketName, string Key)
		{
			LogWithTime("  Creating folder on S3.");

			if (Key.Last() != '/') Key += '/';

			PutObjectRequest request = new PutObjectRequest();
			request.BucketName = BucketName;
			request.Key = Key;
			request.ContentBody = string.Empty;

			try
			{
				PutObjectResponse response = s3Client.PutObject(request);
			}
			catch (Exception e)
			{
				LogWithTime(e.ToString());
			}
		}

		static void UploadObject(string BucketName, string Key, string FileSourcePath, string LastModified = null)
		{
			LogWithTime("  Uploading object.");

			PutObjectRequest request = new PutObjectRequest();
			request.BucketName = BucketName;
			request.Key = Key;
			request.FilePath = FileSourcePath;

			try
			{
				PutObjectResponse response = s3Client.PutObject(request);
			}
			catch (Exception e)
			{
				LogWithTime(e.ToString());
			}
		}

		static void UploadBucket(string BucketName, string SourceFolder)
		{
			var s3_dict = GetBucketDictionary(BucketName);
			
			var local_files = GetFilePaths(SourceFolder);
			var local_dirs = GetDirPaths(SourceFolder);

			int Count, Total;

			Count = 1;
			Total = local_dirs.Length;
			foreach (var dir in local_dirs)
			{
				var last_modified = Directory.GetLastWriteTimeUtc(Path.Combine(SourceFolder, dir));
				if (last_modified > LastUploadSynch)
				{
					Log("  {1}/{2} : Synching folder {0}", dir, Count, Total);
					EnsureFolderOnS3(BucketName, dir);
				}
				else
				{
					Log("  {1}/{2} : Folder {0} already synched", dir, Count, Total);
				}

				Count++;
			}

			Count = 1;
			Total = local_files.Length;
			foreach (var file in local_files)
			{
				var last_modified = File.GetLastWriteTimeUtc(Path.Combine(SourceFolder, file));
				if (last_modified > LastUploadSynch && (!s3_dict.ContainsKey(file) || !HashCompare(Path.Combine(SourceFolder, file), s3_dict[file].ETag)))
				//if (true)
				{
					Log("  {1}/{2} : Synching file {0}", file, Count, Total);
					UploadObject(BucketName, file, Path.Combine(SourceFolder, file));
				}
				else
				{
					Log("  {1}/{2} : File {0} already synched", file, Count, local_files.Length);
				}

				Count++;
			}

			UpdateUploadSynchTimeStamp();
		}

		static string PathToKey(string Path, string RootPath)
		{
			Path = Path.Substring(RootPath.Length + 1);
			Path = Path.Replace('\\', '/');
			return Path;
		}

		static void PathsToKeys(string[] Paths, string RootPath)
		{
			for (int i = 0; i < Paths.Length; i++)
			{
				Paths[i] = PathToKey(Paths[i], RootPath);
			}
		}

		static string[] GetFilePaths(string DirPath)
		{
			var files = Directory.GetFiles(DirPath, "*.*", SearchOption.AllDirectories);
			PathsToKeys(files, DirPath);
			return files;
		}

		static string[] GetDirPaths(string DirPath)
		{
			var dirs = Directory.GetDirectories(DirPath, "*.*", SearchOption.AllDirectories);
			PathsToKeys(dirs, DirPath);
			return dirs;
		}

		static bool SynchFileExists()
		{
			var synch_file = Path.Combine(LocalPath, "LastSynch.txt");

			return File.Exists(synch_file);
		}

		static DateTime GetLastUploadSynch()
		{
			var synch_file = Path.Combine(LocalPath, "LastSynch.txt");

			if (File.Exists(synch_file))
			{
				var s = File.ReadAllText(synch_file);
				LastUploadSynch = DateTime.Parse(s);
			}
			else
			{
				LastUploadSynch = new DateTime(0);
			}

			return LastUploadSynch;
		}

		static void UpdateUploadSynchTimeStamp()
		{
			File.WriteAllText(Path.Combine(LocalPath, "LastSynch.txt"), DateTime.Now.ToString());
		}

		static bool ModifiedSinceLastUploadSynch(string FilePath)
		{
			if (File.Exists(FilePath))
			{
				var last_file_mod = File.GetLastWriteTimeUtc(FilePath);
				return last_file_mod > LastUploadSynch;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns true if the local file was modified before the S3 file was last modified.
		/// </summary>
		/// <param name="LocalFilePath"></param>
		/// <param name="LastModified"></param>
		static bool Synched(string LocalFilePath, string LastModified)
		{
			var LastModifiedTime_Local = File.GetLastWriteTimeUtc(LocalFilePath);
			var LastModifiedTime_S3 = DateTime.Parse(LastModified);

			return LastModifiedTime_Local > LastModifiedTime_S3;
		}

		static bool HashCompare(string LocalFilePath, string S3ETag)
		{
			string Hash_Local = BitConverter.ToString(Crypto.ComputeHash(File.ReadAllBytes(LocalFilePath))).Replace("-", string.Empty).ToLower();
			string Hash_S3 = S3ETag;

			if (Hash_S3 == '"' + Hash_Local + '"')
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		static void EnsureDirectory(string DirectoryPath)
		{
			if (Directory.Exists(DirectoryPath)) return;

			Directory.CreateDirectory(DirectoryPath);
		}

		/// <summary>
		/// Downloads all the contents of a bucket to a local folder.
		/// </summary>
		/// <param name="BucketName">Name of the bucket.</param>
		/// <param name="FolderDestinationPath">Path to the folder to download the objects to. Directory does not need to exit.</param>
		static void DownloadObjects(List<S3Object> ObjectList, string BucketName, string FolderDestinationPath)
		{
			int Count = 1;
			foreach (var obj in ObjectList)
			{
				string FileDestination = Path.Combine(FolderDestinationPath, obj.Key);

				Log("Downloading object {0} of {1}", Count, ObjectList.Count + 1);
				DownloadObject(obj, FileDestination);

				Count++;
			}
		}

		/// <summary>
		/// Downloads all the contents of a bucket to a local folder.
		/// </summary>
		/// <param name="BucketName">Name of the bucket.</param>
		/// <param name="FolderDestinationPath">Path to the folder to download the objects to. Directory does not need to exit.</param>
		static void DownloadBucket(string BucketName, string FolderDestinationPath)
		{
			var list = GetBucketObjects(BucketName);

			DownloadObjects(list, BucketName, FolderDestinationPath);

			UpdateUploadSynchTimeStamp();
		}

		static string BucketListAll()
		{
			StringBuilder s = new StringBuilder(1024);

			ListBucketsResponse response = s3Client.ListBuckets();
			foreach (S3Bucket b in response.Buckets)
			{
				s.AppendFormat("{0}\t{1}", b.BucketName, b.CreationDate);
			}

			return s.ToString();
		}

		static void PressAnyKeyToContinue()
		{
			Console.WriteLine("\nPress any key to continue.");
			Console.ReadKey(true);
		}

		static string ReplaceKeywords(string text, string filepath)
		{
			text = text.Replace(RootKeyword, SiteRoot);

			text = text.Replace(BlogPreviewCut, BlogPreviewCut_Replace);

			if (text.Contains(PostRootKeyword))
			{
				string dir = Path.GetDirectoryName(filepath);
				dir = dir.Replace(MainBucket, "");
				string fullpath = SiteRoot + dir;
				text = text.Replace(PostRootKeyword, fullpath);
			}

			return text;
		}

		static string CompileSmu(string SmuPath, string Root)
		{
			Dictionary<string, string> Variables = GetIdentifiers(SmuPath);

			string Template = File.ReadAllText(Path.Combine(Root, Variables["Template"]));

			var compiled = Parse.CompileTemplate(Template, Variables);

			compiled = ReplaceKeywords(compiled, SmuPath);

			return compiled;
		}

		public static Dictionary<string, string> GetIdentifiers(string SmuPath)
		{
			string smu = File.ReadAllText(SmuPath);

			smu = ReplaceKeywords(smu, SmuPath);

			var Lines = Parse.ParseSmu(smu);

			Dictionary<string, string> Identifiers = new Dictionary<string, string>();
			
			foreach (var line in Lines)
			{
				Identifiers.Add(line.Identifier, line.Value);
			}

			Identifiers.Add("FileName", Path.GetFileName(SmuPath));
			Identifiers.Add("FilePath", SmuPath.Replace(MainBucket, SiteRoot));
			Identifiers.Add("DirPath", Path.GetDirectoryName(SmuPath).Replace(MainBucket, SiteRoot).Replace('\\', '/'));

			return Identifiers;
		}

        static void CompileLeaderboard(string Root)
        {
            HighScores.GetHighScore();

            List<Dictionary<string, string>> Entries = new List<Dictionary<string, string>>();

            for (int i = 0; i < HighScores.Entries.Count; i++)
            {
                var d = new Dictionary<string, string>();
                d.Add("Name", HighScores.Entries[i].Name);
                d.Add("Rank", HighScores.Entries[i].Rank.ToString());
                d.Add("Score", string.Format("{0:n0}", HighScores.Entries[i].Score));
				d.Add("Link", "http://www.pwnee.com");

                Entries.Add(d);
            }

            var index = File.ReadAllText(Path.Combine(Root, "generate_index.smu"));

            index = ReplaceKeywords(index, Root);

            var index_pieces = index.Split('✙');

            string s = "";

            s += index_pieces[0];

            var repeat_section = index_pieces[1];
            var index_varpieces = repeat_section.Split('✪');
            for (int repeat = 0; repeat < HighScores.Entries.Count; repeat++)
            {
                for (int i = 0; i < index_varpieces.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        s += index_varpieces[i];
                    }
                    else
                    {
                        s += Entries[repeat][index_varpieces[i]];
                    }
                }
            }

            s += index_pieces[2];

            File.WriteAllText(Path.Combine(Root, "index.smu"), s);
        }

		static void CompileBlogHome(string Root)
		{
			var entries = File.ReadAllText(Path.Combine(Root, "Entries.txt"));
			var entry_lines = entries.Split('\n');
			
			int num_entries = Math.Min(int.Parse(entry_lines[0]), entry_lines.Length - 1);

			List<Dictionary<string, string>> BlogPosts = new List<Dictionary<string, string>>();

			for (int i = 1; i <= num_entries; i++)
			{
				string path = Path.Combine(Root, "entries", entry_lines[i], "index.smu");

				var identifiers = GetIdentifiers(path);

				BlogPosts.Add(identifiers);
			}
			

			var index = File.ReadAllText(Path.Combine(Root, "generate_index.smu"));

			index = ReplaceKeywords(index, Root);

			var index_pieces = index.Split('✙');
			
			string s = "";

			s += index_pieces[0];

			var repeat_section = index_pieces[1];
			var index_varpieces = repeat_section.Split('✪');
			for (int repeat = 0; repeat < num_entries; repeat++)
			{
				for (int i = 0; i < index_varpieces.Length; i++)
				{
					if (i % 2 == 0)
					{
						s += index_varpieces[i];
					}
					else
					{
						s += BlogPosts[repeat][index_varpieces[i]];
					}
				}
			}

			s += index_pieces[2];

			File.WriteAllText(Path.Combine(Root, "index.smu"), s);
		}

		static void CompileAllSmus(string Root)
		{
			var files = Directory.GetFiles(Root, "*.smu", SearchOption.AllDirectories);

			foreach (var file in files)
			{
				var compiled = CompileSmu(file, Root);
				File.WriteAllText(Path.ChangeExtension(file, ".html"), compiled);
			}
		}

		public static void Main(string[] args)
		{
			_Main(args);

			//try
			//{
			//    _Main(args);
			//}
			//catch (Exception e)
			//{
			//    LogWithTime(e.ToString());
			//}
		}





		static void _Main(string[] args)
		{
			Initialize();

			if (args.Length > 0 && args[0] == "download")
			{
				if (SynchFileExists())
				{
					Log("Local content has been downloaded once before. Delete LastSynch.txt and re-run if you are sure you want to continue.");
					PressAnyKeyToContinue();
				}
				else
				{
					Log("Local content does not exist. Downloading!");

					DownloadBucket(MainBucket, LocalPath);
				}
			}
			else
			{
				Log("Uploading local content to S3.");

                //CompileLeaderboard(Path.Combine(LocalPath, "infinity_cup"));

				CompileBlogHome(Path.Combine(LocalPath, "blog"));

				CompileAllSmus(LocalPath);

				UploadBucket(MainBucket, LocalPath);
			}

			LogWithTime("Program done! Terminating.");
		}
	}
}