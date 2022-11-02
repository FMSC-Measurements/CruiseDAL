# All Database Schema Changes
 1 using the unit test CruiseDAL.V3.Test.CruiseDatastore_V3_Test.Constructor_file_create_withAllTables_test
	create a cruise file for testing updates against and rename it to idicate the current schema version and place 
	into CruiseDAL.V3.Test/TestFile directory. Output from the unit test should tell you where the file created
	during the test was put.

 2 impement the chages to the schema, modifying the table definition, creating the updater, modifying 
	CruiseDatastoreBuilder_V3 and Updater_V3 according to the type of schema change you are making
 3 update `CruiseDatastoreBuilder_V3.DATABASE_VERSION` and `CruiseDALv3SchemaVersion` property in the `src/Directory.Build.props` file
 3 run the src/GenerateV3Models.bat` sript
 4 Create a unit test in the `src/CruiseDAL.V3.Test/Update` dir to test you Updater class 
 

# Add/modify Field To Table
 1 Increment schema version in src/Directory.Build.props and in CruiseDatabaseBuilder_V3.cs
 2 Create a copy of the existing table definition File in Schema dir
 3 Change suffix on table definition file according to next schema version and update the class name on the table definition
 4 Update sql in the new table definition file
 5 Change the table definition in CruiseDatabaseBuilder_V3 to the new table definition class
 6 Create new updater class in CruiseDAL.V3/Update dir and add updater instace to Updater_V3.UPDATES collection
 	- if just adding field then use the SQL command ALTER TABLE
 	- if modifying field you want to to use the DbUpdateBase.RebuildTable method. See comments on method for propper useage


# Remove Fields from Table
Removing a field from a table can be tricky and special consideration needs to be made to how older version software using
the database might behave if the field was removed. If a field was never used directly it might be easier, but consider that 
the CruiseDAL.V3.Models library is auto genterated and will be somewhat dependant on those fields. And the CruiseDAL.V3.Sync 
library uses CruiseDAL.V3.Models to sync and copy data between files. 
In most cases it is probably best to leave the field in the database but remove all code that relies on it. The CruiseDAL.V3.Models.csproj
file has a property (ModelGenIgnoreColumns) that allows you to tell it what fields to not generate properties for.

# Adding Table
 1 Create new folder in t CruiseDAL.V3/Schema for table
 2 Create table definition class in said folder
 3 Add instance of table definition to CruiseDatastoreBuilder_V3
Also if you are adding a table take a look at the CruiseDAL.V3.Sync project to see if you need to update anything. 
Specificly the CruiseCopier CruiseSyncer and DeleteSyncer classes. 
 