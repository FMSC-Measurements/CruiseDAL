# Deletion Tracking
Tracking Deletions serve the following needs
 - Syncing Database
 - Recovery of deleted records
 - Tracking history of data for the purpose of debugging


## Tables that will utilize Deletion Tracking

 - CuttingUnit
 - CuttingUnit_Stratum
 - Stratum
     - TreeFieldSetup
     - LogFieldSetup
 - SampleGroup 
    - SamplerState
 - Subpopulation
     - FixCNTTallyPopulation
 - TreeDefaultValue
 - Plot
     - Plot_Stratum 
     - PlotLocation
 - TallyLedger
 - Tree
     - TreeLocation
     - TreeMeasurment
     - TreeFieldValue
 - Log
 - Stem
 - TreeAuditRule
     - TreeAuditRuleSelector
     - TreeAuditResolution
 - LogGradeAuditRule
 - Reports

## No deletion tracking for Sale or Cruise records
When deleting Sale or Cruise records we are assuming the intention of the user is to permanently remove the whole sale or cruise.
Because of this we do not need deletion tracking for recovery of data.
Additionally when a cruise is deleted it can not be synced. Because of this we do not need deletion tracking for syncing.

## Tombstone Implementation
Tombstone tables are named the same as their corresponding live table but with a `_Tombstone` suffix. 
Column definitions of Tombstone tables should mirror their live table but with some differences
 - auto increment _CN field removed
 - DEFAULT element removed
 - add DeletedDate field
Tombstone tables wont have foreign keys

  

## Soft Delete vs Tombstone
 ### Logical Delete
 Pros:
 - full data retention
 - doesn't require additional tables
 - easy to switch data between deleted states

 Cons:
 - without views requires application code to be aware of deleted state 
 - all queries against tables will need to take deleted state into account
 - need to modify indexes to take deleted state into account
 - Cascading deletes don't work



 ### Tombstone
 Pros:
 - wide control on data retention (all or some)
 - doesn't require change to application code

 Cons:
 - requires mirroring table design
 - creates issues with tallyLedger 

### Conclusion: 
In a database (not SQLite) with better support of multiple schemas, views and virtual tables this might work better. Soft deletes are better in situations where situations where we allow the user to see deleted data or where retention of deleted data is a primary function of the database. But our primary need for keeping deleted records is tracking deletions across multiple copies of the database. We see the ability to undelete records as an added benefit as well as a helpful tool in debugging. A tool in our tool belt for when things go sideways rather than a primary function of the application. 