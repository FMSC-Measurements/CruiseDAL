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
    public partial class SyncPlotsFeature : object, Xunit.IClassFixture<SyncPlotsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "SyncPlots.feature"
#line hidden
        
        public SyncPlotsFeature(SyncPlotsFeature.FixtureData fixtureData, CruiseDAL_V3_Sync_Test_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Sync Plots", null, ProgrammingLanguage.CSharp, ((string[])(null)));
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
#line 2
#line hidden
            TechTalk.SpecFlow.Table table110 = new TechTalk.SpecFlow.Table(new string[] {
                        "FileAlias",
                        "DeviceAlias"});
            table110.AddRow(new string[] {
                        "source",
                        "srcDevice"});
            table110.AddRow(new string[] {
                        "dest",
                        "destDevice"});
#line 3
 testRunner.Given("the following cruise files exist:", ((string)(null)), table110, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table111 = new TechTalk.SpecFlow.Table(new string[] {
                        "CuttingUnitCode"});
            table111.AddRow(new string[] {
                        "u1"});
            table111.AddRow(new string[] {
                        "u2"});
#line 8
 testRunner.And("in \'source, dest\' the following units exist:", ((string)(null)), table111, "* ");
#line hidden
            TechTalk.SpecFlow.Table table112 = new TechTalk.SpecFlow.Table(new string[] {
                        "StratumCode"});
            table112.AddRow(new string[] {
                        "st1"});
            table112.AddRow(new string[] {
                        "st2"});
#line 13
 testRunner.And("in \'source, dest\' the following strata exist:", ((string)(null)), table112, "* ");
#line hidden
            TechTalk.SpecFlow.Table table113 = new TechTalk.SpecFlow.Table(new string[] {
                        "SampleGroupCode",
                        "StratumCode"});
            table113.AddRow(new string[] {
                        "sg1",
                        "st1"});
            table113.AddRow(new string[] {
                        "sg2",
                        "st1"});
            table113.AddRow(new string[] {
                        "sg1",
                        "st2"});
            table113.AddRow(new string[] {
                        "sg2",
                        "st2"});
#line 18
 testRunner.And("in \'source, dest\' file the following sample groups exist:", ((string)(null)), table113, "* ");
#line hidden
            TechTalk.SpecFlow.Table table114 = new TechTalk.SpecFlow.Table(new string[] {
                        "SpeciesCode"});
            table114.AddRow(new string[] {
                        "sp1"});
#line 25
 testRunner.And("in \'source, dest\' the following species exist:", ((string)(null)), table114, "* ");
#line hidden
            TechTalk.SpecFlow.Table table115 = new TechTalk.SpecFlow.Table(new string[] {
                        "PlotID",
                        "CuttingUnitCode",
                        "PlotNumber",
                        "Strata"});
            table115.AddRow(new string[] {
                        "plot1",
                        "u1",
                        "1",
                        "st1, st2"});
            table115.AddRow(new string[] {
                        "plot2d",
                        "u1",
                        "2",
                        "st1, st2"});
            table115.AddRow(new string[] {
                        "plot3d",
                        "u1",
                        "3",
                        "st1, st2"});
#line 29
 testRunner.And("in \'dest\' the following plots exist:", ((string)(null)), table115, "* ");
#line hidden
            TechTalk.SpecFlow.Table table116 = new TechTalk.SpecFlow.Table(new string[] {
                        "PlotID",
                        "CuttingUnitCode",
                        "PlotNumber",
                        "Strata"});
            table116.AddRow(new string[] {
                        "plot1",
                        "u1",
                        "1",
                        "st1, st2"});
            table116.AddRow(new string[] {
                        "plot2s",
                        "u1",
                        "2",
                        "st1, st2"});
            table116.AddRow(new string[] {
                        "plot4s",
                        "u1",
                        "4",
                        "st1, st2"});
#line 35
 testRunner.And("in \'source\' the following plots exist:", ((string)(null)), table116, "* ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check For Conflicts show plot with same plot number but different PlotIDs")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Plots")]
        [Xunit.TraitAttribute("Description", "Check For Conflicts show plot with same plot number but different PlotIDs")]
        public virtual void CheckForConflictsShowPlotWithSamePlotNumberButDifferentPlotIDs()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check For Conflicts show plot with same plot number but different PlotIDs", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 41
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
#line 2
this.FeatureBackground();
#line hidden
#line 42
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table117 = new TechTalk.SpecFlow.Table(new string[] {
                            "SourceRecID",
                            "DestRecID"});
                table117.AddRow(new string[] {
                            "plot2s",
                            "plot2d"});
#line 43
 testRunner.Then("PlotConflicts has:", ((string)(null)), table117, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Plot Conflicts with ChoseDest")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Plots")]
        [Xunit.TraitAttribute("Description", "Resolve Plot Conflicts with ChoseDest")]
        public virtual void ResolvePlotConflictsWithChoseDest()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Plot Conflicts with ChoseDest", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 47
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
#line 2
this.FeatureBackground();
#line hidden
#line 48
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 49
 testRunner.And("I resolve all plot conflicts with \'ChoseDest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 50
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 51
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table118 = new TechTalk.SpecFlow.Table(new string[] {
                            "PlotID"});
                table118.AddRow(new string[] {
                            "plot1"});
                table118.AddRow(new string[] {
                            "plot2d"});
                table118.AddRow(new string[] {
                            "plot3d"});
                table118.AddRow(new string[] {
                            "plot4s"});
#line 52
 testRunner.Then("\'dest\' contains plots:", ((string)(null)), table118, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Plot conflict with ChoseSource")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Plots")]
        [Xunit.TraitAttribute("Description", "Resolve Plot conflict with ChoseSource")]
        public virtual void ResolvePlotConflictWithChoseSource()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Plot conflict with ChoseSource", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 59
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
#line 2
this.FeatureBackground();
#line hidden
#line 60
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 61
 testRunner.And("I resolve all plot conflicts with \'ChoseSource\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 62
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 63
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table119 = new TechTalk.SpecFlow.Table(new string[] {
                            "PlotID"});
                table119.AddRow(new string[] {
                            "plot1"});
                table119.AddRow(new string[] {
                            "plot2s"});
                table119.AddRow(new string[] {
                            "plot3d"});
                table119.AddRow(new string[] {
                            "plot4s"});
#line 64
 testRunner.Then("\'dest\' contains plots:", ((string)(null)), table119, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflict With ModifyDest")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Plots")]
        [Xunit.TraitAttribute("Description", "Resolve Conflict With ModifyDest")]
        public virtual void ResolveConflictWithModifyDest()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflict With ModifyDest", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 71
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
#line 2
this.FeatureBackground();
#line hidden
#line 72
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table120 = new TechTalk.SpecFlow.Table(new string[] {
                            "DestRecID",
                            "PlotNumber"});
                table120.AddRow(new string[] {
                            "plot2d",
                            "5"});
#line 73
 testRunner.And("I resolve plot conflicts with ModifyDest using:", ((string)(null)), table120, "And ");
#line hidden
#line 76
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 77
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table121 = new TechTalk.SpecFlow.Table(new string[] {
                            "PlotID",
                            "PlotNumber"});
                table121.AddRow(new string[] {
                            "plot1",
                            "1"});
                table121.AddRow(new string[] {
                            "plot2s",
                            "2"});
                table121.AddRow(new string[] {
                            "plot3d",
                            "3"});
                table121.AddRow(new string[] {
                            "plot4s",
                            "4"});
                table121.AddRow(new string[] {
                            "plot2d",
                            "5"});
#line 78
 testRunner.Then("\'dest\' contains plots:", ((string)(null)), table121, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflict With ModifySource")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Plots")]
        [Xunit.TraitAttribute("Description", "Resolve Conflict With ModifySource")]
        public virtual void ResolveConflictWithModifySource()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflict With ModifySource", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 86
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
#line 2
this.FeatureBackground();
#line hidden
#line 87
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table122 = new TechTalk.SpecFlow.Table(new string[] {
                            "SourceRecID",
                            "PlotNumber"});
                table122.AddRow(new string[] {
                            "plot2s",
                            "5"});
#line 88
 testRunner.And("I resolve plot conflicts with ModifySource using:", ((string)(null)), table122, "And ");
#line hidden
#line 91
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 92
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table123 = new TechTalk.SpecFlow.Table(new string[] {
                            "PlotID",
                            "PlotNumber"});
                table123.AddRow(new string[] {
                            "plot1",
                            "1"});
                table123.AddRow(new string[] {
                            "plot2d",
                            "2"});
                table123.AddRow(new string[] {
                            "plot3d",
                            "3"});
                table123.AddRow(new string[] {
                            "plot4s",
                            "4"});
                table123.AddRow(new string[] {
                            "plot2s",
                            "5"});
#line 93
 testRunner.Then("\'dest\' contains plots:", ((string)(null)), table123, "Then ");
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
                SyncPlotsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                SyncPlotsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion