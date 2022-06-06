Feature: SyncLogs

Background: Single Tree Three Logs: One Shared, One In Conflict and One Unique
	Given the following cruise files exist:
		| FileAlias |
		| source    |
		| dest      |
	
	* in 'source, dest' the following units exist:
		| CuttingUnitCode |
		| u1              |
		| u2              |

	* in 'source, dest' the following strata exist:
		| StratumCode |
		| st1         |
		| st2         |

	* in 'source, dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode |
		| sg1             | st1         |
		| sg2             | st1         |
		| sg1             | st2         |
		| sg2             | st2         |

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	* in 'source, dest' the following trees exist:
		| CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID |
		| u1              | st1         | sg1             | sp1         | 1          | tree1  |

	* in 'dest' the following logs exist:
		| TreeID | LogNumber | LogID |
		| tree1  | 1         | log1  |
		| tree1  | 2         | log2d |
		| tree1  | 3         | log3  |

	* in 'source' the following logs exist:
		| TreeID | LogNumber | LogID |
		| tree1  | 1         | log1  |
		| tree1  | 2         | log2s |
		| tree1  | 4         | log4  |

Scenario: Checking For Conflict Shows Logs With the Same Log Number on the Same Tree With Different LogIDs
	When I conflict check 'source' file against 'dest'
	Then Log Conflict List has 1 conflict(s)
	And Log Conflict List has conflicts:
		| SourceRecID | DestRecID |
		| log2s       | log2d     |

Scenario: Resolve LogConflicts With ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all log conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains logs:
		| LogID |
		| log1  |
		| log2d |
		| log3  |
		| log4  |

Scenario: Resolve LogConflicts With ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all log conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains logs:
		| LogID |
		| log1  |
		| log2s |
		| log3  |
		| log4  |

Scenario: Resolve Conflict With ModifyDest
	When I conflict check 'source' file against 'dest'
	And I resolve log conflicts with ModifyDest using:
		| DestRecID | LogNumber |
		| log2d     | 5         |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains logs:
		| LogID | LogNumber |
		| log1  | 1         |
		| log2d | 5         |
		| log2s | 2         |
		| log3  | 3         |
		| log4  | 4         |
