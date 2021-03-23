# Merging in existing cruise

compare records 
 - use row version or modified time-stamp to determine most recently modified record
 - list all versions of record
 - ez migrate auto select freshest version of record
 - use version id or hash or the record
     - hash of record would be best because it ignores insignificant changes in record
     - hash would require using extention methods
     - row version number is mostly useles time stamp is better

 - guids needed on logs and stems 
 - logs and stems need to be migrated all or none per tree
     - option for tree all or non recursivly down to logs and stems
 - plot trees all or none?

 - execute deletions as a separate process allowing users to make merging a non-destructive process
     - execute delete on dest db rather than copying tombstone record. so dest db retains its record state at the delete. 
 - merge tally ledger need to re-sort?

# update design codes

# update tree numbers, plot numbers, log numbers

# all or none

# Merging tables without ModifiedDate
for example 

 - CuttingUnitStratum
 - SubPopulation
 - TallyLedger - 
 - Device
 - LogFieldSetup
 - TreeFieldSetup
 - LogGradeAuditRule

# tables with only ModifiedDate
 - SamplerState
 - TreeAuditResolution?

# tables with only Create_TS 
 - TallyLedger


# Merging other semi special tables
 - SamplerState (has only mod date)

 - TreeFieldValue - all or none

# TallyLedger

# handeling plot number/ tree number conflicts
 - ignore conflicts
 - list conflicts and resolve
