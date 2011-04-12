using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetMigrations.Core;

namespace PersistentMigrations
{
    public class PersistentMigrationScriptLocator : IPersistentMigrationScriptLocator
    {
        private const string PerptualMigrationFoldersAppSetting = "persistentMigrateFolders";
        private const string ScriptFileNamePattern = "*.sql";
        private readonly IConfigurationManager _configurationManager;

        public PersistentMigrationScriptLocator()
            : this(new ConfigurationManagerWrapper())
        {
        }

        public PersistentMigrationScriptLocator(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        /// <summary>
        /// Returns the persistent script path(s) from the config file.
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
        /// Returns a list of all the persistent migration script file paths sorted by name.
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
                    logger.WriteWarning("WARNING: Persistent migration script directory does not exist.{1}Check the config settings or create the directory. [Path: {0}]", path, Environment.NewLine);
                }
            }

            if (scriptPaths.Count() == 0)
            {
                logger.WriteWarning("WARNING: No persistent migration script paths are defined.{1}Add a '{0}' appSetting with a semicolon delimited list of paths to use.", PerptualMigrationFoldersAppSetting, Environment.NewLine);
            }

            return scripts;
        }
    }
}