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
    public partial class SyncStrataFeature : object, Xunit.IClassFixture<SyncStrataFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "SyncStrata.feature"
#line hidden
        
        public SyncStrataFeature(SyncStrataFeature.FixtureData fixtureData, CruiseDAL_V3_Sync_Test_Spec_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Sync Strata", null, ProgrammingLanguage.CSharp, ((string[])(null)));
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
            TechTalk.SpecFlow.Table table159 = new TechTalk.SpecFlow.Table(new string[] {
                        "FileAlias"});
            table159.AddRow(new string[] {
                        "source"});
            table159.AddRow(new string[] {
                        "dest"});
#line 4
 testRunner.Given("the following cruise files exist:", ((string)(null)), table159, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table160 = new TechTalk.SpecFlow.Table(new string[] {
                        "CuttingUnitCode",
                        "CuttingUnitID"});
            table160.AddRow(new string[] {
                        "u1",
                        "unit1"});
#line 9
 testRunner.And("in \'source, dest\' the following units exist:", ((string)(null)), table160, "* ");
#line hidden
            TechTalk.SpecFlow.Table table161 = new TechTalk.SpecFlow.Table(new string[] {
                        "StratumCode",
                        "Units",
                        "StratumID"});
            table161.AddRow(new string[] {
                        "st1",
                        "u1",
                        "stratum1s"});
#line 13
 testRunner.And("in \'source\' the following strata exist:", ((string)(null)), table161, "* ");
#line hidden
            TechTalk.SpecFlow.Table table162 = new TechTalk.SpecFlow.Table(new string[] {
                        "StratumCode",
                        "Units",
                        "StratumID"});
            table162.AddRow(new string[] {
                        "st1",
                        "u1",
                        "stratum1d"});
#line 17
 testRunner.And("in \'dest\' the following strata exist:", ((string)(null)), table162, "* ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check For Conflicts")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Strata")]
        [Xunit.TraitAttribute("Description", "Check For Conflicts")]
        public virtual void CheckForConflicts()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check For Conflicts", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 21
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
#line 22
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table163 = new TechTalk.SpecFlow.Table(new string[] {
                            "SourceRecID",
                            "DestRecID"});
                table163.AddRow(new string[] {
                            "stratum1s",
                            "stratum1d"});
#line 23
 testRunner.Then("Strata Conflicts Has:", ((string)(null)), table163, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflicts With ChoseDest")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Strata")]
        [Xunit.TraitAttribute("Description", "Resolve Conflicts With ChoseDest")]
        public virtual void ResolveConflictsWithChoseDest()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflicts With ChoseDest", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 27
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
#line 28
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 29
 testRunner.And("I resolve all Strata conflicts with \'ChoseDest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 30
 testRunner.Then("running conflict resolution of \'source\' file against \'dest\' not supported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflicts With ChoseSource")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Strata")]
        [Xunit.TraitAttribute("Description", "Resolve Conflicts With ChoseSource")]
        public virtual void ResolveConflictsWithChoseSource()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflicts With ChoseSource", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 33
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
#line 34
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 35
 testRunner.And("I resolve all Strata conflicts with \'ChoseSource\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 36
 testRunner.Then("running conflict resolution of \'source\' file against \'dest\' not supported", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflict With ModifyDest")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Strata")]
        [Xunit.TraitAttribute("Description", "Resolve Conflict With ModifyDest")]
        public virtual void ResolveConflictWithModifyDest()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflict With ModifyDest", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 38
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
#line 39
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table164 = new TechTalk.SpecFlow.Table(new string[] {
                            "DestRecID",
                            "StratumCode"});
                table164.AddRow(new string[] {
                            "stratum1d",
                            "st3"});
#line 40
 testRunner.And("I resolve Stratum Conflicts with ModifyDest using:", ((string)(null)), table164, "And ");
#line hidden
#line 43
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 44
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table165 = new TechTalk.SpecFlow.Table(new string[] {
                            "StratumID",
                            "StratumCode"});
                table165.AddRow(new string[] {
                            "stratum1s",
                            "st1"});
                table165.AddRow(new string[] {
                            "stratum1d",
                            "st3"});
#line 45
 testRunner.Then("\'dest\' contains strata:", ((string)(null)), table165, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Resolve Conflict With ModifySource")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Strata")]
        [Xunit.TraitAttribute("Description", "Resolve Conflict With ModifySource")]
        public virtual void ResolveConflictWithModifySource()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resolve Conflict With ModifySource", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 50
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
#line 51
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table166 = new TechTalk.SpecFlow.Table(new string[] {
                            "SourceRecID",
                            "StratumCode"});
                table166.AddRow(new string[] {
                            "stratum1s",
                            "st3"});
#line 52
 testRunner.And("I resolve Stratum Conflicts with ModifySource using:", ((string)(null)), table166, "And ");
#line hidden
#line 55
 testRunner.And("I run conflict resolution of \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 56
 testRunner.And("sync \'source\' into \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table167 = new TechTalk.SpecFlow.Table(new string[] {
                            "StratumID",
                            "StratumCode"});
                table167.AddRow(new string[] {
                            "stratum1s",
                            "st3"});
                table167.AddRow(new string[] {
                            "stratum1d",
                            "st1"});
#line 57
 testRunner.Then("\'dest\' contains strata:", ((string)(null)), table167, "Then ");
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
                SyncStrataFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                SyncStrataFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion