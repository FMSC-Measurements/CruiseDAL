Feature: Sync Sample Groups With Design Mismatch

Background:
	Given the following cruise files exist:
		| FileAlias | DeviceAlias |
		| source    | srcDevice   |
		| dest      | destDevice  |
	
	* in 'source, dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID |
		| u1              | unit1         |

	* in 'source,dest' the following strata exist:
		| StratumCode | Units | StratumID | Method |
		| st1         | u1    | stratum1  | STR    |

Scenario: Check For Sampling Frequency Design Mismatch Errors
	Given in 'source' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | SamplingFrequency |
		| sg1             | st1         | sg1_st1       | 5                 |

	* in 'dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | SamplingFrequency |
		| sg1             | st1         | sg1_st1       | 6                 |

	# design mismatch errors are only flaged if the stratum contains field data
	# we will need to continue setting up the file with some field data for the test

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	* in 'source, dest' the following trees exist:
		| CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID |
		| u1              | st1         | sg1             | sp1         | 1          | tree1  |
	When I conflict check 'source' file against 'dest'
	And I check 'source' for Design Mismatch errors against 'dest'
	Then Has No Conflicts
	* Design Errors Has:
		| error                                                                     |
		| Sample Group Sampling Frequency Mismatch:::: Sg Code:sg1 Stratum Code:st1 |

Scenario: Check For KZ Design Mismatch Errors
	Given in 'source' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | KZ |
		| sg1             | st1         | sg1_st1       | 50 |

	* in 'dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | KZ |
		| sg1             | st1         | sg1_st1       | 60 |

	# design mismatch errors are only flaged if the stratum contains field data
	# we will need to continue setting up the file with some field data for the test

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	* in 'source, dest' the following trees exist:
		| CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID |
		| u1              | st1         | sg1             | sp1         | 1          | tree1  |
	When I conflict check 'source' file against 'dest'
	And I check 'source' for Design Mismatch errors against 'dest'
	Then Has No Conflicts
	* Design Errors Has:
		| error                                                     |
		| Sample Group KZ Mismatch:::: Sg Code:sg1 Stratum Code:st1 |

Scenario: Check For Insurance Frequency Design Mismatch Errors
	Given in 'source' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | SamplingFrequency | InsuranceFrequency |
		| sg1             | st1         | sg1_st1       | 5                 | 5                  |

	* in 'dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | SamplingFrequency | InsuranceFrequency |
		| sg1             | st1         | sg1_st1       | 5                 | 6                  |

	# design mismatch errors are only flaged if the stratum contains field data
	# we will need to continue setting up the file with some field data for the test

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	* in 'source, dest' the following trees exist:
		| CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID |
		| u1              | st1         | sg1             | sp1         | 1          | tree1  |
	When I conflict check 'source' file against 'dest'
	And I check 'source' for Design Mismatch errors against 'dest'
	Then Has No Conflicts
	* Design Errors Has:
		| error                                                                      |
		| Sample Group Insurance Frequency Mismatch:::: Sg Code:sg1 Stratum Code:st1 |

Scenario: Check For BigBAF Design Mismatch Errors
	Given in 'source' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | BigBAF |
		| sg1             | st1         | sg1_st1       | 15     |

	* in 'dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | BigBAF |
		| sg1             | st1         | sg1_st1       | 20     |

	# design mismatch errors are only flaged if the stratum contains field data
	# we will need to continue setting up the file with some field data for the test

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	* in 'source, dest' the following trees exist:
		| CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID |
		| u1              | st1         | sg1             | sp1         | 1          | tree1  |
	When I conflict check 'source' file against 'dest'
	And I check 'source' for Design Mismatch errors against 'dest'
	Then Has No Conflicts
	* Design Errors Has:
		| error                                                         |
		| Sample Group BigBAF Mismatch:::: Sg Code:sg1 Stratum Code:st1 |

Scenario: Check For SmallFPS Design Mismatch Errors
	Given in 'source' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | SmallFPS |
		| sg1             | st1         | sg1_st1       | 15       |

	* in 'dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | SmallFPS |
		| sg1             | st1         | sg1_st1       | 20       |

	# design mismatch errors are only flaged if the stratum contains field data
	# we will need to continue setting up the file with some field data for the test

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	* in 'source, dest' the following trees exist:
		| CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID |
		| u1              | st1         | sg1             | sp1         | 1          | tree1  |
	When I conflict check 'source' file against 'dest'
	And I check 'source' for Design Mismatch errors against 'dest'
	Then Has No Conflicts
	* Design Errors Has:
		| error                                                           |
		| Sample Group SmallFPS Mismatch:::: Sg Code:sg1 Stratum Code:st1 |

Scenario: Check For TallyBySubPop Design Mismatch Errors
	Given in 'source' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | SamplingFrequency | TallyBySubPop |
		| sg1             | st1         | sg1_st1       | 5                 | true          |

	* in 'dest' file the following sample groups exist:
		| SampleGroupCode | StratumCode | SampleGroupID | SamplingFrequency | TallyBySubPop |
		| sg1             | st1         | sg1_st1       | 5                 | false         |

	# design mismatch errors are only flaged if the stratum contains field data
	# we will need to continue setting up the file with some field data for the test

	* in 'source, dest' the following species exist:
		| SpeciesCode |
		| sp1         |

	* in 'source, dest' the following trees exist:
		| CuttingUnitCode | StratumCode | SampleGroupCode | SpeciesCode | TreeNumber | TreeID |
		| u1              | st1         | sg1             | sp1         | 1          | tree1  |
	When I conflict check 'source' file against 'dest'
	And I check 'source' for Design Mismatch errors against 'dest'
	Then Has No Conflicts
	* Design Errors Has:
		| error                                                                |
		| Sample Group TallyBySubPop Mismatch:::: Sg Code:sg1 Stratum Code:st1 |