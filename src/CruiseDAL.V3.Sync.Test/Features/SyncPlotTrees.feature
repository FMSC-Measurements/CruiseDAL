Feature: Sync Plot Trees

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
		| StratumCode |
		| st1         |
		| st2         |

	* in 'source, dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode |
		| sg1             | st1         |
		| sg1             | st2         |

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	* in 'source, dest' the following plots exist:
		| PlotID | CuttingUnitCode | PlotNumber | Strata   |
		| plot1  | u1              | 1          | st1, st2 |

	* in 'source, dest' the following trees exist:
		| CuttingUnitCode | PlotNumber | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID   |
		| u1              | 1          | st1         | sg1             | sp1         | 1          | tree1_p1 |
		| u1              | 1          | st2         | sg1             | sp1         | 2          | tree2_p1 |

	* in 'source' the following trees exist:
		| CuttingUnitCode | PlotNumber | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID    |
		| u1              | 1          | st1         | sg1             | sp1         | 3          | tree3s_p1 |
		| u1              | 1          | st2         | sg1             | sp1         | 4          | tree4s_p1 |

	* in 'dest' the following trees exist:
		| CuttingUnitCode | PlotNumber | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID    |
		| u1              | 1          | st1         | sg1             | sp1         | 3          | tree3d_p1 |
		| u1              | 1          | st1         | sg1             | sp1         | 4          | tree4d_p1 |

Scenario: Check For Conflicts
	When I conflict check 'source' file against 'dest'
	Then PlotTreeConflicts has 2 conflict(s)
	And PlotTreeConflicts has:
		| SourceRecID | DestRecID |
		| tree3s_p1   | tree3d_p1 |
		| tree4s_p1   | tree4d_p1 |

Scenario: Check For Conflicts With AllowDuplicateTreeNumberForNestedStrata
	When I conflict check 'source' file against 'dest' with options
		| Option                                  | Value |
		| AllowDuplicateTreeNumberForNestedStrata | True  |
	Then PlotTreeConflicts has 1 conflict(s)
	And PlotTreeConflicts has:
		| SourceRecID | DestRecID |
		| tree3s_p1   | tree3d_p1 |