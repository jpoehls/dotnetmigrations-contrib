History
=======

 * v0.82
   First official build of the contrib project. Depends on DNM 0.82.
   Adds the PersistentMigrations and TextLog plugins.

PersistentMigrations plugin
===========================

 * Specify folders that contain migration scripts that should be run
   everytime the schema is migrated. (persistentMigrateFolders app setting)

 * Everytime a migrate command is run, all persistent migration scripts are run
   in a separate transaction.

 * Run the 'pmigrate' command directly to execute only the persistent migration scripts.

 * Persistent migrations work great for functions, stored procedures and even views.

   Because persistent migration scripts allow you to keep the full history of an object
   in a single file (rather than each change to the object being in a separate file),
   you can leverage your source contorl's diff and history features
   to manage those object scripts just like you would other source code.

 * Use the ##DNM:VERSION## token in your persistent migration scripts to decide which
   part of the script should run. Here is an example script that drops a table
   if it already exists and only recreates it if the database version is greater than 0.

   IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'People')
   BEGIN
     DROP TABLE [People]
   END

   IF ##DNM:VERSION## > 0
   BEGIN
     CREATE TABLE [People] ([Name] varchar(100))
   END


TextLog plugin
==============

 * Output program information to a text file instead of to the console (STDOUT).