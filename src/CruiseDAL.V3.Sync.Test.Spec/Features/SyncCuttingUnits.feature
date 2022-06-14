Feature: Sync Cutting Units


Background: Single Tree Three Logs: One Shared, One In Conflict and One Unique
	Given the following cruise files exist:
		| FileAlias |
		| source    |
		| dest      |
	
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

Scenario: Cutting Unit Conflict Check
	When I conflict check 'source' file against 'dest'
	Then Cutting Unit Conflicts Has:
		| SourceRecID | DestRecID |
		| unit2s      | unit2d    |

Scenario: Resolve Conflicts With ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all Cutting Unit conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID |
		| unit1         |
		| unit2d        |

Scenario: Resolve Conflicts With ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all Cutting Unit conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CuttingUnitCode |
		| unit1         | u1               |
		| unit2s        | u2               |


Scenario: Resolve Conflict With ModifyDest
	When I conflict check 'source' file against 'dest'
	And I resolve CuttingUnit Conflicts with ModifyDest using:
		| DestRecID | CuttingUnitCode |
		| unit2d    | u3               |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CuttingUnitCode |
		| unit1         | u1              |
		| unit2s        | u2              |
		| unit2d        | u3              |


Scenario: Resolve Conflict With ModifySource
	When I conflict check 'source' file against 'dest'
	And I resolve CuttingUnit Conflicts with ModifySource using:
		| SourceRecID | CuttingUnitCode |
		| unit2s      | u3               |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CuttingUnitCode |
		| unit1         | u1              |
		| unit2d        | u2              |
		| unit2s        | u3              |