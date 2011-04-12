using System.Collections.Generic;
using DotNetMigrations.Core;

namespace PerpetualMigrations
{
    public interface IPerpetualMigrationScriptLocator
    {
        /// <summary>
        /// Returns a list of all the perpetual migration script file paths sorted by name.
        /// </summary>
        IEnumerable<string> GetScripts(ILogger logger);
    }
}