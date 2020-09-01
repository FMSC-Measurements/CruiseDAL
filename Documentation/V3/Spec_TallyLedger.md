## Consolodate Tree Counts
In version 2 tree counts were stored in two locations: on individual trees and in the count tree table as a total tree count, per sample group/sub-population. This created confusion as to what was the best way to enter tree counts. We allowed users to manual edit the tree count value in the count tree table. Edits would be logged but it wasn't very practical for tracking changes. Tree counts from tallying were stored in the count tree table, but when the user needed to untally the total tree count needed to be decremented. Unless checked it could be possible to create a negative tree count. If a tallied tree was deleted the tree count wouldn't automatically go down. 

In version 2 tree count on individual trees would be used for all plot cruised trees. When cruising a plot each tree tallied would be given a tree record. This is done to help with check cruises, where the order (clockwise from north) of all trees (measured or not) needs to be known.    

Version 3 will remove the Tree Count field from the Tree table, as well as remove the Count Tree table for storing tree counts. 
Instead all tree counts will be stored in the count tree table. 

Converting files from  version 2 to version 3 we will keep the same resolution of the tree counts. 
For every count tree record a bulk tree count record will be added to the tally ledger. Each tree with a non-zero tree count will be given a tally ledger entry associated with that tree.  

## Track all entered KPIs
For 3p cruising it is beneficial to keep track of all entered kpi values because it can give a high fidelity image of the volumes of individual trees across a sale. 
Using the tally ledger to store each individual kpi instead of a lump sum will add new reporting capabilities for 3p cruising methods. 


## Multiple Entry types
The tally ledger table will have a EntryType field. This will allow us to determine the purpose of an entry. Entries can be added because of Tally, bulk tree counts, user entered tree counts, left over trees....

# Ongoing considerations

## TreeID vs TreeNumber
Trees can be identified either by TreeID or TreeNumber + CuttingUnit + PlotNumber

## Redundancies between Tree table and Tally Ledger. 
Tally Ledger has UnitCode, StratumCode, SampleGroupCode, SpeciesCode. These are duplicated in the tree table. 
They need to be in the Tally Ledger table because a tally can exist with out a tree, but should they be removed from the tree table, because a tree shouldn't exist without a tally ledger entry. 

## allow single entry per-tree or allow multiple
### allow multiple
Pros: 
 - we get history of edits to tree count on trees
 - allow us to not rely on time stamp to pick latest tree count edit
 - doesn't break idempotentcie on tally ledger
Cons:
 - can hide issue where there are two conflicting edits to a tree count
 - we have to have UnitCode, StratumCode, SampleGroupCode, SpeciesCode in tree table to enforce integrity of data




