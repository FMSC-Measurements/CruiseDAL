Feature: Sync Strata

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

Scenario: Check For Conflicts
	When I conflict check 'source' file against 'dest'
	Then Strata Conflicts Has:
		| SourceRecID | DestRecID |
		| stratum1s   | stratum1d |

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


