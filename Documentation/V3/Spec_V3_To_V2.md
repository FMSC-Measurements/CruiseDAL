# Possible Conversion Issues

## Count Tree
In V2 there was no field in the database that explicitly stored the tally setup configuration 
for a population. Tally setup determined whether count tree records had a TreeDefaultValue foreign key
reference or not, and doing tally setup in Cruise Manager would populate the count tree table for each 
populate. While it was definatly possible to have a sample group level value that indicated the tally setup
configuration. This was decided against to minimize the possibility of having conflicting stats in the data structure. 
So the choice was made, since we had to have count tree records, we would just extrapolate the tally setup state from 
the count tree table.

When the user set up their tally populations as either tally by species, tally by 
sample group, or leave it as don't tally, cruise manager would populate the count tree table with 
records reflecting their choice. With one count tree record per unit for each sample group and optionaly 
species combination. It could be possible for the user to add a unit after doing tally setup. In this case
FScruiser would look at other units to extrapolate the tally setup and create a count tree record on the fly.

Tree bases cruise methods (STR, 3P) would always have their tallies setup. This is partialy because, 
thiese methods require count tree records to store tree counts. 
Plot based methods frequently did not have tally setup, and in earlier versions it wasn't possible. 
Because plot methods do not use the count tree table to store tree counts. 

In V3 with the removal of the count tree table, we were able to switch over to storing tally setup as a value 
at the sample group level. To keep things simple as well as there not being reason to keep the 'don't tally' option. 
This new field was made as a boolean flag idicating Tally By Species (true) or by default Tally By Sample Group (false). 

This means that any converted populations would become Tally by Sample Group by default if they didn't have any count tree
records. The consiquence of this is that it is posible for a population to have count tree records when converting back to V3
that it did not have previously. 

## Tree Default Value Conversion Issues
Because in V3 we allow TreeDefaultValues (TDV) records to be defined with a null value 
for Species, and/or Primary Product. Where by a null value idicating that the 
the default values apply to trees with any value for Species and/or Primary Product.
This means there is may not 1 to 1 conversion from V3 TDVs to V2 TDVs. 

When converting V3 TDV records to V2 we need to extrapolate additional TDV records for 
each possible Species or Primary Product Value. 


As well, because of the removal of the Live Dead field, in V3, replaced by having seperate
'dead' variations of aplicable fields. Each V3 TDV record will generate at least 2 TDV records 
when converting to V2.

### Species vs Primary Product priority
When extrapolating TDVs in the conversion process, it is possible for multiple  
TDV records to be extrapolated with the same Species, Product combination.
In V2 these values should be unique, so that only one TDV can be aplicable to a single tree.
An example of a situation that could cause this. Say we have four V3 TDV records:
 - Species: Pine		, Product: 01 (Sawtember)
 - Species: Pine		, Product: null (ANY)
 - Species: null (ANY)	, Product: 01 (Sawtember)
 - Species: null (ANY)	, Product: null (ANY)
Assuming for the sake of simplicity that the we only have one possible species value: Pine, 
and only one possile Product value: 01.

When extrapolating TDVs during the conversion process, we could get four TDV record with the 
value of Pine for Species and 01 for Product.
 - Species: Pine		, Product: 01 (Sawtember) 	---> Species: Pine, Product: 01 (Sawtember)
 - Species: Pine		, Product: null (ANY) 		---> Species: Pine, Product: 01 (Sawtember)
 - Species: null (ANY)	, Product: 01 (Sawtember)	---> Species: Pine, Product: 01 (Sawtember)
 - Species: null (ANY)	, Product: null (ANY)		---> Species: Pine, Product: 01 (Sawtember)
 
Since we can't actualy have duplicate TDVs we need to apply the rules of priority between Species vs 
Primary Product. These rules state, when we have two TDV records that can apply to a tree we give 
priority to the TDV that matches the tree's species and product in the following order:
 - matches exact species and product
 - matches exact product but not species
 - matches exact species but not product
 - matches explicitly on neither species or product
 
 To achieve this effect we will extrapolate TDVs and insert them in the V2 database in a specific order, 
 skipping inserting a new record where one with the same Species, product already exists. 
 First we extrapolate TDVs with both Species and Products. Followed by TDVs with just Product. Followed by 
 TDVs with just Species. Then lastly TDVs with neither explicit product or species.
 
To prevent creating duplcate TDV records we need to have the database ignore inserting TDV additional
records after one with a higher priority has already been added. Unfortuanatly the V2 design doesn't 
propperly enforce uniqueness on the TDV table, because it include an unused field, Chargable, in the 
unique constraint. To fix this we need to create a unique index on the TDV table as a part of the 
conversion process.

## Tree Audit Rules
Because currently we are only planning on using converted cruises with Cruise Processing and Cruise Design and these applications
Do not make use of Tree Audit Rules I have written a conversion for Tree Audit Rule records.
Implementing Tree Audit Rule conversion is not likely going to be easy and would require extrapolating additional Tree Audit Rules 
in situations where the V3 record doesn't specify Species and/or product. 

## Tree Field Setup and Tree Field Setup Defaults

