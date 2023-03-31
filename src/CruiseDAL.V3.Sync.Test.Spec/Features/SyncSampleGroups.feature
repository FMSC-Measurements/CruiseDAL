Feature: Sync Sample Groups

Background:
	Given the following cruise files exist:
		| FileAlias | DeviceAlias |
		| source    | srcDevice   |
		| dest      | destDevice  |
	
	* in 'source, dest' the following units exist:
		| CuttingUnitCode |
		| u1              |
		| u2              |

	* in 'source, dest' the following strata exist:
		| StratumCode | StratumID |
		| st1         | stratum1  |

	* in 'source' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID |
		| sg1             | st1         | sg1s_st1      |
		| sg2             | st1         | sg2s_st1      |

	* in 'dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID |
		| sg1             | st1         | sg1d_st1      |
		| sg3             | st1         | sg3d_st1      |

Scenario: Check For Conflicts
	When I conflict check 'source' file against 'dest'
	Then Sample Group Conflicts Has:
		| SourceRecID | DestRecID |
		| sg1s_st1    | sg1d_st1  |

Scenario: Resolve Conflicts With ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all Sample Group conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains sample groups:
		| SampleGroupID |
		| sg1d_st1      |
		| sg2s_st1      |
		| sg3d_st1      |


Scenario: Resolve Conflicts With ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all Sample Group conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains sample groups:
		| SampleGroupID |
		| sg1s_st1      |
		| sg2s_st1      |
		| sg3d_st1      |

Scenario: Resolve Conflict With ModifyDest
	When I conflict check 'source' file against 'dest'
	And I resolve Sample Group Conflicts with ModifyDest using:
		| DestRecID | SampleGroupCode |
		| sg1d_st1  | sg4             |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains sample groups:
		| SampleGroupID | SampleGroupCode | StratumCode |
		| sg1s_st1      | sg1             | st1         |
		| sg2s_st1      | sg2             | st1         |
		| sg1d_st1      | sg4             | st1         |
		| sg3d_st1      | sg3             | st1         |

Scenario: Resolve Conflict With ModifySource
When I conflict check 'source' file against 'dest'
	And I resolve Sample Group Conflicts with ModifySource using:
		| SourceRecID | SampleGroupCode |
		| sg1s_st1  | sg4             |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains sample groups:
		| SampleGroupID | SampleGroupCode | StratumCode |
		| sg1s_st1      | sg4             | st1         |
		| sg2s_st1      | sg2             | st1         |
		| sg1d_st1      | sg1             | st1         |
		| sg3d_st1      | sg3             | st1         |


Scenario: Resolve Conflicts With ChoseSourceMergeData
	When I conflict check 'source' file against 'dest'
	And I resolve all Sample Group conflicts with 'ChoseSourceMergeData'
	Then running conflict resolution of 'source' file against 'dest' not supported

Scenario: Resolve Conflicts With ChoseDestMergeData
	When I conflict check 'source' file against 'dest'
	And I resolve all Sample Group conflicts with 'ChoseDestMergeData'
	Then running conflict resolution of 'source' file against 'dest' not supported
