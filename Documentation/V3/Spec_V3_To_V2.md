# Possible Conversion Issues

## Tree Default Value Conversion Issues
When converting V3 Tree Default Value Records back to V2 we need to extrapolate additional Tree Default Value records 
when converting a Tree Default Value that doesn't have a specific species or product. 
Tree Default Value records with dead values will create an additional V2 TDV to store the dead values.

## Tree Audit Rules
Because currently we are only planning on using converted cruises with Cruise Processing and Cruise Design and these applications
Do not make use of Tree Audit Rules I have written a conversion for Tree Audit Rule records.
Implementing Tree Audit Rule conversion is not likely going to be easy and would require extrapolating additional Tree Audit Rules 
in situations where the V3 record doesn't specify Species and/or product. 

