using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateMcpServer
{
    internal static class CreateMcpServerPath
    {
        private static string? rootFolderPath;

        public static string RootFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(rootFolderPath))
                {
                    var args = Environment.GetCommandLineArgs();
                    if(args.Length == 2)
                    {
                        rootFolderPath = args[1];
                    }
                    else
                    {
                        throw new ArgumentException("Invalid argument. Please provide the root folder path as the first argument.");
                    }
                }

                return rootFolderPath;
            }
        }   
    }
}
