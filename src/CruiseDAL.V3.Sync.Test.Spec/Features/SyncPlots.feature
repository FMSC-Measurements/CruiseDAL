Feature: Sync Plots
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

Scenario: Check For Conflicts show plot with same plot number but different PlotIDs
	When I conflict check 'source' file against 'dest'
	Then PlotConflicts has:
		| SourceRecID | DestRecID |
		| plot2s      | plot2d    |

Scenario: Resolve Plot Conflicts with ChoseDest
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

Scenario: Resolve Conflict With ModifyDest
	When I conflict check 'source' file against 'dest'
	And I resolve plot conflicts with ModifyDest using:
		| DestRecID | PlotNumber |
		| plot2d    | 5          |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains plots:
		| PlotID | PlotNumber |
		| plot1  | 1          |
		| plot2s | 2          |
		| plot3d | 3          |
		| plot4s | 4          |
		| plot2d | 5          |

Scenario: Resolve Conflict With ModifySource
	When I conflict check 'source' file against 'dest'
	And I resolve plot conflicts with ModifySource using:
		| SourceRecID | PlotNumber |
		| plot2s      | 5          |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains plots:
		| PlotID | PlotNumber |
		| plot1  | 1          |
		| plot2d | 2          |
		| plot3d | 3          |
		| plot4s | 4          |
		| plot2s | 5          |
