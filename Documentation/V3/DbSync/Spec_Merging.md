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
 - SamplerState **** should we add it anyways 
 - TreeAuditResolution?

# tables with only Create_TS 
 - TallyLedger ******** need to handel updates on tally ledger records... we didn't want to handel updates on this table but if design codes change we need to 


# Merging other semi special tables
 - SamplerState (has only mod date)

 - TreeFieldValue - all or none

# TallyLedger

# handeling plot number/ tree number conflicts
 - ignore conflicts
 - list conflicts and resolve
 
 
 # exclude record id lists
 I added the ability to exclude units, strata, SGs, plots, trees, logs by ID
 I was thinking we should exclude all children as well, this adds considerabley more logic to the process
 Before I was effectively filtering child records by importing child recs by enumerating the parent recs. 
	- this methods has a down side. not all parent recs are compleaty hieracical. could I sync a tree from an excluded stratum
	- on the plus side this method uses less logic, 
	- also sometimes i dont want to fully exclude a stratum, unit, plot, etc... 
		I may not want to sync the source version of a stratum with conflicting code, but I want its data
		- in my Conflict class I had a flag SyncChildren. at somepoint I think I was thinking about this issue. 
		- what use cases could there be for syncing the children but not the record. 
			- could this be used to combine files
		- what situations do we need to exclude children	
			- could be used to combine files. if we only want certain units, strata
			- could be a feature to allow syncing all but troublesome records
		- exclude children is a feature I think we can let go for now. we want child recs to come across. Most of the time we have a code conflicting
				the user was just adding the same design rec to files seperatly. When its field data is when the user may mean seperate real world entities, 
				but still this is 50/50
		- we do need to exclude children when children reference parent records by ID
			- TallyLedger -> TreeID
			- Log -> TreeID
			- some can be covered by only syncing for IDs in the destination file
			- TreeLocation, TreeFieldValue -> TreeID records are covered in this situation because I'm syncing tree data in the SyncTreeData method
			 ! Im combining all Modified_TS fields on these table when getting the latest Modified_TS for tree records
			 
			
- add additional resolution option : dont sync
