
## TreeNumber Indexing 
The Tree entity can have either a plot (PlotNumber, CuttingUnitCode) or cutting unit as it's parrent. 
Depending on the parrent entity type TreeNumber needs to be index differently. To enforce the uniqueness 
of TreeNumber within its parrent, two partial unique indexed exist. 
### UIX_Tree_TreeNumber_CuttingUnitCode_PlotNumber_StratumCode_CruiseID
	This index applys when `PlotNumber IS NOT NULL`. It's worth noting that this index enforces that tree number is unique 
	to the Plot_Stratum rather than the Plot directly. Because in some cruising situations e.g. recon cruises, 
	users will have duplicate tree numbers in a plot for each stratum. 
### UIX_Tree_TreeNumber_CuttingUnitCode_CruiseID
	This index applys when `PlotNumber IS NULL`
 




