# Third Party Libraries
## Microsoft.Data.Sqlite
source: nuget
## System.Data.Sqlite
source: nuget and local dlls for compact framework versions
## Backpack.SqlBuilder
source: custom package source (see NuGet.config), and local dlls for compact framework
## linqBridge
source: local dlls for compact framework

# Legacy versions
 The Compact Framework and .Legacy projects maintain backwards compatibility with older versions. Those changes include:
 - support for unnumbered parameters i.e. "something = ?",current versions changed no longer support this parameter style. The Legacy versions will find unnumbered parameters (?) and replace them with numbered parameters in the new style ("@p1"). This mean that code using the legacy version can use the new style as well
 