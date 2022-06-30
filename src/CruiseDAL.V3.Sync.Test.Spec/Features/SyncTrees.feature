Feature: Sync Trees With No Logs
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

	* in 'dest' the following logs exist:
		| TreeID | LogNumber | LogID    |
		| tree1  | 1         | log1_t1  |

	* in 'source' the following logs exist:
		| TreeID | LogNumber | LogID   |
		| tree1  | 1         | log1_t1 |


Scenario: Check For Conflicts shows trees with same tree number but different TreeIDs
	When I conflict check 'source' file against 'dest'
	Then TreeConflicts has 1 conflict(s)
	And TreeConflicts has no downstream conflicts
	And Log Conflict List has 0 conflict(s)
	And TreeConflicts records has:
		| SourceRecID | DestRecID |
		| tree2s      | tree2d    |

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