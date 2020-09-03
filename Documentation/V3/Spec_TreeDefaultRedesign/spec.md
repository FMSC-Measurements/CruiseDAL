## TreeDefaultValue Redesign

The TreeDefaultValue table from version 2 has several issues. It had issues with being both overly specific but not flexibility enough for the users needs.

## Misuse of the species code
The original intent for the species code was that it would be used to indicate a specific species or group of tree species. However some users started using the species code to define different tree defaults. 
We need a way to expand tree default to allow defining with more flexibility. 

### Defining Tree Defaults using Sample Group Code
One solution to the issue of misuse of species codes is to allow Tree Defaults to be defined using Sample Group codes. Additionally we could allow for partial matching of sample group codes, such as a suffix. 

## Removal of LiveDead column 
In version 2 LiveDead was one of the major key identifiers of tree defaults. However many users find is cumbersome having to deal with LiveDead so frequently. Many users don't even cruise dead trees. As well not all values in are suceptible to being changed based on LiveDead. 

Instead, we have added separate column for dead tree values for relevant columns: CullPrimaryDead, HiddenPrimaryDead, and TreeGradeDead.



## Custimizability per-cruise, with seperate storage for templates
Tree Default settings will need to remain customizable per-cruise, so that if a user needs to change Tree Default and have them apply to  just that cruise they can. Outside of the cruise Tree Defaults need to be stored so that they can be customized by the region, forest and even possibly the district level.  

## Optional Tree Default definers 
In version 2 each tree default had to have all three keys: Species, Product, and LiveDead. This was overly specific for the needs of the users in most cases. 
Allowing some or all of these fields to  be optional would allow users to define Tree Defaults at level of resolution closer to their needs and reduce work needed to configure Tree Defaults as needed. 

When matching possible tree defaults to a tree, Tree Defaults will be need ranked in to determine the best match. 

matches will be given a score from 0 to 7. With a Tree Default that matches specifically for all values given a 7 and a base Tree Default that matches for all trees given a score of 0.

Score added for each match
 - Species : 4 
 - Sample Group Code : 2
 - Primary Product : 1

## Speration of Species, FIACode, and CommonName into new tables
FIACode and CommonName have been moved into a static look-up table: FIA. Since these values shouldn't change and are constant across all regions. 

SpeciesCode and ContractSpecies have been moved to a new Species table that allows users to customize the Species Codes per-cruise

Another Species table will exist for template data to allow users to customize what species will appear in newly created cruises at the region and forest level. 


# need clarification
 - what type of data is MerchHeightType. Is this true/false. Does this need a look up table. 
 - what are the range of values cull primary, hidden primary. 0-100 or 0.0-1.0

# ongoing considerations

## Species Code vs FIACode in Tree Default table
Would FIACode serve better in the Tree Default table instead of Species Code. This would be better in a larger database design where we might want a standardized value for species across all cruises, but would require more cross walking to get the cruise specific Species Code for data entry and processing, as well, would require more mental cross waling when viewing the raw table data.

