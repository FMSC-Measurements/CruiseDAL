# Entirely New Tables 
 - Species
 - TallyLedger
 - TallyPopulation
 
# Modified New Tables
 - CuttingUnit_Stratum (was CuttingUnitStratum)
 - Log_V3 (was Log)
 - Plot_Stratum (part of Plot)
 - Plot_V3 (part of Plot)
 - SampleGroup_Species (was SampleGroupTreeDefaultValue)
 - SampleGroup_V3 (was SampleGroup)
 - Tree_V3 (part of Tree)
 - TreeMeasurments (part of Tree) 
 - SubPopulation_TreeAuditValue (was TreeDefaultValueTreeAuditValue)
 - LogFieldSetup_V3 ( removed unused fields and changed Stratum_CN to StratumCode, no backport? )
 - TreeFieldSetup_V3 ( removed unused fields and changed Stratum_CN to StratumCode, no backport? )

# Old tables with read-only back-ports
 - CountTree
 - CuttingUnitStratum
 - Plot
 - SampleGroup
 - SampleGroupTreeDefaultValue
 - Tree
 - Log
 - TreeDefaultValueTreeAuditValue

## TreeEstimate

# Tables with no major changes
 - Sale
 - CuttingUnit
 - Stratum 
 - TreeDefaultValue 
 - FixCNTTallyClass
 - FixCNTTallyPopulation

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

## TreeAuditValue 
 - added `TreeAuditValueID TEXT` field

## MessageLog
 - added new key field MessageLogID (required)
 - date and time fields now auto populate with current date and time
 - level auto-populates with 'N'
 - level collates nocase

## Globals
 - block defaults to 'Database'
 - add collate nocase to block, and key


# Minor changes to old table
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

# Removed Tables with no back ports
## Stem
It looks like this table wasn't properly designed. The intention of this table was for measuring multi-stems on trees, however, in its current design a tree can only have one stem. Once it is properly redesigned it can be added back to the schema.   

## Tally 
Initially this table made it hard to define tally populations using the CountTree table. The fields that it provided have now been folded into the TallyPopulation table



# Tree 

## Version 3 changes
 - Split out measurement columns into TreeMeasurments table 
 - FiaCode removed, now being read from TreeDefaultValue table
 - CountMeasure column should be considered obsolite but being kept on Tree_V3 for compatibility with updated databases only. 

# TODO 
- ? Separate table for all hot-keys (Stratum_HotKey) 
- figure out update cascades 
- redesign fixcnt tables?
- LogGradeAuditRule reference Species
- whats up with LogFieldSetupDefault.FieldName, TreeFieldSetupDefault has it too!
- change tallyPoplulation to a view that populates from subPopulation with SampleGroup_v3.tallyBySpecies as a condition

#Recomended Changes
 - remove duplicated fields from StratumStats
