using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BadScript.Console.Subsystems.Project.BuildFormats;
using BadScript.Console.Subsystems.Project.Utils;
using Newtonsoft.Json;

namespace BadScript.Console.Subsystems.Project.DataObjects;

public class ProjectSettings : ReflectedObject
{
    private static readonly List<BuildOutputFormat> s_Formats = new()
    {
        new BinaryOutputFormat(),
        new TextOutputFormat()
    };

    public AppInfo AppInfo = new();
    public ProjectBuildTargetCollection BuildTargets;
    public string PreprocessorDirectives = "";
    public Dictionary<string, string> ReferenceResolvers = new();

    [JsonIgnore] public string SaveLocation { get; set; }

    public event Action<PropertyResolveEventArgs> OnResolveProperty;

    #region Unity Event Functions

    public void Update(string data = null)
    {
        JsonConvert.PopulateObject(data ?? File.ReadAllText(SaveLocation), this);
    }

    #endregion

    #region Public

    public ProjectSettings()
    {
    }

    public ProjectSettings(AppInfo info, IEnumerable<BuildTarget> targets = null)
    {
        AppInfo = info;
        BuildTargets = new ProjectBuildTargetCollection(targets?.ToList());
    }

    public static ProjectSettings Deserialize(string data)
    {
        return JsonConvert.DeserializeObject<ProjectSettings>(data);
    }

    public static BuildOutputFormat GetOutputFormat(string fmt)
    {
        return s_Formats.First(x => x.Name == fmt);
    }

    public override string ResolveProperty(int current, string[] parts, ReflectionResolveInfo info)
    {
        PropertyResolveEventArgs args = new(parts, current, info);
        OnResolveProperty?.Invoke(args);

        if (args.Cancel)
        {
            if (args.Result == null) throw new Exception("Property Resolver was Cancelled");

            return args.Result;
        }

        if (parts[current] == "Target")
            return BuildTargets.GetTarget(info.CurrentTarget).ResolveProperty(current + 1, parts, info);

        if (parts[current] == "SubTarget")
        {
            var subTarget = BuildTargets.GetTarget(info.CurrentTarget).SubTarget;

            return BuildTargets.GetTarget(subTarget).ResolveProperty(
                current + 1,
                parts,
                new ReflectionResolveInfo(subTarget, info.Settings)
            );
        }

        if (parts[current] == "BS_DIR") return AppDomain.CurrentDomain.BaseDirectory;

        if (parts[current] == "BS_EXEC")
        {
            var isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
                return AppDomain.CurrentDomain.BaseDirectory + "bs.exe";
            return AppDomain.CurrentDomain.BaseDirectory + "bs";
        }

        return base.ResolveProperty(current, parts, info);
    }

    public string ResolveValue(string input, string target)
    {
        var original = input;

        for (var i = 0; i < input.Length; i++)
            if (input[i] == '%')
            {
                var start = i + 1;
                var end = -1;

                for (var j = i + 1; j < input.Length; j++)
                    if (input[j] == '%')
                    {
                        end = j;

                        break;
                    }

                if (end == -1) throw new Exception($"Invalid Project Settings Syntax: '{original}'");

                var part = input.Substring(start, end - start);
                input = input.Remove(start - 1, end - start + 2);

                input = input.Insert(
                    start - 1,
                    ResolveProperty(
                        0,
                        part.Split('.'),
                        new ReflectionResolveInfo(target, this)
                    )
                );
            }

        return input;
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    #endregion
}