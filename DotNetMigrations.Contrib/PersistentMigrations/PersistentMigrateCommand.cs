using System;
using System.Data.Common;
using System.IO;
using DotNetMigrations.Core;

namespace PersistentMigrations
{
    public class PersistentMigrateCommand : DatabaseCommandBase<DatabaseCommandArguments>, IPostMigrationHook
    {
        private readonly IPersistentMigrationScriptLocator _scriptLocator;

        public PersistentMigrateCommand() : this(new PersistentMigrationScriptLocator()) { }

        public PersistentMigrateCommand(IPersistentMigrationScriptLocator scriptLocator)
        {
            _scriptLocator = scriptLocator;
        }

        public void OnPostMigration(DatabaseCommandArguments args)
        {
            base.Run((IArguments)args);
        }

        protected override void Run(DatabaseCommandArguments args)
        {
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
                        Database.ExecuteScript(tran, File.ReadAllText(script));
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