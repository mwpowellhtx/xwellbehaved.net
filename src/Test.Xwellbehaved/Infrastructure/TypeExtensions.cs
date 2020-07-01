using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Xwellbehaved.Infrastructure
{
    // TODO: TBD: ditto Xml comments...
    internal static class TypeExtensions
    {
        /// <summary>
        /// Be sure to clear test events at the appropriate moment, usually during Disposal.
        /// </summary>
        /// <param name="feature"></param>
        public static void ClearTestEvents(this Type feature)
        {
            /* The original timing of this was a bit off. It worked most of the time, but some of
             * the time it would not work on account of inappropriate resource disposal timing,
             * especially when taken in conjunction with a mass of neighboring tests being run in
             * addition to itself. */

            foreach (var path in Directory.EnumerateFiles(GetDirectoryName(feature), "*." + feature.Name))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureType"></param>
        /// <param name="feature"></param>
        public static void EnqueueFeatureForDisposal(this Type featureType, Feature feature)
            => feature.FeatureArtifacts.Add(featureType);

        public static IEnumerable<string> GetTestEvents(this Type feature) =>
            Directory
                .EnumerateFiles(GetDirectoryName(feature), "*." + feature.Name)
                .Select(fileName => new
                {
                    FileName = fileName,
                    Ticks = long.Parse(File.ReadAllText(fileName), CultureInfo.InvariantCulture),
                })
                .OrderBy(@event => @event.Ticks)
                .Select(@event => Path.GetFileNameWithoutExtension(@event.FileName));

        public static void SaveTestEvent(this Type feature, string @event)
        {
            Thread.Sleep(1);
            using (var file = File.Create(Path.Combine(GetDirectoryName(feature), string.Concat(@event, ".", feature.Name))))
            using (var writer = new StreamWriter(file))
            {
                writer.Write(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static async Task SaveTestEventAsync(this Type feature, string @event)
        {
            await Task.Delay(1);
            using (var file = File.Create(Path.Combine(GetDirectoryName(feature), string.Concat(@event, ".", feature.Name))))
            using (var writer = new StreamWriter(file))
            {
                await writer.WriteAsync(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static string GetDirectoryName(this Type feature) =>
            Path.GetDirectoryName(new Uri(feature.GetTypeInfo().Assembly.CodeBase).LocalPath);
    }
}
