# Summary of Version 3 changes
### Split out tree measurement columns into TreeMeasurments table 
The fields that had made up the Tree table have been split out into two tables. This is done for several reasons. The fields in each table are subject to being changed for different reasons. so separating them makes since from a data management perspective as well as for merging data bases. When merging if two trees are the same but have different measurments it is easier to take one measurment over the other. 
 - FiaCode removed, now being read from TreeDefaultValue table
 - CountMeasure column should be considered obsolite but being kept on Tree_V3 for compatibility with updated databases only. 

# New Stuff 

## Tables
### Species
Single field table for referencing valid species codes. This table will allow us to maintain constancy through out the database. Updates to this table should cascade to all table that reference it. 
### TallyLedger
store all edits to tree counts, or KPI. This table will be populated using the CountTree table and Tree tables

### TallyDescription
Table stores description of tally. 

### TallyHotKey
Table stores hotkey values for tallies. HotKey values must be unique per stratum value. I did consider consolidating TallyHotKey and TallyDescription tables however to enforce the unique constraint per stratum as well as to allow for unique HotKey configuration per file I decided to spererate out the data.

### SamplerState
Table for storing the state of samplers. This data was moved to a separate table from sampleGroup because it changes often. And making it separate would make merging file easier, as well as protecting the SampleGroup table from undesired modification  . 

### TreeAuditResolution 
When a tree audit needs to be suppressed, that is, someone has resolved it an entry can be added to the TreeAuditResolution table indicating the TreeAuditRule and the Tree that the resolution applies to

### TreeField
because of the in database audits we need to be able to query the list of valid tree fields. this table is also useful for maintaining consistancy with the treeFieldSetup and TreeFieldSetupDefaults tables

### TreeFieldValue
treeFieldValue is a table that allows storing values for trees in Entity Attribute Value (EAV) form. 
There are seperate fields for storing values of different types, although sqlite allows storing and type in a field, I chose not to rely on that and that may cause issues when pulling data out of the database.

## Views

### TreeAuditError view
for each tree record we populate a list of valid tree audit rules 
joined with a table that provides a row for each value on a tree record
we can then use the field value and check it with the audit rule to get a list of all errors
then join the list of errors with TreeAuditResolution to see if the errors have been resolved

### TallyPopulation view
A view for the connivance of accessing a list of populations that are being tallied. Previously a records needed to be created depending on if the user was tallying by species or not. 

### TreeFieldValue_TreeMeasurment, TreeFieldValue_All
TreeFieldValue_TreeMeasurment is a view that provides data from the treeMeasurment table in the same format as TreeFieldValue
TreeFieldValue_All combines TreeFieldValue and TreeFieldValue_TreeMeasurment allowing both to be accessed as a single source. 


# Modified Tables
These are tables from the original schema that have been significatly modified and given a different name 

 - CuttingUnitStratum -> CuttingUnit_Stratum
 - Log -> Log_V3
 - Plot => Plot_V3, Plot_Stratum
 - SampleGroupTreeDefaultValue => SubPopulation
 - SampleGroup => SampleGroup_V3
 - Tree => Tree_V3, TreeMeasurment
 - TreeAuditValue => TreeAuditRule
 - TreeDefaultValueTreeAuditValue => TreeDefaultValue_TreeAuditRule
 - LogFieldSetup => LogFieldSetup_V3
 - TreeFieldSetup => TreeFieldSetup_V3
 - FixCNTTallyClass => FixCNTTallyClass_V3
 - FixCNTTallyPopulation => FixCNTTallyPopulation_V3
 - ErrorLog => (see below about tbl_ErrorLog)

### CuttingUnit_Stratum
uses StratumCode and CuttingUnitCode instead of Stratum_CN and CuttingUnit_CN 
### Log_V3
change Tree_CN to TreeID

### Plot_Stratum
 - remove fields dependent on stratum from the plot table, making it easier to prevent inconsistent data on plots
 
### Plot_V3 
 - removed fields dependent on stratum, changed
 
### SubPopulation
 - removes reference on TreeDefaultValue
 - changed SampleGroup_CN to (SampleGroupCode, StratumCode)  
 
### SampleGroup_V3
 - removed samplerState , 
 - changed Stratum_CN to StratumCode

TallyBySubPop value for original file is ignored. Instead the value is determined automatically by detecting if that samplegroup has tally setup with tree defaults.
- CutLeave defaults to "C"
- ? UOM defaults to empty string. It might be preferable to use null as the default value this would better reflect our indended behavior of having UOM be set at the Sale level and allowing it to be overridden at the sample group level 
- DefaultLiveDead defaults to "L"
- PrimaryProduct defaults to empty string

### Tree_V3
 - remove measurment fields 
 - change Tree_GUID to TreeID 
 - change CuttingUnit_CN to CuttingUnitCode 
 - change Stratum_CN to StratumCode
 - change SampleGroupCode to (StratumCode, SampleGroupCode)
 - change Plot_CN to (PlotNumber, CuttingUnitCode) 

### TreeMeasurments
take over the measurment fields from tree talbe

### TreeDefaultValue_TreeAuditRule
- change TreeDefaultValue_CN to (species, LiveDead, PrimaryProduct)
- change TreeAuditValue_CN to TreeAuditRuleID

### LogFieldSetup_V3
 - removed unused fields 
 - changed Stratum_CN to StratumCode, 
Note this table has no backport

### TreeFieldSetup_V3 
 - removed unused fields 
 - changed Stratum_CN to StratumCode 
 note this table has no backport

### FixCNTTallyClass_V3 
 - changed Stratum_CN to StratumCode, 
 - added unique constraint for StratumCode, 
 - changed type of Field to text(this is how it was being stored anyways))

### FixCNTTallyPopulation_V3 
 - removed reference to FixCNTTallyClass_CN because we can use StratumCode instead, 
 - changed SampleGroup_CN to (StratumCode and SampleGroupCode)

### LogGradeAuditRule_V3
 - Split out Valid Grades. This is a slightly better design and allows for audits to be preformed by just querying the data. Also don't store multiple values in on field
 - changed from using 'ANY' to indicate that rule applies to all species values, to NULL indicate that value applies to all species values

### TreeAuditRule
 - removed ErrorMessage (this field hadn't been used in a long time)
 - added TreeAuditRuleID, a unique identifier for audit rules

### tbl_ErrorLog
This is a bit of an odd duck. This table is the same as the old ErrorLog table but since ErrorLog is a view now with mostly records that get generated. tbl_ErrorLog stores any data that is written back to the view



# Old tables with read-only back-ports
Tables that have been significantly modified have been given views to allow for backwards compatibility allowing CruiseProcessing to work with the new data structure. Many of these views can't be witten back to, but we are working under the assumption that CruiseProcessing doesn't write back to them 
 - CountTree
 - CuttingUnitStratum
 - Plot
 - SampleGroup
 - SampleGroupTreeDefaultValue
 - Tree
 - Log
 - TreeDefaultValueTreeAuditValue
 - TreeEstimate

# Tables with no changes

- All Processing tables (except: LogStock, and TreeCalculatedValue)


# Tables with only minor changes
Where possible, some tables have been left with out any significant changes. These tables may not match up with new naming conventions. 

- Sale
- CuttingUnit
- Stratum 
- TreeDefaultValue 
- StratumStats
- SampleGroupStats
- TreeCalculatedValues

In some places fields that had been marked as `NOT NULL` have been changed to have a default value instead. This has been to make the database easier to use and test while maintaining the requirement that those fields always have a non-null value. 
Tables that have a `Code` or `[tableName]Code`, `Species`, `LiveDead` or `PrimaryProduct` key value have been modified so those fields use a `COLLATE NOCASE`. This makes it so that those key fields are treated as case insensitive. 

## Sale
 - Region, Forest, District changed `not-null` to `default ''`
 
## CuttingUnit
 - removed field TallyHistory
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
 - date and time fields now auto populate with current date and time
 - level auto-populates with 'N' for normal
 - level, and program collates nocase

## Globals
 - block defaults to ''
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

 [x] ? Separate table for all hot-keys (TallyHotKey) 
 [] figure out update cascades 
 [x] redesign fixcnt tables?

 [x] whats up with LogFieldSetupDefault.FieldName, TreeFieldSetupDefault has it too! : these fields are not used by cruise manager so I guess they are safe to remove
 [x] change tallyPoplulation to a view that populates from subPopulation with SampleGroup_v3.tallyBySpecies as a condition
 [] change PlotNumber references to PlotID to prevent issues when merging
[x] TreeEstimate view doesn't really work well because it needs to references CountTree which is also a view
[x] dont migrate CuttingUnit.TallyHistory
[x] implement tree auditing within the database
? implement systematic sample selection using just the database
[] create indexes
 [] figure out the best null solution for species codes( tables that need a null option for species: TallyHotKey, TallyDescription, LogGradeAuditRule (extra special because needs to indicate value is for ANY species), TreeAditValue
 [] LogGradeAuditRule reference Species
 [] fIXcnt figure out why FieldName was integer, and fix so it can reference new TreeField talbe - 
 [] use queries to return non-treeAuditRule errors through the TreeError view i.e. checks that tree has a height and diameter 
 [] remove heading from TreeFieldSetup and LogFieldSetup
 [] LogGradeAuditResolution
 [x] only return treeCount if plot tree
 [] remove support for updating pre-2.1.1 version
 [] TallyPopulation as a table or as a veiw?
 [] plot error view
 [] log error view (no log grade audits)
 [] dont migrate if species, livedead, prod is inconsistent with TDV

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

# migration notes 
## Tree
 - generally will return same values and types as original tree table, without additional fields
 - TreeID in the new file will be the same as Tree_GUID the old file
 
### differences: 
 - additional column is TreeID which is a shortcut to the id of the tree in the new Tree_v3 table
 - Values for Tree_GUID may be populated if null in the original file
 - modifiedDate values will be different
 - species will be different if there were tree records in the original file where the species on the tree record didn't match up with the species on the treeDefaultValue 

# TreeFieldSetup, LogFieldSetup, TreeFieldSetupDefault, LogFieldSetupDefault 
It may be possible that some files have tree field setups that use invalid or no longer supported fields. These fields wont be migrated over.

# Tree species and LiveDead
in the previous database it is possible for species and live dead values to not match up with the tree default value on a tree. This could cause a tree to be moved into a different population during the migration process. This is why we audit the database before hand and see that is free of errors. 





