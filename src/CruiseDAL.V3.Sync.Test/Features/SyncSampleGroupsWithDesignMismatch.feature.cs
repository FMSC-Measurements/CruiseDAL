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
    public partial class SyncSampleGroupsWithDesignMismatchFeature : object, Xunit.IClassFixture<SyncSampleGroupsWithDesignMismatchFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "SyncSampleGroupsWithDesignMismatch.feature"
#line hidden
        
        public SyncSampleGroupsWithDesignMismatchFeature(SyncSampleGroupsWithDesignMismatchFeature.FixtureData fixtureData, CruiseDAL_V3_Sync_Test_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Sync Sample Groups With Design Mismatch", null, ProgrammingLanguage.CSharp, ((string[])(null)));
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
            TechTalk.SpecFlow.Table table171 = new TechTalk.SpecFlow.Table(new string[] {
                        "FileAlias",
                        "DeviceAlias"});
            table171.AddRow(new string[] {
                        "source",
                        "srcDevice"});
            table171.AddRow(new string[] {
                        "dest",
                        "destDevice"});
#line 4
 testRunner.Given("the following cruise files exist:", ((string)(null)), table171, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table172 = new TechTalk.SpecFlow.Table(new string[] {
                        "CuttingUnitCode",
                        "CuttingUnitID"});
            table172.AddRow(new string[] {
                        "u1",
                        "unit1"});
#line 9
 testRunner.And("in \'source, dest\' the following units exist:", ((string)(null)), table172, "* ");
#line hidden
            TechTalk.SpecFlow.Table table173 = new TechTalk.SpecFlow.Table(new string[] {
                        "StratumCode",
                        "Units",
                        "StratumID",
                        "Method"});
            table173.AddRow(new string[] {
                        "st1",
                        "u1",
                        "stratum1",
                        "STR"});
#line 13
 testRunner.And("in \'source,dest\' the following strata exist:", ((string)(null)), table173, "* ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check For Sampling Frequency Design Mismatch Errors")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Sample Groups With Design Mismatch")]
        [Xunit.TraitAttribute("Description", "Check For Sampling Frequency Design Mismatch Errors")]
        public virtual void CheckForSamplingFrequencyDesignMismatchErrors()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check For Sampling Frequency Design Mismatch Errors", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 17
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
                TechTalk.SpecFlow.Table table174 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "SamplingFrequency"});
                table174.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "5"});
#line 18
 testRunner.Given("in \'source\' file the following sample groups exist:", ((string)(null)), table174, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table175 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "SamplingFrequency"});
                table175.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "6"});
#line 22
 testRunner.And("in \'dest\' file the following sample groups exist:", ((string)(null)), table175, "* ");
#line hidden
                TechTalk.SpecFlow.Table table176 = new TechTalk.SpecFlow.Table(new string[] {
                            "SpeciesCode"});
                table176.AddRow(new string[] {
                            "sp1"});
#line 29
 testRunner.And("in \'source, dest\' the following species exist:", ((string)(null)), table176, "* ");
#line hidden
                TechTalk.SpecFlow.Table table177 = new TechTalk.SpecFlow.Table(new string[] {
                            "CuttingUnitCode",
                            "StratumCode",
                            "SampleGroupCode",
                            "SpeciesCode",
                            "TreeNumber",
                            "TreeID"});
                table177.AddRow(new string[] {
                            "u1",
                            "st1",
                            "sg1",
                            "sp1",
                            "1",
                            "tree1"});
#line 33
 testRunner.And("in \'source, dest\' the following trees exist:", ((string)(null)), table177, "* ");
#line hidden
#line 36
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 37
 testRunner.And("I check \'source\' for Design Mismatch errors against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 38
 testRunner.Then("Has No Conflicts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table178 = new TechTalk.SpecFlow.Table(new string[] {
                            "error"});
                table178.AddRow(new string[] {
                            "Sample Group Sampling Frequency Mismatch:::: Sg Code:sg1 Stratum Code:st1"});
#line 39
 testRunner.And("Design Errors Has:", ((string)(null)), table178, "* ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check For KZ Design Mismatch Errors")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Sample Groups With Design Mismatch")]
        [Xunit.TraitAttribute("Description", "Check For KZ Design Mismatch Errors")]
        public virtual void CheckForKZDesignMismatchErrors()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check For KZ Design Mismatch Errors", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 43
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
                TechTalk.SpecFlow.Table table179 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "KZ"});
                table179.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "50"});
#line 44
 testRunner.Given("in \'source\' file the following sample groups exist:", ((string)(null)), table179, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table180 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "KZ"});
                table180.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "60"});
#line 48
 testRunner.And("in \'dest\' file the following sample groups exist:", ((string)(null)), table180, "* ");
#line hidden
                TechTalk.SpecFlow.Table table181 = new TechTalk.SpecFlow.Table(new string[] {
                            "SpeciesCode"});
                table181.AddRow(new string[] {
                            "sp1"});
#line 55
 testRunner.And("in \'source, dest\' the following species exist:", ((string)(null)), table181, "* ");
#line hidden
                TechTalk.SpecFlow.Table table182 = new TechTalk.SpecFlow.Table(new string[] {
                            "CuttingUnitCode",
                            "StratumCode",
                            "SampleGroupCode",
                            "SpeciesCode",
                            "TreeNumber",
                            "TreeID"});
                table182.AddRow(new string[] {
                            "u1",
                            "st1",
                            "sg1",
                            "sp1",
                            "1",
                            "tree1"});
#line 59
 testRunner.And("in \'source, dest\' the following trees exist:", ((string)(null)), table182, "* ");
#line hidden
#line 62
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 63
 testRunner.And("I check \'source\' for Design Mismatch errors against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 64
 testRunner.Then("Has No Conflicts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table183 = new TechTalk.SpecFlow.Table(new string[] {
                            "error"});
                table183.AddRow(new string[] {
                            "Sample Group KZ Mismatch:::: Sg Code:sg1 Stratum Code:st1"});
#line 65
 testRunner.And("Design Errors Has:", ((string)(null)), table183, "* ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check For Insurance Frequency Design Mismatch Errors")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Sample Groups With Design Mismatch")]
        [Xunit.TraitAttribute("Description", "Check For Insurance Frequency Design Mismatch Errors")]
        public virtual void CheckForInsuranceFrequencyDesignMismatchErrors()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check For Insurance Frequency Design Mismatch Errors", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 69
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
                TechTalk.SpecFlow.Table table184 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "SamplingFrequency",
                            "InsuranceFrequency"});
                table184.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "5",
                            "5"});
#line 70
 testRunner.Given("in \'source\' file the following sample groups exist:", ((string)(null)), table184, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table185 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "SamplingFrequency",
                            "InsuranceFrequency"});
                table185.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "5",
                            "6"});
#line 74
 testRunner.And("in \'dest\' file the following sample groups exist:", ((string)(null)), table185, "* ");
#line hidden
                TechTalk.SpecFlow.Table table186 = new TechTalk.SpecFlow.Table(new string[] {
                            "SpeciesCode"});
                table186.AddRow(new string[] {
                            "sp1"});
#line 81
 testRunner.And("in \'source, dest\' the following species exist:", ((string)(null)), table186, "* ");
#line hidden
                TechTalk.SpecFlow.Table table187 = new TechTalk.SpecFlow.Table(new string[] {
                            "CuttingUnitCode",
                            "StratumCode",
                            "SampleGroupCode",
                            "SpeciesCode",
                            "TreeNumber",
                            "TreeID"});
                table187.AddRow(new string[] {
                            "u1",
                            "st1",
                            "sg1",
                            "sp1",
                            "1",
                            "tree1"});
#line 85
 testRunner.And("in \'source, dest\' the following trees exist:", ((string)(null)), table187, "* ");
#line hidden
#line 88
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 89
 testRunner.And("I check \'source\' for Design Mismatch errors against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 90
 testRunner.Then("Has No Conflicts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table188 = new TechTalk.SpecFlow.Table(new string[] {
                            "error"});
                table188.AddRow(new string[] {
                            "Sample Group Insurance Frequency Mismatch:::: Sg Code:sg1 Stratum Code:st1"});
#line 91
 testRunner.And("Design Errors Has:", ((string)(null)), table188, "* ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check For BigBAF Design Mismatch Errors")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Sample Groups With Design Mismatch")]
        [Xunit.TraitAttribute("Description", "Check For BigBAF Design Mismatch Errors")]
        public virtual void CheckForBigBAFDesignMismatchErrors()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check For BigBAF Design Mismatch Errors", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 95
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
                TechTalk.SpecFlow.Table table189 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "BigBAF"});
                table189.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "15"});
#line 96
 testRunner.Given("in \'source\' file the following sample groups exist:", ((string)(null)), table189, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table190 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "BigBAF"});
                table190.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "20"});
#line 100
 testRunner.And("in \'dest\' file the following sample groups exist:", ((string)(null)), table190, "* ");
#line hidden
                TechTalk.SpecFlow.Table table191 = new TechTalk.SpecFlow.Table(new string[] {
                            "SpeciesCode"});
                table191.AddRow(new string[] {
                            "sp1"});
#line 107
 testRunner.And("in \'source, dest\' the following species exist:", ((string)(null)), table191, "* ");
#line hidden
                TechTalk.SpecFlow.Table table192 = new TechTalk.SpecFlow.Table(new string[] {
                            "CuttingUnitCode",
                            "StratumCode",
                            "SampleGroupCode",
                            "SpeciesCode",
                            "TreeNumber",
                            "TreeID"});
                table192.AddRow(new string[] {
                            "u1",
                            "st1",
                            "sg1",
                            "sp1",
                            "1",
                            "tree1"});
#line 111
 testRunner.And("in \'source, dest\' the following trees exist:", ((string)(null)), table192, "* ");
#line hidden
#line 114
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 115
 testRunner.And("I check \'source\' for Design Mismatch errors against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 116
 testRunner.Then("Has No Conflicts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table193 = new TechTalk.SpecFlow.Table(new string[] {
                            "error"});
                table193.AddRow(new string[] {
                            "Sample Group BigBAF Mismatch:::: Sg Code:sg1 Stratum Code:st1"});
#line 117
 testRunner.And("Design Errors Has:", ((string)(null)), table193, "* ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check For SmallFPS Design Mismatch Errors")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Sample Groups With Design Mismatch")]
        [Xunit.TraitAttribute("Description", "Check For SmallFPS Design Mismatch Errors")]
        public virtual void CheckForSmallFPSDesignMismatchErrors()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check For SmallFPS Design Mismatch Errors", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 121
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
                TechTalk.SpecFlow.Table table194 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "SmallFPS"});
                table194.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "15"});
#line 122
 testRunner.Given("in \'source\' file the following sample groups exist:", ((string)(null)), table194, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table195 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "SmallFPS"});
                table195.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "20"});
#line 126
 testRunner.And("in \'dest\' file the following sample groups exist:", ((string)(null)), table195, "* ");
#line hidden
                TechTalk.SpecFlow.Table table196 = new TechTalk.SpecFlow.Table(new string[] {
                            "SpeciesCode"});
                table196.AddRow(new string[] {
                            "sp1"});
#line 133
 testRunner.And("in \'source, dest\' the following species exist:", ((string)(null)), table196, "* ");
#line hidden
                TechTalk.SpecFlow.Table table197 = new TechTalk.SpecFlow.Table(new string[] {
                            "CuttingUnitCode",
                            "StratumCode",
                            "SampleGroupCode",
                            "SpeciesCode",
                            "TreeNumber",
                            "TreeID"});
                table197.AddRow(new string[] {
                            "u1",
                            "st1",
                            "sg1",
                            "sp1",
                            "1",
                            "tree1"});
#line 137
 testRunner.And("in \'source, dest\' the following trees exist:", ((string)(null)), table197, "* ");
#line hidden
#line 140
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 141
 testRunner.And("I check \'source\' for Design Mismatch errors against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 142
 testRunner.Then("Has No Conflicts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table198 = new TechTalk.SpecFlow.Table(new string[] {
                            "error"});
                table198.AddRow(new string[] {
                            "Sample Group SmallFPS Mismatch:::: Sg Code:sg1 Stratum Code:st1"});
#line 143
 testRunner.And("Design Errors Has:", ((string)(null)), table198, "* ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Check For TallyBySubPop Design Mismatch Errors")]
        [Xunit.TraitAttribute("FeatureTitle", "Sync Sample Groups With Design Mismatch")]
        [Xunit.TraitAttribute("Description", "Check For TallyBySubPop Design Mismatch Errors")]
        public virtual void CheckForTallyBySubPopDesignMismatchErrors()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Check For TallyBySubPop Design Mismatch Errors", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 147
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
                TechTalk.SpecFlow.Table table199 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "SamplingFrequency",
                            "TallyBySubPop"});
                table199.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "5",
                            "true"});
#line 148
 testRunner.Given("in \'source\' file the following sample groups exist:", ((string)(null)), table199, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table200 = new TechTalk.SpecFlow.Table(new string[] {
                            "SampleGroupCode",
                            "StratumCode",
                            "SampleGroupID",
                            "SamplingFrequency",
                            "TallyBySubPop"});
                table200.AddRow(new string[] {
                            "sg1",
                            "st1",
                            "sg1_st1",
                            "5",
                            "false"});
#line 152
 testRunner.And("in \'dest\' file the following sample groups exist:", ((string)(null)), table200, "* ");
#line hidden
                TechTalk.SpecFlow.Table table201 = new TechTalk.SpecFlow.Table(new string[] {
                            "SpeciesCode"});
                table201.AddRow(new string[] {
                            "sp1"});
#line 159
 testRunner.And("in \'source, dest\' the following species exist:", ((string)(null)), table201, "* ");
#line hidden
                TechTalk.SpecFlow.Table table202 = new TechTalk.SpecFlow.Table(new string[] {
                            "CuttingUnitCode",
                            "StratumCode",
                            "SampleGroupCode",
                            "SpeciesCode",
                            "TreeNumber",
                            "TreeID"});
                table202.AddRow(new string[] {
                            "u1",
                            "st1",
                            "sg1",
                            "sp1",
                            "1",
                            "tree1"});
#line 163
 testRunner.And("in \'source, dest\' the following trees exist:", ((string)(null)), table202, "* ");
#line hidden
#line 166
 testRunner.When("I conflict check \'source\' file against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 167
 testRunner.And("I check \'source\' for Design Mismatch errors against \'dest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 168
 testRunner.Then("Has No Conflicts", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table203 = new TechTalk.SpecFlow.Table(new string[] {
                            "error"});
                table203.AddRow(new string[] {
                            "Sample Group TallyBySubPop Mismatch:::: Sg Code:sg1 Stratum Code:st1"});
#line 169
 testRunner.And("Design Errors Has:", ((string)(null)), table203, "* ");
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
                SyncSampleGroupsWithDesignMismatchFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                SyncSampleGroupsWithDesignMismatchFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion