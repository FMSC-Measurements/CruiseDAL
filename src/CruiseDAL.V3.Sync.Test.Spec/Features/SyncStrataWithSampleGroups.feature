Feature: Sync Strata With Sample Groups

Background:
	Given the following cruise files exist:
		| FileAlias |
		| source    |
		| dest      |
	
	* in 'source, dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u1              | unit1         |

	* in 'source' the following strata exist:
		| StratumCode | Units | StratumID |
		| st1         | u1    | stratum1s |

	* in 'dest' the following strata exist:
		| StratumCode | Units | StratumID |
		| st1         | u1    | stratum1d |

	# add one downstream conflicting sample group (sg1) and a non-conflicting sample group
	* in 'source' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID |
		| sg1             | st1         | sg1_st1s      |
		| sg2             | st1         | sg2_st1s      |

	* in 'dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID |
		| sg1             | st1         | sg1_st1d      |
		| sg3             | st1         | sg3_st1d      |


Scenario: Check For Conflicts
	When I conflict check 'source' file against 'dest'
	Then Strata Conflicts Has:
		| SourceRecID | DestRecID | DownstreamConflictCount |
		| stratum1s   | stratum1d | 1                       |
	And Sample Group Conflicts Has 0 Conflict(s)

Scenario: Resolve Conflicts With ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all Strata conflicts with 'ChoseDest'
	Then running conflict resolution of 'source' file against 'dest' not supported


Scenario: Resolve Conflicts With ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all Strata conflicts with 'ChoseSource'
	Then running conflict resolution of 'source' file against 'dest' not supported

Scenario: Resolve Conflict With ModifyDest
	When I conflict check 'source' file against 'dest'
	And I resolve Stratum Conflicts with ModifyDest using:
		| DestRecID | StratumCode |
		| stratum1d | st3         |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains strata:
		| StratumID | StratumCode |
		| stratum1s | st1         |
		| stratum1d | st3         |
	* 'dest' contains sample groups:
	| SampleGroupID | SampleGroupCode | StratumCode |
	| sg1_st1s      | sg1             | st1         |
	| sg2_st1s      | sg2             | st1         |
	| sg1_st1d      | sg1             | st3         |
	| sg3_st1d      | sg3             | st3         |

Scenario: Resolve Conflict With ModifySource
	When I conflict check 'source' file against 'dest'
	And I resolve Stratum Conflicts with ModifySource using:
		| SourceRecID | StratumCode |
		| stratum1s   | st3         |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains strata:
		| StratumID | StratumCode |
		| stratum1s | st3         |
		| stratum1d | st1         |
	* 'dest' contains sample groups:
	| SampleGroupID | SampleGroupCode | StratumCode |
	| sg1_st1s      | sg1             | st3         |
	| sg2_st1s      | sg2             | st3         |
	| sg1_st1d      | sg1             | st1         |
	| sg3_st1d      | sg3             | st1         |

Scenario: Resolve Conflicts With ChoseSourceMergeData and ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve stratum conflicts with 'ChoseSourceMergeData' and downstream conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains strata:
		| StratumID | StratumCode |
		| stratum1s | st1         |
	* 'dest' contains sample groups:
		| SampleGroupID | SampleGroupCode | StratumCode |
		| sg1_st1s      | sg1             | st1         |
		| sg2_st1s      | sg2             | st1         |
		| sg3_st1d      | sg3             | st1         |


Scenario: Resolve Conflicts With ChoseSourceMergeData and ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve stratum conflicts with 'ChoseSourceMergeData' and downstream conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains strata:
		| StratumID | StratumCode |
		| stratum1s | st1         |
	* 'dest' contains sample groups:
		| SampleGroupID | SampleGroupCode | StratumCode |
		| sg1_st1d      | sg1             | st1         |
		| sg2_st1s      | sg2             | st1         |
		| sg3_st1d      | sg3             | st1         |


Scenario: Resolve Conflicts With ChoseDestMergeData and ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve stratum conflicts with 'ChoseDestMergeData' and downstream conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains strata:
		| StratumID | StratumCode |
		| stratum1d | st1         |
	* 'dest' contains sample groups:
		| SampleGroupID | SampleGroupCode | StratumCode |
		| sg1_st1d      | sg1             | st1         |
		| sg2_st1s      | sg2             | st1         |
		| sg3_st1d      | sg3             | st1         |


Scenario: Resolve Conflicts With ChoseDestMergeData and Downstream conflicts with ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve stratum conflicts with 'ChoseDestMergeData' and downstream conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains strata:
		| StratumID | StratumCode |
		| stratum1d | st1         |
	* 'dest' contains sample groups:
		| SampleGroupID | SampleGroupCode | StratumCode |
		| sg1_st1s      | sg1             | st1         |
		| sg2_st1s      | sg2             | st1         |
		| sg3_st1d      | sg3             | st1         |


