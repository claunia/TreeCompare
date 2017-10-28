using System;
using System.IO;
using System.Collections.Generic;

namespace TreeCompare
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            List<string> leftFiles = new List<string>();
            List<string> rightFiles = new List<string>();
            string progressBar = "-";
            long counter = 0;
            List<string> differences = new List<string>();

            Console.Error.WriteLine("TreeCompare v0.1 © 2017 Natalia Portillo");

            if(args.Length != 2)
            {
                Console.Error.WriteLine("Usage: TreeCompare <dir1> <dir2>");
                return;
            }

			if (args[0] == args[1])
			{
				Console.Error.WriteLine("Directories must not be the same");
				return;
			}

			if(!Directory.Exists(args[0]))
            {
                Console.Error.WriteLine("{0} does not exist or is not a directory", args[0]);
                return;
            }

			if (!Directory.Exists(args[1]))
			{
				Console.Error.WriteLine("{0} does not exist or is not a directory", args[1]);
				return;
			}

            DirectoryInfo dir0Info = new DirectoryInfo(args[0]);
            DirectoryInfo dir1Info = new DirectoryInfo(args[1]);
            string dir0 = Path.GetFullPath(args[0]);
            string dir1 = Path.GetFullPath(args[1]);

			Console.Error.Write("Counting files in {0}", dir0);
            foreach (string left in Directory.GetFiles(dir0, "*", SearchOption.AllDirectories))
            {
                switch(counter % 20)
                {
                    case 0:
					case 1:
					case 2:
					case 3:
					case 4:
						progressBar = "-";
                        break;
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
						progressBar = "\\";
                        break;
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
						progressBar = "|";
						break;
					case 15:
					case 16:
					case 17:
					case 18:
					case 19:
						progressBar = "/";
						break;
				}
                Console.Error.Write("\rCounting files in {0}... {1}", dir0, progressBar);
				string relative = Path.GetFullPath(left).Substring(dir0.Length);
				if (relative[0] == '/')
					relative = relative.Substring(1);
				leftFiles.Add(relative);
				counter++;
            }
            Console.Error.WriteLine();

            counter = 0;
			Console.Error.Write("Counting files in {0}", dir1);
            foreach (string right in Directory.GetFiles(dir1, "*", SearchOption.AllDirectories))
			{
				switch (counter % 20)
				{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
						progressBar = "-";
						break;
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
						progressBar = "\\";
						break;
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
						progressBar = "|";
						break;
					case 15:
					case 16:
					case 17:
					case 18:
					case 19:
						progressBar = "/";
						break;
				}
				Console.Error.Write("\rCounting files in {0}... {1}", dir1, progressBar);
                string relative = Path.GetFullPath(right).Substring(dir1.Length);
                if (relative[0] == '/')
                    relative = relative.Substring(1);
				rightFiles.Add(relative);
                counter++;
			}
			Console.Error.WriteLine();

			counter = 0;
			Console.Error.Write("Comparing files...");
			List<string> leftNotInRight = new List<string>();
            foreach (string left in leftFiles)
            {
				switch (counter % 20)
				{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
						progressBar = "-";
						break;
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
						progressBar = "\\";
						break;
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
						progressBar = "|";
						break;
					case 15:
					case 16:
					case 17:
					case 18:
					case 19:
						progressBar = "/";
						break;
				}
                Console.Error.Write("\rComparing files... {0}", progressBar);

				if (!rightFiles.Contains(left))
                {
                    leftNotInRight.Add(left);
                    continue;
                }

				rightFiles.Remove(left);
                FileInfo lfi = new FileInfo(Path.Combine(dir0, left));
                FileInfo rfi = new FileInfo(Path.Combine(dir1, left));

                string ctimeDif;
                string atimeDif;
                string mtimeDif;
                string attrDif;
                string sizeDif;
                bool sameContents = true;

                if (lfi.CreationTimeUtc == rfi.CreationTimeUtc)
                    ctimeDif = "Same";
                else
                    ctimeDif = string.Format("{0} - {1}", lfi.CreationTimeUtc, rfi.CreationTimeUtc);

                if (lfi.LastAccessTime == rfi.LastAccessTime)
					atimeDif = "Same";
				else
					atimeDif = string.Format("{0} - {1}", lfi.LastAccessTime, rfi.LastAccessTime);

                if (lfi.LastWriteTimeUtc == rfi.LastWriteTimeUtc)
					mtimeDif = "Same";
				else
					mtimeDif = string.Format("{0} - {1}", lfi.LastWriteTimeUtc, rfi.LastWriteTimeUtc);

                if (lfi.Attributes == rfi.Attributes)
					attrDif = "Same";
				else
					attrDif = string.Format("{0} - {1}", lfi.Attributes, rfi.Attributes);

                if (lfi.Length == rfi.Length)
                {
                    sizeDif = "Same";
                    byte[] lbuf = new byte[1048576];
                    byte[] rbuf = new byte[1048576];
                    FileStream lfs = lfi.Open(FileMode.Open, FileAccess.Read);
					FileStream rfs = rfi.Open(FileMode.Open, FileAccess.Read);

                    while (lfs.Read(lbuf, 0, lbuf.Length) > 0 && rfs.Read(rbuf, 0, rbuf.Length) > 0 && sameContents)
                    {
                        for (int i = 0; i < lbuf.Length; i++)
                        {
                            if(lbuf[i] != rbuf[i])
                            {
                                sameContents = false;
                                break;
                            }
                        }
                    }
				}
                else
                {
                    sizeDif = string.Format("{0} - {1}", lfi.Length, rfi.Length);
                    sameContents = false;
                }

                differences.Add(string.Format("\"{0}\";{1};{2};{3};{4};{5};{6};{7};{8}", left, "Yes", "Yes", ctimeDif, atimeDif, mtimeDif, attrDif, sizeDif, sameContents ? "Same" : "Different"));
                counter++;
			}
			Console.Error.WriteLine();

			foreach (string left in leftNotInRight)
                differences.Add(string.Format("\"{0}\";{1};{2};;;;;;", left, "Yes", "No"));

            foreach (string right in rightFiles)
				differences.Add(string.Format("\"{0}\";{1};{2};;;;;;", right, "No", "Yes"));

            differences.Sort();

            Console.WriteLine("Path;Exists in {0};Exists in {1};Creation time;Access time;Modification time;Attributes;Size;Contents", dir0, dir1);
            foreach (string diff in differences)
                Console.WriteLine("{0}", diff);
		}
    }
}
