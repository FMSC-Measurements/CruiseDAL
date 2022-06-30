Feature: Sync Cutting Units With Trees


Background: Single Tree Three Logs: One Shared, One In Conflict and One Unique
	Given the following cruise files exist:
		| FileAlias |
		| source    |
		| dest      |
	
	* in 'source, dest' the following strata exist:
		| StratumCode |
		| st1         |

	* in 'source, dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode |
		| sg1             | st1         |

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	* in 'source, dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u1              | unit1         |

	# create a conflicting unit
	* in 'source' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u2              | unit2s        |

	* in 'dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u2              | unit2d        |

	# create a conflicting tree
	* in 'source' the following trees exist:
		| CuttingUnitCode | TreeNumber | StratumCode | SampleGroupCode | SpeciesCode | TreeID    |
		| u2              | 1          | st1         | sg1             | sp1         | tree1_u2s |

	* in 'dest' the following trees exist:
		| CuttingUnitCode | TreeNumber | StratumCode | SampleGroupCode | SpeciesCode | TreeID     |
		| u2              | 1          | st1         | sg1             | sp1         | tree1_u2d |

	# create a non conflicting tree
	* in 'source' the following trees exist:
		| CuttingUnitCode | TreeNumber | StratumCode | SampleGroupCode | SpeciesCode | TreeID    |
		| u2              | 2          | st1         | sg1             | sp1         | tree2_u2s |

	* in 'dest' the following trees exist:
		| CuttingUnitCode | TreeNumber | StratumCode | SampleGroupCode | SpeciesCode | TreeID    |
		| u2              | 3          | st1         | sg1             | sp1         | tree3_u2d |


Scenario: Cutting Unit Conflict Check
	When I conflict check 'source' file against 'dest'
	Then Cutting Unit Conflicts Has:
		| SourceRecID | DestRecID | DownstreamConflictCount |
		| unit2s      | unit2d    | 1                        |

Scenario: Resolve Conflicts With ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all Cutting Unit conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID |
		| unit1         |
		| unit2d        |
	And 'dest' contains trees:
		| TreeID    |
		| tree1_u2d |
		| tree3_u2d |

Scenario: Resolve Conflicts With ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all Cutting Unit conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CuttingUnitCode |
		| unit1         | u1              |
		| unit2s        | u2              |
		And 'source' contains trees:
		 | TreeID    |
		 | tree1_u2s |
		 | tree2_u2s |


Scenario: Resolve Conflict With ModifyDest
	When I conflict check 'source' file against 'dest'
	And I resolve CuttingUnit Conflicts with ModifyDest using:
		| DestRecID | CuttingUnitCode |
		| unit2d    | u3              |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CuttingUnitCode |
		| unit1         | u1              |
		| unit2s        | u2              |
		| unit2d        | u3              |
	And 'dest' contains trees:
	| TreeID    | CuttingUnitCode |
	| tree1_u2s | u2              |
	| tree2_u2s | u2              |
	| tree1_u2d | u3              |
	| tree3_u2d | u3              |



Scenario: Resolve Conflict With ModifySource
	When I conflict check 'source' file against 'dest'
	And I resolve CuttingUnit Conflicts with ModifySource using:
		| SourceRecID | CuttingUnitCode |
		| unit2s      | u3              |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CuttingUnitCode |
		| unit1         | u1              |
		| unit2d        | u2              |
		| unit2s        | u3              |
	And 'dest' contains trees:
	| TreeID    | CuttingUnitCode |
	| tree1_u2s | u3              |
	| tree2_u2s | u3              |
	| tree1_u2d | u2              |
	| tree3_u2d | u2              |