# Cruise Log Table
The purpose of the cruise log table is log important events associated with a cruise as well as particular records in it.

Because cruise data can be moved between files we need a way to store log data associated with a cruise. 
Also adding CuttingUnitID, StratumID, SampleGroupID, TreeID, LogID allows us to associate log data with particular records.
This can also double as a sort of change tracking mechinism. But the primary benifit of associating logs with records is the 
need to track changes that could affect the validity of the cruise data. 

## Uses
 - logging changes that can invalidate cruise data
 - debugging
 - change tracking?

# Fields

 - CruiseID
 - Message
 - TimeStamp
 - Program - logs which application made the change (mobile or desktop)
 - Level - Error(E)/Warning(W)/Information(I)

 - CuttingUnitID
 - StratumID
 - SampleGroupID
 - PlotID
 - TreeID
 - LogID
 - TallyLedgerID - since tally ledger isn't mutable in most cases this field might not be needed




