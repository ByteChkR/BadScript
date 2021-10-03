using System;

namespace BadScript.Console.AppPackage
{

    public struct BSAppPackageManifest
    {

        public string AppName;
        public string AppVersion;
        public string ExecutablePath;
        public string ResourcePath;
        public string RuntimeMinVersion;
        public string RuntimeMaxVersion;
        public string[] RequiredInterfaces;

        public static BSAppPackageManifest CreateDefault()
        {
            return new BSAppPackageManifest
                   {
                       AppName = "App",
                       AppVersion = "0.0.0.0",
                       RequiredInterfaces = Array.Empty < string >(),
                       RuntimeMinVersion = "0.0.0.0",
                       RuntimeMaxVersion = "9999.9999.9999.9999",
                       ExecutablePath = "src/Main.bs",
                       ResourcePath = "res/"
                   };
        }

    }

}
