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
namespace CruiseDAL.V3.Sync.Test.Spec.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class SyncTreesWithLogsFeature : object, Xunit.IClassFixture<SyncTreesWithLogsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "SyncTreesWithLogs.feature"
#line hidden
        
        public SyncTreesWithLogsFeature(SyncTreesWithLogsFeature.FixtureData fixtureData, CruiseDAL_V3_Sync_Test_Spec_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Sync Trees With Logs", "Sync Tree Records between two cruise files", ProgrammingLanguage.CSharp, ((string[])(null)));
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
#line 5
#line hidden
            TechTalk.SpecFlow.Table table205 = new TechTalk.SpecFlow.Table(new string[] {
                        "FileAlias"});
            table205.AddRow(new string[] {
                        "source"});
            table205.AddRow(new string[] {
                        "dest"});
#line 6
 testRunner.Given("the following cruise files exist:", ((string)(null)), table205, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table206 = new TechTalk.SpecFlow.Table(new string[] {
                        "CuttingUnitCode"});
            table206.AddRow(new string[] {
                        "u1"});
            table206.AddRow(new string[] {
                        "u2"});
#line 11
 testRunner.And("in \'source, dest\' the following units exist:", ((string)(null)), table206, "* ");
#line hidden
            TechTalk.SpecFlow.Table table207 = new TechTalk.SpecFlow.Table(new string[] {
                        "StratumCode"});
            table207.AddRow(new string[] {
                        "st1"});
            table207.AddRow(new string[] {
                        "st2"});
#line 16
 testRunner.And("in \'source, dest\' the following strata exist:", ((string)(null)), table207, "* ");
#line hidden
            TechTalk.SpecFlow.Table table208 = new TechTalk.SpecFlow.Table(new string[] {
                        "SampleGroupCode",
                        "StratumCode"});
            table208.AddRow(new string[] {
                        "sg1",
                        "st1"});
            table208.AddRow(new string[] {
                        "sg2",
                        "st1"});
            table208.AddRow(new string[] {
                        "sg1",
                        "st2"});
            table208.AddRow(new string[] {
                        "sg2",
                        "st2"});
#line 21
 testRunner.And("in \'source, dest\' file the following sample groups exist:", ((string)(null)), table208, "* ");
#line hidden
            TechTalk.SpecFlow.Table table209 = new TechTalk.SpecFlow.Table(new string[] {
                        "SpeciesCode"});
            table209.AddRow(new string[] {
                        "sp1"});
#line 28
 testRunner.And("in \'source, dest\' the following species exist:", ((string)(null)), table209, "* ");
#line hidden
            TechTalk.SpecFlow.Table table210 = new TechTalk.SpecFlow.Table(new string[] {
                        "CuttingUnitCode",
                        "StratumCode",
                        "SampleGroupCode",
                        "SpeciesCode",
                        "TreeNumber",
                        "TreeID"});
            table210.AddRow(new string[] {
                        "u1",
                        "st1",
                        "sg1",
                        "sp1",
                        "1",
                        "tree1"});
            table210.AddRow(new string[] {
                        "u1",
                        "st1",
                        "sg1",
                        "sp1",
                        "2",
                        "tree2d"});
            table210.AddRow(new string[] {
                        "u1",
                        "st1",
                        "sg1",
                        "sp1",
                        "3",
                        "tree3"});
#line 33
 testRunner.And("in \'dest\' the following trees exist:", ((string)(null)), table210, "* ");
#line hidden
            TechTalk.SpecFlow.Table table211 = new TechTalk.SpecFlow.Table(new string[] {
                        "CuttingUnitCode",
                        "StratumCode",
                        "SampleGroupCode",
                        "SpeciesCode",
                        "TreeNumber",
                        "TreeID"});
            table211.AddRow(new string[] {
                        "u1",
                        "st1",
                        "sg1",
                        "sp1",
                        "1",
                        "tree1"});
            table211.AddRow(new string[] {
                        "u1",
                        "st1",
                        "sg1",
                        "sp1",
                        "2",
                        "tree2s"});
            table211.AddRow(new string[] {
                        "u1",
                        "st1",
                        "sg1",
                        "sp1",
                        "4",
                        "tree4"});
#line 39
 testRunner.And("in \'source\' the following trees exist:", ((string)(null)), table211, "* ");
#line hidden
            TechTalk.SpecFlow.Table table212 = new TechTalk.SpecFlow.Table(new string[] {
                        "TreeID",
                        "LogNumber",
                        "LogID"});
            table212.AddRow(new string[] {
                        "tree1",
                        "1",
                        "log1_t1"});
            table212.AddRow(new string[] {
                        "tree2d",
                        "1",
                        "log1_t2d"});
            table212.AddRow(new string[] {
                        "tree2d",
                        "2",
                        "log2_t2d"});
#line 48
 testRunner.And("in \'dest\' the following logs exist:", ((string)(null)), table212, "* ");
#line hidden
            TechTalk.SpecFlow.Table table213 = new TechTalk.SpecFlow.Table(new string[] {
                        "TreeID",
                        "LogNumber",
                        "LogID"});
            table213.AddRow(new string[] {
                        "tree1",
                        "1",
                        "log1_t1"});
            table213.AddRow(new string[] {
                        "tree2s",
                        "1",
                        "log1_t2s"});
            table213.AddRow(new string[] {
                        "tree2s",
                        "3",
                        "log3_t2s"});
#line 55
 testRunner.And("in \'source\' the following logs exist:", ((string)(null)), table213, "* ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check For Conflicts")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Trees With Logs")]
        [Xunit.TraitAttribute("Description", "Check For Conflicts")]
        public virtual void CheckForConflicts()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check For Conflicts", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 62
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
#line 5
this.FeatureBackground();
#line hidden
#line 63
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 64
 testRunner.Then("TreeConflicts has 1 conflict(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 65
 testRunner.And("Log Conflict List has 0 conflict(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table214 = new TechTalk.SpecFlow.Table(new string[] {
                            "SourceRecID",
                            "DestRecID",
                            "DownstreamConflictCount"});
                table214.AddRow(new string[] {
                            "tree2s",
                            "tree2d",
                            "0"});
#line 66
 testRunner.And("TreeConflicts records has:", ((string)(null)), table214, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Tree Conflicts With ChoseDest")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Trees With Logs")]
        [Xunit.TraitAttribute("Description", "Resolve Tree Conflicts With ChoseDest")]
        public virtual void ResolveTreeConflictsWithChoseDest()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Tree Conflicts With ChoseDest", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 70
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
#line 5
this.FeatureBackground();
#line hidden
#line 71
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 72
 testRunner.And("I resolve all tree conflicts with \'ChoseDest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 73
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 74
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table215 = new TechTalk.SpecFlow.Table(new string[] {
                            "TreeID"});
                table215.AddRow(new string[] {
                            "tree1"});
                table215.AddRow(new string[] {
                            "tree2d"});
                table215.AddRow(new string[] {
                            "tree3"});
                table215.AddRow(new string[] {
                            "tree4"});
#line 75
 testRunner.Then("\'dest\' contains trees:", ((string)(null)), table215, "Then ");
#line hidden
                TechTalk.SpecFlow.Table table216 = new TechTalk.SpecFlow.Table(new string[] {
                            "LogID",
                            "LogNumber"});
                table216.AddRow(new string[] {
                            "log1_t1",
                            "1"});
                table216.AddRow(new string[] {
                            "log1_t2d",
                            "1"});
                table216.AddRow(new string[] {
                            "log2_t2d",
                            "2"});
#line 81
 testRunner.And("\'dest\' contains logs:", ((string)(null)), table216, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Tree Conflicts With ChoseSource")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Trees With Logs")]
        [Xunit.TraitAttribute("Description", "Resolve Tree Conflicts With ChoseSource")]
        public virtual void ResolveTreeConflictsWithChoseSource()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Tree Conflicts With ChoseSource", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 87
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
#line 5
this.FeatureBackground();
#line hidden
#line 88
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 89
 testRunner.And("I resolve all tree conflicts with \'ChoseSource\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 90
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 91
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table217 = new TechTalk.SpecFlow.Table(new string[] {
                            "TreeID"});
                table217.AddRow(new string[] {
                            "tree1"});
                table217.AddRow(new string[] {
                            "tree2s"});
                table217.AddRow(new string[] {
                            "tree3"});
                table217.AddRow(new string[] {
                            "tree4"});
#line 92
 testRunner.Then("\'dest\' contains trees:", ((string)(null)), table217, "Then ");
#line hidden
                TechTalk.SpecFlow.Table table218 = new TechTalk.SpecFlow.Table(new string[] {
                            "LogID",
                            "LogNumber"});
                table218.AddRow(new string[] {
                            "log1_t1",
                            "1"});
                table218.AddRow(new string[] {
                            "log1_t2s",
                            "1"});
                table218.AddRow(new string[] {
                            "log3_t2s",
                            "3"});
#line 98
 testRunner.And("\'dest\' contains logs:", ((string)(null)), table218, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflict With ModifyDest")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Trees With Logs")]
        [Xunit.TraitAttribute("Description", "Resolve Conflict With ModifyDest")]
        public virtual void ResolveConflictWithModifyDest()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflict With ModifyDest", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 104
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
#line 5
this.FeatureBackground();
#line hidden
#line 105
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table219 = new TechTalk.SpecFlow.Table(new string[] {
                            "DestRecID",
                            "TreeNumber"});
                table219.AddRow(new string[] {
                            "tree2d",
                            "5"});
#line 106
 testRunner.And("I resolve tree conflicts with ModifyDest using:", ((string)(null)), table219, "And ");
#line hidden
#line 109
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 110
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table220 = new TechTalk.SpecFlow.Table(new string[] {
                            "TreeID",
                            "TreeNumber"});
                table220.AddRow(new string[] {
                            "tree1",
                            "1"});
                table220.AddRow(new string[] {
                            "tree2d",
                            "5"});
                table220.AddRow(new string[] {
                            "tree2s",
                            "2"});
                table220.AddRow(new string[] {
                            "tree3",
                            "3"});
                table220.AddRow(new string[] {
                            "tree4",
                            "4"});
#line 111
 testRunner.Then("\'dest\' contains trees:", ((string)(null)), table220, "Then ");
#line hidden
                TechTalk.SpecFlow.Table table221 = new TechTalk.SpecFlow.Table(new string[] {
                            "LogID",
                            "LogNumber"});
                table221.AddRow(new string[] {
                            "log1_t1",
                            "1"});
                table221.AddRow(new string[] {
                            "log1_t2d",
                            "1"});
                table221.AddRow(new string[] {
                            "log2_t2d",
                            "2"});
                table221.AddRow(new string[] {
                            "log1_t2s",
                            "1"});
                table221.AddRow(new string[] {
                            "log3_t2s",
                            "3"});
#line 118
 testRunner.And("\'dest\' contains logs:", ((string)(null)), table221, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflict With ModifySource")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Trees With Logs")]
        [Xunit.TraitAttribute("Description", "Resolve Conflict With ModifySource")]
        public virtual void ResolveConflictWithModifySource()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflict With ModifySource", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 126
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
#line 5
this.FeatureBackground();
#line hidden
#line 127
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table222 = new TechTalk.SpecFlow.Table(new string[] {
                            "SourceRecID",
                            "TreeNumber"});
                table222.AddRow(new string[] {
                            "tree2s",
                            "5"});
#line 128
 testRunner.And("I resolve tree conflicts with ModifySource using:", ((string)(null)), table222, "And ");
#line hidden
#line 131
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 132
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table223 = new TechTalk.SpecFlow.Table(new string[] {
                            "TreeID",
                            "TreeNumber"});
                table223.AddRow(new string[] {
                            "tree1",
                            "1"});
                table223.AddRow(new string[] {
                            "tree2d",
                            "2"});
                table223.AddRow(new string[] {
                            "tree2s",
                            "5"});
                table223.AddRow(new string[] {
                            "tree3",
                            "3"});
                table223.AddRow(new string[] {
                            "tree4",
                            "4"});
#line 133
 testRunner.Then("\'dest\' contains trees:", ((string)(null)), table223, "Then ");
#line hidden
                TechTalk.SpecFlow.Table table224 = new TechTalk.SpecFlow.Table(new string[] {
                            "LogID",
                            "LogNumber"});
                table224.AddRow(new string[] {
                            "log1_t1",
                            "1"});
                table224.AddRow(new string[] {
                            "log1_t2d",
                            "1"});
                table224.AddRow(new string[] {
                            "log2_t2d",
                            "2"});
                table224.AddRow(new string[] {
                            "log1_t2s",
                            "1"});
                table224.AddRow(new string[] {
                            "log3_t2s",
                            "3"});
#line 140
 testRunner.And("\'dest\' contains logs:", ((string)(null)), table224, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflict With ChoseSourceMergeData Is Not Supported")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Trees With Logs")]
        [Xunit.TraitAttribute("Description", "Resolve Conflict With ChoseSourceMergeData Is Not Supported")]
        public virtual void ResolveConflictWithChoseSourceMergeDataIsNotSupported()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflict With ChoseSourceMergeData Is Not Supported", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 148
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
#line 5
this.FeatureBackground();
#line hidden
#line 149
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 150
 testRunner.And("I resolve all tree conflicts with \'ChoseSourceMergeData\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 151
 testRunner.Then("running conflict resolution of \'source\' file against \'dest\' not supported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflict With ChoseDestMergeData Is Not Supported")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Trees With Logs")]
        [Xunit.TraitAttribute("Description", "Resolve Conflict With ChoseDestMergeData Is Not Supported")]
        public virtual void ResolveConflictWithChoseDestMergeDataIsNotSupported()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflict With ChoseDestMergeData Is Not Supported", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 153
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
#line 5
this.FeatureBackground();
#line hidden
#line 154
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 155
 testRunner.And("I resolve all tree conflicts with \'ChoseDestMergeData\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 156
 testRunner.Then("running conflict resolution of \'source\' file against \'dest\' not supported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
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
                SyncTreesWithLogsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                SyncTreesWithLogsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion