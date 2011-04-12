using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetMigrations.Core;

namespace PerpetualMigrations
{
    public class PerpetualMigrationScriptLocator : IPerpetualMigrationScriptLocator
    {
        private const string PerptualMigrationFoldersAppSetting = "perpetualMigrationFolders";
        private const string ScriptFileNamePattern = "*.sql";
        private readonly IConfigurationManager _configurationManager;

        public PerpetualMigrationScriptLocator()
            : this(new ConfigurationManagerWrapper())
        {
        }

        public PerpetualMigrationScriptLocator(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        /// <summary>
        /// Returns the perpetual script path(s) from the config file.
        /// </summary>
        private IEnumerable<string> GetPaths()
        {
            string configPaths = _configurationManager.AppSettings[PerptualMigrationFoldersAppSetting];

            var paths = Enumerable.Empty<string>();
            if (!string.IsNullOrEmpty(configPaths))
            {
                paths = configPaths.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim());
            }

            return paths;
        }

        /// <summary>
        /// Returns a list of all the perpetual migration script file paths sorted by name.
        /// </summary>
        public IEnumerable<string> GetScripts(ILogger logger)
        {
            IEnumerable<string> scriptPaths = GetPaths();
            IEnumerable<string> scripts = Enumerable.Empty<string>();

            foreach (var path in scriptPaths)
            {
                if (Directory.Exists(path))
                {
                    scripts = scripts.Union(Directory.GetFiles(path, ScriptFileNamePattern));
                }
                else
                {
                    logger.WriteWarning("WARNING: Perptual migration script directory does not exist.{1}Check the config settings or create the directory. [Path: {0}]", path, Environment.NewLine);
                }
            }

            if (scriptPaths.Count() == 0)
            {
                logger.WriteWarning("WARNING: No perpetual migration script paths are defined.{1}Add a '{0}' appSetting with a semicolon delimited list of paths to use.", PerptualMigrationFoldersAppSetting, Environment.NewLine);
            }

            return scripts;
        }
    }
}