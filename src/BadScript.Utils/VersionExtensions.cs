using System;
using System.Globalization;

namespace BadScript.Utils
{

    public static class VersionExtensions
    {
        public static Version ChangeVersion(this Version version, string changeStr)
        {
            string[] subVersions = changeStr.Split('.');
            int[] wrapValues = { ushort.MaxValue, ushort.MaxValue, ushort.MaxValue, ushort.MaxValue };
            int[] original = { version.Major, version.Minor, version.Build, version.Revision };
            int[] versions = { version.Major, version.Minor, version.Build, version.Revision };
            bool[] changeReset = new bool[4];

            for (int i = 4 - 1; i >= 0; i--)
            {
                string current = subVersions[i];

                if (current.StartsWith("("))
                {
                    int j = 0;

                    for (; j < current.Length; j++)
                    {
                        if (current[j] == ')')
                        {
                            break;
                        }
                    }

                    if (j == current.Length)
                    {
                        continue; //Broken. No number left. better ignore
                    }

                    string max = current.Substring(1, j - 1);

                    if (max == "~")
                    {
                        changeReset[i] = true;
                    }
                    else if (int.TryParse(max, out int newMax))
                    {
                        if (i == 0)
                        {
                            continue; //Can not wrap the last digit
                        }

                        wrapValues[i] = newMax;
                    }

                    current = current.Remove(0, j + 1);
                }

                if (i != 0) //Check if we wrapped
                {
                    if (versions[i] >= wrapValues[i])
                    {
                        versions[i] = 0;
                        versions[i - 1]++;
                    }
                }

                if (current == "+")
                {
                    versions[i]++;
                }
                else if (current == "-" && versions[i] != 0)
                {
                    versions[i]--;
                }
                else if (current.ToLower(CultureInfo.InvariantCulture) == "x")
                {
                    //Do nothing, X stands for leave the value as is, except the next lower version part wrapped around.
                }
                else if (current.StartsWith("{") && current.EndsWith("}"))
                {
                    string format = current.Remove(current.Length - 1, 1).Remove(0, 1);

                    string value = DateTime.Now.ToString(format);

                    if (long.TryParse(value, out long newValue))
                    {
                        versions[i] = (int)(newValue % ushort.MaxValue);
                    }
                }
                else if (int.TryParse(current, out int v))
                {
                    versions[i] = v;
                }
            }

            ApplyChangeReset(changeReset, original, versions);

            return new Version(
                versions[0],
                versions[1] < 0 ? 0 : versions[1],
                versions[2] < 0 ? 0 : versions[2],
                versions[3] < 0 ? 0 : versions[3]
            );
        }

        private static void ApplyChangeReset(bool[] changeReset, int[] original, int[] versions)
        {
            for (int j = 0; j < changeReset.Length; j++)
            {
                if (changeReset[j] && versions[j] != original[j])
                {
                    for (int i = j + 1; i < versions.Length; i++)
                    {
                        if (!changeReset[i])
                        {
                            versions[i] = 0;
                        }
                    }
                }
            }
        }
    }

}
