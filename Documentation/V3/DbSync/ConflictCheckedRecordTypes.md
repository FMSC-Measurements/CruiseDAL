# Conflict Checked Record Types
There are 6 different record types that are checked for conflicts when syncing 

 -  Cutting Units
 - Strata
 - Sample Groups
 - Plots
 - Trees
 - Logs

## Treatment of records belonging to trees
For the purpose of reducing the amount of conflict checking and possible conflicts
we decided to treat all record types that belong to a tree as belonging directly to that unique tree record.
Because of this records record associated with a tree such as logs will either all come across or not 
when resolving tree conflicts. This is why when resolving tree conflicts the Chose Destination and Merge/Chose Source and Merge 
options are not available. 

## Non Check Record Types
There are some record types or bits of the cruise design that do not require conflict checking. 
Generally this is because some record types don't contain any information beyond one or two key values.
For these record types all that need to be resolved is whether or not the record exists in the cruise. 
For example Subpopulations and, Species . 
Other situations all that is needed is to check which file has the most recently modified record or set of records
and chose that version. Such as when syncing field setups. 



