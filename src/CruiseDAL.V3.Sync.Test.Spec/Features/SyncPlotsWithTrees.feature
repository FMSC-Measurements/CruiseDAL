Feature: Sync Plots With Trees

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

	* in 'dest' the following plots exist:
		| PlotID | CuttingUnitCode | PlotNumber | Strata   |
		| plot1  | u1              | 1          | st1, st2 |
		| plot2d | u1              | 2          | st1, st2 |
		| plot3d | u1              | 3          | st1, st2 |

	* in 'source' the following plots exist:
		| PlotID | CuttingUnitCode | PlotNumber | Strata   |
		| plot1  | u1              | 1          | st1, st2 |
		| plot2s | u1              | 2          | st1, st2 |
		| plot4s | u1              | 4          | st1, st2 |	

	* in 'source, dest' the following trees exist:
		| CuttingUnitCode | PlotNumber | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID   |
		| u1              | 1          | st1         | sg1             | sp1         | 1          | tree1_p1 |

	* in 'source' the following trees exist:
		| CuttingUnitCode | PlotNumber | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID    |
		| u1              | 2          | st1         | sg1             | sp1         | 1          | tree1_p2s |
		| u1              | 2          | st1         | sg1             | sp1         | 2          | tree2_p2s |

	* in 'dest' the following trees exist:
		| CuttingUnitCode | PlotNumber | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID    |
		| u1              | 2          | st1         | sg1             | sp1         | 1          | tree1_p2d |
		| u1              | 2          | st1         | sg1             | sp1         | 3          | tree3_p2d |

Scenario: Check For Conflicts 
	When I conflict check 'source' file against 'dest'
	Then Plot Conflicts has 1 conflict(s)
	And TreeConflicts has 0 conflict(s)
	And PlotConflicts has:
		| SourceRecID | DestRecID | DownstreamConflictCount |
		| plot2s      | plot2d    | 1                       |

Scenario: Resolve Plot Conflicts With ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all plot conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains plots:
		| PlotID |
		| plot1  |
		| plot2d |
		| plot3d |
		| plot4s |
	And 'dest' contains trees:
		| TreeID    | TreeNumber |
		| tree1_p1  | 1          |
		| tree1_p2d | 1          |
		| tree3_p2d | 3          |

Scenario: Resolve Plot conflict with ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all plot conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains plots:
		| PlotID |
		| plot1  |
		| plot2s |
		| plot3d |
		| plot4s |
	And 'dest' contains trees:
		| TreeID    | TreeNumber |
		| tree1_p1  | 1          |
		| tree1_p2s | 1          |
		| tree2_p2s | 2          |

Scenario: Resolve Plot Conflicts With ChoseSourceMergeData and ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all plot conflicts with 'ChoseSourceMergeData' and downstream conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains plots:
		| PlotID |
		| plot1  |
		| plot2s |
		| plot3d |
		| plot4s |
	And 'dest' contains trees:
		| TreeID    | TreeNumber |
		| tree1_p1  | 1          |
		| tree1_p2s | 1          |
		| tree2_p2s | 2          |
		| tree3_p2d | 3          |

Scenario: Resolve Plot Conflicts With ChoseSourceMergeData and ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all plot conflicts with 'ChoseSourceMergeData' and downstream conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains plots:
		| PlotID |
		| plot1  |
		| plot2s |
		| plot3d |
		| plot4s |
	And 'dest' contains trees:
		| TreeID    | TreeNumber |
		| tree1_p1  | 1          |
		| tree1_p2d | 1          |
		| tree2_p2s | 2          |
		| tree3_p2d | 3          |


Scenario: Resolve Plot Conflicts With ChoseDestMergeData and ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all plot conflicts with 'ChoseDestMergeData' and downstream conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains plots:
		| PlotID |
		| plot1  |
		| plot2d |
		| plot3d |
		| plot4s |
	And 'dest' contains trees:
		| TreeID    | TreeNumber |
		| tree1_p1  | 1          |
		| tree1_p2s | 1          |
		| tree2_p2s | 2          |
		| tree3_p2d | 3          |

Scenario: Resolve Plot Conflicts With ChoseDestMergeData and ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all plot conflicts with 'ChoseDestMergeData' and downstream conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains plots:
		| PlotID |
		| plot1  |
		| plot2d |
		| plot3d |
		| plot4s |
	And 'dest' contains trees:
		| TreeID    | TreeNumber |
		| tree1_p1  | 1          |
		| tree1_p2d | 1          |
		| tree2_p2s | 2          |
		| tree3_p2d | 3          |

