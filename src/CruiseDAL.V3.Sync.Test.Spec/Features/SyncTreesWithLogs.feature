Feature: Sync Trees With Logs
Sync Tree Records between two cruise files


Background:
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

		# tree 2 is in conflict 
	* in 'dest' the following trees exist:
		| CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID |
		| u1              | st1         | sg1             | sp1         | 1          | tree1  |
		| u1              | st1         | sg1             | sp1         | 2          | tree2d | 
		| u1              | st1         | sg1             | sp1         | 3          | tree3  |

	* in 'source' the following trees exist:
		| CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID |
		| u1              | st1         | sg1             | sp1         | 1          | tree1  |
		| u1              | st1         | sg1             | sp1         | 2          | tree2s |
		| u1              | st1         | sg1             | sp1         | 4          | tree4  |

		# on tree 2 we will set up 2 logs in each file
		# log 1 will be a downstream conflict
		# but each tree will also have another log that won't be in conflict
	* in 'dest' the following logs exist:
		| TreeID | LogNumber | LogID    |
		| tree1  | 1         | log1_t1  |
		| tree2d | 1         | log1_t2d |
		| tree2d | 2         | log2_t2d |


	* in 'source' the following logs exist:
		| TreeID | LogNumber | LogID    |
		| tree1  | 1         | log1_t1  |
		| tree2s | 1         | log1_t2s |
		| tree2s | 3         | log3_t2s |


Scenario: Check For Conflicts 
	When I conflict check 'source' file against 'dest'
	Then TreeConflicts has 1 conflict(s)
	And Log Conflict List has 0 conflict(s)
	And TreeConflicts records has:
		| SourceRecID | DestRecID | DownstreamConflictCount |
		| tree2s      | tree2d    | 0                       |

Scenario: Resolve Tree Conflicts With ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all tree conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains trees:
		| TreeID |
		| tree1  |
		| tree2d |
		| tree3  |
		| tree4  |
	And 'dest' contains logs:
		| LogID    | LogNumber |
		| log1_t1  | 1         |
		| log1_t2d | 1         |
		| log2_t2d | 2         |

Scenario: Resolve Tree Conflicts With ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all tree conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains trees:
		| TreeID |
		| tree1  |
		| tree2s |
		| tree3  |
		| tree4  |
	And 'dest' contains logs:
		| LogID    | LogNumber |
		| log1_t1  | 1         |
		| log1_t2s | 1         |
		| log3_t2s | 3         |

Scenario: Resolve Conflict With ModifyDest
	When I conflict check 'source' file against 'dest'
	And I resolve tree conflicts with ModifyDest using:
		| DestRecID | TreeNumber |
		| tree2d    | 5          |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains trees:
		| TreeID | TreeNumber |
		| tree1  | 1          |
		| tree2d | 5          |
		| tree2s | 2          |
		| tree3  | 3          |
		| tree4  | 4          |
	And 'dest' contains logs:
		| LogID    | LogNumber |
		| log1_t1  | 1         |
		| log1_t2d | 1         |
		| log2_t2d | 2         |
		| log1_t2s | 1         |
		| log3_t2s | 3         |

Scenario: Resolve Conflict With ModifySource
	When I conflict check 'source' file against 'dest'
	And I resolve tree conflicts with ModifySource using:
		| SourceRecID | TreeNumber |
		| tree2s      | 5          |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains trees:
		| TreeID | TreeNumber |
		| tree1  | 1          |
		| tree2d | 2          |
		| tree2s | 5          |
		| tree3  | 3          |
		| tree4  | 4          |
	And 'dest' contains logs:
		| LogID    | LogNumber |
		| log1_t1  | 1         |
		| log1_t2d | 1         |
		| log2_t2d | 2         |
		| log1_t2s | 1         |
		| log3_t2s | 3         |

Scenario: Resolve Conflict With ChoseSourceMergeData Is Not Supported
	When I conflict check 'source' file against 'dest'
	And I resolve all tree conflicts with 'ChoseSourceMergeData'
	Then running conflict resolution of 'source' file against 'dest' not supported

Scenario: Resolve Conflict With ChoseDestMergeData Is Not Supported
	When I conflict check 'source' file against 'dest'
	And I resolve all tree conflicts with 'ChoseDestMergeData'
	Then running conflict resolution of 'source' file against 'dest' not supported