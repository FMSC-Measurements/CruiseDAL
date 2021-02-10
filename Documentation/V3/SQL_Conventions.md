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
