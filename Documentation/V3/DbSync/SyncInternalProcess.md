# Sync Internal Process

The sync and resolve conflicts process is comprised with 4 distinct steps

 - Resolve deletions between both files
 - Generate a list of all conflicts for each of the [checked record type](./ConflictCheckedRecordTypes.md)
 - (If database state not persisted after generating list of conflicts)
 - Resolve conflicts for each generated conflict record
 - Sync database tables
