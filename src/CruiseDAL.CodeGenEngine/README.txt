CruiseDAL.CodeGenEngine:
Prior to this project, all code that facilitated in generating code for dependent projects was embeded in T4 scripts. 
The purpose of the project is house the logic for the T4 code generation file in CruiseDAL.AutoGen 

The reason why this is a seperate project from the project containing the actual template files is because of a problem in VS2008 that makes it hard to reference assemblies from T4 files. In VS2010 you can use macros within a T4 file that allow you to reference the output assemblie of a project using the $(TargetPath) macro in the <#@ Assembly Name=... #> directive of a T4 file. However because a project can't reference itself and one way T4 resolves Assembly locations is to look in the references of a project, I decided to create a seperate project to contain the actual T4 files


DataDictionary:

Default - Note that all numeric fields automaticly default to 0, unless you explicity give a field a null default. This is oposite from how SQLite behaves  
Dont Track - If in TableSettings you enable Track Modified, or Track Merge, Dont Track allows you say that you don't want changes to that field to trigger updates to ModifiedDate or MergeState
Depreciated - 