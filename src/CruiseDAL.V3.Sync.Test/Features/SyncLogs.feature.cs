﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace CruiseDAL.V3.Sync.Test.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class SyncLogsFeature : object, Xunit.IClassFixture<SyncLogsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "SyncLogs.feature"
#line hidden
        
        public SyncLogsFeature(SyncLogsFeature.FixtureData fixtureData, CruiseDAL_V3_Sync_Test_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "SyncLogs", null, ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 3
#line hidden
            TechTalk.SpecFlow.Table table97 = new TechTalk.SpecFlow.Table(new string[] {
                        "FileAlias",
                        "DeviceAlias"});
            table97.AddRow(new string[] {
                        "source",
                        "srcDevice"});
            table97.AddRow(new string[] {
                        "dest",
                        "destDevice"});
#line 4
 testRunner.Given("the following cruise files exist:", ((string)(null)), table97, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table98 = new TechTalk.SpecFlow.Table(new string[] {
                        "CuttingUnitCode"});
            table98.AddRow(new string[] {
                        "u1"});
            table98.AddRow(new string[] {
                        "u2"});
#line 9
 testRunner.And("in \'source, dest\' the following units exist:", ((string)(null)), table98, "* ");
#line hidden
            TechTalk.SpecFlow.Table table99 = new TechTalk.SpecFlow.Table(new string[] {
                        "StratumCode"});
            table99.AddRow(new string[] {
                        "st1"});
            table99.AddRow(new string[] {
                        "st2"});
#line 14
 testRunner.And("in \'source, dest\' the following strata exist:", ((string)(null)), table99, "* ");
#line hidden
            TechTalk.SpecFlow.Table table100 = new TechTalk.SpecFlow.Table(new string[] {
                        "SampleGroupCode",
                        "StratumCode"});
            table100.AddRow(new string[] {
                        "sg1",
                        "st1"});
            table100.AddRow(new string[] {
                        "sg2",
                        "st1"});
            table100.AddRow(new string[] {
                        "sg1",
                        "st2"});
            table100.AddRow(new string[] {
                        "sg2",
                        "st2"});
#line 19
 testRunner.And("in \'source, dest\' file the following sample groups exist:", ((string)(null)), table100, "* ");
#line hidden
            TechTalk.SpecFlow.Table table101 = new TechTalk.SpecFlow.Table(new string[] {
                        "SpeciesCode"});
            table101.AddRow(new string[] {
                        "sp1"});
#line 26
 testRunner.And("in \'source, dest\' the following species exist:", ((string)(null)), table101, "* ");
#line hidden
            TechTalk.SpecFlow.Table table102 = new TechTalk.SpecFlow.Table(new string[] {
                        "CuttingUnitCode",
                        "StratumCode",
                        "SampleGroupCode",
                        "SpeciesCode",
                        "TreeNumber",
                        "TreeID"});
            table102.AddRow(new string[] {
                        "u1",
                        "st1",
                        "sg1",
                        "sp1",
                        "1",
                        "tree1"});
#line 30
 testRunner.And("in \'source, dest\' the following trees exist:", ((string)(null)), table102, "* ");
#line hidden
            TechTalk.SpecFlow.Table table103 = new TechTalk.SpecFlow.Table(new string[] {
                        "TreeID",
                        "LogNumber",
                        "LogID"});
            table103.AddRow(new string[] {
                        "tree1",
                        "1",
                        "log1"});
            table103.AddRow(new string[] {
                        "tree1",
                        "2",
                        "log2d"});
            table103.AddRow(new string[] {
                        "tree1",
                        "3",
                        "log3"});
#line 34
 testRunner.And("in \'dest\' the following logs exist:", ((string)(null)), table103, "* ");
#line hidden
            TechTalk.SpecFlow.Table table104 = new TechTalk.SpecFlow.Table(new string[] {
                        "TreeID",
                        "LogNumber",
                        "LogID"});
            table104.AddRow(new string[] {
                        "tree1",
                        "1",
                        "log1"});
            table104.AddRow(new string[] {
                        "tree1",
                        "2",
                        "log2s"});
            table104.AddRow(new string[] {
                        "tree1",
                        "4",
                        "log4"});
#line 40
 testRunner.And("in \'source\' the following logs exist:", ((string)(null)), table104, "* ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Checking For Conflict Shows Logs With the Same Log Number on the Same Tree With D" +
            "ifferent LogIDs")]
        [Xunit.TraitAttribute("FeatureTitle", "SyncLogs")]
        [Xunit.TraitAttribute("Description", "Checking For Conflict Shows Logs With the Same Log Number on the Same Tree With D" +
            "ifferent LogIDs")]
        public virtual void CheckingForConflictShowsLogsWithTheSameLogNumberOnTheSameTreeWithDifferentLogIDs()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Checking For Conflict Shows Logs With the Same Log Number on the Same Tree With D" +
                    "ifferent LogIDs", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 46
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 47
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 48
 testRunner.Then("Log Conflict List has 1 conflict(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table105 = new TechTalk.SpecFlow.Table(new string[] {
                            "SourceRecID",
                            "DestRecID"});
                table105.AddRow(new string[] {
                            "log2s",
                            "log2d"});
#line 49
 testRunner.And("Log Conflict List has conflicts:", ((string)(null)), table105, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve LogConflicts With ChoseDest")]
        [Xunit.TraitAttribute("FeatureTitle", "SyncLogs")]
        [Xunit.TraitAttribute("Description", "Resolve LogConflicts With ChoseDest")]
        public virtual void ResolveLogConflictsWithChoseDest()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve LogConflicts With ChoseDest", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 53
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 54
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 55
 testRunner.And("I resolve all log conflicts with \'ChoseDest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 56
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 57
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table106 = new TechTalk.SpecFlow.Table(new string[] {
                            "LogID"});
                table106.AddRow(new string[] {
                            "log1"});
                table106.AddRow(new string[] {
                            "log2d"});
                table106.AddRow(new string[] {
                            "log3"});
                table106.AddRow(new string[] {
                            "log4"});
#line 58
 testRunner.Then("\'dest\' contains logs:", ((string)(null)), table106, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve LogConflicts With ChoseSource")]
        [Xunit.TraitAttribute("FeatureTitle", "SyncLogs")]
        [Xunit.TraitAttribute("Description", "Resolve LogConflicts With ChoseSource")]
        public virtual void ResolveLogConflictsWithChoseSource()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve LogConflicts With ChoseSource", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 65
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 66
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 67
 testRunner.And("I resolve all log conflicts with \'ChoseSource\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 68
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 69
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table107 = new TechTalk.SpecFlow.Table(new string[] {
                            "LogID"});
                table107.AddRow(new string[] {
                            "log1"});
                table107.AddRow(new string[] {
                            "log2s"});
                table107.AddRow(new string[] {
                            "log3"});
                table107.AddRow(new string[] {
                            "log4"});
#line 70
 testRunner.Then("\'dest\' contains logs:", ((string)(null)), table107, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflict With ModifyDest")]
        [Xunit.TraitAttribute("FeatureTitle", "SyncLogs")]
        [Xunit.TraitAttribute("Description", "Resolve Conflict With ModifyDest")]
        public virtual void ResolveConflictWithModifyDest()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflict With ModifyDest", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 77
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 78
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table108 = new TechTalk.SpecFlow.Table(new string[] {
                            "DestRecID",
                            "LogNumber"});
                table108.AddRow(new string[] {
                            "log2d",
                            "5"});
#line 79
 testRunner.And("I resolve log conflicts with ModifyDest using:", ((string)(null)), table108, "And ");
#line hidden
#line 82
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 83
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table109 = new TechTalk.SpecFlow.Table(new string[] {
                            "LogID",
                            "LogNumber"});
                table109.AddRow(new string[] {
                            "log1",
                            "1"});
                table109.AddRow(new string[] {
                            "log2d",
                            "5"});
                table109.AddRow(new string[] {
                            "log2s",
                            "2"});
                table109.AddRow(new string[] {
                            "log3",
                            "3"});
                table109.AddRow(new string[] {
                            "log4",
                            "4"});
#line 84
 testRunner.Then("\'dest\' contains logs:", ((string)(null)), table109, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                SyncLogsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                SyncLogsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion