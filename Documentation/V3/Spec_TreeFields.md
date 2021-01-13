We need a lookup table to define all the fields that a tree may have. As well, at the cruise level we need to configure the user friendly "Heading" values for those fields. 

## Query Field Data Types
For the validation views as well at the TreeFieldValue table, we want to be able to query the datatype of a field

DbType will be one of 4 values : `REAL`, `TEXT`, `BOOLEAN`, `INTEGER`

These four dbType values corespond to the `ValueReal`, `ValueText`, `ValueBool`, and `ValueInt` fields on the TreeFieldValue table and the `DefaultValueReal`, `DefaultValueText`, `DefaultValueBool` and `DefaultValueInt` fields on the TreeFieldSetup table

## Cruise Level Heading configuration
Field Headings are usually standardized across regions but we don't want to have to sync changes across DB instances when Field Headings are changed. The easiest solution is to lock them in at the cruise level. 
Although this could be a bit confusing, with the possibility of fields headings changing between cruises.

## Built-in fields 
These are fields used by the National Cruise System. 


## Ongoing Considerations 

### Short names in addition to headings
A shorted name in addition to field heading would be nice for the tree edit control in FScruisers tally screen