CREATE TABLE MultiPropTable
(
	ID INTEGER Primary Key AUTOINCREMENT,
	StringField TEXT, 
	IntField INTEGER, 
	NIntField INTEGER, 
	LongField INTEGER, 
	NLongField INTEGER, 
	FloatField REAL, 
	NFloatField REAL, 
	DoubleField REAL, 
	NDoubleField REAL,
	BoolField BOOLEAN, 
	NBoolField BOOLEAN, 
	GuidField TEXT,
	DateTimeField DATETIME
	PartialyPublicField TEXT,
	AutomaticStringField TEXT,
	PrivateField TEXT,
	CreatedBy TEXT,
	ModifiedBy TEXT
)