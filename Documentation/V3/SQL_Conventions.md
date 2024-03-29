# Database Versioning
 ## Major Version Incremtnes
Major version increments will be used to indicate broad breaking changes. Significant removal or renaming of tables or view. 
It may be possible for breaking changes to be made if it is on a field, table or view that is not in use, for a period of time or never was utilized.

## Minor Version Increments
Minor version increments are used to indicate a significant changes but arn't expected to break reacent version. 
Examples of changes that may require a minor version increment:
 - Adding a table
 - Removing a depreciated field 
 - Removing a depreciated table
 - Removing a depreciated view
 
## Patch Version Increments
Patch version increments are used to indicate a minor change
Examples of changes:
 - Adding a field
 - Add/Modify a field constraint
 - Add/Modify a Trigger

# Tables 
## Singular Names 
Table names should be singular, for example, "Customer" instead of "Customers". This avoids errors due to pluralization of English nouns. For instance some nouns are the same in their plural form i.e. Species, Geese. While others change significantly i.e. person becomes persions. 

## Prefixes/Suffixes   
Do not use table names prefixes like 'TB' or 'TBL' these are redundant. In you may need to refactor changing a table into a view. This can make your code confusing if you a view with a 'TBL' prefix. 

## Special Characters
Table names should only contain alphabetical characters and under scores (where allowed)

## Intersection Tables
Mapping tables should be a concatenation of the names of the tables that have a one to many relationship with the intersection table.

# Indexes
## Naming 
Indexes will be named 
{U/N}IX_{TableName}_{underscore separated list of columns}


# Columns

## Primary Key
Fields that uniquely identify each record and are reference as foreign keys in other tables should be a concatination the [table name] + 'ID' e.g. for a table Customer the primary key would be CustomerID


## Data Type specific naming
### Boolean
Boolean fields should have boolean names i.e. IsDeleted, HasChanges

### Date and Time
Date and time fields should have relevant naming using suffixes such as Date, Time, and _TS for time stamp.

## Change Tracking Columns
Tables that need to track should have the following columns
 - CreatedBy
 - Created_TS
 - ModifiedBy
 - Modified_TS
 
 
 
 # Change Tracking
 ## Tombstone Tables
  - Naming Convention: `BaseTableName_Tombstone`
  - Records coppied over to tombstone table on delete from base table using trigger named `BaseTableName_OnDelete`
  - No tombstones tables for Lookup Tables
  
## Mapping Table Tombstones
Because the existance of a record in a mapping table is the 'state' that is being stored, and that state must be binary.
And generaly we don't give mapping records unique IDs because of their nature. We arn't concerned that a mapping record is unique, we 
just care if it exists or not. 
Re-adding a mapping record should trigger clearing any matching mapping record from the tombstone table. 
  
