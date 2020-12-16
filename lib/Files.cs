using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fantactics
{
    public class FileHelper
    {
        public List<string> Files {get; private set;} = new List<string>();
        public List<string> Directories {get; private set;} = new List<string>();
        private List<string> ignores = new List<string>();
        private List<string> only = new List<string>();

        public List<string> FilesWithPathTrimmed()
        {
            return(
                from file in Files
                let trim = file.Split(".").First().Split("/").Last()
                select trim
                ).ToList();
        }

        public FileHelper Ignore(string pattern)
        {
            ignores.Add(pattern);
            return this;
        }

        public FileHelper IgnoreGit()
        {
            return Ignore(".git");
        }

        public FileHelper IgnoreMono()
        {
            return Ignore(".mono");
        }

        public FileHelper IgnoreImports()
        {
            return Ignore(".import");
        }

        public FileHelper OpenDirectory(string directory)
        {
            Directory root = new Directory();
            root.Open(directory);
            root.ListDirBegin(true, false);

            DiscoverFiles(root, Files, Directories, ignores);
            return this;
        }

        public FileHelper WithExtension(string ext)
        {
            IEnumerable<string> files = 
                from file in Files
                let split = file.Split(".")
                where split.Last() == ext
                select file;

            Files = files.ToList();
            return this;
        }

        public FileHelper WithFileNameFilter(List<string> fileNames)
        {
            IEnumerable<string> files = 
                from file in Files
                where fileNames.Contains(file.Split(".").First().Split("/").Last())
                select file;

            Files = files.ToList();
            return this;
        }

        private void DiscoverFiles(Directory dir, List<string> files, List<string> dirs, List<string> ignores)
        {
            string fileName = dir.GetNext();
            while(fileName != "")
            {
                string path = dir.GetCurrentDir() + "/" + fileName;

                if(!ignores.Contains(fileName))
                {
                    if(dir.CurrentIsDir())
                    {
                        Directory subDir = new Directory();
                        subDir.Open(path);
                        subDir.ListDirBegin(true, false);
                        dirs.Add(path);
                        DiscoverFiles(subDir, files, dirs, ignores);
                    }
                    else
                    {
                        files.Add(path);
                    }
                }

                fileName = dir.GetNext();
            }

            dir.ListDirEnd();
        }
    }
}
