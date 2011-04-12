using System;
using System.Data.Common;
using System.IO;
using DotNetMigrations.Core;

namespace PersistentMigrations
{
    public class PersistentMigrateCommand : DatabaseCommandBase<DatabaseCommandArguments>, IPostMigrationHook
    {
        private readonly IPersistentMigrationScriptLocator _scriptLocator;

        public bool ShouldRun(MigrationDirection direction)
        {
            return true;
        }

        public PersistentMigrateCommand() : this(new PersistentMigrationScriptLocator()) { }

        public PersistentMigrateCommand(IPersistentMigrationScriptLocator scriptLocator)
        {
            _scriptLocator = scriptLocator;
        }

        public void OnPostMigration(DatabaseCommandArguments args, MigrationDirection direction)
        {
            base.Run((IArguments)args);
        }

        protected override void Run(DatabaseCommandArguments args)
        {
            const string versionVariableName = "##DNM:VERSION##";
            var version = base.GetDatabaseVersion();

            Log.WriteLine("Executing persistent migrations...");
            var scripts = _scriptLocator.GetScripts(Log);

            using (DbTransaction tran = Database.BeginTransaction())
            {
                string currentScript = null;
                try
                {
                    foreach (var script in scripts)
                    {
                        currentScript = script;
                        Log.WriteLine("  {0}", Path.GetFileName(script));

                        string sql = File.ReadAllText(script);
                        sql = sql.Replace(versionVariableName, version.ToString(), StringComparison.OrdinalIgnoreCase);
                        Database.ExecuteScript(tran, sql);
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    string filePath = currentScript ?? "NULL";
                    throw new ApplicationException("Error executing persistent migration script: " + filePath, ex);
                }
            }
        }

        public override string CommandName
        {
            get { return "pmigrate"; }
        }

        public override string Description
        {
            get { return "Executes all of the persistent migration scripts."; }
        }
    }
}