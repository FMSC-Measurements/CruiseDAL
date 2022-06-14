Feature: Sync Cutting Units With Plots


Background: Single Tree Three Logs: One Shared, One In Conflict and One Unique
	Given the following cruise files exist:
		| FileAlias |
		| source    |
		| dest      |
	
	* in 'source, dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u1              | unit1         |

	* in 'source' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u2              | unit2s        |

	* in 'dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u2              | unit2d        |
	
	# create a conflicting plot (1) and a non conflicting plot
	* in 'source' the following plots exist:
		| PlotNumber | CuttingUnitCode | PlotID    |
		| 1          | u2              | plot1_u2s |
		| 2          | u2              | plot2_u2s |
	
	* in 'dest' the following plots exist:
		| PlotNumber | CuttingUnitCode | PlotID    |
		| 1          | u2              | plot1_u2d |
		| 3          | u2              | plot3_u2d |

Scenario: Cutting Unit Conflict Check
	When I conflict check 'source' file against 'dest'
	Then Cutting Unit Conflicts Has:
		| SourceRecID | DestRecID | DownstreamConflictCount |
		| unit2s      | unit2d    | 1                       |

Scenario: Resolve Conflicts With ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all Cutting Unit conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID |
		| unit1         |
		| unit2d        |
	And 'dest' contains plots:
		| PlotID    |
		| plot1_u2d |
		| plot3_u2d |

Scenario: Resolve Conflicts With ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all Cutting Unit conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CuttingUnitCode |
		| unit1         | u1              |
		| unit2s        | u2              |
		And 'dest' contains plots:
		| PlotID    |
		| plot1_u2s |
		| plot2_u2s |


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
	And 'dest' contains plots:
		| PlotID    | CuttingUnitCode |
		| plot1_u2s | u2              |
		| plot2_u2s | u2              |
		| plot1_u2d | u3              |
		| plot3_u2d | u3              |

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
	And 'dest' contains plots:
		| PlotID    | CuttingUnitCode |
		| plot1_u2s | u3              |
		| plot2_u2s | u3              |
		| plot1_u2d | u2              |
		| plot3_u2d | u2              |
