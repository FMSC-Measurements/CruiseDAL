# 08041 Marshall HFR.cruise
When this file was being designed a tree default value was removed from one of the FixCNT sample groups
This caused a FixCNTTallyPopulation to exist that didn't match up with a SampleGroupTreeDefaultValue record.
When upconverting the file a ForeignKey error was being thrown when converting the FixCNTTallyPopulation table
See: FixCNTTallyPopulationMigrator_Test.cs_