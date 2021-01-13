# Summary of Version 3 changes

Version 3 has the following primary goals: remove redundancies between tables, consolidate storage of tree counts, divorce storage of tree counts from tally setup, allow storing multiple cruises in the same database, and provide better support for syncing data between multiple files.

Secondary goals: remove the need to save validation results back to the database, separate tree data from tree default data.

# schema compatibility
 - sqlite considers all null values different from all other null values.


# New Tables and Views

## Tables

### Cruise

### Plot_Stratum 
Previously the plot table had a reference to the Stratum table. This caused duplicated data when a plot had multiple strata. 
After the stratum reference was removed from the Plot table this table was created to fill the need of having a Plot Stratum relationship.

### SpeciesCode
Single field table for referencing valid species codes. This table will allow us to maintain constancy through out the database. Updates to this table should cascade to all table that reference it.
The table was named `SpeciesCode` instead of `Species` because of the common field name `Species` was causing some issues... i think.

### FIA 


### TallyLedger

Running log of all changes to tree count, and KPI totals.

### TallyDescription
Table stores description of tally.

### TallyHotKey
Table stores hotkey values for tallies. HotKey values must be unique per stratum value. I did consider consolidating TallyHotKey and TallyDescription tables however to enforce the unique constraint per stratum as well as to allow for unique HotKey configuration per file I decided to separate out these two fields.

### Device
Table for saving information specific to each device that is using the cruise.
This table is reference by the samplerState to allow each device to have its own sampling states.

### SamplerState
Table for storing the state of samplers. This data was moved to a separate table from sampleGroup because it changes often. Because sampler state is specific to a single file and shouldn't be merged it should be kept separate  from other sample group data, as well keeping is separate will help ensure sample group data is protected from undesired modification.

### TreeAuditResolution
When a tree audit warnings need to be suppressed, i.e. someone has resolved the warning. An entry can be added to the TreeAuditResolution table indicating the TreeAuditRule and the Tree that the resolution applies to.

### TreeField
Because of the in database audits the database needs to be aware of all the valid tree fields. Also for providing fields that can be used with TreeFieldSetup and TreeFieldSetupDefaults, and TreeFieldValue tables.

### TreeFieldValue
TreeFieldValue is a table that allows storing values for trees in Entity Attribute Value (EAV) form.
There are separate fields for storing values of different types, although sqlite allows storing and type in a field, I chose not to rely on that and that may cause issues when pulling data out of the database.

## Views

### TreeAuditError view
Rather than requiring the client to perform validation and then save the results back to the database.
The TreeAuditError view performs the validation, checks to see if it is marked as resolved and returns the
results as the view.
for each tree record we populate a list of valid tree audit rules
joined with a table that provides a row for each value on a tree record
we can then use the field value and check it with the audit rule to get a list of all errors
then join the list of errors with TreeAuditResolution to see if the errors have been resolved

### TallyPopulation view
A view for the connivance of accessing a list of populations that are being tallied. Previously a records needed to be created depending on if the user was tallying by species or not.

### TreeFieldValue_TreeMeasurment, TreeFieldValue_All
TreeFieldValue_TreeMeasurment is a view that provides data from the treeMeasurment table in the same format as TreeFieldValue
TreeFieldValue_All combines TreeFieldValue and TreeFieldValue_TreeMeasurment allowing both to be accessed as a single source.

### TallyLedger_Totals, TallyLedger_Plot_Totals, TallyLeddger_Tree_Totals views
The purpose of these views is to provide treecount and KPI totals from the tallyLeddger table and provide a single dependency to filter deleted records

#### TallyLedger_Totals
provides tree count and kpi sums at the tally population level.
used by the CountTree view

#### TallyLedger_Plot_Totals
provides tree counts and kpi sums at the plot level

#### TallyLeddger_Tree_Totals
provides tree counts at the tree level. because trees can have multiple tally ledger records this is useful.
used by the Tree view


# Modified Tables
These are some tables from the original schema that have been significantly modified, split up, or Renamed.
Here is a list of modified Version 2 tables, with the version 3 tables they correspond to:

 - CuttingUnitStratum -> CuttingUnit_Stratum
 - Plot => Plot and Plot_Stratum
 - SampleGroup => SampleGroup, SamplerState
 - SampleGroupTreeDefaultValue => SubPopulation
 - Tree => Tree, TallyLedger, TreeMeasurment and TreeCalculatedValues
 - CountTree => TallyLedger, SampleGroup 
 - TreeAuditValue => TreeAuditRule
 - TreeDefaultValueTreeAuditValue => TreeAuditRuleSelector
 - FixCNTTallyClass => Stratum
 - ErrorLog => (see below about tbl_ErrorLog)


### Plot
 - removed fields dependent on stratum. Stratum dependent fields are now in Plot_Stratum

### SubPopulation
 - removes reference to TreeDefaultValue
 - changed SampleGroup_CN to (SampleGroupCode, StratumCode)  

### SampleGroup
 - removed samplerState ,
 - changed Stratum_CN to StratumCode
 - CutLeave defaults to "C"
 - ? UOM defaults to null. It might be preferable to use null as the default value this would better reflect our indented behavior of having UOM be set at the Sale level and allowing it to be overridden at the sample group level
 - DefaultLiveDead defaults to "L"
 - PrimaryProduct defaults to null

#### TallyBySubPop
When setting up tally populations users are given a choice to either tally by species or by sample group. 
In version 2 the user would make their choice and then nessicary records would be created in the CountTree table reflecting their choice.
When reopening a cruise file their selected choice would have to be inferred by analyzing the CountTree table. 
 

Version 3 explicitly stores this choice using the TallyBySubPop flag. Where `Tally By SubPop` is the same as what was `Tally by Species` 

In Version 2 to Version 3 conversion TallyBySubPop value from original file is ignored. Version 2 had a TallyBySubPop field but wasn't used because it could differ from what was inferred and that could cause issues. Instead the Version 3 TallyBySubPop value is determined by using the same process of inferring from the CountTree table used by Cruise Manager. 


### Tree
 - remove measurment fields
 - change Tree_GUID to TreeID

#### Split out tree measurement columns into TreeMeasurments table
The fields that had made up the Tree table have been split out into two tables. This has been done primarily for the purpose of tracking and segregating changes while syncing cruise files. The advantages of having the tree table split up give to syncing files is to ensure that changes to tree data and changes to the identity of the tree can be isolated, that we can track those changes separately, and we can sync tree data without worrying about side effects from changes to identifying values.
Additionally there's the consideration that it may be better to store tree field data as attributes, the way the TreeFieldValues_ALL view provides. By compartmentalizing tree data we have more flexibility with our data structure, should the need for custom tree fields materialize.
Some additional notes about this change:

 - The FiaCode field has been removed. This field is redundant and should be retrieved from the TreeDefaultValue table.
 - KPI, STM, and TreeCount have been moved to the TallyLedger table to remove redundancies.
 - CountMeasure column is only being used for plot based cruises and insurance trees, when adding a tree the default value for CountMeasure is 'M'
 - A check has been added to ensure that count measure values are either 'C', 'M', or 'I'. case is ignored
 - A check has been added to ensure that LiveDead values are either 'L', 'D' or null. case is ignored


### TreeMeasurments
take over the measurement fields from tree table

### TreeDefaultValue_TreeAuditRule
- change TreeDefaultValue_CN to (species, LiveDead, PrimaryProduct)
- change TreeAuditValue_CN to TreeAuditRuleID

### LogFieldSetup_V3
 - removed unused fields
 - changed Stratum_CN to StratumCode,
Note this table has no backport

### TreeFieldSetup
 - removed unused fields
 - Added SampleGroupCode
 - Added DefaultValues
 - Added IsHidden flag

#### Expanding the function of TreeFieldSetup
TreeFieldSetup has taken on some of the functionality of Tree Default Values. 
We have added the ability to set DefaultValues on fields in FieldSetup. 
This adds a second method to set up default values on trees that suplements the features provided by the TreeDefaultValue table.
The purpose of this is to allow defining default values at the SampleGroup or Stratum level independent of species or product. 


Note this table has no backport


### LogGradeAuditRule
 - Split out Valid Grades. This is a slightly better design and allows for audits to be preformed by just querying the data. Also don't store multiple values in on field
 - changed from using 'ANY' to indicate that rule applies to all species values, to NULL indicate that value applies to all species values

### TreeAuditRule
 - removed ErrorMessage (this field hadn't been used in a long time)
 - added TreeAuditRuleID, a unique identifier for audit rules

### TreeAuditRuleSelector
Before tree audit rules linked to a tree based on the TreeDefaultValue on the tree. 
Now rule are linked to a tree based on Species, LiveDead, or Product. 



## General Changes

### Look up tables
Lookup tables have been created for 
 - CruiseMethods
 - FIA
 - Log Fields
 - Logging Methods
 - Products
 - Regions
 - Tree Fields
 - UOM Codes

Tables that use these field now reference those look up tables and their values are enforced using foreign keys. 

### Cascading Updates and Deletes
Foreign Keys on most tables have been setup to cascade deletes and updates. 
There are some cases where cascading deletes or updates have not been setup. Such as tables referencing lookup tables where we don't expect records to be deleted from a lookup table. 

Cascading updates have not been setup where we never expect a key to change. Such as keys that use a GUID.  



# Removed depreciated fields
## TreeDefaultValue
 - Chargeable

## TreeAuditValue
 - ErrorMessage

## TreeMeasurment
 - UpperStemDOB





# Removed Tables with no back ports

## Tally
Initially this table made it hard to define tally populations using the CountTree table. The fields that it provided have now been folded into the TallyPopulation table





# TODO


 - [] figure out update cascades



? implement systematic sample selection using just the database
 - [] create indexes
 - [] figure out the best null solution for species codes( tables that need a null option for species: TallyHotKey, TallyDescription, LogGradeAuditRule (extra special because needs to indicate value is for ANY species), TreeAditValue


 - [!] remove heading from TreeFieldSetup and LogFieldSetup
 - [!] LogGradeAuditResolution

 - [!] remove support for updating pre-2.1.1 version - although we could keep this ability, the primary issue is that triggers might not be all there for older converted versions.

 - [?] log error view (non log grade audits)
 - [!] dont migrate if species, livedead, prod is inconsistent with TDV - added auto fix to fix mismatched livedead to solution that auto fixes mismatched species

 - [] add method to tallyPopulation?


 - [!] override TallyBySubPop in TallyPopulation View when sg has only one species


 - [!] implement logical deletes
 - [] if using treeID for tallyLedgerID aswell, update migration command
 - [] allow multiple tally ledgers per tree

 - [x] indicate tally by clicker in SG, subpop or SampleSelector - added UseExternalSampler to sample group
 - [x] add view for easy access to tally population tree counts
 - [x] use queries to return non-treeAuditRule errors through the TreeError view i.e. checks that tree has a height and diameter
 - [x] check IsFallBuckScale was bool or 'y'/'n' - was integer now bool, added in 2.1.2
 - [x] change type on stm to bool
 - [x] redesign fixcnt tables?
 - [x] ? Separate table for all hot-keys (TallyHotKey)
 - [x] whats up with LogFieldSetupDefault.FieldName, TreeFieldSetupDefault has it too! : these fields are not used by cruise manager so I guess they are safe to remove
 - [x] change tallyPoplulation to a view that populates from subPopulation with SampleGroup_v3.tallyBySpecies as a condition
 - [x] only return treeCount if plot tree
 - [x] plot error view
 - [x] LogGradeAuditRule reference Species
 - [x] fIXcnt figure out why FieldName was integer, and fix so it can reference new TreeField talbe - we were using a enum to access fieldName
 - [x] TreeEstimate view doesn't really work well because it needs to references CountTree which is also a view
 - [x] dont migrate CuttingUnit.TallyHistory
 - [x] implement tree auditing within the database

 - [86] change PlotNumber references to PlotID to prevent issues when merging

## Tree_V3
 - [x] create index on tree number

## Log_V3
 - [x] create index on log number

## Plot_V3
 - [x] create index on PlotNumber

#Recomended Changes
 - remove duplicated fields from StratumStats


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
 - treeCount will always be 0 on tree records for non-plot trees

#count tree
## guarintees
 - CountTree_CN will remain consistant across reads unless TallyLedger records are deleted. CountTree_CN is taken from the lowest TallyLedger_CN value in a population.

## diferences
 - tally_CN will always be 0
 - component_CN will always be 0
 - createdBy will always be an empty string
 - CreatedDate will always be an empty string
 - modifiedBy will always be null
 - modifiedDate will always be null
 - number of countTree records may be different. this may be because of
	 - consolidation of treeCounts across components
	 - filling in of counts based on tally setup (either by populations that had already been set up but had a missing countTree on a unit or by populations that were given a default tally setup)

# ErrorLog
In V3 most of the validation will be handled within the database using views to query the data and return detected errors rather than reading the data out of the database and performing audits in the client and saving the results of the audits back to the database.
For backwards compatibility errorlogs can still be saved back to the database. The data from inserts to the ErrorLog view will be stored in the tbl_ErrorLog table.

To differentiate generated error records from error records stored in the tbl_ErrorLog table, generated records will have a negative RowID.
The RowID value of generated records will be -1 * ( GENERATED_ID) where GENERATED_ID is an integer in the format: CN_Number value (last four bits), TreeField_CN (2nd four bits), TableName (first 4 bits) (Tree = 1, Log = 2, Plot = 3)
When updating or deleting generated records from the ErrorLog view will have no effect on the database.


# TreeFieldSetup, LogFieldSetup, TreeFieldSetupDefault, LogFieldSetupDefault
It may be possible that some files have tree field setups that use invalid or no longer supported fields. These fields wont be migrated over.

# Tree species and LiveDead
in the previous database it is possible for species and live dead values to not match up with the tree default value on a tree. This could cause a tree to be moved into a different population during the migration process. This is why we audit the database before hand and see that is free of errors.


 # ongoing considerations
 - custom fields for cutting unit
 - remove unit code length constraint and add separate field for TIM unit code
 - species codes
 - device specific hotkeys
 - de-duplicate fields across tree and tally ledger
 - remove foreign key reference to tree default value in treeAuditRule_treeDefaultValue table to allow for generalized rules
 - translate guids coming from the old database, or use generated ids, or just generate a fresh new guid because I think a backwards translation is out.



 # Todo 2
 - [x] generate plantuml ERD

 - [-] add root Cruise entity type
     - a sale could contain multiple cruises or we could combine cruises
     - change purpose to int or collate no case
     -


 - [ ] cutting unit attributes
 - [ ] move month and year from stratum to sale *** this change is associated with reconsidering backwards compatibility view and removing the _V3 suffix
     - remove Stratum.VolumeFactor to a processing table
 - [x] remove foreign key reference to tree default value in treeAuditRule_treeDefaultValue table to allow for generalized rules
 - [x] change table names, removing _V3 and other things done to maintain backwards compatibility

 - [x] remove backwards compatibility views
 - [x] remove error log table
 - [ ] move yeald component somewhere else
 - [ ] logical deletes ?
     - keep on tally ledger?
 - [ ] add isDeleted flags to tree, log, plot, unit, stratum, sampleGroup. triggers to cascade isDeleted flags?
 - [x] remove logMatrix table because the only person who used it has retired.
 - [ ] look up tables for purpose, methods, forest, regions, uom
 - [ ] update migrations
     - pull errorlog.suppressed into TreeAuditRuleResolutions
 - [-] move fiaCode, contract species from tdv to species table
 - [x] take another look at uniques on LogGradeAuditRule
 - [ ] change ifnull() to coalesce() in ddl
 - [x] common name on species table( currently in volEq)
 - [ ] clarify stem.diameterType

 - [ ] trigger to prevent switching from tally by sg to tally by sp if sg has trees or tree count
 - [ ] track original device when creating copy of sample state, or use guid id for sample states and track that.
     - check that there is a modified time stamp on sampler state
 - [ ] support for multi variable 3p
 - [x] get rid of value equations
 - [ ] fix log migration so that its not using tree_cn
 - [ ] Tie UOM to product or leave at sample group?
 - [ ] Keep tally description?
 - [ ] Move FixCNT Field to Stratum
 - [ ] Switch from _CN suffix to _OID suffix
 - [ ] remove TreeMeasurments.MetaData
 - [ ] add mapping table Device_Cruise or leave Device as is?
 - [ ] figure out any changes for check cruising. do we add seperate table for check measurments? field for signing off on check?
 - [ ] get updated list of forest codes for region 4
 - [ ] depreciate PCMTRE cruise method for PCM
 - [ ] backport of TreeAuditRules
