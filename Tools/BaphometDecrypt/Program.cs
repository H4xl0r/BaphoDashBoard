using System;
using System.IO;

namespace BaphometDecrypt
{
    class Program
    {
        static void Main()
        {
            Decrypt decrypt = new Decrypt();

            var userName = Environment.UserName;
            //Directorios cifrados("Desktop","Documents","Pictures" etc)
            //debemos tener los mismos directorios que colocamos en el proyecto Baphomet
            var dirs = new[] { "Desktop","Documents","Downloads","Music","Pictures","Videos" };
            var userDir = Path.Combine("C:\\Users\\",userName);
            var password = "<symmetric key here>";

            for (int d = 0; d < dirs.Length; d++)//recorro cada uno de los dirs validos
            {
                var decryp_targetPath = Path.Combine(userDir,dirs[d]);
                decrypt.directoryRoad(decryp_targetPath, password);
            }
        }
    }
}
