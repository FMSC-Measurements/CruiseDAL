# updating project files to VS 2017 sdk style projects 
In this tutorial we will be updating a older style Visual Studio solution to a VS 2017 solution using the new sky style project file. 
This is typically not a very complex conversion. The new sdk style project file is primarily a slimmed down version of the older style project file and most of our work involves removing the unnecessary stuff. 

## Step 1 - clean up your existing project

First thing we want to do is clean up your source code folders. With your existing project open in Visual Studio take a look at the list of code file in your project in the Solution Explorer. At the top of the solution Explorer 
there is a button to show/hide all files. Set it so that all files are visible. This will make it so that all files in your project directory are visible. 
Files and folders that aren't included in your project will appear will a ghosted icon next to them. Go through and take note of all code files that are in your project directory that aren't a part of your solution. Ignore the `bin` and `obj` directories. These might code files that you had been working on but are now long forgotten or something you mean to comeback to at a later date. 
If you have any of these such files, change the file extension so that they don't end with a code (.cs, .xaml, .xml) extension and distinguish unused code files. I prefer the convention of adding a squiggle `~` to the end of such files. This is also the convention used in git repositories to indicate that a file should be ignored. 
The reason why we are doing this bit of clean up right now is because the new project style assumes that all code files are a part of your project unless stated otherwise. This will save us some time down the road but its also feels nice to have an organized work space.

## Step 2 - create your new solution and project files
### 2.1
In the same folder as your existing project files create a new `.csproj` file.

You can rename your old solution file and name the new one the same as your old one, if you like, but if you prefer the comfort of knowing you can fall back on the old one, you can give the new solution and project files unique file names.

Create the new project file by creating a new empty text file and adding the following contents

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup Label="Assembily Info">
	<AssemblyName>{your library/program name</AssemblyName>
    <Version>{your version here}</Version>
    <Copyright>{your copyright info here}</Copyright>
	<Company>USDA FS FMSC</Company>
    <Authors>{your name here}</Authors>
  </PropertyGroup>

  <PropertyGroup Label="Build Config">
	<TargetFramework>net35</TargetFramework>
    <Platform>Any Cpu</Platform>
	<OutputType>WinExe</OutputType>
    <StartupObject>$(RootNamespace).Program</StartupObject>
	<ApplicationIcon>{path to your icon (ex ./icons/myIcon.ico)}</ApplicationIcon>
  </PropertyGroup>

  <!--Required for System.Data.Sqlite See: https://stackoverflow.com/questions/32639630/sqlite-interop-dll-files-does-not-copy-to-project-output-path-when-required-by-r/32639631#32639631-->
  <PropertyGroup>
    <ContentSQLiteInteropFiles>true</ContentSQLiteInteropFiles>
    <CopySQLiteInteropFiles>false</CopySQLiteInteropFiles>
    <CleanSQLiteInteropFiles>false</CleanSQLiteInteropFiles>
    <CollectSQLiteInteropFiles>false</CollectSQLiteInteropFiles>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="System.Windows.Forms" />
  </ItemGroup>

  <ItemGroup>
    <None Remove="Properties\AssemblyInfo.cs" />
  </ItemGroup>

</Project>
```
Go ahead and fill in the bits that need to be filled in. Most of this info can be found in your `Properties/AssemblyInfo.cs` file. The new project style generates the AssemblyInfo.cs based on the info in your project file.

### 2.2
In the same folder as your existing solution file, create a new `.sln` file and add the following contents.

```
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.26730.10
MinimumVisualStudioVersion = 10.0.40219.1
```

### 2.3 - add your project to your new solution file
Open the newly created solution file. It should open using Visual Studio 2017. If it doesn't, open Visual Studio 2017 and open it manually. 

In the Solution Explorer right click the solution, go to `Add->Existing Project` and add the new project file we created in the first step. At this point your project might not build but don't worry, as long as it is loading correctly we are happy.

If you got a message saying that the project could not be loaded, right click the project in the solution explorer and select the entry that says `Edit {your project file name}`. If there are any errors in the project file you might be able to see them now. 


## Step 3 - adding references
!!! for all CruiseDAL users !!! 
In the same folder as your solution create a new text file and name it `NuGet.config` and add the following contents 
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="SolutionNugetRepository" value="local-packages" />
	<add key="sqlbuilder" value="https://ci.appveyor.com/nuget/backpack-sqlbuilder-x7uemlx3fbb7" />
  </packageSources>
</configuration>
``` 
Create a new `local-packages` folder in your solution directory and copy the CruiseDAL.{version}.nupkg and FMSC.ORM.{version}.nupkg into it. 

Note: you may need to restart visual studio at this point. 

From the Solution Explorer right click on your project and click `Manage Nuget Packages`. In the package manager window make sure the Package source drop-down in the top right is set to `All`. Go to the Browse tab and enter CruiseDAL into the search bar. Select the CruiseDAL package and hit Install. 


!!! specifically for Cruise Processing !!!

Install the following packages
	- ZedGraph - version 5.1.5
	- iTextSharp - version 5.4.2

You may be using the System.Data.Sqlite library directly but you don't need to add a reference because it will be pulled in by the CruiseDAL reference

## Step 4 - clean up after
Once you have everything running all nicely and feel confidant enough to clean up the old project files - no use letting them lay around gathering dust. 

Delete the bin and obj directories in your project directory. These folders are automatically generated and its good to make sure nothing is lurking in them that doesn't need to be there.

Consider deleting those unused code files from step 1. 

Delete the now unused  Properties/AssemblyInfo.cs file - you can also remove the section from the project file that says to ignore the AssemblyInfo file



