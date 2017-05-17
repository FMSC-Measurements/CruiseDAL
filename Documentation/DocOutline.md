#Seperation of Components
Initially there was just the CruiseDAL with the DAL class, the data object classes and a few other helper classes. And the code version was the same as the database schema version. 

I separated the components that were purely concerned with database operations and were not specific to the design of the database for cruise files. 
The binifits of doing so are

- core logic could be versioned separately from the database schema. 
- easier to work with code that is better separated by concerns
- easier to test code that is better separated by concerns
- ability to reuse core code down the road.

#FMSC.ORM
##Core


##EntityModel

##SQLite

#CruiseDAL


#Core Mechanics
##DataStore
This is were most of the core functionality of the DAL class comes from. Its designed so it shouldn't care about the specifics of how SQLite works. Its areas of concern are 

- providing access to cached data
- providing access to data type meta data
- Query, Read, Update, Delete and Save methods
- Transaction management
- Connection Management

##SQLiteDataStore
This is the direct base class of the DAL class and deals with some of the spacifics of SQLite, such as, file operatins (Move, Copy...), interpreting error codes, and making changes to the schema. 

##Exceptions and Exception routing

##SQL builder classes

##Creating DataObjects(Entitys)

##Test code as documentation 




