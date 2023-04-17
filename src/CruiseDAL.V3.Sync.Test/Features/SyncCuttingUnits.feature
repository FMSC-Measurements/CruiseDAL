Feature: Sync Cutting Units


Background: Single Tree Three Logs: One Shared, One In Conflict and One Unique
	Given the following cruise files exist:
		| FileAlias | DeviceAlias |
		| source    | srcDevice   |
		| dest      | destDevice  |
	
	* in 'source, dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID | CreatedBy  |
		| u1              | unit1         | destDevice |

	# create a conflicting unit
	* in 'source' the following units exist:
		| CuttingUnitCode | CuttingUnitID | CreatedBy |
		| u2              | unit2s        | srcDevice |

	* in 'dest' the following units exist:
		| CuttingUnitCode | CuttingUnitID | CreatedBy  |
		| u2              | unit2d        | destDevice |

Scenario: Cutting Unit Conflict Check
	When I conflict check 'source' file against 'dest'
	Then Cutting Unit Conflicts Has:
		| SourceRecID | DestRecID | SrcDevice | DestDevice |
		| unit2s      | unit2d    | srcDevice | destDevice |

Scenario: Resolve Conflicts With ChoseDest
	When I conflict check 'source' file against 'dest'
	And I resolve all Cutting Unit conflicts with 'ChoseDest'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CreatedBy  |
		| unit1         | destDevice |
		| unit2d        | destDevice |

Scenario: Resolve Conflicts With ChoseSource
	When I conflict check 'source' file against 'dest'
	And I resolve all Cutting Unit conflicts with 'ChoseSource'
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CuttingUnitCode | CreatedBy  |
		| unit1         | u1              | destDevice |
		| unit2s        | u2              | srcDevice  |


Scenario: Resolve Conflict With ModifyDest
	When I conflict check 'source' file against 'dest'
	And I resolve CuttingUnit Conflicts with ModifyDest using:
		| DestRecID | CuttingUnitCode |
		| unit2d    | u3              |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CuttingUnitCode | CreatedBy  |
		| unit1         | u1              | destDevice |
		| unit2s        | u2              | srcDevice  |
		| unit2d        | u3              | destDevice |


Scenario: Resolve Conflict With ModifySource
	When I conflict check 'source' file against 'dest'
	And I resolve CuttingUnit Conflicts with ModifySource using:
		| SourceRecID | CuttingUnitCode |
		| unit2s      | u3              |
	And I run conflict resolution of 'source' file against 'dest'
	And sync 'source' into 'dest'
	Then 'dest' contains cutting units:
		| CuttingUnitID | CuttingUnitCode | CreatedBy  |
		| unit1         | u1              | destDevice |
		| unit2d        | u2              | destDevice |
		| unit2s        | u3              | srcDevice  |