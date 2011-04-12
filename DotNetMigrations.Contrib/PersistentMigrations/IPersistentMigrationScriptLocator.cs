using System.Collections.Generic;
using DotNetMigrations.Core;

namespace PersistentMigrations
{
    public interface IPersistentMigrationScriptLocator
    {
        /// <summary>
        /// Returns a list of all the persistent migration script file paths sorted by name.
        /// </summary>
        IEnumerable<string> GetScripts(ILogger logger);
    }
}