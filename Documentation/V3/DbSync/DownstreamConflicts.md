# Downstream Conflicts

When a record type that contains other types of data i.e. Cutting Units and Plots
has is in conflict, the child data that belongs to it might also be in conflict.

For example we have two cutting units that are in conflict and each of those cutting units
has a few trees. Among those trees there are a few trees that have the same tree numbers as the 
some of the trees in the other unit. If we were to just sync all the trees in both files
those trees would be in conflict because they belong to cutting units with the same code and 
have the same tree numbers, but in this situation we consider them to be downstream conflicts
of their parent cutting units. Because depending on how we chose to resolve the cutting unit
conflicts will affect how we treat any conflicts on the trees in them.

# Limited scope of downstream conflicts
To limit the levels of complexity when it comes to dealing with downstream conflicts
we limit the number of levels we check for downstream conflicts to 1. 
So a cutting unit can have plots and in those plots are trees and those trees may have logs. 
By saying we limit the number of levels we check for downstream conflicts, that would mean we only 
go down to the plots or the non-plot trees in that unit.

Additionally logs are not included as downstream conflicts. 
This is because of the special rules with syncing log records. [See Treatment of records belonging to trees](./ConflictCheckedRecordTypes.md)

