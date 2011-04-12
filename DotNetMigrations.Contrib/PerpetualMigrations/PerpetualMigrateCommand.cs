using System;
using System.Data.Common;
using System.IO;
using DotNetMigrations.Core;

namespace PerpetualMigrations
{
    public class PerpetualMigrateCommand : DatabaseCommandBase<DatabaseCommandArguments>, IPostMigrationHook
    {
        private readonly IPerpetualMigrationScriptLocator _scriptLocator;

        public PerpetualMigrateCommand() : this(new PerpetualMigrationScriptLocator()) { }

        public PerpetualMigrateCommand(IPerpetualMigrationScriptLocator scriptLocator)
        {
            _scriptLocator = scriptLocator;
        }

        public void OnPostMigration(DatabaseCommandArguments args)
        {
            base.Run((IArguments)args);
        }

        protected override void Run(DatabaseCommandArguments args)
        {
            Log.WriteLine("Migrating perpetuals...");
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
                        Database.ExecuteScript(tran, script);
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    string filePath = currentScript ?? "NULL";
                    throw new ApplicationException("Error executing perpetual migration script: " + filePath, ex);
                }
            }
        }

        public override string CommandName
        {
            get { return "pmigrate"; }
        }

        public override string Description
        {
            get { return "Executes all of the perpetual migration scripts."; }
        }
    }
}