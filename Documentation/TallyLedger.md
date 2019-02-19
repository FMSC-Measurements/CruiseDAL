 # changes to the database in regard to the new TallyLedger table
The purpose of this document is to document the changes to the cruise database schema in regard to the new TallyLedger table

# Reasoning for the changes
 The purpose for the addition of the new TallyLedger table is to provide better support for three core requirements of the cruise domain model
 - tree counts need to be stored and edited at the population level
 - tree counts need to be stored associated with tree records 
 - a history of tallies needs to be kept to provide users visual feedback of their progress and as well as a way to undo unintended actions 

The TallyLedger consolidates storage of tree counts. Which was being stored separately in the CountTree table as well as the Tree table. 

Additionally the TallyLedger table will be used to provide additionally functionality, and data that can be used to help provide support to the users.  

# Breaking changes
With the addition of the TallyLedger table as well as accompanying  changes in the applications that depend on it there are some breaking changes. There for newer cruise files and cruise files that have been updated should not be able to be used by older versions of cruise applications. 

 - older cruise applications are in consistent in how GUID fields are stored and may change their representation when updating records, unintentionally breaking foreign keys. Although the addition of the TallyLedger table doesn't directly require this breaking change, it does mean that older apps have the ability to significantly break the new schema. 
 - with the changes to how tree counts are stored. There is a potential for errors if a older cruise application attempts to modify count data. Therefor newer applications should only attempt to modify count data through the TallyLedger table.   

# Fields
 - TallyLedgerID (TEXT - GUID or semantic value, PRIMARY KEY)
 - UnitCode (TEXT) //could be foreign key to CuttingUnit.Code
 - StratumCode (TEXT) //could be foreign key to Stratum.Code
 - SampleGroupCode (TEXT) //could be foreign key to SampleGroup.Code
 - Species (TEXT)
 - LiveDead (TEXT)
 - TreeCount (TEXT, NOT NULL)
 - KPI (INTEGER, DEFAULT 0)
 - ThreePRandomValue (INTEGER, DEFAULT 0)
 - Tree_GUID (TEXT - GUID, FOREIGN KEY, CASCADING DELETE)
 - TimeStamp (TEXT)
 - Signature
 
 //could have foreign key constrain on TallyPopulation, however this is a view not a table but that may change

1/24/2019 potential additions
 - PlotNumber (INTEGER, Foreign key?)//although, with plot tally we will continue to store counts per tree, there is potential to want tally ledger for plot tree counts
 - Reason (TEXT) per-defined common reasons for edits
 - Remarks (TEXT) user entered remarks on edit. 
 - EntryType (TEXT) indicate type of entry e.g. tally, untally, treeCountEdit, utility
