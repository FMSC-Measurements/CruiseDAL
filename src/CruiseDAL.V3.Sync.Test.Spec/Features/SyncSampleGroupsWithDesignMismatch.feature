Feature: Sync Sample Groups With Design Mismatch

Background:
	Given the following cruise files exist:
		| FileAlias |
		| source    |
		| dest      |
	
	* in 'source, dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u1              | unit1         |

	* in 'source' the following strata exist:
		| StratumCode | Units | StratumID | Method |
		| st1         | u1    | stratum1  | STR    |

	* in 'dest' the following strata exist:
		| StratumCode | Units | StratumID | Method |
		| st1         | u1    | stratum1  | 100    |

	* in 'source' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | SamplingFrequency |
		| sg1             | st1         | sg1_st1       | 5					|

	* in 'dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | SamplingFrequency |
		| sg1             | st1         | sg1_st1       | 6					|

	# design mismatch errors are only flaged if the stratum contains field data
	# we will need to continue setting up the file with some field data for the test

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	* in 'source, dest' the following trees exist:
		| CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID |
		| u1              | st1         | sg1             | sp1         | 1          | tree1  |

Scenario: Check For Conflicts
	When I conflict check 'source' file against 'dest'
	Then Has No Conflicts

Scenario: Check For Design Mismatch
	When I check 'source' for Design Mismatch errors against 'dest'
	Then Design Errors Has:
		| error     |
		| Sample Group Sampling Frequency Mismatch:::: Sg Code:sg1 Stratum Code:st1 |