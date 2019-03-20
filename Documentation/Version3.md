# New Stuff 
## Species
Single field table for referencing valid species codes. This table will allow us to maintain constancy through out the database. Updates to this table should cascade to all table that reference it. 
## TallyLedger
store all edits to tree counts, or KPI. This table will be populated using the CountTree table and Tree tables

## TallyPopulation view
A view for the connivance of accessing a list of populations that are being tallied. Previously a records needed to be created depending on if the user was tallying by species or not. 

## TallyDescription
Table stores description of tally. 

## TallyHotKey
Table stores hotkey values for tallies. HotKey values must be unique per stratum value. I did consider consolidating TallyHotKey and TallyDescription tables however to enforce the unique constraint per stratum as well as to allow for unique HotKey configuration per file I decided to spererate out the data.

## SamplerState
Table for storing the state of samplers. This data was moved to a separate table from sampleGroup because it changes often. And making it separate would make merging file easier, as well as protecting the SampleGroup table from undesired modification  . 
 
# Modified New Tables
 - CuttingUnit_Stratum (was CuttingUnitStratum)
 - Log_V3 (was Log)
 - Plot_Stratum (was part of Plot)
 - Plot_V3 (was part of Plot)
 - SubPopulation (was SampleGroupTreeDefaultValue)
 - SampleGroup_V3 (was SampleGroup)
 - Tree_V3 (was part of Tree)
 - TreeMeasurments (part of Tree) 
 - SubPopulation_TreeAuditValue (was TreeDefaultValueTreeAuditValue)
 - LogFieldSetup_V3 ( removed unused fields and changed Stratum_CN to StratumCode, no backport? )
 - TreeFieldSetup_V3 ( removed unused fields and changed Stratum_CN to StratumCode, no backport? )
 - FixCNTTallyClass_V3 (changed Stratum_CN to StratumCode, added unique constraint for StratumCode)
 - FixCNTTallyPopulation_V3 (removed reference to FixCNTTallyClass_CN because we can use StratumCode instead, changed SampleGroup_CN to StratumCode and SampleGroupCode)

## SampleGroup_V3
TallyBySubPop value for original file is ignored. Instead the value is determined automatically by detecting if that samplegroup has tally setup with tree defaults.

## Tree 

### Version 3 changes
 - Split out measurement columns into TreeMeasurments table 
 - FiaCode removed, now being read from TreeDefaultValue table
 - CountMeasure column should be considered obsolite but being kept on Tree_V3 for compatibility with updated databases only. 

# Old tables with read-only back-ports
Tables that have been significantly modified have been given views to allow for backwards compatibility
 - CountTree
 - CuttingUnitStratum
 - Plot
 - SampleGroup
 - SampleGroupTreeDefaultValue
 - Tree
 - Log
 - TreeDefaultValueTreeAuditValue
 - TreeEstimate

# Tables with no major changes

 - Sale
 - CuttingUnit
 - Stratum 
 - TreeDefaultValue 
 - FixCNTTallyClass
 - FixCNTTallyPopulation



## TreeAuditValue 
 - added `TreeAuditValueID TEXT` field




# Tables with only minor changes
Where possible, some tables have been left with out any significant changes. These tables may not match up with new naming conventions. 

In some places fields that had been marked as `NOT NULL` have been changed to have a default value instead. This has been to make the database easier to use and test while maintaining the requirement that those fields always have a non-null value. 
Tables that have a `Code` or `[tableName]Code`, `Species`, `LiveDead` or `PrimaryProduct` key value have been modified so those fields use a `COLLATE NOCASE`. This makes it so that those key fields are treated as case insensitive. 

## Sale
 - Region, Forest, District changed `not-null` to `default ''`
 
## CuttingUnit
 - Code add `COLLATE NOCASE`
 - Area changed `NOT NULL` to `default 0.0`
 
## Stratum 
 - Code add `COLLATE NOCASE` 
 - Method changed `NOT NULL` to `DEFAULT ''`
 
## TreeDefaultValue 
 - PrimaryProduct, Species, LiveDead, TreeGrade added `COLLATE NOCASE`
 - TODO: add index on PrimaryProduct, LiveDead, Species since generally these are treated as the compound key for the table. 

## LogStock
 - deletes from tree will cascade to LogStock
 - modifiedDate no longer updated on modifications, because table doenst really need to track modifications

## SampleGroupStatsTreeDefaultValue
 - TreeDefaultValue and SampleGroupStats deletes cascade

## StratumStats
 - deletes from stratum cascade 
 - added not null to `stratum_cn` 

## SampleGroupStats 
 - deletes from StratumStats cascade

## MessageLog
 - added new key field MessageLogID (required)
 - date and time fields now auto populate with current date and time
 - level auto-populates with 'N'
 - level collates nocase

## Globals
 - block defaults to 'Database'
 - add collate nocase to block, and key

# Removed depreciated fields 
## TreeDefaultValue 
 - Chargeable

## TreeAuditValue 
 - ErrorMessage 

## TreeMeasurment
 - UpperStemDOB 





# Removed Tables with no back ports
## Stem
It looks like this table wasn't properly designed. The intention of this table was for measuring multi-stems on trees, however, in its current design a tree can only have one stem. Once it is properly redesigned it can be added back to the schema.   

## Tally 
Initially this table made it hard to define tally populations using the CountTree table. The fields that it provided have now been folded into the TallyPopulation table





# TODO 
- ? Separate table for all hot-keys (Stratum_HotKey) 
- figure out update cascades 
- redesign fixcnt tables?
- LogGradeAuditRule reference Species
- whats up with LogFieldSetupDefault.FieldName, TreeFieldSetupDefault has it too!
- change tallyPoplulation to a view that populates from subPopulation with SampleGroup_v3.tallyBySpecies as a condition
- change PlotNumber references to PlotID to prevent issues when merging
- TreeEstimate view doesn't really work well because it needs to references CountTree which is also a view
- dont migrate CuttingUnit.TallyHistory
- implement tree auditing within the database
- implement systematic sample selection using just the database
- create indexes

## Tree
 - only return treeCount if plot tree

## CountTree 
 ? issues if countTree returns treecount > 0 for plot tally populations
 - only read tree counts from tallyLedger for non plot trees

## Tree_V3
 - create index on tree number

## Log_V3
 - create index on log number

## Plot_V3
 - create index on PlotNumber

#Recomended Changes
 - remove duplicated fields from StratumStats

# API changes 

 - added overload method  DAL.AttachDatabase(string dbPath, string alias)
 - removed internal property DAL.SchemaVersion
 - removed property DAL.User

## QueryGeneric 
a generic version of the Query method that returns a IDictionary<string, object> for each row in the query.

## CruiseDAL DbConnection extentions
many methods that had previously only been available through the DAL class are new available as extension methods on DbConnection. The purpose of this change is to make avalible some DAL utility methods available when working directly with database connection and transaction objects. This is useful when updating or modifying the database schema, which requires that commands be ran on a connection before starting a transaction. 
Additionally it is useful for testing as well as for flexibility of the code to have the ability to execute utility methods directly on a connection. 
New methods are: 
 - LogMessage(this DbConnection connection, string message, string level, DbTransaction transaction)
 - string ReadDatabaseVersion(this DbConnection connection)
 - string ReadGlobalValue(this DbConnection connection, String block, String key, DbTransaction transaction)
 - WriteGlobalValue(this DbConnection connection, String block, String key, String value, DbTransaction transaction)

## DatabaseVersion 
origionaly database version has been populated when the database is first initialized or when it has been updated. 
now the DatabaseVersion will read directly from the value stored in the database

# Behavior changes
## Foreign Keys on by default
By default the Sqlite has foreign keys turned off. Previously CruiseDAL didn't do anything to change the default behavior of foreign keys. So when foreign keys were broken the database would allow it. The only check on foreign keys would be when opening files, the application would run a check on foreign keys and alert the user if there were errors. This would lead a lot of extra work providing support to users. 
Starting in version 3 foreign keys will be set on by default. The new database design will make use of cascading deletes and updates where appropriate to resolve broken foreign keys. 

# Assumptions
- species codes are not case sensitive
- stratum, sampleGroup and cuttingUnitCodes should not be case sensitive
- cruise method should not be case sensitive

# Conventions
 - Code fields i.e. SampleGroupCode, StratumCode are not case sensitive
 - ID fields i.e. are used for storing guid values but in the case of migrating data from an older file a value will be generated from the rowid of the source record
 - CreatedDate fields are auto populated using DEFAULT (datetime('now', 'localtime'))
 - boolean fields are auto populated with DEFAULT 0
 - BOOLEAN fields are considered false if == 0 and true if != 0
 - CreatedBy fields default to ''

# migration guarintees 
## Tree
 - generally will return same values and types as original tree table, without additional fields
 - TreeID in the new file will be the same as Tree_GUID the old file
 
### Exceptions: 
 - additional column is TreeID which is a shortcut to the id of the tree in the new Tree_v3 table
 - Values for Tree_GUID may be populated if null in the original file
 - modifiedDate values will be different
 - species will be different if there were tree records in the original file where the species on the tree record didn't match up with the species on the treeDefaultValue 



