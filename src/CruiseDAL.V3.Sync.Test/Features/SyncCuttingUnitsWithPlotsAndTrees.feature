Feature: Sync Cutting Units With Plots And Trees


Background: Unit with plots and trees

	Given the following cruise files exist:
		| FileAlias | DeviceAlias |
		| source    | srcDevice   |
		| dest      | destDevice  |
	
	* in 'source, dest' the following strata exist:
		| StratumCode |
		| st1         |

	* in 'source, dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode |
		| sg1             | st1         |

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	# create units
	* in 'source, dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u1              | unit1         | 

	* in 'source' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u2              | unit2s        |

	* in 'dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u2              | unit2d        |
	
	# create a conflicting plot (1) and a non conflicting plot (2 & 3)
	* in 'source' the following plots exist:
		| PlotNumber | CuttingUnitCode | PlotID    |
		| 1          | u2              | plot1_u2s |
		| 2          | u2              | plot2_u2s |
	
	* in 'dest' the following plots exist:
		| PlotNumber | CuttingUnitCode | PlotID    |
		| 1          | u2              | plot1_u2d |
		| 3          | u2              | plot3_u2d |


	# Add Trees to the conflicting plot (1)
	* in 'dest' the following trees exist:
	| TreeNumber | PlotNumber | CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeID          |
	| 1          | 1          | u2              | st1         | sg1             | sp1         | tree1_plot1_u2d |

	* in 'source' the following trees exist:
	| TreeNumber | PlotNumber | CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeID          |
	| 1          | 1          | u2              | st1         | sg1             | sp1         | tree1_plot1_u2s |

Scenario: Cutting Unit Conflict Check
	When I conflict check 'source' file against 'dest'
	# although tree 1 is a duplicate tree it shouldn't show up as a downstream conflict
	Then Cutting Unit Conflicts Has:
		| SourceRecID | DestRecID | DownstreamConflictCount |
		| unit2s      | unit2d    | 1                       |
	* Plot Conflicts has 0 conflict(s)
	* TreeConflicts has 0 conflict(s)

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
	And 'dest' contains trees:
		| TreeID          |
		| tree1_plot1_u2d |

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
		And 'dest' contains trees:
		| TreeID          |
		| tree1_plot1_u2s |


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
	And 'dest' contains trees:
		| TreeID          | CuttingUnitCode |
		| tree1_plot1_u2s | u2              |
		| tree1_plot1_u2d | u3              |

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
	And 'dest' contains trees:
		| TreeID          | CuttingUnitCode |
		| tree1_plot1_u2s | u3              |
		| tree1_plot1_u2d | u2              |

Scenario: Resolve Conflicts With ChoseDestMergeData and ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve unit conflicts with 'ChoseDestMergeData' and downstream conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID |
		| unit1         |
		| unit2d        |
	And 'dest' contains plots:
		| PlotID    |
		| plot1_u2d | 
		| plot2_u2s | 
		| plot3_u2d |
	And 'dest' contains trees:
		| TreeID          |
		| tree1_plot1_u2d |


Scenario: Resolve Conflicts With ChoseDestMergeData and Downstream conflicts with ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve unit conflicts with 'ChoseDestMergeData' and downstream conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID |
		| unit1         |
		| unit2d        |
	And 'dest' contains plots:
		| PlotID    |
		| plot1_u2s | 
		| plot2_u2s | 
		| plot3_u2d |
	And 'dest' contains trees:
		| TreeID          |
		| tree1_plot1_u2s |

Scenario: Resolve Conflicts With ChoseSourceMergeData and ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve unit conflicts with 'ChoseSourceMergeData' and downstream conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID |
		| unit1         |
		| unit2s        |
	And 'dest' contains plots:
		| PlotID    |
		| plot1_u2s | 
		| plot2_u2s | 
		| plot3_u2d |
	And 'dest' contains trees:
		| TreeID          |
		| tree1_plot1_u2s |

Scenario: Resolve Conflicts With ChoseSourceMergeData and ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve unit conflicts with 'ChoseSourceMergeData' and downstream conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID |
		| unit1         |
		| unit2s        |
	And 'dest' contains plots:
		| PlotID    |
		| plot1_u2d | 
		| plot2_u2s | 
		| plot3_u2d |
	And 'dest' contains trees:
		| TreeID          |
		| tree1_plot1_u2d |