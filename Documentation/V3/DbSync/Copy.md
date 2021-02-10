 # Merging in new cruise (copy)

Copy all cruise data and design at the cruise level from one database to another. Copy sale data if needed

 - need to copy tombstones? option to not copy tombstones?
 
# tables

General
 - Device


 - Species

Cruise
 - Cruise
 - Sale
 - CuttingUnit
 - Stratum
 - CuttingUnit_Stratum
 - SampleGroup
    - SamplerState
 - Subpopulation
    - TallyDescription
    - TallyHotkey
    - FixCNTTallyPopulation
 - Plot
    - PlotLocation
    - Plot_Stratum
 
 - Tree
    - TreeMeasurment
    - TreeFieldValue
    - TreeLocation
 - Log
 - Stem
 - TallyLedger
 
FieldSetup
 - LogFieldSetup
 - TreeFieldSetup*
 
Validation
 - TreeAuditRule
 - TreeAuditRuleSelector
 - LogGradeAuditRule*

processing
 - Reports


Tombstone tables
 - CuttingUnit
 - Stratum
 - CuttingUnit_Stratum
 - SampleGroup
 - SamplerState
 - Subpopulation