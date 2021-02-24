using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync.Util;
using System;
using System.Linq;

namespace CruiseDAL.V3.Sync
{
    public class DatabaseInitializer
    {
        public string CruiseID { get; set; }
        public string SaleID { get; set; }
        public string SaleNumber { get; set; }

        public string[] Units { get; set; }
        public Stratum[] Strata { get; set; }
        public CuttingUnit_Stratum[] UnitStrata { get; set; }
        public string[] Species { get; set; }
        public SampleGroup[] SampleGroups { get; set; }
        public TreeDefaultValue[] TreeDefaults { get; set; }
        public SubPopulation[] Subpops { get; set; }
        public Stratum[] PlotStrata { get; set; }
        public Stratum[] NonPlotStrata { get; set; }

        public DatabaseInitializer()
        {
            CruiseID = Guid.NewGuid().ToString();
            var saleID = SaleID = Guid.NewGuid().ToString();
            SaleNumber = (saleID.GetHashCode() % 10000).ToString();

            var units = Units = new string[] { "u1", "u2" };

            var plotStrata = PlotStrata = new[]
            {
                new Stratum{ StratumCode = "st1", Method = "PNT" },
                new Stratum{ StratumCode = "st2", Method = "PCM" },
            };

            var nonPlotStrata = NonPlotStrata = new[]
            {
                new Stratum{ StratumCode = "st3", Method = "STR" },
                new Stratum{ StratumCode = "st4", Method = "STR" },
            };

            var strata = Strata = plotStrata.Concat(nonPlotStrata).ToArray();

            UnitStrata = new[]
            {
                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = plotStrata[0].StratumCode },
                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = plotStrata[1].StratumCode},
                new CuttingUnit_Stratum {CuttingUnitCode = units[1], StratumCode = plotStrata[1].StratumCode},

                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = nonPlotStrata[0].StratumCode },
                new CuttingUnit_Stratum {CuttingUnitCode = units[0], StratumCode = nonPlotStrata[1].StratumCode},
                new CuttingUnit_Stratum {CuttingUnitCode = units[1], StratumCode = nonPlotStrata[1].StratumCode},
            };

            var species = Species = new string[] { "sp1", "sp2", "sp3" };

            var sampleGroups = SampleGroups = new[]
            {
                new SampleGroup {SampleGroupCode = "sg1", StratumCode = plotStrata[0].StratumCode, SamplingFrequency = 101, TallyBySubPop = true},
                new SampleGroup {SampleGroupCode = "sg2", StratumCode = plotStrata[1].StratumCode, SamplingFrequency = 102, TallyBySubPop = false},

                new SampleGroup {SampleGroupCode = "sg1", StratumCode = nonPlotStrata[0].StratumCode, SamplingFrequency = 101, TallyBySubPop = true},
                new SampleGroup {SampleGroupCode = "sg2", StratumCode = nonPlotStrata[1].StratumCode, SamplingFrequency = 102, TallyBySubPop = false},
            };

            TreeDefaults = new[]
            {
                new TreeDefaultValue {SpeciesCode = species[0], PrimaryProduct = "01"},
                new TreeDefaultValue {SpeciesCode = species[1], PrimaryProduct = "01"},
                new TreeDefaultValue {SpeciesCode = species[2], PrimaryProduct = "01"},
            };

            Subpops = new[]
            {
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = species[0],
                    LiveDead = "L",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = species[0],
                    LiveDead = "D",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = species[1],
                    LiveDead = "L",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[0].StratumCode,
                    SampleGroupCode = sampleGroups[0].SampleGroupCode,
                    SpeciesCode = species[2],
                    LiveDead = "L",
                },

                // plot strata
                new SubPopulation {
                    StratumCode = sampleGroups[2].StratumCode,
                    SampleGroupCode = sampleGroups[2].SampleGroupCode,
                    SpeciesCode = species[0],
                    LiveDead = "L",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[2].StratumCode,
                    SampleGroupCode = sampleGroups[2].SampleGroupCode,
                    SpeciesCode = species[1],
                    LiveDead = "L",
                },
                new SubPopulation {
                    StratumCode = sampleGroups[2].StratumCode,
                    SampleGroupCode = sampleGroups[2].SampleGroupCode,
                    SpeciesCode = species[2],
                    LiveDead = "L",
                },
            };
        }

        public CruiseDatastore_V3 CreateDatabase(string cruiseID = null, string saleID = null, string saleNumber =  null)
        {
            cruiseID = cruiseID ?? CruiseID;
            saleID = saleID ?? SaleID;
            saleNumber = saleNumber ?? SaleNumber;

            var units = Units;

            var strata = Strata;

            var unitStrata = UnitStrata;

            var sampleGroups = SampleGroups;

            var species = Species;

            var tdvs = TreeDefaults;

            var subPops = Subpops;

            var database = new CruiseDatastore_V3();

            InitializeDatabase(database, cruiseID, saleID, saleNumber, units, strata, unitStrata, sampleGroups, species, tdvs, subPops);

            return database;
        }

        public CruiseDatastore_V3 CreateDatabaseFile(string path, string cruiseID = null, string saleID = null, string saleNumber = null)
        {
            cruiseID = cruiseID ?? CruiseID;
            saleID = saleID ?? SaleID;
            saleNumber = saleNumber ?? SaleNumber;

            var units = Units;

            var strata = Strata;

            var unitStrata = UnitStrata;

            var sampleGroups = SampleGroups;

            var species = Species;

            var tdvs = TreeDefaults;

            var subPops = Subpops;

            var database = new CruiseDatastore_V3(path, true);

            InitializeDatabase(database, cruiseID, saleID, saleNumber, units, strata, unitStrata, sampleGroups, species, tdvs, subPops);

            return database;
        }

        public void InitializeDatabase(CruiseDatastore_V3 db,
            string cruiseID,
            string saleID,
            string saleNumber,
            string[] units,
            CruiseDAL.V3.Models.Stratum[] strata,
            CruiseDAL.V3.Models.CuttingUnit_Stratum[] unitStrata,
            CruiseDAL.V3.Models.SampleGroup[] sampleGroups,
            string[] species,
            CruiseDAL.V3.Models.TreeDefaultValue[] tdvs,
            CruiseDAL.V3.Models.SubPopulation[] subPops)
        {
            db.Insert(new Sale()
            {
                SaleID = saleID,
                SaleNumber = saleNumber,
            }); ;

            db.Insert(new Cruise()
            {
                CruiseID = cruiseID,
                SaleID = saleID,
                CruiseNumber = saleNumber,
            });

            //Cutting Units
            foreach (var unit in units.OrEmpty())
            {
                var unitID = Guid.NewGuid().ToString();
                db.Execute(
                    "INSERT INTO CuttingUnit (" +
                    "CruiseID, CuttingUnitID, CuttingUnitCode" +
                    ") VALUES " +
                    $"('{cruiseID}', '{unitID}', '{unit}');");
            }

            //Strata
            foreach (var st in strata.OrEmpty())
            {
                st.CruiseID = cruiseID;
                st.StratumID = Guid.NewGuid().ToString();
                db.Insert(st);
            }

            //Unit - Strata
            foreach (var cust in unitStrata.OrEmpty())
            {
                cust.CruiseID = cruiseID;
                db.Insert(cust);
            }

            //Sample Groups
            foreach (var sg in sampleGroups.OrEmpty())
            {
                sg.SampleGroupID = Guid.NewGuid().ToString();
                sg.CruiseID = cruiseID;
                db.Insert(sg);
            }

            foreach (var sp in species.OrEmpty())
            {
                db.Execute($"INSERT INTO Species (CruiseID, SpeciesCode) VALUES ('{cruiseID}', '{sp}');");
            }

            foreach (var tdv in tdvs.OrEmpty())
            {
                tdv.CruiseID = cruiseID;
                db.Insert(tdv);
            }

            foreach (var sub in subPops.OrEmpty())
            {
                sub.SubPopulationID = Guid.NewGuid().ToString();
                sub.CruiseID = cruiseID;
                db.Insert(sub);
            }
        }
    }
}
